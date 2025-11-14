using ChatElioraSystem.Core.Domain.Resources.Bases;

namespace ChatElioraSystem.Core.Domain.Resources
{
    public interface IRAGPromptReflection : IBaseRAGPrompt
    {
        string Theme { get; }
        string Emotional { get; }

    }
}