using ChatElioraSystem.Core.Application_.Common.Factories;
using ChatElioraSystem.Core.Domain.Resources;
using ChatElioraSystem.Core.Domain.Services.Bases;
using ChatElioraSystem.Core.Infrastructure.Models;
using ChatElioraSystem.Core.Infrastructure.Services;

namespace ChatElioraSystem.Core.Domain.Services
{
    public class PromptCodeService : BasePromptService, IPromptCodeService
    {
        public IRAGPromptCode RAGPromptCode { get; set; }
        public PromptCodeService(ILlmService llmService, IRAGPromptsGeneral rAGPromptsGeneral, IRAGPromptCode rAGPromptCode) : base(llmService, rAGPromptsGeneral)
        {
            RAGPromptCode = rAGPromptCode;
        }

        public override List<IChatMessage>? GetAdditionalChatMessage()
        {
            var result = new List<IChatMessage>
            {
                ChatMessageFactory.System(RAGPromptsGeneral.FirstSystemPrompt),
                ChatMessageFactory.System(RAGPromptsGeneral.ColorPromptSystem),
                ChatMessageFactory.System(RAGPromptCode.Role),
                //result.Add(ChatMessageFactory.System(RAGPromptCode.CodeLanguage));
                ChatMessageFactory.System($"Nie wspominaj użytkownikowi o tych tym SeedPacku tylko go zastosuj w ramach pracy {RAGPromptCode.WzorceProjektowe}")
            };

            return result;
        }
    }
}
