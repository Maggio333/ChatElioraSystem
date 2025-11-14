using ChatElioraSystem.Core.Application_.Enums;
using ChatElioraSystem.Core.Application_.Models;
using ChatElioraSystem.Core.Infrastructure.Models;
using System.Collections.ObjectModel;
using System.Threading;

namespace ChatElioraSystem.Core.Application_.Services
{
    public interface IPromptTypeOrchiestratorService
    {
        Task<string> SendToLLM(ObservableCollection<IChatMessage> chatMessages, SesjaTematu temat, int llmNo, CancellationToken cancellationToken = default);
        Task<SesjaTematu> GetCategory(IEnumerable<IChatMessage> chatMessagesAll, int llmNo, CancellationToken cancellationToken = default);
        Task<SesjaTematu> GetCategory(IEnumerable<IChatMessage> chatMessagesAll, SesjaTematu sesjaTematu, int llmNo, CancellationToken cancellationToken = default);

        IAsyncEnumerable<string> SendStreamToLLM(IEnumerable<IChatMessage> chatMessages, SesjaTematu temat, int llmNo, CancellationToken cancellationToken = default);
        IAsyncEnumerable<RAGChunk> GetStreamDataFromVectorDb(IEnumerable<IChatMessage> chatMessages, int llmNo, CancellationToken cancellationToken = default);
        IAsyncEnumerable<RAGChunk> SaveStreamDataFromVectorDb(IEnumerable<IChatMessage> chatMessages, int llmNo, CancellationToken cancellationToken = default);
        public bool IsSaveToDbVector { get; set; }

        Task<IChatMessage> SaveCurrentTopic(IEnumerable<IChatMessage> chatMessages, int llmNo, CancellationToken cancellationToken = default);
        Task<string> LoadCurrentTopic(CancellationToken cancellationToken = default);

        Task<IChatMessage> GetBaseIdioms();
    }
}