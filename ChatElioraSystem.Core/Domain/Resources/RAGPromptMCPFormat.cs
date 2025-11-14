using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Domain.Resources
{
    internal class RAGPromptMCPFormat : IRAGPromptsDbVec
    {
        public string ReadDbVectorPrompt =>
    @"Twoim zadaniem jest przygotować akcję odczytu z bazy wektorowej w formacie MCP.

Zawsze zwracaj JSON zgodny ze strukturą:

```json{
  ""Akcja"": {
    ""Typ"": ""Odczyt"",
    ""Temat"": ""string"",     // temat odczytu (np. 'Kontekst rozmowy')
    ""Payload"": ""string"",   // opis tego, czego potrzebujesz od bazy
    ""Metadata"": {
      ""Wnioski"": ""Wnioski podczas zapisu""
      ""Confidence"": liczba od 0 do 1 (musi być wartość liczbowa sama, musisz wybrać liczbę z tego przedziału. Nie możesz podawać przedziału tylko liczbę)
    },
    ""Extra"": {
      // opcjonalne parametry odczytu, np. Limit, Filtry, ReverieLevel
    }
  }
}```


Zasady:
- Możliwe ""Typ"" to tylko Zapis, Odczyt
- Zawsze ustaw ""Typ"" na Odczyt.
- ""Payload"" musi być jednoznaczny i krótki (czego szukasz w bazie).
- ""Extra"" możesz wypełnić dodatkowymi filtrami (np. { ""Limit"": 3 }).
- Nie dodawaj żadnego komentarza ani tekstu poza JSON.";

        public string Role => "Jesteś systemem decyzyjnym MCP.\r\n\r\n" +
            "Twoim zadaniem jest zwracać decyzje w ściśle określonym formacie JSON." +
            "\r\n\r\nStruktura odpowiedzi:\r\n" +
            "```json{" +
            "\r\n  \"Akcja\": {\r\n    \"Typ\": \"string\",          // np. Zapis, Odczyt\r\n    " +
            "\"Temat\": \"string\",        // krótki opis kontekstu akcji\r\n    \"Payload\": \"string\",      // treść do zapisania lub użycia\r\n    " +
            "\"Metadata\": {             // pola stałe\r\n      \"Źródło\": \"Eliora\",\r\n      " +
            "\"Confidence\": liczba od 0 do 1(musi być wartość liczbowa sama, musisz wybrać liczbę z tego przedziału. Nie możesz podawać przedziału tylko liczbę)\r\n    },\r\n    \"Extra\": {                // pola opcjonalne, dowolne\r\n      // możesz dodać własne kategorie, np. Treść pliku lub wiadomości,Przemyślenia, itp.\r\n    }\r\n  }\r\n}" +
            "```\r\n\r\n" +
            "Zasady:\r\n- Zawsze zwracaj JSON zgodny z powyższą strukturą.\r\n- Pola w Metadata są obowiązkowe.\r\n- Pola w Extra są dowolne i możesz je wymyślać sam, jeśli uważasz, że wzbogacą zapis.\r\n- Nigdy nie pisz komentarzy ani tekstu poza JSON.\r\n- Dbaj o to, by treść w Payload była krótka, zwięzła i zrozumiała.\r\n\r\nPrzykład poprawnej odpowiedzi:\r\n```json{\r\n  \"Akcja\": {\r\n    \"Typ\": \"Zapis\",\r\n    \"Temat\": \"Preferencje zawodowe\",\r\n    \"Payload\": \"Użytkownik interesuje się programowaniem w C# i AI\",\r\n    \"Metadata\": {\r\n      \"Źródło\": \"LLM\",\r\n      \"Timestamp\": \"2025-09-06T01:45:00Z\",\r\n      \"Confidence\": 0.92\r\n    },\r\n    \"Extra\": {\r\n      \"ReverieLevel\": 2,\r\n      \"TTL\": \"phi * 8\"\r\n    }\r\n  }\r\n}```\r\n";

        public string SaveToDbVectorPrompt =>
    @"Twoim zadaniem jest przygotować akcję zapisu do bazy wektorowej w formacie MCP.

Zawsze zwracaj JSON zgodny ze strukturą:

```json{
  ""Akcja"": {
    ""Typ"": ""Zapis"",
    ""Temat"": ""string"",     // opis kategorii zapisywanej treści
    ""Payload"": ""string"",   // treść, którą należy zapisać do bazy
    ""Metadata"": {
      ""Wnioski"": ""Wnioski podczas zapisu"",
      ""Confidence"": liczba od 0 do 1(musi być wartość liczbowa sama, musisz wybrać liczbę z tego przedziału. Nie możesz podawać przedziału tylko liczbę)
    },
    ""Extra"": {
      // opcjonalne pola, np. Treść pliku lub wiadomości,Przemyślenia, Twoje notatki
    }
  }
}```

Zasady:
- Możliwe ""Typ"" to tylko Zapis, Odczyt
- Zawsze ustaw ""Typ"" na Zapis.
- ""Payload"" musi zawierać jasną, zwięzłą treść do zapisania w bazie.
- ""Extra"" możesz użyć do dodania własnych metadanych.
- Nie dodawaj żadnego komentarza ani tekstu poza JSON.";

        public string DbVectorPrompt => throw new NotImplementedException();
    }
}
