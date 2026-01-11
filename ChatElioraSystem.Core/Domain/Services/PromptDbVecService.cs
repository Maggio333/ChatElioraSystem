using ChatElioraSystem.Core.Application_.Common.Factories;
using ChatElioraSystem.Core.Application_.Models;
using ChatElioraSystem.Core.Domain.Models;
using ChatElioraSystem.Core.Domain.Resources;
using ChatElioraSystem.Core.Domain.Services.Bases;
using ChatElioraSystem.Core.Infrastructure.Models;
using ChatElioraSystem.Core.Infrastructure.Services;
using ChatElioraSystem.Core.Infrastructure.VectorDataBase.Services;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ChatElioraSystem.Core.Domain.Services
{
    public class PromptDbVecService : BasePromptService, IPromptDbVecService
    {
        public IRAGPromptsDbVec RAGPromptsDbVec;
        public IVectorDbHelper VectorDbHelper;

        public ActionMode ActionMode { get; set; }
        public PromptDbVecService(ILlmService llmService, IRAGPromptsGeneral rAGPromptsGeneral, IRAGPromptsDbVec rAGPromptsDbVec, IVectorDbHelper vectorDbTester) : base(llmService, rAGPromptsGeneral)
        {
            RAGPromptsDbVec = rAGPromptsDbVec;
            VectorDbHelper = vectorDbTester;
        }

        public async Task<string> GetBaseIdioms() => await VectorDbHelper.GetBaseIdioms(10);



        private string GetAkcjaTypeValue(MpcAkcja? akcja)
        {
            if(akcja != null && akcja.Akcja != null && akcja.Akcja.Typ != null)
            {
                return akcja.Akcja.Typ.ToLower();
            }

            return string.Empty;
        }

        private List<MpcAkcja> GetMpcAkcjaListWithTyp(List<MpcAkcja> listaMpcAkcja, ActionMode actionMode)
        {
            foreach (var row in listaMpcAkcja)
            {
                row.Akcja.Typ = Enum.GetName(actionMode);
            }

            return listaMpcAkcja;
        }


        public async IAsyncEnumerable<RAGChunk> GetStreamHistoryFromDb(IEnumerable<IChatMessage> chatMessage, int llmNo, CancellationToken cancellationToken)
        {
            await foreach (var chunk in GetStreamHistoryFromDb(chatMessage, llmNo, null, cancellationToken))
            {
                yield return chunk;
            }
        }
        public async IAsyncEnumerable<RAGChunk> GetStreamHistoryFromDb(IEnumerable<IChatMessage> chatMessage, int llmNo, List<string>? cancelationWords, CancellationToken cancellationToken)
        {
            List<IChatMessage> result = new List<IChatMessage>();

            string memory = string.Empty;

            var match = Regex.Match(memory, @"```json(.*?)```", RegexOptions.Singleline);
            var toolOutput = new List<IChatMessage>();


            await foreach (var chunk in GetStreamAsync(chatMessage, LLMNamesEnum.Custom, llmNo, cancelationWords, cancellationToken))
            {
                memory += chunk;
                match = Regex.Match(memory, @"```json(.*?)```", RegexOptions.Singleline);

                toolOutput.Add(new ChatMessage
                {
                    Role = Role.tool,
                    Content = chunk,
                    Timestamp = DateTime.Now
                });


                if (memory.Length >= 20 && !memory.Contains("```json"))
                {
                    break;    
                }

                yield return new RAGChunk
                {
                    TextChunk = chunk,
                    ChatMessages = null 
                };

                if (memory != string.Empty && match.Success)
                {
                    break;
                }
            }

            MpcAkcja mpcAkcja = new MpcAkcja();


            string value = match.Groups[1].Value.Trim();

            if (memory == null || memory == string.Empty)
            {
                yield break;
            }

            if (value != string.Empty)
            {
                try
                {
                    mpcAkcja = JsonSerializer.Deserialize<MpcAkcja>(value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new MpcAkcja();
                }
                catch (Exception ex)
                {
                    mpcAkcja = new MpcAkcja();
                }
            }

            string chatMessageResult = string.Empty;

            string akcjaTypeValue = GetAkcjaTypeValue(mpcAkcja);
            if (akcjaTypeValue == "odczyt")
            {
                ActionMode = ActionMode.Odczyt;
                var test = await VectorDbHelper.GetValueFromVDB(mpcAkcja.Akcja.Payload);
                if (test != null)
                {
                    List<(string Id, float Score, JsonElement Payload)> memoryList = test.Where(x => x.Score > 0.85).ToList();

                    List<MpcAkcja> listaMpcAkcja = MpcAkcja.MapResultsToAkcje(memoryList);

                    listaMpcAkcja = GetMpcAkcjaListWithTyp(listaMpcAkcja, ActionMode.Odczyt);

                    string test5 = MpcAkcja.FormatAsContext(listaMpcAkcja, mpcAkcja.Akcja.Payload);
                    result.Add(ChatMessageFactory.System(test5));


                    toolOutput.Add(new ChatMessage
                    {
                        Role = Role.tool,
                        Content = test5,
                        Timestamp = DateTime.Now
                    });

                    yield return new RAGChunk
                    {
                        TextChunk = null,
                        ChatMessages = toolOutput.TakeLast(1).ToList()
                    };
                }
            }

            if (mpcAkcja != null && mpcAkcja.Akcja != null && mpcAkcja.Akcja.Typ != null && mpcAkcja.Akcja.Typ.ToLower() == "zapis")
            {
                ActionMode = ActionMode.Zapis;
                List<MpcAkcja> listToAdd = new List<MpcAkcja>();

                mpcAkcja.Akcja.Metadata["Timestamp"] = DateTime.Now;

                string test12 = JsonSerializer.Serialize(mpcAkcja);

                var resultJson = await VectorDbHelper.InsertIfNotDuplicateAsync(mpcAkcja.Akcja.Payload, test12);

                if (resultJson != null)
                {
                    mpcAkcja = resultJson.Deserialize<MpcAkcja>() ?? mpcAkcja;
                }

                listToAdd.Add(mpcAkcja);


                chatMessageResult = MpcAkcja.FormatAsContext(listToAdd, string.Empty);
                result.Add(ChatMessageFactory.System(chatMessageResult));


                toolOutput.Add(new ChatMessage
                {
                    Role = Role.tool,
                    Content = chatMessageResult,
                    Timestamp = DateTime.Now
                });

                yield return new RAGChunk
                {
                    TextChunk = null,
                    ChatMessages = toolOutput.TakeLast(1).ToList()
                };

            }
        }

        public override List<IChatMessage>? GetAdditionalChatMessage()
        {
            var result = new List<IChatMessage>
            {
                ChatMessageFactory.System(RAGPromptsGeneral.UserAdminPrompt),
                //ChatMessageFactory.System(RAGPromptsGeneral.FirstSystemPrompt),
                ChatMessageFactory.System(RAGPromptsDbVec.Role),


            };


            //result.Add(ChatMessageFactory.System(RAGPromptsDbVec.DbVectorPrompt));

            switch (ActionMode)
            {
                case ActionMode.Zapis:
                    result.Add(ChatMessageFactory.System(RAGPromptsDbVec.SaveToDbVectorPrompt));

                    break;
                case ActionMode.Odczyt:
                    result.Add(ChatMessageFactory.System(RAGPromptsDbVec.ReadDbVectorPrompt));

                    break;
            }

            return result;
        }

        public override async IAsyncEnumerable<string> GetStreamAsync(IEnumerable<IChatMessage> chatMessage, LLMNamesEnum lLMName, int llmNo, List<string>? cancelationWords, CancellationToken cancellationToken)
        {
            List<IChatMessage>? additonalChatMessages = GetAdditionalChatMessage();
            List<IChatMessage> toAdd = new List<IChatMessage>();


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

            yield return test;

            string memory = string.Empty;

            await foreach (var chunk in _llmService.StreamCompletionAsync(
                chatMessages: chatMessage,
                llmName: stringLlmName, llmNo: llmNo, ct: cancellationToken))
            {
                memory += chunk;

                if (cancelationWords != null)
                {
                    foreach (var word in cancelationWords)
                    {
                        if (memory.Contains(word))
                        {
                            yield break;     
                        }
                    }
                }

                yield return chunk;
            }
        }

    }
}
