using ChatElioraSystem.Core.Application_.Common.Factories;
using ChatElioraSystem.Core.Domain.Resources;
using ChatElioraSystem.Core.Domain.Services.Bases;
using ChatElioraSystem.Core.Infrastructure.Models;
using ChatElioraSystem.Core.Infrastructure.Services;

namespace ChatElioraSystem.Core.Domain.Services
{
    public class PromptArchitectureCodeService : BasePromptService, IPromptArchitectureCodeService
    {
        public IRAGPromptArchitectureCode RAGPromptArchitectureCode;
        public PromptArchitectureCodeService(ILlmService llmService, IRAGPromptsGeneral _rAGPromptsGeneral, IRAGPromptArchitectureCode rAGPromptArchitectureCode) : base(llmService, _rAGPromptsGeneral)
        {
            RAGPromptArchitectureCode = rAGPromptArchitectureCode;
        }

        public override List<IChatMessage>? GetAdditionalChatMessage()
        {
            var result = new List<IChatMessage>
            {
                ChatMessageFactory.System(RAGPromptsGeneral.FirstSystemPrompt),
                ChatMessageFactory.System(RAGPromptsGeneral.ColorPromptSystem),
                ChatMessageFactory.System(RAGPromptArchitectureCode.Role),
                ChatMessageFactory.System(RAGPromptArchitectureCode.WzorceProjektowe),
                ChatMessageFactory.System(RAGPromptArchitectureCode.Zasady)
            };


            return result;
        }
    }
}
