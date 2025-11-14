using ChatElioraSystem.Core.Domain.Models;
using ChatElioraSystem.Core.Domain.Services.Bases;
using ChatElioraSystem.Core.Infrastructure.Models;

namespace ChatElioraSystem.Core.Domain.Services
{
    public interface IPromptMCPTopicsService : IBasePromptService
    {
        ActionMode ActionMode { get; set; }

        List<IChatMessage>? GetAdditionalChatMessage();
    }
}