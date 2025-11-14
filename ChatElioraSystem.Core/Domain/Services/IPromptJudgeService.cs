using ChatElioraSystem.Core.Domain.Services.Bases;
using ChatElioraSystem.Core.Infrastructure.Models;

namespace ChatElioraSystem.Core.Domain.Services
{
    public interface IPromptJudgeService : IBasePromptService
    {
        new List<IChatMessage>? GetAdditionalChatMessage();
        IAsyncEnumerable<string> GetStreamAsyncJudge(List<IChatMessage> chatMessage, int llmNo, List<string> cancelationWords, CancellationToken cancellationToken);

    }
}