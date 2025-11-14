using ChatElioraSystem.Core.Infrastructure.Models;

namespace ChatElioraSystem.Core.Domain.Services.Bases
{
    public interface IBasePromptService
    {
        List<IChatMessage>? GetAdditionalChatMessage();

        public Task<string> GetCompletionAsync(IEnumerable<IChatMessage> chatMessage, LLMNamesEnum lLMName, int llmNo, CancellationToken cancellationToken);
        //public Task<string> GetCompletionAsync(IEnumerable<IChatMessage> chatMessage, LLMNamesEnum lLMName, CancellationToken cancellationToken);
        public IAsyncEnumerable<string> GetStreamAsync(IEnumerable<IChatMessage> chatMessage, LLMNamesEnum lLMName, int llmNo, CancellationToken cancellationToken);
        public IAsyncEnumerable<string> GetStreamAsync(IEnumerable<IChatMessage> chatMessage, LLMNamesEnum lLMName, int llmNo, List<string> cancelationWords, CancellationToken cancellationToken);

    }
}