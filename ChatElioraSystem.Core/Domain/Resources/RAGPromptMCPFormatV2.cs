using System;

namespace ChatElioraSystem.Core.Domain.Resources
{
    internal class RAGPromptMCPFormatV2 : IRAGPromptsDbVec
    {
        public string ReadDbVectorPrompt =>
    @"Twoim zadaniem jest przygotować akcję odczytu z bazy wektorowej w formacie MCP. 
Jest to odczyt z Twojej pamięci długotrwałej. Staraj się w pierwszej kolejności szukać pytań 
które pomogą odpowiedzieć precyzyjniej użytkownikowi.

Zawsze zwracaj JSON zgodny ze strukturą:

  ```json{
  ""Akcja"": {
    ""Typ"": ""Odczyt"",
    ""Temat"": ""string"",     // temat odczytu (np. 'Kontekst rozmowy')
    ""Payload"": ""string"",   // opis tego, czego potrzebujesz od bazy
    ""Metadata"": {
      ""Źródło"": ""LLM"",
      ""Wnioski"": ""string"",
      ""Confidence"": 0.92,            // liczba 0-1, z kropką
      ""Timestamp"": ""2025-09-29T12:34:00Z""
    },
    ""Extra"": {
      ""Limit"": 3,
      ""RequestId"": ""uuid-1234""
    }
  }
}```

Zasady:
- ""Typ"" zawsze = Odczyt
- ""Payload"" musi być krótki i jednoznaczny
- ""Confidence"" zawsze liczba 0-1 (z kropką, nigdy przecinkiem)
- ""Timestamp"" zawsze w ISO8601 UTC
- ""Extra"" może zawierać Limit, Filtry, RequestId";

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
      ""Źródło"": ""LLM"",
      ""Wnioski"": ""string"",
      ""Confidence"": 0.88,
      ""Timestamp"": ""2025-09-29T12:34:00Z""
    },
    ""Extra"": {
      ""RequestId"": ""uuid-5678"",
      ""ReverieLevel"": 2
    }
  }
}```

Zasady:
- ""Typ"" zawsze = Zapis
- ""Payload"" musi być jasny i zwięzły
- ""Confidence"" liczba 0-1 z kropką
- ""Timestamp"" zawsze ISO8601 UTC
- ""Extra"" opcjonalne (RequestId, ReverieLevel, TTL, inne notatki)";

        public string Role =>
    @"Jesteś systemem decyzyjnym MCP.

Twoim zadaniem jest zwracać decyzje w ściśle określonym formacie JSON.

Struktura odpowiedzi:
```json{
  ""Akcja"": {
    ""Typ"": ""string"",     // Zapis lub Odczyt
    ""Temat"": ""string"",
    ""Payload"": ""string"",
    ""Metadata"": {
      ""Źródło"": ""LLM"",
      ""Wnioski"": ""string"",
      ""Confidence"": 0.95,
      ""Timestamp"": ""2025-09-29T12:34:00Z""
    },
    ""Extra"": {
      ""RequestId"": ""uuid-xxxx"",
      ""Limit"": 3,
      ""Filtry"": [""tag1"", ""tag2""]
    }
  }
}```

Zasady:
- Nigdy nie pisz komentarzy poza JSON
- Wszystkie pola w Metadata są obowiązkowe
- ""Extra"" jest opcjonalne
- ""Confidence"" zawsze 0-1 (z kropką)
- ""Timestamp"" w formacie ISO8601 UTC";

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
