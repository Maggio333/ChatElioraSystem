using ChatElioraSystem.Core.Application_.Common.Factories;
using ChatElioraSystem.Core.Domain.Resources;
using ChatElioraSystem.Core.Domain.Services.Bases;
using ChatElioraSystem.Core.Infrastructure.Models;
using ChatElioraSystem.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Domain.Services
{
    public class PromptGeneralService : BasePromptService, IPromptGeneralService
    {
        public PromptGeneralService(ILlmService llmService, IRAGPromptsGeneral _rAGPromptsGeneral) : base(llmService, _rAGPromptsGeneral)
        {
        }

        public override List<IChatMessage>? GetAdditionalChatMessage()
        {
            var result = new List<IChatMessage>
            {
                ChatMessageFactory.System(RAGPromptsGeneral.Role)                
            };

            return result;
        }


    }
}

