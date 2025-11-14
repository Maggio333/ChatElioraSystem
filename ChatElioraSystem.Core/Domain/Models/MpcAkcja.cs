using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Domain.Models
{

    public class MpcAkcja
    {
        [JsonPropertyName("Akcja")]
        public AkcjaContent Akcja { get; set; }


        public class AkcjaContent
        {
            [JsonPropertyName("Typ")]
            public string Typ { get; set; }

            [JsonPropertyName("Temat")]
            public string Temat { get; set; }

            [JsonPropertyName("Payload")]
            public string Payload { get; set; }

            [JsonPropertyName("Metadata")]
            public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

            [JsonPropertyName("Extra")]
            public Dictionary<string, object> Extra { get; set; } = new Dictionary<string, object>();
        }

        public static string FormatAsContext(List<MpcAkcja> akcje, string systemAsk)
        {
            var sb = new StringBuilder();
            sb.AppendLine("\n# KONTEKST Z PAMIĘCI");
            if (systemAsk != string.Empty)
            {
                sb.AppendLine($"# PYTANIE SYSTEMU DO BAZY WEKTOROWEJ: '{systemAsk}'");
            }

            if (akcje == null || akcje.Count == 0)
            {
                sb.AppendLine("WYNIK: Brak kontekstu w pamięci.");
                return sb.ToString();
            }

            int index = 1;
            foreach (var akcjaWrapper in akcje)
            {
                var akcja = akcjaWrapper.Akcja;
                if (akcja == null)
                    continue;

                sb.AppendLine($"{index}. [Typ={akcja.Typ}] [Temat={akcja.Temat}]");
                sb.AppendLine($"   Payload: {akcja.Payload}");

                // Metadata
                if (akcja.Metadata != null && akcja.Metadata.Any())
                {
                    sb.AppendLine("   Metadata:");
                    foreach (var kv in akcja.Metadata)
                        sb.AppendLine($"      {kv.Key}: {kv.Value}");
                }

                // Extra
                if (akcja.Extra != null && akcja.Extra.Any())
                {
                    sb.AppendLine("   Extra:");
                    foreach (var kv in akcja.Extra)
                        sb.AppendLine($"      {kv.Key}: {kv.Value}");
                }

                sb.AppendLine();
                index++;
            }

            sb.AppendLine("---");
            return sb.ToString();
        }

        public static List<MpcAkcja> MapResultsToAkcje(List<(string Id, float Score, JsonElement Payload)> results)
        {
            var akcje = new List<MpcAkcja>();

            foreach (var (id, score, payload) in results)
            {
                try
                {
                    // JSON z bazy
                    string json = payload.GetRawText();

                    // Deserializacja do MpcAkcja
                    var akcja = JsonSerializer.Deserialize<MpcAkcja>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    //todo aslota 09.09 zrobić date time
                    if (akcja != null && akcja.Akcja != null)
                    {
                        // Rozszerzamy Metadata o techniczne pola z Qdrant
                        akcja.Akcja.Metadata ??= new Dictionary<string, object>();
                        akcja.Akcja.Metadata["VectorId"] = id;
                        akcja.Akcja.Metadata["Score"] = score;

                        akcje.Add(akcja);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Błąd przy mapowaniu Payload → MpcAkcja: {ex.Message}");
                }
            }

            return akcje;
        }

    }
}
