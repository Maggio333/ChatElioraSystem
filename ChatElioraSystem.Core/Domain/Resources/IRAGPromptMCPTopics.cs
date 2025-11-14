namespace ChatElioraSystem.Core.Domain.Resources
{
    public interface IRAGPromptMCPTopics
    {
        string DbVectorPrompt { get; }
        string ReadDbVectorPrompt { get; }
        string Role { get; }
        string SaveToDbVectorPrompt { get; }
    }
}