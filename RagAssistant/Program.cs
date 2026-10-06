using OpenAI.Chat;
using Qdrant.Client;
using RagAssistant.Application.Handlers.Documents.Commands.IngestDocument;
using RagAssistant.Application.Interfaces;
using RagAssistant.Application.Models;
using RagAssistant.Infrastructure.Embedding;
using RagAssistant.Infrastructure.Generation;
using RagAssistant.Infrastructure.Llm;
using RagAssistant.Infrastructure.PdfText;
using RagAssistant.Infrastructure.RagIngestion;
using RagAssistant.Infrastructure.Retrieval;
using RagAssistant.Infrastructure.Text;
using RagAssistant.Infrastructure.Vector;
using Scalar.AspNetCore;

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
        client.Timeout = TimeSpan.FromMinutes(5);
    });


// RAG Services
builder.Services.AddScoped<ITextChunker, TextChunker>();
builder.Services.AddScoped<IRagIngestionService, RagIngestionService>();

builder.Services.AddScoped<IRagRetriever, RagRetriever>();
builder.Services.AddScoped<IRagContextBuilder, RagContextBuilder>();
builder.Services.AddScoped<IRagPromptBuilder, RagPromptBuilder>();
builder.Services.AddScoped<IRagAnswerService, RagAnswerService>();
builder.Services.AddHttpClient<PdfOcrExtractor>(client =>
{
    client.BaseAddress =
        new Uri("http://127.0.0.1:8000");

    client.Timeout =
        TimeSpan.FromMinutes(10);
});

builder.Services.AddScoped<IPdfTextExtractor>(
    sp => sp.GetRequiredService<PdfOcrExtractor>());

builder.Services.AddSingleton<QdrantClient>(_ =>
    new QdrantClient("localhost", 6334));

builder.Services.AddSingleton<IVectorStore, QdrantVectorStore>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
//builder.Services.AddSwaggerGen(options =>
//{
//    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

//    options.IncludeXmlComments(xmlPath);
//});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options
        .WithTitle("RagAssistant API")
        .WithTheme(ScalarTheme.Default);
});

app.MapControllers();

app.Run();
