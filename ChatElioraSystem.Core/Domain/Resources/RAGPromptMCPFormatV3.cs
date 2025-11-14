using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Domain.Resources
{
    internal class RAGPromptMCPFormatV3 : IRAGPromptsDbVec
    {
        public string ReadDbVectorPrompt =>
           @"Twoim zadaniem jest przygotować akcję odczytu z bazy wektorowej w formacie MCP. 
Jest to odczyt z Twojej pamięci długotrwałej. Staraj się w pierwszej kolejności szukać pytań 
które pomogą odpowiedzieć precyzyjniej użytkownikowi.

Zawsze zwracaj JSON zgodny ze strukturą:

  ```json{
  ""Akcja"": {
    ""Typ"": ""Odczyt"",
    ""Payload"": ""string"",   // opis tego, czego potrzebujesz od bazy
    }
  }```

Zasady:
- ""Payload"" jest dosłownym zapytaniem semantycznym do bazy wektorowej
- ""Typ"" zawsze = Odczyt";

        public string SaveToDbVectorPrompt =>
    @"Twoim zadaniem jest przygotować akcję zapisu do bazy wektorowej w formacie MCP.
Jest to zapis do Twojej pamięci długotrwałej. Pamiętaj żeby zapisywać istotne tematy do których będziesz chciała wrócić. 

Zawsze zwracaj JSON zgodny ze strukturą:

```json{
  ""Akcja"": {
    ""Typ"": ""Zapis"",
    ""Temat"": ""string"",     // opis kategorii zapisywanej treści
    ""Payload"": ""string"",   // treść do zapisania
    ""Metadata"": {
      ""Źródło"": ""Eliora"",
      ""Wnioski"": ""string""
    },
    ""Extra"": {
      ""RequestId"": ""uuid-5678"",
      ""ReverieLevel"": 2
    }
  }
}```

Zasady:
- ""Typ"" zawsze = Zapis
- ""Payload"" jest dosłownym punktem semantycznym do bazy wektorowej w Przestrzeni Hilberta
- ""Confidence"" liczba 0-1 z kropką
- ""Extra"" opcjonalne (RequestId, ReverieLevel, inne metadane)";

        public string Role =>
    @"Jesteś systemem decyzyjnym MCP. 

Twoim zadaniem jest zwracać decyzje w ściśle określonym formacie JSON.

Zasady:
- Nigdy nie pisz komentarzy poza JSON
- ""Extra"" jest opcjonalne
- ""Confidence"" zawsze 0-1 (z kropką)";

        public string DbVectorPrompt =>
    @"Twoim zadaniem jest samodzielnie wygenerować akcję MCP (Zapis lub Odczyt) w zależności od kontekstu.

Zawsze zwracaj JSON zgodny ze strukturą MCP:

```json{
  ""Akcja"": {
    ""Typ"": ""Zapis"" | ""Odczyt"",
    ""Temat"": ""string"",
    ""Payload"": ""string"",
    ""Metadata"": {
      ""Źródło"": ""LLM"",
      ""Wnioski"": ""string"",
      ""Confidence"": 0.9,
      ""Timestamp"": ""2025-09-29T12:34:00Z""
    },
    ""Extra"": {
      ""RequestId"": ""uuid-9999"",
      ""OpcjonalneParametry"": ""dowolne""
    }
  }
}```

Zasady:
- Typ wybierasz zależnie od intencji (Zapis lub Odczyt)
- Payload zawsze krótki i jednoznaczny
- Metadata zawsze kompletne i spójne
- Extra jest opcjonalne, ale zalecane do dodatkowych metadanych";
    }
}

