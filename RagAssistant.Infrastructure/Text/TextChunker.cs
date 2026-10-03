using RagAssistant.Application.Interfaces;
using RagAssistant.Application.Models;
using RagAssistant.Domain.Utility;

namespace RagAssistant.Infrastructure.Text
{
    public class TextChunker : ITextChunker
    {
        private readonly ChunkOptions _options;

        public TextChunker(ChunkOptions options)
        {
            _options = options;
        }

        public IReadOnlyList<string> Chunk(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return [];

            var paragraphs = ChunkText.SplitIntoParagraphs(text);

            var chunks = new List<string>();
            var currentChunk = string.Empty;

            foreach (var paragraph in paragraphs)
            {
                var sentences = ChunkText.SplitIntoSentences(paragraph);

                foreach (var sentence in sentences)
                {
                    if (string.IsNullOrWhiteSpace(sentence))
                        continue;

                    if (sentence.Length > _options.MaxChunkSize)
                    {
                        if (!string.IsNullOrWhiteSpace(currentChunk))
                        {
                            chunks.Add(currentChunk);
                            currentChunk = string.Empty;
                        }

                        var partsentences = ChunkText.SplitLongSentence(sentence, _options.MaxChunkSize);
                        foreach (var part in partsentences)
                        {
                            chunks.Add(part);
                        }

                        currentChunk = string.Empty;

                        continue;
                    }

                    var candidate = string.IsNullOrEmpty(currentChunk)
                        ? sentence
                        : $"{currentChunk} {sentence}";

                    if (candidate.Length <= _options.MaxChunkSize)
                    {
                        currentChunk = candidate;
                        continue;
                    }

                    chunks.Add(currentChunk);

                    currentChunk = CreateChunkWithOverlap(
                        currentChunk,
                        sentence);
                }
            }

            if (!string.IsNullOrWhiteSpace(currentChunk))
            {
                chunks.Add(currentChunk);
            }

            return chunks;
        }

        private string CreateChunkWithOverlap(
            string previousChunk,
            string nextSentence)
        {
            if (string.IsNullOrWhiteSpace(nextSentence))
                return string.Empty;

            if (string.IsNullOrWhiteSpace(previousChunk))
                return nextSentence;

            var overlap = GetOverlap(previousChunk);

            while (!string.IsNullOrWhiteSpace(overlap))
            {
                var candidate = $"{overlap} {nextSentence}";

                if (candidate.Length <= _options.MaxChunkSize)
                    return candidate;

                overlap = RemoveFirstWord(overlap);
            }

            return nextSentence;
        }
        private string RemoveFirstWord(string text)
        {
            var index = text.IndexOf(' ');

            if (index == -1)
                return string.Empty;

            return text[(index + 1)..];
        }
        private string GetOverlap(string text)
        {
            var words = text.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            var overlapWords = new List<string>();

            var currentLength = 0;

            for (var i = words.Length - 1; i >= 0; i--)
            {
                var word = words[i];

                var newLength = currentLength == 0
                    ? word.Length
                    : word.Length + 1 + currentLength;

                if (newLength > _options.OverlapSize)
                    break;

                overlapWords.Insert(0, word);

                currentLength = newLength;
            }

            return string.Join(" ", overlapWords);
        }

    }
}
