using ChatElioraSystem.Core.Application_.Models;
using ChatElioraSystem.Core.Domain.Models;
using ChatElioraSystem.Core.Infrastructure.Models;

namespace ChatElioraSystem.Core.Domain.Services
{
    public interface IPromptDbVecService
    {
        List<IChatMessage>? GetAdditionalChatMessage();
        IAsyncEnumerable<string> GetStreamAsync(IEnumerable<IChatMessage> chatMessage, LLMNamesEnum lLMName, int llmNo, List<string> cancelationWords, CancellationToken cancellationToken);
        IAsyncEnumerable<RAGChunk> GetStreamHistoryFromDb(IEnumerable<IChatMessage> chatMessage, int llmNo, List<string> cancelationWords, CancellationToken cancellationToken);
        IAsyncEnumerable<RAGChunk> GetStreamHistoryFromDb(IEnumerable<IChatMessage> chatMessage, int llmNo, CancellationToken cancellationToken);

        Task<string> GetBaseIdioms();
        //IAsyncEnumerable<string> SetAIMemory(IEnumerable<IChatMessage> chatMessage, List<string> cancelationWords, CancellationTokenSource cancellationTokenSource);
        //IAsyncEnumerable<string> SetAIMemory(IEnumerable<IChatMessage> chatMessage);

        public ActionMode ActionMode { get; set; }

    }
}