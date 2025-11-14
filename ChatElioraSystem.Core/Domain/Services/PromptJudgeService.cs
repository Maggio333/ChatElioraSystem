using ChatElioraSystem.Core.Application_.Common.Factories;
using ChatElioraSystem.Core.Domain.Resources;
using ChatElioraSystem.Core.Domain.Services.Bases;
using ChatElioraSystem.Core.Infrastructure.Models;
using ChatElioraSystem.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Domain.Services
{
    public class PromptJudgeService : BasePromptService, IPromptJudgeService
    {
        private IRAGPromptJudge rAGPromptJudge;
        public PromptJudgeService(ILlmService llmService, IRAGPromptsGeneral rAGPromptsGeneral, IRAGPromptJudge _rAGPromptJudge) : base(llmService, rAGPromptsGeneral)
        {
            rAGPromptJudge = _rAGPromptJudge;
        }

        public override List<IChatMessage>? GetAdditionalChatMessage()
        {
            var result = new List<IChatMessage>
            {
                ChatMessageFactory.System(rAGPromptJudge.Role),
                ChatMessageFactory.System(rAGPromptJudge.GetTheme()),
                ChatMessageFactory.System(rAGPromptJudge.Description)
            };

            return result;
        }

        public async IAsyncEnumerable<string> GetStreamAsyncJudge(List<IChatMessage> chatMessage, int llmNo, List<string> cancelationWords, CancellationToken cancellationToken)
        {
            var additonalChatMessages = GetAdditionalChatMessage();

            if (additonalChatMessages != null)
            {
                var newChatMessageList = additonalChatMessages;
                newChatMessageList.AddRange(chatMessage);
                chatMessage = newChatMessageList;
            }

            chatMessage.Add(ChatMessageFactory.System("Masz podać tylko samą kategorię"));


            string test = string.Empty;


            yield return test;

            string newWord = string.Empty;
            //var cts = new CancellationTokenSource();

            await foreach (var chunk in _llmService.StreamCompletionAsync(chatMessage, LLMNames.Custom, llmNo : 1, ct : cancellationToken))
            {
                newWord += chunk;

                if (cancelationWords != null)
                {
                    foreach (var cancelationWord in cancelationWords)
                    {

                        if (newWord.Length >= 30 && !newWord.Contains(cancelationWord))
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        if (newWord.Contains(cancelationWord))
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                    }
                }

                yield return chunk;
            }
        }
    }
}
