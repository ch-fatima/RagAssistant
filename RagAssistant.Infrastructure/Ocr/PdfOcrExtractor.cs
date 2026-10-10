using RagAssistant.Application.Interfaces;
using RagAssistant.Domain.Dtos.Ocr;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RagAssistant.Infrastructure.PdfText;

public sealed partial class PdfOcrExtractor : IPdfTextExtractor
{
    private readonly HttpClient _httpClient;

    public PdfOcrExtractor(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> ExtractTextAsync(
        Stream pdfStream,
        CancellationToken cancellationToken = default)
    {
        if (pdfStream is null)
            throw new ArgumentNullException(nameof(pdfStream));

        cancellationToken.ThrowIfCancellationRequested();

        using var content = new MultipartFormDataContent();

        using var fileContent = new StreamContent(pdfStream);

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue("application/pdf");

        content.Add(
            fileContent,
            "file",
            "document.pdf");

        using var response = await _httpClient.PostAsync(
            "/ocr",
            content,
            cancellationToken);

        var responseBody =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"OCR service returned " +
                $"{(int)response.StatusCode}: " +
                responseBody);
        }

        var result =
            JsonSerializer.Deserialize<OcrResponse>(
                responseBody);

        if (result is null ||
            string.IsNullOrWhiteSpace(result.text))
        {
            throw new InvalidOperationException(
                "OCR service returned empty text.");
        }

        return result.text;
    }
}