using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Application_.Models
{
    // Podstawowy model akcji MCP
    public class McpAkcjaBase
    {
        public string Typ { get; set; } = string.Empty;       // "Zapis" | "Odczyt"
        public string Temat { get; set; } = string.Empty;     // Krótki opis
        public string Payload { get; set; } = string.Empty;   // Treść do zapisania / odczytu
        public McpMetadata Metadata { get; set; } = new();
    }

    public class McpMetadata
    {
        public string Źródło { get; set; } = "LLM";
        public string Wnioski { get; set; } = string.Empty;
        public double Confidence { get; set; } = 0.0;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class McpWrapper<T>
    {
        public T Akcja { get; set; } = default!;
    }

    // 🔹 Akcja ZAPISU
    public class McpZapisAkcja : McpAkcjaBase
    {
        public McpZapisExtra Extra { get; set; } = new();
    }

    public class McpZapisExtra
    {
        public string Kolekcja { get; set; } = "TematyRozmowy";
        public string? RequestId { get; set; } = null;
        public int ReverieLevel { get; set; } = 1;
    }

    // 🔹 Akcja ODCZYTU
    public class McpOdczytAkcja : McpAkcjaBase
    {
        public McpOdczytExtra Extra { get; set; } = new();
    }

    public class McpOdczytExtra
    {
        public string Kolekcja { get; set; } = "TematyRozmowy";
        public int Limit { get; set; } = 20;
        public string Sort { get; set; } = "Timestamp DESC";
    }
}
