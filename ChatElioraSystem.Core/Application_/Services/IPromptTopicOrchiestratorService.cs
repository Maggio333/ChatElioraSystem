using ChatElioraSystem.Core.Domain.Models;
using ChatElioraSystem.Core.Infrastructure.Models;
using System.Text.Json;

namespace ChatElioraSystem.Core.Application_.Services
{
    public interface IPromptTopicOrchiestratorService
    {
        Task<string> GetLastTopics();

        IAsyncEnumerable<string> ManageCurrentTopic(IEnumerable<IChatMessage> messages, int llmNo, ActionMode actionMode, CancellationToken cancellationToken);
    }
}