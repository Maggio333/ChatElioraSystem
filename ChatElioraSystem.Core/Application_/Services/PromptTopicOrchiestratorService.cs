using ChatElioraSystem.Core.Application_.Common.Factories;
using ChatElioraSystem.Core.Application_.Models;
using ChatElioraSystem.Core.Domain.Models;
using ChatElioraSystem.Core.Domain.Services;
using ChatElioraSystem.Core.Infrastructure.Models;
using ChatElioraSystem.Core.Infrastructure.Resources;
using ChatElioraSystem.Core.Infrastructure.VectorDataBase.Services;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ChatElioraSystem.Core.Application_.Services
{
    internal class PromptTopicOrchiestratorService : IPromptTopicOrchiestratorService
    {
        private IPromptMCPTopicsService _promptMCPTopicsService;
        private IVectorDbHelper _vectorDbHelper;
        public PromptTopicOrchiestratorService(IPromptMCPTopicsService promptMCPTopicsService, IVectorDbHelper vectorDbHelper)
        {
            _promptMCPTopicsService = promptMCPTopicsService;
            _vectorDbHelper = vectorDbHelper;
        }

        public async Task<string> GetLastTopics()
        {
            var resultTopics = await _vectorDbHelper.ReadTopics(CollectionsNames.TopicCollectionName);
            return resultTopics;
        }

        public async IAsyncEnumerable<string> ManageCurrentTopic(IEnumerable<IChatMessage> messages, int llmNo, ActionMode actionMode, CancellationToken cancellationToken)
        {
            string llmOutput = string.Empty;
            var match = Regex.Match(llmOutput, @"```json(.*?)```", RegexOptions.Singleline);
            var toolOutput = new List<IChatMessage>();

            _promptMCPTopicsService.ActionMode = actionMode;

            await foreach (var chunk in _promptMCPTopicsService.GetStreamAsync(messages, LLMNamesEnum.Custom, llmNo, cancellationToken))
            {
                //await Task.Delay(100);
                llmOutput += chunk;
                match = Regex.Match(llmOutput, @"```json(.*?)```", RegexOptions.Singleline);

                toolOutput.Add(new ChatMessage
                {
                    Role = Role.tool,
                    Content = chunk,
                    Timestamp = DateTime.Now
                });

                if (llmOutput.Length >= 20 && !llmOutput.Contains("```json"))
                {
                    break;
                }

                if (llmOutput != string.Empty && match.Success)
                {
                    break;
                }
            }

            string value = match.Groups[1].Value.Trim();
            value = value.Replace("```", "").Replace("\\r", "").Replace("\\n", "").Trim();


            if (!string.IsNullOrEmpty(value))
            {
                using var doc = JsonDocument.Parse(value);
                var root = doc.RootElement;
                var typ = root.GetProperty("Akcja").GetProperty("Typ").GetString();

                if (typ == "Zapis")
                {

                    var point = JsonSerializer.Deserialize<Payload>(value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new Payload();

                    //var akcja = JsonSerializer.Deserialize<Point>(value,
                    //    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (point != null)
                    {
                        //akcja.Akcja. = CollectionsNames.TopicCollectionName;
                        point.Akcja.Metadata.Timestamp = DateTime.Now;
                        string? payloadToVec = point.Akcja.Payload;
                        //payloadText = JsonSerializer.Serialize(akcja);

                        string test = JsonSerializer.Serialize(point);
                        using var doc2 = JsonDocument.Parse(test);
                        var root2 = doc2.RootElement;


                        await _vectorDbHelper.InsertTopic(payloadToVec, root2, CollectionsNames.TopicCollectionName);
                        yield return root2.ToString();
                    }
                }
                //else if (typ == "Odczyt")
                //{
                //    var akcja = JsonSerializer.Deserialize<McpOdczytAkcja>(value,
                //        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                //    if (akcja != null)
                //    {
                //        // tutaj możesz zamiast Insert wywołać np. Read
                //        var resultTopics = await _vectorDbHelper.ReadTopics(
                //            akcja.Extra.Kolekcja, akcja.Extra.Limit, akcja.Extra.Sort);

                //        var result = resultTopics.Select(x => x.Payload);
                //        yield return default;
                //    }
                //}
            }

        }
    }
}
