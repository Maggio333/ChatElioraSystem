# Architektura ChatElioraSystem

Ten dokument szczegółowo opisuje architekturę systemu ChatElioraSystem.

## 🏗️ Clean Architecture

Projekt wykorzystuje **Clean Architecture** (Uncle Bob), która zapewnia:

- **Niezależność od frameworków** - Logika biznesowa nie zależy od zewnętrznych bibliotek
- **Testowalność** - Logika biznesowa może być testowana bez UI, bazy danych, serwerów
- **Niezależność od UI** - UI można łatwo zmienić bez zmiany reszty systemu
- **Niezależność od bazy danych** - Można zamienić Qdrant na inną bazę wektorową
- **Niezależność od zewnętrznych serwisów** - LM Studio można zamienić na inny LLM provider

## 📐 Warstwy architektury

### 1. Domain Layer (Warstwa Domenowa)

**Lokalizacja:** `ChatElioraSystem.Core/Domain/`

**Odpowiedzialność:**
- Logika biznesowa
- Encje i modele domenowe
- Interfejsy repozytoriów i serwisów
- Zasady biznesowe

**Zasady:**
- ❌ **NIE** zależy od żadnej innej warstwy
- ✅ Zawiera tylko czystą logikę biznesową
- ✅ Definiuje interfejsy (abstrakcje)
- ✅ Nie zawiera implementacji infrastruktury

**Przykłady:**
- `IRAGPromptCode`, `IRAGPromptReflection` - interfejsy promptów
- `MpcAkcja`, `MpcTopics` - modele domenowe
- `IPromptCodeService`, `IPromptDbVecService` - interfejsy serwisów

### 2. Application Layer (Warstwa Aplikacyjna)

**Lokalizacja:** `ChatElioraSystem.Core/Application_/`

**Odpowiedzialność:**
- Use case'y (przypadki użycia)
- Orkiestracja operacji
- Koordynacja między warstwami
- DTO (Data Transfer Objects)

**Zasady:**
- ✅ Zależy tylko od Domain Layer
- ✅ Definiuje interfejsy dla Infrastructure
- ✅ Zawiera logikę aplikacyjną (nie biznesową)

**Przykłady:**
- `PromptTypeOrchiestratorService` - orkiestruje różne typy promptów
- `PromptTopicOrchiestratorService` - zarządza tematami rozmów
- `CategoryRegiester` - rejestr kategorii

### 3. Infrastructure Layer (Warstwa Infrastruktury)

**Lokalizacja:** `ChatElioraSystem.Core/Infrastructure/`

**Odpowiedzialność:**
- Implementacje interfejsów z Domain/Application
- Integracje z zewnętrznymi systemami
- Dostęp do danych (pliki, bazy danych)
- Komunikacja z API (LM Studio, Qdrant)

**Zasady:**
- ✅ Implementuje interfejsy z Domain/Application
- ✅ Może zależeć od Domain i Application
- ✅ Zawiera szczegóły techniczne

**Przykłady:**
- `LmStudioClientService` - implementacja `ILlmService`
- `QdrantVectorDbService` - implementacja `IVectorDbService`
- `ChatLogService` - zapis/odczyt plików JSON
- `DesktopStoragePathProvider` / `MauiStoragePathProvider` - ścieżki plików

### 4. Presentation Layer (Warstwa Prezentacji)

**Lokalizacja:** 
- `ChatElioraSystem/` (WPF)
- `ChatElioraSystemMobile/` (MAUI)

**Odpowiedzialność:**
- Interfejs użytkownika
- ViewModels (MVVM)
- Widoki (XAML)
- Konwertery wartości
- Behaviors

**Zasady:**
- ✅ Zależy od Application i Infrastructure (przez DI)
- ✅ Zawiera tylko logikę UI
- ✅ Używa Dependency Injection

**Przykłady:**
- `ChatViewModel` - główny ViewModel
- `ChatWindow2.xaml` - główne okno (WPF)
- `MainPage.xaml` - główna strona (MAUI)

## 🔄 Przepływ zależności

```
Presentation Layer
    ↓ (zależy od)
Application Layer
    ↓ (zależy od)
Domain Layer
    ↑ (implementuje)
Infrastructure Layer
```

**Zasada:** Zależności wskazują **do wewnątrz** (w kierunku Domain).

## 🎯 Wzorce projektowe

### 1. Dependency Injection

Wszystkie zależności są wstrzykiwane przez konstruktor:

```csharp
public class PromptTypeOrchiestratorService : IPromptTypeOrchiestratorService
{
    private readonly IPromptGeneralService _promptGeneralService;
    private readonly IPromptCodeService _promptCodeService;
    // ...
    
    public PromptTypeOrchiestratorService(
        IPromptGeneralService promptGeneralService,
        IPromptCodeService promptCodeService,
        // ...
    )
    {
        _promptGeneralService = promptGeneralService;
        _promptCodeService = promptCodeService;
    }
}
```

**Korzyści:**
- Łatwe testowanie (mockowanie zależności)
- Luźne powiązania
- Wymuszanie interfejsów

### 2. Strategy Pattern

Różne typy promptów są implementowane jako strategie:

```csharp
IBasePromptService promptService;

switch (temat)
{
    case SesjaTematu.Ogólna:
        promptService = promptGeneralService;
        break;
    case SesjaTematu.Kod:
        promptService = promptCodeService;
        break;
    // ...
}
```

**Korzyści:**
- Łatwe dodawanie nowych typów promptów
- Każdy typ ma własną logikę
- Możliwość podmiany strategii w runtime

### 3. Repository Pattern

Abstrakcja dostępu do danych:

```csharp
// Interfejs w Domain
public interface IVectorDbService
{
    Task<List<(string Id, float Score, JsonElement Payload)>> SearchAsync(float[] vector);
    Task InsertAsync(string id, float[] vector, JsonDocument payload);
}

// Implementacja w Infrastructure
public class QdrantVectorDbService : IVectorDbService
{
    // Implementacja z Qdrant
}
```

**Korzyści:**
- Możliwość zamiany implementacji (np. Qdrant → Pinecone)
- Łatwe testowanie (mockowanie)
- Izolacja szczegółów technicznych

### 4. Factory Pattern

Tworzenie obiektów przez fabryki:

```csharp
public static class ChatMessageFactory
{
    public static IChatMessage User(string content) => 
        Create(Role.user, content);
    
    public static IChatMessage Assistant(string content) => 
        Create(Role.assistant, content);
}
```

**Korzyści:**
- Centralizacja tworzenia obiektów
- Spójność obiektów
- Łatwe rozszerzanie

### 5. Orchestrator Pattern

Koordynacja złożonych operacji:

```csharp
public class PromptTypeOrchiestratorService
{
    // Orkiestruje różne serwisy promptów
    // Koordynuje przepływ danych
    // Zarządza kontekstem
}
```

**Korzyści:**
- Separacja odpowiedzialności
- Łatwiejsze testowanie
- Centralizacja logiki koordynacji

## 🔌 Dependency Injection

### Rejestracja serwisów

Wszystkie serwisy są rejestrowane w `DependencyInjection.cs`:

```csharp
public static IServiceCollection AddChatElioraCore(this IServiceCollection services)
{
    // Singleton - jedna instancja na całą aplikację
    services.AddSingleton<IRAGPromptCode, RAGPromptCode>();
    
    // Scoped - jedna instancja na scope (np. request)
    services.AddScoped<IPromptCodeService, PromptCodeService>();
    
    // Transient - nowa instancja przy każdym żądaniu
    services.AddHttpClient<ILlmService, LmStudioClientService>();
}
```

### Lifetime

- **Singleton**: Dla stateless serwisów (prompty, konfiguracja)
- **Scoped**: Dla serwisów z kontekstem (prompt services)
- **Transient**: Dla HttpClient i serwisów bezstanowych

## 🧪 Testowalność

Architektura została zaprojektowana z myślą o testach:

1. **Interfejsy** - Wszystkie zależności są abstrakcjami
2. **Dependency Injection** - Łatwe mockowanie
3. **Separacja warstw** - Testowanie każdej warstwy osobno

**Przykład testu:**

```csharp
var mockPromptService = new Mock<IPromptGeneralService>();
var service = new PromptTypeOrchiestratorService(
    mockPromptService.Object,
    // ...
);
```

## 📊 Przepływ danych

### Przykład: Wysłanie wiadomości

```
1. User Input (UI)
   ↓
2. ChatViewModel.SendMessageAsync()
   ↓
3. PromptTypeOrchiestratorService.SendStreamToLLM()
   ↓
4. PromptGeneralService.GetStreamAsync()
   ↓
5. BasePromptService.GetStreamAsync()
   ↓
6. LmStudioClientService.StreamCompletionAsync()
   ↓
7. LM Studio API (HTTP)
   ↓
8. Stream chunks → ChatViewModel → UI
```

## 🔐 Zasady SOLID

### Single Responsibility Principle (SRP)
Każda klasa ma jedną odpowiedzialność:
- `ChatLogService` - tylko zapis/odczyt logów
- `PromptCodeService` - tylko prompty kodu
- `VectorDbHelper` - tylko operacje na bazie wektorowej

### Open/Closed Principle (OCP)
System jest otwarty na rozszerzenia, zamknięty na modyfikacje:
- Dodanie nowego typu promptu nie wymaga zmiany istniejącego kodu
- Nowe implementacje przez interfejsy

### Liskov Substitution Principle (LSP)
Implementacje mogą być zamieniane:
- `QdrantVectorDbService` może być zamienione na inną implementację `IVectorDbService`

### Interface Segregation Principle (ISP)
Interfejsy są małe i specyficzne:
- `IRAGPromptCode`, `IRAGPromptReflection` - osobne interfejsy
- `IPromptCodeService`, `IPromptGeneralService` - osobne interfejsy

### Dependency Inversion Principle (DIP)
Zależności od abstrakcji, nie od konkretnych implementacji:
- ViewModel zależy od `IPromptTypeOrchiestratorService`, nie od konkretnej klasy

## 🎓 Wnioski

Ta architektura zapewnia:
- ✅ **Elastyczność** - łatwe dodawanie nowych funkcji
- ✅ **Testowalność** - każda warstwa może być testowana osobno
- ✅ **Utrzymywalność** - zmiany w jednej warstwie nie wpływają na inne
- ✅ **Skalowalność** - łatwe rozszerzanie systemu
- ✅ **Czytelność** - jasny podział odpowiedzialności

---

**To jest przykład profesjonalnej architektury w .NET, który może służyć jako wzorzec dla innych projektów.**

