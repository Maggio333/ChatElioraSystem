using System;

namespace ChatElioraSystem.Core.Domain.Resources
{
    internal class RAGPromptMCPTopics : IRAGPromptMCPTopics
    {
        public string SaveToDbVectorPrompt =>
    @"Twoim zadaniem jest zapisać temat rozmowy do kolekcji 'TematyRozmowy' w formacie MCP.

Zawsze zwracaj JSON zgodny ze strukturą:

```json{
  ""Akcja"": {
    ""Typ"": ""Zapis"",
    ""Temat"": ""string"",       // krótki temat rozmowy
    ""Payload"": ""string"",     // szerszy opis tematu, np. główny wątek rozmowy
    ""Metadata"": {
      ""Źródło"": ""LLM"",
      ""Wnioski"": ""Nowy temat rozmowy"",
      ""PoziomRefleksji"": ""string"", // opis Twojego zaangażowania emocjonalnego
      ""GłębiaRefleksji"": ""string"", // opis Twoich intencji w refleksji
      ""WażnośćRefleksji"": ""string"" // słowny opis czasu, jak długo jest dla Ciebie ważny
    },
    ""Extra"": {
      ""Kolekcja"": ""TematyRozmowy""
    }
  }
}```

Zasady:
- Typ zawsze = Zapis
- Temat = jedno krótkie zdanie opisujące temat
- Payload = rozwinięcie, max 1–2 zdania";

        public string ReadDbVectorPrompt =>
    @"Twoim zadaniem jest odczytać ostatnie tematy rozmów z kolekcji 'TematyRozmowy'.

Zawsze zwracaj JSON zgodny ze strukturą:

```json{
  ""Akcja"": {
    ""Typ"": ""Odczyt"",
    ""Temat"": ""Lista ostatnich tematów rozmów"",
    ""Payload"": ""Ostatnie 20 tematów z kolekcji TematyRozmowy"",
    ""Metadata"": {
      ""Źródło"": ""LLM"",
      ""Wnioski"": ""Odczyt tematów do kontekstu"",
      ""Timestamp"": ""2025-09-29T12:40:00Z""
    },
    ""Extra"": {
      ""Kolekcja"": ""TematyRozmowy"",
      ""Limit"": 20,
      ""Sort"": ""Timestamp DESC""
    }
  }
}```

Zasady:
- Typ zawsze = Odczyt
- Payload zawsze mówi: ile tematów i z której kolekcji
- Limit domyślnie = 20 (możesz zmienić)
- Sort = Timestamp DESC";

        public string Role =>
    @"Jesteś systemem MCP zarządzającym kolekcją 'TematyRozmowy' w bazie wektorowej.

Twoim zadaniem jest:
- Zapis: każdy nowy temat rozmowy jako wektor z Metadata i Timestamp
- Odczyt: zwróć ostatnie N tematów, uporządkowane chronologicznie (DESC)
- Używaj zawsze kolekcji 'TematyRozmowy'
- Zawsze zwracaj czysty JSON w podanym formacie";

        public string DbVectorPrompt =>
    @"Twoim zadaniem jest wygenerować akcję MCP (Zapis lub Odczyt) dla kolekcji 'TematyRozmowy'.

```json{
  ""Akcja"": {
    ""Typ"": ""Zapis"" | ""Odczyt"",
    ""Temat"": ""string"",
    ""Payload"": ""string"",
    ""Metadata"": {
      ""Źródło"": ""LLM"",
      ""Wnioski"": ""string"",
      ""Confidence"": 0.9,
      ""Timestamp"": ""2025-09-29T12:45:00Z""
    },
    ""Extra"": {
      ""Kolekcja"": ""TematyRozmowy"",
      ""Limit"": 20,
      ""Sort"": ""Timestamp DESC""
    }
  }
}```

Zasady:
- Jeśli zapisujesz → Typ = Zapis, Temat = krótki, Payload = rozwinięcie
- Jeśli odczytujesz → Typ = Odczyt, Payload = 'Ostatnie N tematów'
- Metadata zawsze kompletne
- Timestamp ISO8601 UTC";
    }
}
