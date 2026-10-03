using RagAssistant.Domain.Common;

namespace RagAssistant.Domain.Entities
{
    public class Document : BaseEntity
    {
        public string FileName { get; private set; }
        public string Content { get; private set; }

        private Document()
        {
        }

        public Document(string fileName, string content)
        {
            FileName = fileName;
            Content = content;
        }
    }
}
