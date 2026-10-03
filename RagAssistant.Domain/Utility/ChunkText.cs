using System.Text.RegularExpressions;

namespace RagAssistant.Domain.Utility
{
    public static class ChunkText
    {
        public static string[] SplitIntoParagraphs(string text)
        {
            return Regex.Split(
                text.Trim(),
                @"\r?\n\s*\r?\n");
        }

        public static string[] SplitIntoSentences(string paragraph)
        {
            return Regex.Split(
                paragraph.Trim(),
                @"(?<=[.!?؟])\s+");
        }

        public static IEnumerable<string> SplitLongSentence(string sentence, int maxChunkSize)
        {
            var words = sentence.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            var current = string.Empty;

            foreach (var word in words)
            {
                var candidate = string.IsNullOrEmpty(current)
                    ? word
                    : $"{current} {word}";

                if (candidate.Length <= maxChunkSize)
                {
                    current = candidate;
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(current))
                {
                    yield return current;
                }

                current = word;
            }

            if (!string.IsNullOrWhiteSpace(current))
            {
                yield return current;
            }
        }

    }
}
