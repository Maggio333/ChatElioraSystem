using ChatElioraSystem.Core.Application_.Common.Factories;
using ChatElioraSystem.Core.Domain.Resources;
using ChatElioraSystem.Core.Infrastructure.Models;
using ChatElioraSystem.Core.Infrastructure.Services;

namespace ChatElioraSystem.Core.Domain.Services.Bases
{
    public abstract class BasePromptService : IBasePromptService
    {
        public ILlmService _llmService;
        public IRAGPromptsGeneral RAGPromptsGeneral;

        public BasePromptService(ILlmService llmService, IRAGPromptsGeneral rAGPromptsGeneral)
        {
            _llmService = llmService;
            RAGPromptsGeneral = rAGPromptsGeneral;
        }

        public abstract List<IChatMessage>? GetAdditionalChatMessage();
        public virtual List<IChatMessage>? GetGlobalChatMessages()
        {
            var result = new List<IChatMessage>
            {
                ChatMessageFactory.System(RAGPromptsGeneral.FirstSystemPrompt),
                ChatMessageFactory.System(RAGPromptsGeneral.ColorPromptSystem),
                ChatMessageFactory.System(RAGPromptsGeneral.UserAdminPrompt)
            };

            return result;
        }

        //public virtual async IAsyncEnumerable<string> GetStreamAsync(IEnumerable<IChatMessage> chatMessage, LLMNamesEnum lLMName, int llmNo)
        //{
        //    await foreach (var chunk in GetStreamAsync(chatMessage, lLMName, llmNo, new CancellationToken()))
        //    {
        //        yield return chunk;
        //    }         
        //}
        public virtual async IAsyncEnumerable<string> GetStreamAsync(IEnumerable<IChatMessage> chatMessage, LLMNamesEnum lLMName, int llmNo, List<string> cancelationWords, CancellationToken cancellationToken)
        {
            var globalChatMessages = GetGlobalChatMessages();
            var additonalChatMessages = GetAdditionalChatMessage();

            List<IChatMessage> toAdd = new List<IChatMessage>();


            if (globalChatMessages != null)
            {
                toAdd.AddRange(globalChatMessages);
            }

            if (additonalChatMessages != null)
            {
                toAdd.AddRange(additonalChatMessages);
            }

            toAdd.AddRange(chatMessage);

            chatMessage = toAdd; //kiedyś to będzie mniej prymitywnie, pewnie serwis



            string test = string.Empty;

            string stringLlmName = string.Empty;
            switch (lLMName)
            {
                case LLMNamesEnum.Qwen1_7B:
                    stringLlmName = LLMNames.Qwen1_7B;
                    break;
                case LLMNamesEnum.Pllum_8b:
                    stringLlmName = LLMNames.Pllum_8b;
                    break;
                case LLMNamesEnum.Custom:
                    stringLlmName = LLMNames.Custom;
                    break;
            }

            //yield return test;

            string newWord = string.Empty;
            //var cts = new CancellationTokenSource();

            await foreach (var chunk in _llmService.StreamCompletionAsync(
                chatMessages: chatMessage,
                llmName: stringLlmName, llmNo: llmNo, ct: cancellationToken))
            {
                newWord += chunk;

                if (cancelationWords != null)
                {
                    foreach (var word in cancelationWords)
                    {
                        if (newWord.Contains(word))
                        {
                            yield break;
                        }
                    }
                }

                yield return chunk;
            }
        }

        public virtual async IAsyncEnumerable<string> GetStreamAsync(IEnumerable<IChatMessage> chatMessage, LLMNamesEnum lLMName, int llmNo, CancellationToken cancellationToken) 
        {
            await foreach (var chunk in GetStreamAsync(chatMessage, lLMName, llmNo, null, cancellationToken))
            {
                yield return chunk;
            }
        }

        public virtual async Task<string> GetCompletionAsync(IEnumerable<IChatMessage> chatMessage, LLMNamesEnum lLMName, CancellationToken cancellationToken) => await GetCompletionAsync(chatMessage, lLMName, 1, cancellationToken);
        public virtual async Task<string> GetCompletionAsync(IEnumerable<IChatMessage> chatMessage, LLMNamesEnum lLMName, int llmNo, CancellationToken cancellationToken)
        {
            var additonalChatMessages = GetAdditionalChatMessage();

            if (additonalChatMessages != null)
            {
                var newChatMessageList = additonalChatMessages;
                newChatMessageList.AddRange(chatMessage);
                chatMessage = newChatMessageList;
            }

            string stringLlmName = string.Empty;
            switch (lLMName)
            {
                case LLMNamesEnum.Qwen1_7B:
                    stringLlmName = LLMNames.Qwen1_7B;
                    break;
                case LLMNamesEnum.Pllum_8b:
                    stringLlmName = LLMNames.Pllum_8b;
                    break;
                case LLMNamesEnum.Custom:
                    stringLlmName = LLMNames.Custom;
                    break; 
            }

            if(string.IsNullOrEmpty(stringLlmName))
            {
                return "Brak podanej nazwy LLM w LLMNamesEnum";
            }



            return await _llmService.GetCompletionAsync(chatMessage, stringLlmName, new CancellationToken(), llmNo);
        }
    }
}
