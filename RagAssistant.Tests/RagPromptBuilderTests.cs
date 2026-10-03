using RagAssistant.Infrastructure.Generation;
using Xunit;

namespace RagAssistant.Tests
{
    public class RagPromptBuilderTests
    {
        [Fact]
        public void Build_ShouldIncludeQuestionAndContext()
        {
            var builder = new RagPromptBuilder();

            var question = "What is Redis?";
            var context = "Redis is an in-memory data store.";

            var prompt = builder.Build(
                question,
                context);

            Assert.Contains(question, prompt);
            Assert.Contains(context, prompt);
        }

        [Fact]
        public void Build_ShouldIncludeInstructionToUseOnlyContext()
        {
            var builder = new RagPromptBuilder();

            var prompt = builder.Build(
                "What is Redis?",
                "Redis is an in-memory data store.");

            Assert.Contains(
                "using only the provided context",
                prompt);
        }

        [Fact]
        public void Build_ShouldThrow_WhenQuestionIsEmpty()
        {
            var builder = new RagPromptBuilder();

            Assert.Throws<ArgumentException>(
                () => builder.Build(
                    "",
                    "Some context."));
        }

        [Fact]
        public void Build_ShouldThrow_WhenContextIsEmpty()
        {
            var builder = new RagPromptBuilder();

            Assert.Throws<ArgumentException>(
                () => builder.Build(
                    "What is Redis?",
                    ""));
        }
    }
}
