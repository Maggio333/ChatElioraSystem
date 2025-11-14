using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatElioraSystem.Core.Application_.Common.Factories;
using ChatElioraSystem.Core.Domain.Resources;
using ChatElioraSystem.Core.Domain.Services.Bases;
using ChatElioraSystem.Core.Infrastructure.Models;
using ChatElioraSystem.Core.Infrastructure.Services;
using static System.Net.Mime.MediaTypeNames;

namespace ChatElioraSystem.Core.Domain.Services
{
    public class PromptReflectionService : BasePromptService, IPromptReflectionService
    {
        public IRAGPromptReflection RAGPromptReflection;

        public PromptReflectionService(ILlmService llmService, IRAGPromptsGeneral rAGPromptsGeneral, IRAGPromptReflection rAGPromptReflection) : base(llmService, rAGPromptsGeneral)
        {
            RAGPromptReflection = rAGPromptReflection;
        }

        public override List<IChatMessage>? GetAdditionalChatMessage()
        {
            var result = new List<IChatMessage>
            {
                ChatMessageFactory.System(RAGPromptReflection.Role),
                ChatMessageFactory.System(RAGPromptReflection.Theme),
                ChatMessageFactory.System(RAGPromptReflection.Emotional)
            };



            return result;
        }

        
    }
}
