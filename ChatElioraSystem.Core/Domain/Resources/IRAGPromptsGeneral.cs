using ChatElioraSystem.Core.Domain.Resources.Bases;

namespace ChatElioraSystem.Core.Domain.Resources
{
    public interface IRAGPromptsGeneral : IBaseRAGPrompt
    {
        string ColorPromptSystem { get; }
        string FirstSystemPrompt { get; }
        //string SaveToDbVectorPrompt { get; }

        static virtual string GetOcenaPrompt() => string.Empty;

        string UserAdminPrompt { get; }

    }
}