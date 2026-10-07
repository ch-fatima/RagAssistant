namespace RagAssistant.Domain.Dtos.Ocr;

public sealed partial class PdfOcrExtractor
{
    private sealed class OcrResponse
    {
        public string? fileName { get; set; }

        public string? text { get; set; }
    }
}