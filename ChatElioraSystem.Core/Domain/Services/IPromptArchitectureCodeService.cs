using ChatElioraSystem.Core.Domain.Services.Bases;
using ChatElioraSystem.Core.Infrastructure.Models;

namespace ChatElioraSystem.Core.Domain.Services
{
    public interface IPromptArchitectureCodeService : IBasePromptService
    {
        List<IChatMessage>? GetAdditionalChatMessage();
    }
}