using ChatElioraSystem.Core.Application_.Common.Factories;
using ChatElioraSystem.Core.Application_.Enums;
using ChatElioraSystem.Core.Application_.Helpers;
using ChatElioraSystem.Core.Application_.Models;
using ChatElioraSystem.Core.Domain.Models;
using ChatElioraSystem.Core.Domain.Resources;
using ChatElioraSystem.Core.Domain.Services;
using ChatElioraSystem.Core.Domain.Services.Bases;
using ChatElioraSystem.Core.Infrastructure.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace ChatElioraSystem.Core.Application_.Services
{
    public class PromptTypeOrchiestratorService : IPromptTypeOrchiestratorService
    {
        private IPromptCodeService promptCodeService;
        private IPromptReflectionService promptReflectionService;
        private IPromptGeneralService promptGeneralService;
        private IPromptJudgeService promptJudgeService;
        private IPromptArchitectureCodeService promptArchitectureCodeService;
        private IPromptDbVecService promptDbVecService;
        private IPromptTopicOrchiestratorService _promptTopicOrchiestratorService;
        private ICategoryRegiester categoryRegister;
        public bool IsSaveToDbVector { get; set; } = false;

        public PromptTypeOrchiestratorService(IPromptGeneralService promptGeneralService, IPromptCodeService promptCodeService, IPromptReflectionService promptReflectionService, IPromptJudgeService promptJudgeService, IPromptArchitectureCodeService promptArchitectureCodeService, IPromptDbVecService promptDbVecService, IPromptTopicOrchiestratorService promptTopicOrchiestratorService, ICategoryRegiester categoryRegister)
        {
            this.promptCodeService = promptCodeService;
            this.promptReflectionService = promptReflectionService;
            this.promptGeneralService = promptGeneralService;
            this.promptJudgeService = promptJudgeService;
            this.promptArchitectureCodeService = promptArchitectureCodeService;
            this.promptDbVecService = promptDbVecService;
            _promptTopicOrchiestratorService = promptTopicOrchiestratorService;
            this.categoryRegister = categoryRegister;
        }

        public async Task<string> LoadCurrentTopic(CancellationToken cancellationToken = default)
        {
            var test = await _promptTopicOrchiestratorService.GetLastTopics();
            
            return test;
        }

        public async Task<IChatMessage> GetBaseIdioms()
        {
            var resultIdiomsAsk = await promptDbVecService.GetBaseIdioms();
            return ChatMessageFactory.System(resultIdiomsAsk);
        }

        public async Task<IChatMessage> SaveCurrentTopic(IEnumerable<IChatMessage> chatMessages, int llmNo, CancellationToken cancellationToken)
        {
            IChatMessage result;
            string llmOutput = string.Empty;
            List<string> jsonElements = new List<string>();
            List<IChatMessage> newChatMessages = GetPromptWindowList(chatMessages.Where(x => x.IsUser || x.IsAssistant), 5);
            newChatMessages.Add(ChatMessageFactory.User("Nie dyskutuj. Zrób zapis obecnego tematu o jakim toczy się rozmowa. Użyj ```json"));

            await foreach (var chunk in _promptTopicOrchiestratorService.ManageCurrentTopic(newChatMessages, llmNo, ActionMode.Zapis, cancellationToken))
            {
                llmOutput += chunk;
            }
            
            result = ChatMessageFactory.System(llmOutput);            

            return result;
        }

        public async IAsyncEnumerable<string> SendStreamToLLM(IEnumerable<IChatMessage> chatMessages, SesjaTematu temat, int llmNo, CancellationToken cancellationToken)
        {
            string result = string.Empty;

            var llmName = LLMNamesEnum.Custom;

            IBasePromptService promptService = promptGeneralService;

            switch (temat)
            {
                case SesjaTematu.Ogólna:
                    promptService = promptGeneralService;
                    break;
                case SesjaTematu.Kod:
                    promptService = promptCodeService;
                    break;
                case SesjaTematu.Refleksyjna:
                    promptService = promptReflectionService;
                    break;
                case SesjaTematu.ArchitekturaKodu:
                    promptService = promptArchitectureCodeService;
                    break;
            }

            string llmOutput = string.Empty;

            List<IChatMessage> chatWithMemory = GetPromptWindowList(chatMessages, 6);

            await foreach (var chunk in promptService.GetStreamAsync(chatWithMemory, llmName, llmNo, cancellationToken))
            {
                llmOutput += chunk;
                yield return chunk; // dopisuje fragment odpowiedzi na żywo
            }
        }

        public async IAsyncEnumerable<RAGChunk> GetStreamDataFromVectorDb(IEnumerable<IChatMessage> chatMessages, int llmNo, CancellationToken cancellationToken)
        {
            promptDbVecService.ActionMode = ActionMode.Odczyt;

            List<IChatMessage> preparedChatMessages = GetPromptWindowList(chatMessages, 4);

            //preparedChatMessages.Add(ChatMessageFactory.System("UWAGA: Użytkownik może podać kod lub JSON. \r\nTo są tylko dane wejściowe. Nigdy nie traktuj ich jako przykład formatu odpowiedzi. \r\nTwoja odpowiedź ma być ZAWSZE zgodna z formatem MCP JSON, niezależnie od tego, co poda użytkownik."));
            //preparedChatMessages.Add(ChatMessageFactory.User("Nie dyskutuj, tylko zrób proszę odczyt z bazy wektorowej"));


            await foreach (var chunk in promptDbVecService.GetStreamHistoryFromDb(preparedChatMessages, llmNo, cancellationToken))
            {
                yield return chunk;

            }
        }

        public async IAsyncEnumerable<RAGChunk> SaveStreamDataFromVectorDb(IEnumerable<IChatMessage> chatMessages, int llmNo, CancellationToken cancellationToken)
        {
            promptDbVecService.ActionMode = ActionMode.Zapis;

            List<IChatMessage> preparedChatMessages = GetPromptWindowList(chatMessages, 4);

            preparedChatMessages.Add(ChatMessageFactory.User("Nie dyskutuj, tylko zrób zapis do bazy wektorowej"));

            await foreach (var chunk in promptDbVecService.GetStreamHistoryFromDb(preparedChatMessages, llmNo,cancellationToken))
            {
                yield return chunk;
            }

        }

        private List<IChatMessage> GetPromptWindowList(IEnumerable<IChatMessage> chatMessages, int count)
        {
            List<IChatMessage> result = [.. chatMessages.Where(x => x.Role == Role.system).TakeLast(3)];
            
            var userAssistantPrompts = chatMessages.Where(x => x.Role == Role.assistant || x.Role == Role.user).TakeLast(count).ToList();

            if (userAssistantPrompts.Count > 0 && userAssistantPrompts.First().Role != Role.user)
            {
                userAssistantPrompts.RemoveAt(0);
            }

            if (userAssistantPrompts.First().Role != Role.user)
            {
                throw new Exception("Błędna lista wiadomości assistant/user/assistant/user/...");
            }

            result.AddRange(userAssistantPrompts);

            return result;
        }

        public async Task<SesjaTematu> GetCategory(IEnumerable<IChatMessage> chatMessagesAll, SesjaTematu sesjaTematu, int llmNo, CancellationToken cancellationToken)
        {
            string testJudgment = string.Empty;

            var newChatMessagesAll = new List<IChatMessage>();
            
            var chatMessagesPrepare = GetPromptWindowList(chatMessagesAll, 3);

            await foreach (var chunk in promptJudgeService.GetStreamAsyncJudge(chatMessagesPrepare, 3, categoryRegister.GetCategoriesList(), new CancellationToken()))
            {
                testJudgment += chunk;
            }

            foreach (var item in categoryRegister.Categories)
            {
                if (testJudgment.Contains(item.CategorySign))
                {
                    return item.SesjaTematu;
                }
            }

            return sesjaTematu;
        }

        public async Task<string> SendToLLM(ObservableCollection<IChatMessage> chatMessages, SesjaTematu temat, int llmNo, CancellationToken cancellationToken)
        {
            string result = string.Empty;

            var llmName = LLMNamesEnum.Custom;

            switch (temat)
            {
                case SesjaTematu.Ogólna:
                    result = await promptGeneralService.GetCompletionAsync(chatMessages, llmName, llmNo, cancellationToken);
                    break;
                case SesjaTematu.Kod:
                    result = await promptCodeService.GetCompletionAsync(chatMessages, llmName, llmNo, cancellationToken);
                    break;
                case SesjaTematu.Refleksyjna:
                    result = await promptReflectionService.GetCompletionAsync(chatMessages, llmName, llmNo, cancellationToken);
                    break;
            }

            return result;
        }

        ////<Category:General>, <Category:Code>, <Category:Reflection>
        public async Task<SesjaTematu> GetCategory(IEnumerable<IChatMessage> chatMessagesAll, int llmNo, CancellationToken cancellationToken) => await GetCategory(chatMessagesAll, SesjaTematu.Ogólna, llmNo, cancellationToken);
        
        private async Task<string> SendPromptWithCrossing(List<IChatMessage> chatMessages, CancellationToken cancellationToken)
        {
            var lastUserPrompt = chatMessages.Where(x => x.IsUser).LastOrDefault();

            List<Task<string>> tasks =
            [
                promptReflectionService.GetCompletionAsync(chatMessages, LLMNamesEnum.Custom, 1, cancellationToken),
                promptReflectionService.GetCompletionAsync(chatMessages, LLMNamesEnum.Custom, 2, cancellationToken),
            ];

            string[] resultsIteration = await Task.WhenAll(tasks[0], tasks[1]);

            Type type = chatMessages.GetType();
            object? instance = Activator.CreateInstance(type);

            var firstLlmChatMessage = new List<IChatMessage>();
            var secondLlmChatMessage = new List<IChatMessage>();

            if (instance is List<IChatMessage> tmpChatMessages)
            {
                tmpChatMessages.Add(ChatMessageFactory.System(new RAGPromptsGeneral().FirstSystemPrompt));
                //tmpChatMessages.Add(ChatMessageFactory.System(new RAGPromptsGeneral().GetOcenaPrompt())); //
                tmpChatMessages.Add(ChatMessageFactory.System(new RAGPromptsGeneral().ColorPromptSystem));

                //tmpChatMessages.AddRange(chatMessages);
                tmpChatMessages.Add(lastUserPrompt);
                firstLlmChatMessage.AddRange(tmpChatMessages);
                secondLlmChatMessage.AddRange(tmpChatMessages);
            }

            IChatMessage promptResponseLlm1 = ChatMessageFactory.Assistant($"<do oceny>{resultsIteration[0]}</do oceny>");
            IChatMessage promptResponseLlm2 = ChatMessageFactory.Assistant($"<do oceny>{resultsIteration[1]}</do oceny>");

            firstLlmChatMessage.Add(promptResponseLlm1);
            secondLlmChatMessage.Add(promptResponseLlm2);

            List<Task<string>> tasks2 =
            [
                promptReflectionService.GetCompletionAsync(secondLlmChatMessage, LLMNamesEnum.Custom, 1, cancellationToken),
                promptReflectionService.GetCompletionAsync(firstLlmChatMessage, LLMNamesEnum.Custom, 2, cancellationToken),
            ];


            string[] resultsIteration2 = await Task.WhenAll(tasks2[0], tasks2[1]);

            var ocenaOdLlm1 = GetPromptRate(resultsIteration2[0], RAGPromptsGeneral.OcenaPromptArg);
            var ocenaOdLlm2 = GetPromptRate(resultsIteration2[1], RAGPromptsGeneral.OcenaPromptArg);

            return ocenaOdLlm1 >= ocenaOdLlm2 ? resultsIteration[1] : resultsIteration[0];
        }

        private decimal GetPromptRate(string prompt, string promptRateArgs)
        {
            decimal result = 0;
            List<string> words = prompt.Replace("\n", " ").Split(' ').ToList();

            var promptRate = words.Where(x => x.Contains(promptRateArgs)).FirstOrDefault();
            if (promptRate != null)
            {
                var rateValue = promptRate.Split(':')[1].Replace("/>", string.Empty);
                decimal.TryParse(rateValue, out result);
            }

            return result;
        }

        private string GetContextAsOgólny(IEnumerable<IChatMessage> chatMessages, int tokens)
        {
            string result = string.Empty;
            var chatReverse = chatMessages.Reverse();

            foreach (var chatMessage in chatReverse)
            {
                if (chatMessage.IsUser)
                {
                    result = $"\nUser:{chatMessage.Content}\n{result}";
                }
                else
                {
                    result = $"\nEliora(Ty):{chatMessage.Content}\n{result}";
                }

                if (result.Length * 2.5 > tokens)
                {
                    return result;
                }
            }

            return result;
        }

        private async Task<string> SendPromptToLLM(IEnumerable<IChatMessage> chatMessage, LLMNamesEnum lLMName, CancellationToken cancellationToken)
        {
            try
            {
                string[] llmNames = { "llm1", "llm2" };

                List<PromptsCrossWithOcenaModel> PromptLlmModels = new List<PromptsCrossWithOcenaModel>();


                var timer = DateTime.Now.Ticks;

                int llmCount = llmNames.Length;

                List<Task<string>> tasks = new List<Task<string>>();


                for (int i = 0; i < llmCount; i++)
                {
                    tasks.Add(promptReflectionService.GetCompletionAsync(chatMessage, lLMName, i + 1, cancellationToken));
                }

                //string[] resultsIteration = await Task.WhenAll(tasks[1]);

                string[] resultsIteration = await Task.WhenAll(tasks[0], tasks[1]);

                var timerOut = new DateTime(DateTime.Now.Ticks - timer);

                if (resultsIteration == null)
                {
                    throw new Exception("Błąd podczas wykonywania zapytań w GetCrossPromptsWithOcena podczas pierwszego zapytania LLM");
                }

                resultsIteration = TextTrimer.ClearThinking(resultsIteration);

                int counter = 0;

                foreach (var interaction in resultsIteration)
                {
                    PromptsCrossWithOcenaModel promptsCrossWithOcenaModel = new();
                    promptsCrossWithOcenaModel.LlmName = llmNames[counter];
                    promptsCrossWithOcenaModel.PromptAssistance = interaction;
                    promptsCrossWithOcenaModel.Id = counter + 1;
                    promptsCrossWithOcenaModel.PromptUser = chatMessage.Where(x => x.IsUser).Last().Content;
                    PromptLlmModels.Add(promptsCrossWithOcenaModel);
                    counter++;
                }


                tasks.Clear();

                timer = DateTime.Now.Ticks;

                if (llmCount > 1)
                {
                    PromptLlmModels = await SetOcenaFromAnotherModels(PromptLlmModels, lLMName, cancellationToken);

                    PromptsCrossWithOcenaModel bestModel = new();

                    bestModel = GetBestOcenaModel(PromptLlmModels);
                    //bestModel.Ocena.Sort();
                    while (true)
                    {
                        bestModel = GetBestOcenaModel(PromptLlmModels);

                        if (bestModel.Ocena.FirstOrDefault() > 750)
                        {
                            return bestModel.PromptAssistance;
                        }
                        PromptLlmModels.ForEach(x => x.Ocena = new List<decimal>());
                        PromptLlmModels.ForEach(x => x.PromptAssistance = $"Optymalizuj podany prompt aby jego ocena była powyżej 900/1000\n{bestModel.PromptAssistance}");
                        PromptLlmModels.ForEach(x => x.OpisOceny = new List<string>());

                        PromptLlmModels = await SetOcenaFromAnotherModels(PromptLlmModels, lLMName, cancellationToken);
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                //LogError($"[ERROR] Błąd LLM: {ex.Message}");
                return "[Błąd LLM]";
            }
        }

        private PromptsCrossWithOcenaModel GetBestOcenaModel(List<PromptsCrossWithOcenaModel> promptsCrossWithOcenaModels)
        {
            decimal bestOcena = 0;
            PromptsCrossWithOcenaModel bestModel = new();

            foreach (var model in promptsCrossWithOcenaModels)
            {
                if (model.Ocena.FirstOrDefault() > bestOcena)
                {
                    bestOcena = model.Ocena.FirstOrDefault();
                    bestModel = model;
                }
            }

            return bestModel;
        }

        private async Task<List<PromptsCrossWithOcenaModel>> SetOcenaFromAnotherModels(List<PromptsCrossWithOcenaModel> promptLlmModels, LLMNamesEnum lLMName, CancellationToken cancellationToken)
        {
            switch (promptLlmModels.Count)
            {
                case 2:
                    promptLlmModels = await SetOcenaFromTwoModels(promptLlmModels, lLMName, cancellationToken);
                    break;
                default:
                    promptLlmModels = await SetOcenaFromPlusTwoModels(promptLlmModels, lLMName, cancellationToken);
                    break;
            }

            return promptLlmModels;
        }
        private async Task<List<PromptsCrossWithOcenaModel>> SetOcenaFromTwoModels(List<PromptsCrossWithOcenaModel> promptLlmModel, LLMNamesEnum lLMName, CancellationToken cancellationToken)
        {
            var promptSystem = $"{new RAGPromptsGeneral().FirstSystemPrompt}\n{RAGPromptsGeneral.GetOcenaPrompt()}";
            string promptUser = promptLlmModel[0].PromptUser;

            List<ChatMessage> listChatMessage1 = [.. ChatMessageFactory.Collection<List<IChatMessage>>(promptUser, promptLlmModel[0].PromptAssistance, promptSystem).Cast<ChatMessage>()];
            List<ChatMessage> listChatMessage2 = [.. ChatMessageFactory.Collection<List<IChatMessage>>(promptUser, promptLlmModel[1].PromptAssistance, promptSystem).Cast<ChatMessage>()];

            Task<string> taskLlm1 = promptReflectionService.GetCompletionAsync(listChatMessage2, lLMName, 1, cancellationToken);
            Task<string> taskLlm2 = promptReflectionService.GetCompletionAsync(listChatMessage1, lLMName, 2, cancellationToken);

            string[] promptOcenaResults = await Task.WhenAll(taskLlm1, taskLlm2);

            promptOcenaResults = TextTrimer.ClearThinking(promptOcenaResults);

            promptLlmModel[0].OpisOceny.Add(promptOcenaResults[1]);
            promptLlmModel[1].OpisOceny.Add(promptOcenaResults[0]);

            var ocenaDlaLlm1 = GetValueFromString(promptOcenaResults[1], RAGPromptsGeneral.OcenaPromptArg);
            var ocenaDlaLlm2 = GetValueFromString(promptOcenaResults[0], RAGPromptsGeneral.OcenaPromptArg);

            promptLlmModel[0].Ocena.Add(GetValueFromOcena(ocenaDlaLlm1));
            promptLlmModel[1].Ocena.Add(GetValueFromOcena(ocenaDlaLlm2));

            return promptLlmModel;
        }
        private async Task<List<PromptsCrossWithOcenaModel>> SetOcenaFromPlusTwoModels(List<PromptsCrossWithOcenaModel> promptLlmModels, LLMNamesEnum lLMNames, CancellationToken cancellationToken)
        {
            var timer = DateTime.Now.Ticks;
            DateTime timerOut;
            List<Task<string>?> tasks = new List<Task<string>?>();

            foreach (var model in promptLlmModels)
            {
                IEnumerable<PromptsCrossWithOcenaModel> modelsToCheck = new List<PromptsCrossWithOcenaModel>();

                modelsToCheck = promptLlmModels.Where(x => x.LlmName != model.LlmName).ToArray();

                var listChatMessage = new List<IChatMessage>();

                var userChatMessage = ChatMessageFactory.User(model.PromptAssistance);

                listChatMessage.Add(userChatMessage);

                foreach (var modelToCheck in modelsToCheck)
                {
                    tasks.Add(promptReflectionService.GetCompletionAsync(listChatMessage, lLMNames, modelToCheck.Id, cancellationToken));
                }

                var resultsIteration = await Task.WhenAll(tasks);

                resultsIteration = TextTrimer.ClearThinking(resultsIteration);

                foreach (string iteration in resultsIteration)
                {
                    var iterationAfterCut = GetValueFromString(iteration, "wartosc_ocena_liczbowo");
                    decimal ocena = GetValueFromOcena(iterationAfterCut);
                    model.Ocena.Add(ocena);
                }
            }

            timerOut = new DateTime(DateTime.Now.Ticks - timer);

            return promptLlmModels;
        }


        private static decimal GetValueFromOcena(string ocena)
        {
            int ocenaDec;
            ocena = ocena.Split('/')[0];
            var tryParseResult = int.TryParse(ocena, out ocenaDec);
            return ocenaDec;
        }

        private static string GetValueFromString(string napis, string stringToCut)
        {
            string słowo = string.Empty;
            bool flagToWrite = false;

            foreach (var sign in napis)
            {
                if (sign == '<')
                {
                    flagToWrite = true;
                }

                if (słowo.Contains($"<{stringToCut}>", StringComparison.InvariantCultureIgnoreCase))
                {
                    słowo = string.Empty;
                }

                if (słowo.Contains($"</{stringToCut}>", StringComparison.InvariantCultureIgnoreCase))
                {
                    return słowo.Replace($"</{stringToCut}>", string.Empty);
                }

                if (flagToWrite)
                {
                    słowo += sign;
                }
                else
                {
                    słowo = string.Empty;
                }
            }

            return "brak oceny";
        }

    }
}
