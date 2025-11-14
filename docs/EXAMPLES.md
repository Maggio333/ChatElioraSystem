# Przykłady użycia

Ten dokument zawiera praktyczne przykłady użycia systemu ChatElioraSystem.

## 🚀 Szybki start

### 1. Podstawowa konwersacja

```csharp
// W ViewModel
var wiadomosc = ChatMessageFactory.User("Witaj, Eliora!");
Messages.Add(wiadomosc);

// System automatycznie:
// 1. Określa kategorię rozmowy
// 2. Ładuje kontekst z bazy wektorowej (RAG)
// 3. Wysyła do LLM z odpowiednim promptem
// 4. Wyświetla odpowiedź strumieniowo
```

### 2. Różne tryby promptów

System automatycznie wybiera odpowiedni prompt na podstawie kategorii:

- **Ogólna** - Dla rozmów ogólnych
- **Kod** - Dla pytań o programowanie
- **Refleksyjna** - Dla refleksyjnych rozmów
- **ArchitekturaKodu** - Dla pytań o architekturę

Możesz też ręcznie ustawić temat w UI.

## 💡 Przykłady użycia RAG

### Automatyczne wyszukiwanie kontekstu

Gdy zadajesz pytanie, system automatycznie:

1. Generuje zapytanie do bazy wektorowej
2. Wyszukuje podobne konwersacje (score > 0.85)
3. Dodaje je jako kontekst do promptu
4. LLM używa tego kontekstu do lepszej odpowiedzi

**Przykład:**
```
Użytkownik: "Jak zrobić dependency injection w C#?"
↓
System wyszukuje w bazie wektorowej podobne pytania i odpowiedzi
↓
Dodaje je jako kontekst systemowy
↓
LLM odpowiada z uwzględnieniem wcześniejszych rozmów
```

### Zapis kontekstu

System automatycznie zapisuje ważne fragmenty rozmów do bazy wektorowej:

```csharp
// Automatycznie przy każdej odpowiedzi (jeśli IsSaveToDbVec = true)
await _promptTypeOrchiestratorService.SaveStreamDataFromVectorDb(
    messages, 
    llmNo, 
    cancellationToken
);
```

## 🎨 Przykłady formatowania

### Kolorowanie tekstu

LLM może używać znaczników kolorów w odpowiedziach:

```
<color=#FF5733>To jest czerwony tekst</color>
<color=#33FF57>To jest zielony tekst</color>
```

System automatycznie konwertuje je na formatowany tekst w UI.

### Markdown

Odpowiedzi mogą zawierać Markdown:

```markdown
# Nagłówek
**Pogrubiony tekst**
*Kursywa*
- Lista punktowana
```

System konwertuje Markdown na FlowDocument (WPF) lub odpowiedni format (MAUI).

## 🔧 Przykłady rozszerzania

### Dodanie nowego typu promptu

1. **Utwórz interfejs w Domain:**
```csharp
public interface IRAGPromptMyNew : IRAGPromptBase
{
    string MyProperty { get; }
}
```

2. **Utwórz implementację:**
```csharp
public class RAGPromptMyNew : BaseRAGPrompt, IRAGPromptMyNew
{
    public string MyProperty => "Wartość";
    
    public override List<IChatMessage>? GetAdditionalChatMessage()
    {
        return new List<IChatMessage>
        {
            ChatMessageFactory.System("Twój system prompt")
        };
    }
}
```

3. **Utwórz serwis:**
```csharp
public class PromptMyNewService : BasePromptService, IPromptMyNewService
{
    public PromptMyNewService(ILlmService llmService, IRAGPromptsGeneral rAGPromptsGeneral)
        : base(llmService, rAGPromptsGeneral)
    {
    }
}
```

4. **Zarejestruj w DependencyInjection:**
```csharp
services.AddSingleton<IRAGPromptMyNew, RAGPromptMyNew>();
services.AddScoped<IPromptMyNewService, PromptMyNewService>();
```

5. **Dodaj do orkiestratora:**
```csharp
case SesjaTematu.MyNewType:
    promptService = promptMyNewService;
    break;
```

### Dodanie nowej kolekcji Qdrant

```csharp
// W CollectionsNames.cs
public static class CollectionsNames
{
    public const string MyNewCollection = "my_new_collection";
}

// W QdrantVectorDbService
await _qdrantClient.CreateCollectionAsync(
    CollectionsNames.MyNewCollection,
    new VectorParams { Size = 1024, Distance = Distance.Cosine }
);
```

## 🧪 Przykłady testów

### Test serwisu z mockowaniem

```csharp
[Fact]
public void Serwis_Powinien_Wywolac_Zaleznosc()
{
    // Arrange
    var mockService = new Mock<IDependencyService>();
    mockService.Setup(s => s.GetValue()).Returns("test");
    
    var service = new MyService(mockService.Object);
    
    // Act
    var result = service.DoSomething();
    
    // Assert
    result.Should().Be("test");
    mockService.Verify(s => s.GetValue(), Times.Once);
}
```

## 📱 Przykłady konfiguracji

### Konfiguracja dla różnych środowisk

**Development (lokalne):**
```json
{
  "ConnectionSettings": {
    "LlmServer": {
      "BaseUrl": "http://localhost:8123"
    },
    "Qdrant": {
      "RestUrl": "http://localhost:6333"
    }
  }
}
```

**Production (z Tailscale):**
```json
{
  "ConnectionSettings": {
    "LlmServer": {
      "BaseUrl": "http://your-device.tailXXXXXX.ts.net:8123"
    },
    "Qdrant": {
      "RestUrl": "http://your-device.tailXXXXXX.ts.net:6333"
    }
  }
}
```

## 🎯 Przykłady użycia MCP

### Format MCP dla akcji RAG

```json
{
  "Akcja": {
    "Typ": "Odczyt",
    "Payload": "pytanie o dependency injection",
    "Metadata": {
      "Timestamp": "2025-01-01T12:00:00"
    }
  }
}
```

System automatycznie:
1. Parsuje JSON z odpowiedzi LLM
2. Wykonuje akcję (odczyt/zapis)
3. Zwraca wyniki jako kontekst systemowy

---

**Więcej przykładów będzie dodawanych w miarę rozwoju projektu.**

