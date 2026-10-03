using OpenAI.Chat;
using RagAssistant.Application.Handlers.Documents.Commands.IngestDocument;
using RagAssistant.Application.Interfaces;
using RagAssistant.Application.Models;
using RagAssistant.Infrastructure.Embedding;
using RagAssistant.Infrastructure.Generation;
using RagAssistant.Infrastructure.Llm;
using RagAssistant.Infrastructure.RagIngestion;
using RagAssistant.Infrastructure.Retrieval;
using RagAssistant.Infrastructure.Text;
using RagAssistant.Infrastructure.Vector;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMediatR(
    cfg => cfg.RegisterServicesFromAssembly(
        typeof(IngestDocumentCommand).Assembly));

builder.Services.AddSingleton(new ChunkOptions
{
    MaxChunkSize = 500,
    OverlapSize = 100
});

var apiKey = builder.Configuration["OpenAI:ApiKey"]
    ?? throw new InvalidOperationException(
        "OpenAI API key is not configured.");

// OpenAI Embedding
//var embeddingClient = new EmbeddingClient(
//    "text-embedding-3-small",
//    apiKey);

//builder.Services.AddSingleton(embeddingClient);

var modelPath = Path.Combine(
    AppContext.BaseDirectory,
    "Models",
    "multilingual-e5-small",
    "model_qint8_avx512_vnni.onnx");

var tokenizerPath = Path.Combine(
    AppContext.BaseDirectory,
    "Models",
    "multilingual-e5-small",
    "sentencepiece.bpe.model");

builder.Services.AddSingleton<IEmbeddingGenerator>(
    new LocalEmbeddingGenerator(
        modelPath,
        tokenizerPath));

//builder.Services.AddScoped<
//    IEmbeddingGenerator,
//    OpenAIEmbeddingGenerator>();


// OpenAI Chat
var chatClient = new ChatClient(
    model: "gpt-4o-mini",
    apiKey: apiKey);

builder.Services.AddSingleton(chatClient);

builder.Services.AddHttpClient<ILlmClient, LocalLlmClient>(
    client =>
    {
        client.BaseAddress =
            new Uri("http://localhost:11434");
    });


// RAG Services
builder.Services.AddSingleton<IVectorStore, InMemoryVectorStore>();

builder.Services.AddScoped<ITextChunker, TextChunker>();
builder.Services.AddScoped<IRagIngestionService, RagIngestionService>();

builder.Services.AddScoped<IRagRetriever, RagRetriever>();
builder.Services.AddScoped<IRagContextBuilder, RagContextBuilder>();
builder.Services.AddScoped<IRagPromptBuilder, RagPromptBuilder>();
builder.Services.AddScoped<IRagAnswerService, RagAnswerService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
