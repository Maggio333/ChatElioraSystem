using ChatElioraSystem.Core.Domain.Resources.Bases;

namespace ChatElioraSystem.Core.Domain.Resources
{
    public interface IRAGPromptCode : IBaseRAGPrompt   
    {
        public string WzorceProjektowe { get; }
        public string CodeLanguage { get; }

    }
}
