namespace RagAssistant.Domain.Dtos.Ocr;

public sealed class OcrResponse
{
    public string? fileName { get; set; }

    public string? text { get; set; }
}