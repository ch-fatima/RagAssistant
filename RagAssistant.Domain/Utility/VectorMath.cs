namespace RagAssistant.Domain.Utility
{
    public static class VectorMath
    {
        public static float CosineSimilarity(
            ReadOnlySpan<float> a,
            ReadOnlySpan<float> b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException(
                    "Vectors must have the same dimension.");

            if (a.Length == 0)
                throw new ArgumentException(
                    "Vectors cannot be empty.");

            double dotProduct = 0;
            double magnitudeA = 0;
            double magnitudeB = 0;

            for (int i = 0; i < a.Length; i++)
            {
                dotProduct += a[i] * b[i];

                magnitudeA += a[i] * a[i];
                magnitudeB += b[i] * b[i];
            }

            if (magnitudeA == 0 || magnitudeB == 0)
                return 0;

            return (float)(
                dotProduct /
                (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB)));
        }
    }
}
