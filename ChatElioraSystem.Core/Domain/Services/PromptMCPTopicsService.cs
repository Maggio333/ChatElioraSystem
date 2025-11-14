using ChatElioraSystem.Core.Application_.Common.Factories;
using ChatElioraSystem.Core.Domain.Models;
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
    public class PromptMCPTopicsService : BasePromptService, IPromptMCPTopicsService
    {
        private IRAGPromptMCPTopics _rAGPromptMCPTopics;
        public ActionMode ActionMode { get; set; } = ActionMode.Odczyt;

        public PromptMCPTopicsService(ILlmService llmService, IRAGPromptsGeneral rAGPromptsGeneral, IRAGPromptMCPTopics rAGPromptMCPTopics) : base(llmService, rAGPromptsGeneral)
        {
            _rAGPromptMCPTopics = rAGPromptMCPTopics;
        }


        public override List<IChatMessage>? GetGlobalChatMessages() => null;

        public override List<IChatMessage>? GetAdditionalChatMessage()
        {
            var result = new List<IChatMessage>
            {
                ChatMessageFactory.System(_rAGPromptMCPTopics.Role)
            };


            //result.Add(ChatMessageFactory.System(RAGPromptsDbVec.DbVectorPrompt));

            switch (ActionMode)
            {
                case ActionMode.Zapis:
                    result.Add(ChatMessageFactory.System(_rAGPromptMCPTopics.SaveToDbVectorPrompt));

                    break;
                case ActionMode.Odczyt:
                    result.Add(ChatMessageFactory.System(_rAGPromptMCPTopics.ReadDbVectorPrompt));

                    break;
            }

            return result;
        }
    }
}
