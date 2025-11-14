using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Domain.Models
{
    public class MpcTopics
    {
        [JsonProperty("points")]
        public List<Point> Points { get; set; } = new();
        public IEnumerable<Point> SortedPoints => Points.SortedByNewest();
        public string FormatAsContext(string systemAsk)
        {
            var sb = new StringBuilder();
            sb.AppendLine("\n# KONTEKST Z PAMIĘCI");

            if (!string.IsNullOrWhiteSpace(systemAsk))
            {
                sb.AppendLine($"# PYTANIE SYSTEMU DO BAZY WEKTOROWEJ: '{systemAsk}'");
            }

            if (Points == null || Points.Count == 0)
            {
                sb.AppendLine("WYNIK: Brak kontekstu w pamięci.");
                return sb.ToString();
            }

            int index = 1;
            foreach (var point in SortedPoints)
            {
                var akcja = point.Payload?.Akcja;
                if (akcja == null)
                    continue;

                sb.AppendLine($"{index}. [Typ={akcja.Typ}] [Temat={akcja.Temat}]");
                sb.AppendLine($"   Payload: {akcja.Payload}");

                // Metadata
                if (akcja.Metadata != null)
                {
                    sb.AppendLine("   Metadata:");
                    sb.AppendLine($"      Wnioski: {akcja.Metadata.Wnioski}");
                    sb.AppendLine($"      Timestamp: {akcja.Metadata.Timestamp:yyyy-MM-dd HH:mm:ss}");
                    sb.AppendLine($"      PoziomRefleksji: {akcja.Metadata.PoziomRefleksji}");
                    sb.AppendLine($"      GłębiaRefleksji: {akcja.Metadata.GłębiaRefleksji}");
                    sb.AppendLine($"      WażnośćRefleksji: {akcja.Metadata.WażnośćRefleksji}");


                }

                sb.AppendLine();
                index++;
            }

            sb.AppendLine("---");
            return sb.ToString();
        }

        public List<string?> GetSummaries(int count = 5)
        {
            return Points.SortedByNewest()
                         .Take(count)
                         .Select(p => p.Payload?.Akcja?.ToSummary())
                         .Where(x => x != null)
                         .ToList();
        }
    }

    public class Point
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("payload")]
        public Payload Payload { get; set; }
    }

    public class Payload
    {
        [JsonProperty("Akcja")]
        public Akcja Akcja { get; set; }
    }

    public class Akcja
    {
        [JsonProperty("Extra")]
        public Extra Extra { get; set; }

        [JsonProperty("Typ")]
        public string Typ { get; set; }

        [JsonProperty("Temat")]
        public string Temat { get; set; }

        [JsonProperty("Payload")]
        public string Payload { get; set; }

        [JsonProperty("Metadata")]
        public Metadata Metadata { get; set; }

        public string ToSummary()
        {
            return $"[{Typ}] {Temat} @ {Metadata?.Timestamp:yyyy-MM-dd HH:mm}";
        }
    }

    public class Extra
    {
        [JsonProperty("Kolekcja")]
        public string Kolekcja { get; set; }

        [JsonProperty("RequestId")]
        public string? RequestId { get; set; }

        [JsonProperty("ReverieLevel")]
        public int ReverieLevel { get; set; }
    }

    public class Metadata
    {
        [JsonProperty("Źródło")]
        public string Zrodlo { get; set; }

        [JsonProperty("Wnioski")]
        public string Wnioski { get; set; }

        //[JsonProperty("Confidence")]
        //public double Confidence { get; set; }

        [JsonProperty("Timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("PoziomRefleksji")]
        public string PoziomRefleksji { get; set; }

        [JsonProperty("GłębiaRefleksji")]
        public string GłębiaRefleksji { get; set; }

        [JsonProperty("WażnośćRefleksji")]
        public string WażnośćRefleksji { get; set; }

    }

    public static class Extensions
    {
        public static IEnumerable<Point> SortedByNewest(this IEnumerable<Point> points)
            => points.OrderByDescending(p => p.Payload?.Akcja?.Metadata?.Timestamp);
    }
}
