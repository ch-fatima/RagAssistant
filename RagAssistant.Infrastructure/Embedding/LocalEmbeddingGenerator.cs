using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Tokenizers;
using RagAssistant.Application.Interfaces;

namespace RagAssistant.Infrastructure.Embedding
{
    public sealed class LocalEmbeddingGenerator
    : IEmbeddingGenerator, IDisposable
    {
        private const int MaxSequenceLength = 512;

        private readonly InferenceSession _session;
        private readonly SentencePieceTokenizer _tokenizer;

        public LocalEmbeddingGenerator(
            string modelPath,
            string tokenizerPath)
        {
            if (!File.Exists(modelPath))
            {
                throw new FileNotFoundException(
                    "Embedding model was not found.",
                    modelPath);
            }

            if (!File.Exists(tokenizerPath))
            {
                throw new FileNotFoundException(
                    "SentencePiece tokenizer was not found.",
                    tokenizerPath);
            }

            _session = new InferenceSession(modelPath);

            using var tokenizerStream =
                File.OpenRead(tokenizerPath);

            _tokenizer = SentencePieceTokenizer.Create(
                tokenizerStream,
                addBeginningOfSentence: true,
                addEndOfSentence: true);
        }

        public Task<float[]> GenerateAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(
                    "Text cannot be empty.",
                    nameof(text));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var embedding = GenerateEmbedding(text);

            return Task.FromResult(embedding);
        }

        private float[] GenerateEmbedding(string text)
        {
            // E5 expects different prefixes for documents and queries.
            // For the ingestion pipeline we treat the input as a passage.
            var inputText = $"passage: {text}";

            var tokenIds = _tokenizer.EncodeToIds(
                inputText,
                true,
                true,
                MaxSequenceLength,
                out _,
                out _);

            if (tokenIds.Count == 0)
            {
                throw new InvalidOperationException(
                    "Tokenizer produced no tokens.");
            }

            var inputIds =
                new DenseTensor<long>(
                    new[] { 1, MaxSequenceLength });

            var attentionMask =
                new DenseTensor<long>(
                    new[] { 1, MaxSequenceLength });

            for (var i = 0; i < tokenIds.Count; i++)
            {
                inputIds[0, i] = tokenIds[i];
                attentionMask[0, i] = 1;
            }
            var tokenTypeIds =
                new DenseTensor<long>(
                    new[] { 1, MaxSequenceLength });
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(
                    "input_ids",
                    inputIds),
                
                NamedOnnxValue.CreateFromTensor(
                    "attention_mask",
                    attentionMask),
                
                NamedOnnxValue.CreateFromTensor(
                    "token_type_ids",
                    tokenTypeIds)
            };

            using var results = _session.Run(inputs);

            var output =
                results.FirstOrDefault(x =>
                    x.Name == "last_hidden_state");

            if (output is null)
            {
                throw new InvalidOperationException(
                    "ONNX model did not return 'last_hidden_state'.");
            }

            var hiddenStates =
                output.AsTensor<float>();

            return MeanPoolAndNormalize(
                hiddenStates,
                attentionMask);
        }

        private static float[] MeanPoolAndNormalize(
            Tensor<float> hiddenStates,
            Tensor<long> attentionMask)
        {
            if (hiddenStates.Rank != 3)
            {
                throw new InvalidOperationException(
                    $"Expected 3D tensor but received " +
                    $"{hiddenStates.Rank} dimensions.");
            }

            var sequenceLength =
                hiddenStates.Dimensions[1];

            var embeddingSize =
                hiddenStates.Dimensions[2];

            var embedding =
                new float[embeddingSize];

            var validTokenCount = 0;

            for (var tokenIndex = 0;
                 tokenIndex < sequenceLength;
                 tokenIndex++)
            {
                if (attentionMask[0, tokenIndex] == 0)
                    continue;

                validTokenCount++;

                for (var dimension = 0;
                     dimension < embeddingSize;
                     dimension++)
                {
                    embedding[dimension] +=
                        hiddenStates[
                            0,
                            tokenIndex,
                            dimension];
                }
            }

            if (validTokenCount == 0)
            {
                throw new InvalidOperationException(
                    "No valid tokens were found.");
            }

            // Mean Pooling
            for (var i = 0;
                 i < embedding.Length;
                 i++)
            {
                embedding[i] /= validTokenCount;
            }

            // L2 Normalization
            double sumOfSquares = 0;

            for (var i = 0;
                 i < embedding.Length;
                 i++)
            {
                sumOfSquares +=
                    embedding[i] * embedding[i];
            }

            var norm = Math.Sqrt(sumOfSquares);

            if (norm == 0)
                return embedding;

            for (var i = 0;
                 i < embedding.Length;
                 i++)
            {
                embedding[i] =
                    (float)(embedding[i] / norm);
            }

            return embedding;
        }

        public void Dispose()
        {
            _session.Dispose();
        }
    }
}
