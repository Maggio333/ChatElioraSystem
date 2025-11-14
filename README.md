# ChatElioraSystem

<div align="center">

**Zaawansowany system czatu z AI wykorzystujący RAG (Retrieval Augmented Generation) do inteligentnych konwersacji z pamięcią długoterminową**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-WPF%20%7C%20MAUI-0078D4)](https://dotnet.microsoft.com/apps/maui)

</div>

---

## 📋 Spis treści

- [O projekcie](#-o-projekcie)
- [Kluczowe funkcjonalności](#-kluczowe-funkcjonalności)
- [Architektura](#-architektura)
- [Wymagania](#-wymagania)
- [Instalacja](#-instalacja)
- [Konfiguracja](#-konfiguracja)
- [Użycie](#-użycie)
- [Struktura projektu](#-struktura-projektu)
- [Przepływ danych](#-przepływ-danych)
- [Komponenty systemu](#-komponenty-systemu)
- [Rozwój](#-rozwój)
- [Współpraca](#-współpraca)
- [Architektura](#-architektura)
- [Licencja](#-licencja)

---

## 🎯 O projekcie

**ChatElioraSystem** to zaawansowany system asystenta AI zbudowany w .NET 8.0, który łączy lokalne modele językowo (LLM) z technologią RAG (Retrieval Augmented Generation) do tworzenia inteligentnych konwersacji z pamięcią długoterminową. System oferuje zarówno aplikację desktopową (WPF) jak i mobilną (MAUI) z wspólnym rdzeniem biznesowym.

### 💡 Dlaczego ten projekt?

Ten projekt powstał jako **kompletna, działająca implementacja** systemu RAG z lokalnymi LLM w .NET. W przeciwieństwie do wielu przykładów "hello world", ten projekt:

- ✅ **Działa end-to-end** - od UI do bazy wektorowej
- ✅ **Ma prawdziwą architekturę** - Clean Architecture z pełnym podziałem warstw
- ✅ **Jest produkcyjny** - używany na co dzień, nie tylko demo
- ✅ **Ma testy** - 23 testy jednostkowe pokrywające kluczowe komponenty
- ✅ **Jest dokumentowany** - szczegółowa dokumentacja każdego aspektu
- ✅ **Pokazuje best practices** - SOLID, DI, MVVM, wzorce projektowe

**To nie jest tutorial - to działający system, który możesz użyć jako:**
- 🎓 **Przykład implementacji RAG** w .NET
- 📚 **Wzorzec Clean Architecture** dla większych projektów
- 🔧 **Bazę do własnych projektów** z lokalnymi LLM
- 💼 **Portfolio project** pokazujący zaawansowane umiejętności

### Główne cechy

- 🤖 **Lokalne LLM** - Integracja z LM Studio dla pełnej prywatności
- 🧠 **RAG System** - Pamięć długoterminowa z bazą wektorową Qdrant
- 🎨 **Wieloplatformowość** - WPF (Windows) i MAUI (Android, iOS, macOS, Windows)
- 📝 **Zarządzanie tematami** - Automatyczna kategoryzacja i organizacja konwersacji
- 🎭 **Różne tryby promptów** - Kod, refleksja, ogólne, architektura
- 🔄 **Streaming odpowiedzi** - Real-time wyświetlanie odpowiedzi AI
- 🎨 **Formatowanie tekstu** - Kolorowanie i formatowanie odpowiedzi

---

## 🚀 Kluczowe funkcjonalności

### 1. Inteligentny Chat z AI
- Konwersacje wspierane przez lokalne modele LLM (LM Studio)
- Streaming odpowiedzi w czasie rzeczywistym
- Obsługa wielu modeli LLM jednocześnie
- Formatowanie tekstu z kolorami i znacznikami

### 2. RAG (Retrieval Augmented Generation)
- System pamięci długoterminowej wykorzystujący bazę wektorową Qdrant
- Automatyczne wyszukiwanie kontekstu z historii rozmów
- Zapisywanie i odczytywanie kontekstu z bazy wektorowej
- Własny format JSON (MCP-inspired) dla akcji RAG

### 3. Zarządzanie tematami
- Organizacja konwersacji w tematy (Ogólna, Kod, Refleksyjna, ArchitekturaKodu)
- Automatyczna kategoryzacja rozmów
- Przechowywanie tematów w bazie wektorowej
- Ładowanie historii tematów

### 4. Różne tryby promptów
- **Ogólna** - Rozmowy ogólne z dialektyką
- **Kod** - Pomoc w programowaniu i wzorcach projektowych
- **Refleksyjna** - Refleksyjne rozmowy z emocjonalnym kontekstem
- **ArchitekturaKodu** - Ekspercka pomoc w architekturze oprogramowania

### 5. Wieloplatformowość
- **Desktop (WPF)** - Pełnoprawna aplikacja Windows
- **Mobile (MAUI)** - Aplikacja mobilna dla Android, iOS, macOS, Windows

---

## 🏗️ Architektura

Projekt wykorzystuje **Clean Architecture** z wyraźnym podziałem na warstwy:

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                       │
│  ┌──────────────┐              ┌──────────────┐           │
│  │   WPF App    │              │  MAUI App    │           │
│  │  (Desktop)   │              │  (Mobile)    │           │
│  └──────┬───────┘              └──────┬───────┘           │
│         │                              │                    │
│         └──────────────┬───────────────┘                    │
│                        │                                     │
└────────────────────────┼─────────────────────────────────────┘
                          │
┌─────────────────────────┼─────────────────────────────────────┐
│                    APPLICATION LAYER                          │
│         ┌───────────────────────────────┐                    │
│         │ PromptTypeOrchiestratorService │                    │
│         │  (Orkiestracja promptów)      │                    │
│         └───────────────┬───────────────┘                    │
│                         │                                     │
│         ┌───────────────┴───────────────┐                    │
│         │ PromptTopicOrchiestratorService│                    │
│         │  (Zarządzanie tematami)        │                    │
│         └───────────────────────────────┘                    │
└─────────────────────────┼─────────────────────────────────────┘
                          │
┌─────────────────────────┼─────────────────────────────────────┐
│                      DOMAIN LAYER                              │
│  ┌─────────────────────────────────────────────────────┐     │
│  │              Prompt Services                         │     │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐          │     │
│  │  │  Code   │ │Reflection│ │ General  │          │     │
│  │  │ Service │ │ Service  │ │ Service  │          │     │
│  │  └──────────┘ └──────────┘ └──────────┘          │     │
│  └─────────────────────────────────────────────────────┘     │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐     │
│  │              RAG Resources                           │     │
│  │  (Prompty, Idiomy, Wzorce projektowe)               │     │
│  └─────────────────────────────────────────────────────┘     │
└─────────────────────────┼─────────────────────────────────────┘
                          │
┌─────────────────────────┼─────────────────────────────────────┐
│                  INFRASTRUCTURE LAYER                           │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐        │
│  │ LLM Service  │  │Vector DB     │  │Embedding    │        │
│  │ (LM Studio)  │  │(Qdrant)      │  │Service      │        │
│  └──────────────┘  └──────────────┘  └──────────────┘        │
│                                                               │
│  ┌──────────────┐  ┌──────────────┐                          │
│  │Tailscale     │  │Chat Log      │                          │
│  │Service       │  │Service       │                          │
│  └──────────────┘  └──────────────┘                          │
└───────────────────────────────────────────────────────────────┘
```

### Wzorce projektowe

- **MVVM** (Model-View-ViewModel) - Separacja logiki od UI
- **Dependency Injection** - Zarządzanie zależnościami przez Microsoft.Extensions.DependencyInjection
- **Strategy Pattern** - Różne typy promptów i serwisów
- **Repository Pattern** - Abstrakcja dostępu do danych
- **Orchestrator Pattern** - Koordynacja złożonych operacji

### Zasady architektury

1. **Dependency Rule** - Zależności wskazują do wewnątrz (od zewnętrznych do domeny)
2. **Separation of Concerns** - Każda warstwa ma określoną odpowiedzialność
3. **Testability** - Łatwe testowanie dzięki DI i abstrakcjom
4. **SOLID Principles** - Zastosowanie wszystkich zasad SOLID

---

## 📋 Wymagania

### Oprogramowanie

- **.NET 8.0 SDK** lub nowszy
- **Visual Studio 2022** (zalecane) lub **JetBrains Rider**
- **LM Studio** - do uruchomienia lokalnych modeli LLM
- **Qdrant** - baza danych wektorowa (opcjonalnie, jeśli używana jest funkcja RAG)

### Zależności zewnętrzne

- **Tailscale** (opcjonalnie) - do połączenia między urządzeniami w sieci VPN

### Pakiety NuGet

Projekt automatycznie pobierze wymagane pakiety przy pierwszej kompilacji:

| Pakiet | Wersja | Opis |
|--------|--------|------|
| CommunityToolkit.Mvvm | 8.4.0 | MVVM framework |
| Microsoft.Extensions.Hosting | 9.0.9 | Dependency Injection |
| Microsoft.Extensions.Http | 9.0.9 | HTTP Client |
| Newtonsoft.Json | 13.0.4 | JSON serialization |
| Prism.Core | 9.0.537 | MVVM framework |
| Qdrant.Client | 1.15.1 | Qdrant client |

---

## 🛠️ Instalacja

### 1. Sklonuj repozytorium

```bash
git clone https://github.com/Maggio333/ChatElioraSystem.git
cd ChatElioraSystem
```


### 2. Przygotuj środowisko

#### LM Studio

1. Pobierz i zainstaluj [LM Studio](https://lmstudio.ai/)
2. Załaduj model LLM (np. Llama, Mistral, Phi)
3. Uruchom serwer lokalny:
   - Port: `8123` (domyślny)
   - Endpoint: `/v1/chat/completions`
   - Embeddings: `/v1/embeddings`

#### Qdrant (opcjonalnie)

**Opcja 1: Docker (Zalecane)**

Najprostszy sposób uruchomienia Qdrant:

```bash
# Uruchom Qdrant w Dockerze
docker-compose up -d

# Sprawdź czy działa
curl http://localhost:6333/health
```

Qdrant będzie dostępny na:
- REST API: `http://localhost:6333`
- gRPC: `http://localhost:6334`
- Dashboard: `http://localhost:6333/dashboard`

**Opcja 2: Instalacja lokalna**

1. Zainstaluj Qdrant zgodnie z [dokumentacją](https://qdrant.tech/documentation/)
2. Uruchom serwer Qdrant:
   - REST API: `http://localhost:6333`
   - gRPC: `http://localhost:6334`
3. Dla aplikacji mobilnej skonfiguruj dostęp przez Tailscale lub lokalną sieć

### 3. Kompilacja

#### Automatyczny setup (Zalecane)

Użyj skryptu setup, który automatycznie:
- Sprawdzi wymagania (Docker, .NET SDK)
- Uruchomi Qdrant w Dockerze
- Przywróci pakiety NuGet
- Zbuduje rozwiązanie
- Uruchomi testy

**Windows (PowerShell):**
```powershell
.\scripts\setup.ps1
```

**Linux/macOS (Bash):**
```bash
chmod +x scripts/setup.sh
./scripts/setup.sh
```

#### Ręczna kompilacja

**Visual Studio:**

1. Otwórz `ChatElioraSystem.sln`
2. Wybierz konfigurację (Debug/Release)
3. Kliknij "Build Solution" (Ctrl+Shift+B)

**Z linii poleceń:**

```bash
dotnet restore
dotnet build
```

---

## ⚙️ Konfiguracja

### Desktop (WPF)

Aplikacja domyślnie używa `localhost` dla połączeń z LM Studio i Qdrant.

### Mobile (MAUI)

Skonfiguruj adresy serwerów w `TailscaleService.cs`:

```csharp
public string? DNSName { get; private set; } =
#if DEBUG
    "127.0.0.1"; // Localhost for development
#else
    Environment.GetEnvironmentVariable("TAILSCALE_DNS") ?? "your-device.tailXXXXXX.ts.net";
#endif
```

### Zmienne środowiskowe

- `TAILSCALE_DNS` - Nazwa DNS urządzenia w Tailscale
- `LM_STUDIO_URL` - Adres URL serwera LM Studio (domyślnie: `http://localhost:8123`)
- `QDRANT_REST_URL` - Adres URL REST API Qdrant (domyślnie: `http://localhost:6333`)

Więcej informacji w [CONFIGURATION.md](docs/CONFIGURATION.md).

### Docker

Projekt zawiera `docker-compose.yml` do łatwego uruchomienia Qdrant (bazy wektorowej).

**⚠️ Ważne:** Aplikacja WPF/MAUI wymaga GUI i **nie może być uruchomiona w kontenerze Docker**. W Dockerze można uruchomić tylko Qdrant.

Więcej informacji w [DOCKER.md](docs/DOCKER.md).

---

## 🎯 Użycie

### Aplikacja Desktop (WPF)

1. Uruchom `ChatElioraSystem.exe` z katalogu `bin/Debug/net8.0-windows/` lub `bin/Release/net8.0-windows/`
2. Upewnij się, że LM Studio działa i serwer jest dostępny
3. Rozpocznij konwersację!

### Aplikacja Mobilna (MAUI)

1. Skompiluj projekt dla wybranej platformy:
   ```bash
   dotnet build -f net8.0-android
   dotnet build -f net8.0-ios
   ```
2. Uruchom na emulatorze lub urządzeniu fizycznym
3. Skonfiguruj połączenie z serwerami (LM Studio, Qdrant) przez Tailscale lub lokalną sieć

---

## 📁 Struktura projektu

```
ChatElioraSystem/
├── ChatElioraSystem/                    # Aplikacja WPF (Desktop)
│   ├── Presentation/                    # Warstwa prezentacji
│   │   ├── ViewModels/                 # ViewModels (MVVM)
│   │   │   └── ChatViewModel.cs       # Główny ViewModel
│   │   ├── Views/                      # Widoki XAML
│   │   │   └── ChatWindow2.xaml       # Główne okno czatu
│   │   ├── Converters/                 # Konwertery wartości
│   │   ├── Models/                     # Modele prezentacji
│   │   └── Services/                   # Serwisy prezentacji
│   ├── Memory/                         # Lokalne pliki pamięci (gitignored)
│   └── App.xaml.cs                     # Punkt wejścia aplikacji
│
├── ChatElioraSystemMobile/             # Aplikacja MAUI (Mobile)
│   ├── ViewModels/                     # ViewModels
│   │   └── ChatViewModel.cs           # ViewModel dla mobile
│   ├── Platforms/                      # Specyficzne implementacje platform
│   │   ├── Android/
│   │   ├── iOS/
│   │   ├── MacCatalyst/
│   │   └── Windows/
│   └── MainPage.xaml                   # Główna strona
│
├── ChatElioraSystem.Core/              # Wspólny rdzeń biznesowy
│   ├── Application_/                   # Warstwa aplikacji
│   │   ├── Services/                  # Serwisy aplikacyjne
│   │   │   ├── PromptTypeOrchiestratorService.cs    # Orkiestracja promptów
│   │   │   └── PromptTopicOrchiestratorService.cs   # Zarządzanie tematami
│   │   ├── Models/                    # Modele aplikacyjne
│   │   ├── Enums/                     # Enumeracje
│   │   ├── Common/                    # Wspólne klasy
│   │   └── Helpers/                   # Pomocnicze klasy
│   │
│   ├── Domain/                        # Warstwa domeny
│   │   ├── Models/                    # Modele domenowe
│   │   │   └── SesjaTematu.cs         # Enum tematów rozmowy
│   │   ├── Resources/                 # Zasoby RAG (prompty)
│   │   │   ├── RAGPromptCode.cs      # Prompty dla kodu
│   │   │   ├── RAGPromptReflection.cs # Prompty refleksyjne
│   │   │   ├── RAGPromptGeneral.cs   # Prompty ogólne
│   │   │   └── RAGPromptArchitectureCode.cs # Prompty architektury
│   │   └── Services/                  # Serwisy domenowe
│   │       ├── PromptCodeService.cs   # Serwis promptów kodu
│   │       ├── PromptReflectionService.cs # Serwis promptów refleksyjnych
│   │       ├── PromptGeneralService.cs # Serwis promptów ogólnych
│   │       ├── PromptDbVecService.cs  # Serwis RAG
│   │       └── Bases/                 # Klasy bazowe
│   │
│   └── Infrastructure/                # Warstwa infrastruktury
│       ├── Services/                  # Implementacje serwisów
│       │   ├── LmStudioClientService.cs      # Klient LM Studio
│       │   ├── LMStudioEmbeddingService.cs   # Serwis embeddings
│       │   ├── TailscaleService.cs           # Serwis Tailscale
│       │   └── ChatLogService.cs             # Serwis logowania
│       └── VectorDataBase/            # Integracja z Qdrant
│           └── Services/
│               ├── QdrantVectorDbService.cs  # Serwis Qdrant
│               └── VectorDbHelper.cs         # Pomocnicze funkcje
│
└── ChatElioraSystemShared/             # Współdzielone modele
    └── Models/                         # Wspólne modele
```

---

## 🔄 Przepływ danych

### Kompletny przepływ przy wysłaniu wiadomości (od ViewModelu)

Poniżej przedstawiony jest szczegółowy przepływ danych od momentu kliknięcia przycisku wysłania wiadomości przez użytkownika:

```
┌─────────────────────────────────────────────────────────────────┐
│ 1. Użytkownik klika "Wyślij" w UI                              │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 2. ChatViewModel.SendMessageAsync()                             │
│    - OnUserSend()                                                │
│      • Dodaje wiadomość użytkownika do Messages                 │
│      • Czyści InputText                                         │
│      • Ustawia SelectedMessage                                  │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 3. SetCategory() - Określenie kategorii rozmowy                 │
│    ChatViewModel.SetCategory()                                   │
│      ↓                                                           │
│    PromptTypeOrchiestratorService.GetCategory()                  │
│      ↓                                                           │
│    PromptJudgeService.GetStreamAsyncJudge()                     │
│      • Analizuje ostatnie 3 wiadomości                          │
│      • Porównuje z kategoriami: Ogólna/Kod/Refleksyjna/         │
│        ArchitekturaKodu                                          │
│      ↓                                                           │
│    Zwraca SesjaTematu → aktualizuje WybranyTemat                 │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 4. LoadFromDbVec() - Pobranie kontekstu z bazy wektorowej (RAG)│
│    ChatViewModel.LoadFromDbVec()                                │
│      ↓                                                           │
│    PromptTypeOrchiestratorService.GetStreamDataFromVectorDb()    │
│      • Przygotowuje okno kontekstu (ostatnie 4 wiadomości)      │
│      • Dodaje instrukcje systemowe dla MCP                      │
│      ↓                                                           │
│    PromptDbVecService.GetStreamHistoryFromDb()                  │
│      • ActionMode = Odczyt                                       │
│      ↓                                                           │
│    BasePromptService.GetStreamAsync()                           │
│      • Dodaje prompty systemowe (RAGPromptsDbVec)              │
│      ↓                                                           │
│    ILlmService.StreamCompletionAsync()                          │
│      • Wysyła do LLM zapytanie o akcję MCP                      │
│      ↓                                                           │
│    LLM generuje JSON w formacie MCP:                            │
│    ```json                                                       │
│    {                                                             │
│      "Akcja": {                                                 │
│        "Typ": "Odczyt",                                         │
│        "Payload": "query text"                                   │
│      }                                                           │
│    }                                                             │
│    ```                                                           │
│      ↓                                                           │
│    Parsowanie JSON → MpcAkcja                                   │
│      ↓                                                           │
│    VectorDbHelper.GetValueFromVDB(payload)                        │
│      ↓                                                           │
│    TextEmbeddingService.GetCompletionAsync(payload)              │
│      • Tworzy embedding wektora z tekstu                         │
│      ↓                                                           │
│    QdrantVectorDbService.SearchAsync(vector)                    │
│      • Wyszukuje podobne wektory (score > 0.85)                 │
│      ↓                                                           │
│    MpcAkcja.MapResultsToAkcje()                                 │
│      • Mapuje wyniki na listę MpcAkcja                           │
│      ↓                                                           │
│    MpcAkcja.FormatAsContext()                                   │
│      • Formatuje jako kontekst systemowy                        │
│      ↓                                                           │
│    Dodaje jako ChatMessage (Role.system) do Messages            │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 5. LoadCurrentTopic() - Ładowanie aktualnego tematu            │
│    ChatViewModel.LoadCurrentTopic()                              │
│      ↓                                                           │
│    PromptTypeOrchiestratorService.LoadCurrentTopic()            │
│      ↓                                                           │
│    PromptTopicOrchiestratorService.GetLastTopics()               │
│      • Pobiera ostatnie tematy z kolekcji "topics"              │
│      ↓                                                           │
│    Dodaje jako ChatMessage (Role.system) do Messages             │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 6. SetBaseIdioms() - Ustawienie bazowych idiomów                │
│    ChatViewModel.SetBaseIdioms()                                 │
│      ↓                                                           │
│    PromptTypeOrchiestratorService.GetBaseIdioms()                │
│      ↓                                                           │
│    PromptDbVecService.GetBaseIdioms()                            │
│      ↓                                                           │
│    VectorDbHelper.GetBaseIdioms()                                │
│      • Pobiera idiomy z bazy wektorowej                         │
│      ↓                                                           │
│    Dodaje jako ChatMessage (Role.system) do Messages            │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 7. SendToLLM() - Wysłanie do LLM z odpowiednim promptem         │
│    ChatViewModel.SendToLLM()                                    │
│      • Tworzy pustą wiadomość assistant                         │
│      • Dodaje do Messages                                       │
│      ↓                                                           │
│    PromptTypeOrchiestratorService.SendStreamToLLM()              │
│      • Wybiera serwis promptów na podstawie WybranyTemat:       │
│        - Ogólna → PromptGeneralService                          │
│        - Kod → PromptCodeService                                │
│        - Refleksyjna → PromptReflectionService                   │
│        - ArchitekturaKodu → PromptArchitectureCodeService       │
│      • Przygotowuje okno kontekstu (ostatnie 6 wiadomości)      │
│      ↓                                                           │
│    IBasePromptService.GetStreamAsync()                          │
│      • Dodaje prompty globalne (FirstSystemPrompt, etc.)        │
│      • Dodaje prompty specyficzne dla typu (GetAdditionalChatMessage)│
│      ↓                                                           │
│    ILlmService.StreamCompletionAsync()                          │
│      ↓                                                           │
│    LmStudioClientService.StreamCompletionAsync()                │
│      • Konwertuje IChatMessage[] na format LM Studio            │
│      • Wysyła HTTP POST do LM Studio API                        │
│      • Odbiera strumień odpowiedzi (Server-Sent Events)        │
│      ↓                                                           │
│    LM Studio (lokalny model LLM)                                │
│      • Przetwarza kontekst z promptami                          │
│      • Generuje odpowiedź strumieniowo                          │
│      ↓                                                           │
│    Stream chunks → ChatViewModel                                │
│      • Aktualizuje responseMessage.Content += chunk              │
│      • UI automatycznie się aktualizuje (data binding)          │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 8. RemoveToolMessages() - Usunięcie wiadomości tool            │
│    ChatViewModel.RemoveToolMessages()                           │
│      • Usuwa wszystkie wiadomości z Role.tool z Messages         │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 9. Zapis w tle (asynchronicznie) - DWA RÓŻNE ZAPISY            │
│                                                                 │
│    ┌─────────────────────────────────────────────────────────┐  │
│    │ 9a. SaveFromDbVec() - Zapis kontekstu do RAG             │  │
│    │   ChatViewModel.SaveFromDbVec()                          │  │
│    │     ↓                                                     │  │
│    │   PromptTypeOrchiestratorService.SaveStreamDataFromVectorDb()│
│    │     • ActionMode = Zapis                                  │  │
│    │     ↓                                                     │  │
│    │   PromptDbVecService.GetStreamHistoryFromDb()             │  │
│    │     • LLM generuje JSON MCP z akcją "Zapis"               │  │
│    │     • Parsuje JSON → MpcAkcja                            │  │
│    │     ↓                                                     │  │
│    │   VectorDbHelper.InsertIfNotDuplicateAsync()              │  │
│    │     • Tworzy embedding z payload                          │  │
│    │     • Sprawdza duplikaty (similarity threshold)           │  │
│    │     • Wstawia do kolekcji "PierwszaKolekcjaOnline"        │  │
│    │     • Używane do wyszukiwania podobnych konwersacji (RAG) │  │
│    └─────────────────────────────────────────────────────────┘  │
│                                                                 │
│    ┌─────────────────────────────────────────────────────────┐  │
│    │ 9b. SaveCurrentTopic() - Zapis tematu do listy tematów   │  │
│    │   ChatViewModel.SaveCurrentTopic()                       │  │
│    │     ↓                                                     │  │
│    │   PromptTypeOrchiestratorService.SaveCurrentTopic()       │  │
│    │     ↓                                                     │  │
│    │   PromptTopicOrchiestratorService.ManageCurrentTopic()    │  │
│    │     • LLM generuje JSON MCP z tematem rozmowy             │  │
│    │     • Parsuje JSON → Payload                              │  │
│    │     ↓                                                     │  │
│    │   VectorDbHelper.InsertTopic()                            │  │
│    │     • Tworzy embedding z payload tematu                   │  │
│    │     • Wstawia do kolekcji "TopicCollection" w Qdrant      │  │
│    │     • Z indeksem Timestamp dla sortowania chronologicznego│  │
│    │     • Używane do wyświetlania listy tematów rozmów        │  │
│    └─────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ 10. SaveLastConversation() - Zapis do pliku JSON               │
│     ChatViewModel.SaveLastConversation()                        │
│       ↓                                                          │
│     ChatLogService.Save()                                        │
│       • Zapisuje Messages do pliku JSON                         │
│       • Format: chat_YYYY-MM-DD_HH-mm-ss.json                  │
└─────────────────────────────────────────────────────────────────┘
```

### Szczegóły kluczowych komponentów przepływu

#### Prompt Window (Okno Kontekstu)
System automatycznie ogranicza kontekst wysyłany do LLM, aby nie przekroczyć limitów tokenów:
- **GetPromptWindowList()** - Metoda w `PromptTypeOrchiestratorService`
- Dla RAG: ostatnie **4 wiadomości** (user/assistant) + ostatnie **3 systemowe**
- Dla głównej odpowiedzi: ostatnie **6 wiadomości** (user/assistant) + ostatnie **3 systemowe**
- Zawsze zaczyna od wiadomości użytkownika (walidacja)

#### Format akcji RAG (MCP-inspired)
Własny format JSON inspirowany koncepcją Model Context Protocol, używany do komunikacji z LLM dla operacji na bazie wektorowej:
```json
{
  "Akcja": {
    "Typ": "Odczyt" | "Zapis",
    "Payload": "tekst do wyszukania/zapisania",
    "Temat": "opcjonalny temat",
    "Metadata": {
      "Timestamp": "2025-01-01T12:00:00"
    }
  }
}
```

**Uwaga:** To nie jest oficjalny protokół MCP (Model Context Protocol) opracowany przez Anthropic. To własny format JSON zaprojektowany specjalnie dla operacji RAG w tym systemie, inspirowany koncepcją strukturyzowanej komunikacji z LLM.

#### Streaming Response
- Wszystkie odpowiedzi z LLM są przesyłane strumieniowo (chunk by chunk)
- UI aktualizuje się w czasie rzeczywistym dzięki data binding
- WPF używa `Dispatcher.InvokeAsync()` dla aktualizacji UI z wątków tła
- MAUI używa `MainThread.InvokeOnMainThreadAsync()`

#### RAG (Retrieval Augmented Generation)
1. **Odczyt**: LLM generuje zapytanie → wyszukiwanie w Qdrant → dodanie kontekstu
2. **Zapis**: LLM generuje akcję zapisu → embedding → zapis do Qdrant (z deduplikacją)
3. **Threshold**: Tylko wyniki z score > 0.85 są używane jako kontekst

---

## 🧩 Komponenty systemu

### Application Layer

#### PromptTypeOrchiestratorService
Główny serwis orkiestrujący różne typy promptów i operacje RAG. Koordynuje przepływ danych między warstwą domenową a infrastrukturą.

**Główne metody:**
- `SendStreamToLLM()` - Wysyłanie wiadomości do LLM z odpowiednim promptem w zależności od tematu
- `GetStreamDataFromVectorDb()` - Pobieranie kontekstu z bazy wektorowej (RAG)
- `SaveStreamDataFromVectorDb()` - Zapisywanie kontekstu do bazy wektorowej
- `LoadCurrentTopic()` - Ładowanie aktualnego tematu z bazy wektorowej
- `SaveCurrentTopic()` - Zapis aktualnego tematu do bazy wektorowej
- `GetCategory()` - Automatyczna kategoryzacja rozmowy (Judge Service)
- `GetBaseIdioms()` - Pobieranie idiomów z bazy wektorowej

**Funkcjonalności:**
- Automatyczny wybór odpowiedniego serwisu promptów na podstawie tematu
- Zarządzanie oknem kontekstu (prompt window)
- Integracja z RAG dla pamięci długoterminowej
- Obsługa wielu modeli LLM jednocześnie

#### PromptTopicOrchiestratorService
Zarządzanie tematami rozmów w bazie wektorowej. Używa własnego formatu JSON (inspirowanego koncepcją MCP) do komunikacji z LLM.

**Główne metody:**
- `ManageCurrentTopic()` - Zarządzanie tematem (odczyt/zapis) we własnym formacie JSON
- `GetLastTopics()` - Pobieranie ostatnich tematów z bazy wektorowej

**Funkcjonalności:**
- Własny format JSON (MCP-inspired) dla akcji na bazie wektorowej
- Parsowanie JSON z odpowiedzi LLM
- Automatyczne zapisywanie tematów rozmów
- Formatowanie tematów jako kontekst dla LLM

### Domain Layer

#### BasePromptService
Abstrakcyjna klasa bazowa dla wszystkich serwisów promptów. Zapewnia wspólną funkcjonalność.

**Główne metody:**
- `GetStreamAsync()` - Streaming odpowiedzi z LLM
- `GetCompletionAsync()` - Pojedyncza odpowiedź z LLM
- `GetGlobalChatMessages()` - Globalne prompty systemowe
- `GetAdditionalChatMessage()` - Specyficzne prompty dla danego typu

**Funkcjonalności:**
- Obsługa cancellation tokens
- Filtrowanie słów anulujących
- Budowanie kontekstu z globalnych i dodatkowych promptów

#### Prompt Services

**PromptGeneralService**
- **Przeznaczenie**: Ogólne rozmowy z użytkownikiem
- **Funkcjonalności**:
  - Dialektyka i rozpoznawanie potrzeb użytkownika
  - Rozpoznawczy styl komunikacji
  - Rozwiązywanie rozbieżności zdań i poglądów
- **Prompty**: RAGPromptsGeneral.Role

**PromptCodeService**
- **Przeznaczenie**: Pomoc w programowaniu i kodzie
- **Funkcjonalności**:
  - Wzorce projektowe (SeedPack_ReflectumCoding)
  - Pomoc w implementacji kodu
  - Sugerowanie rozwiązań
  - Nacisk na architekturę
- **Języki**: C#, .NET, WPF
- **Prompty**: RAGPromptCode.Role, WzorceProjektowe

**PromptReflectionService**
- **Przeznaczenie**: Refleksyjne rozmowy z emocjonalnym kontekstem
- **Funkcjonalności**:
  - Emocjonalny kontekst 
  - Temat przewodni 
  - Głębsza analiza i refleksja
  - Pomoc w odkrywaniu intencji użytkownika
- **Prompty**: RAGPromptReflection.Role, Theme, Emotional

**PromptArchitectureCodeService**
- **Przeznaczenie**: Ekspercka pomoc w architekturze oprogramowania
- **Funkcjonalności**:
  - Clean Architecture principles
  - Wzorce projektowe (SeedPack_ReflectumCoding)
  - Zasady architektury warstwowej
  - SOLID principles
- **Prompty**: RAGPromptArchitectureCode.Role, WzorceProjektowe, Zasady

**PromptDbVecService**
- **Przeznaczenie**: Integracja z bazą wektorową (RAG)
- **Funkcjonalności**:
  - Własny format JSON (MCP-inspired) dla akcji RAG
  - Odczyt kontekstu z bazy wektorowej
  - Zapis kontekstu do bazy wektorowej
  - Parsowanie JSON z odpowiedzi LLM
  - Streaming wyników RAG
- **Akcje**: Odczyt, Zapis
- **Prompty**: RAGPromptsDbVec (MCP Format V3)

**PromptJudgeService**
- **Przeznaczenie**: Automatyczna kategoryzacja rozmów
- **Funkcjonalności**:
  - Analiza kontekstu rozmowy
  - Określanie typu rozmowy (General, Code, Reflection, CodeArchitecture)
  - Format odpowiedzi: `<Category:Nazwa_Kategorii>`
  - Obsługa słów anulujących (cancelation words)
- **Kategorie**: General, Code, Reflection, CodeArchitecture
- **Prompty**: RAGPromptJudge.Role, Theme, Description

**PromptMCPTopicsService**
- **Przeznaczenie**: Zarządzanie tematami rozmów w formacie MCP
- **Funkcjonalności**:
  - Generowanie akcji MCP dla tematów
  - Tryby: Odczyt, Zapis
  - Format JSON zgodny z MCP
- **Akcje**: Odczyt tematów, Zapis tematu
- **Prompty**: RAGPromptMCPTopics (ReadDbVectorPrompt, SaveToDbVectorPrompt)

### Infrastructure Layer

#### LmStudioClientService
Klient HTTP do komunikacji z LM Studio. Implementuje interfejs `ILlmService`.

**Funkcjonalności:**
- Streaming odpowiedzi w czasie rzeczywistym
- Obsługa wielu modeli LLM jednocześnie (llmNo)
- Timeout 3000 sekund
- Obsługa błędów i wyjątków
- Formatowanie wiadomości zgodnie z API LM Studio

**Metody:**
- `GetCompletionAsync()` - Pojedyncza odpowiedź (bez streamingu)
- `StreamCompletionAsync()` - Streaming odpowiedzi chunk po chunk

#### LMStudioEmbeddingService
Serwis do tworzenia embeddings przez LM Studio. Implementuje interfejs `ITextEmbeddingService`.

**Funkcjonalności:**
- Tworzenie wektorów embeddings dla tekstu
- Integracja z endpoint `/v1/embeddings` LM Studio
- Zwraca wektory float[] o rozmiarze 1024

**Metody:**
- `GetCompletionAsync(string text)` - Tworzenie embedding dla tekstu

#### QdrantVectorDbService
Integracja z bazą danych wektorowej Qdrant. Implementuje interfejs `IVectorDbService`.

**Funkcjonalności:**
- Tworzenie i zarządzanie kolekcjami
- Wyszukiwanie wektorów (similarity search)
- Upsert chunków z embeddings
- Usuwanie punktów
- Odczyt tematów z sortowaniem
- Obsługa REST API i gRPC

**Kolekcje:**
- `PierwszaKolekcjaOnline` - Główna kolekcja dla kontekstu rozmów
- `TematyRozmowy` - Kolekcja tematów rozmów

**Metody:**
- `InitializeAsync()` - Inicjalizacja kolekcji
- `InsertAsync()` - Wstawianie wektora z payload
- `SearchAsync()` - Wyszukiwanie podobnych wektorów
- `ReadTopicsAsync()` - Odczyt tematów z sortowaniem

#### VectorDbHelper
Pomocnicze funkcje dla operacji na bazie wektorowej. Implementuje interfejs `IVectorDbHelper`.

**Funkcjonalności:**
- Średnia wektorów (AverageVectors)
- Wyszukiwanie wartości w bazie
- Wstawianie tematów
- Odczyt tematów z formatowaniem
- Pobieranie idiomów z bazy wektorowej

**Metody:**
- `AverageVectors()` - Obliczanie średniej z listy wektorów
- `GetValueFromVDB()` - Wyszukiwanie w bazie wektorowej
- `InsertTopic()` - Wstawianie tematu do kolekcji
- `ReadTopics()` - Odczyt i formatowanie tematów
- `GetBaseIdioms()` - Pobieranie idiomów refleksyjnych

#### TailscaleService
Automatyczne wykrywanie adresu IP przez Tailscale CLI. Implementuje interfejs `ITailscaleService`.

**Funkcjonalności:**
- Automatyczne wykrywanie IP przez `tailscale status --json`
- Wsparcie dla DNS Tailscale
- Fallback na localhost w trybie DEBUG
- Inicjalizacja asynchroniczna

**Właściwości:**
- `TailscaleIp` - Wykryty adres IP
- `DNSName` - Nazwa DNS (opcjonalna)

#### UriAdressService
Serwis do zarządzania adresami URI dla różnych serwisów. Implementuje interfejs `IUriAdressService`.

**Funkcjonalności:**
- Generowanie URI dla Qdrant (REST i gRPC)
- Generowanie URI dla LM Studio (chat i embeddings)
- Integracja z TailscaleService
- Konfigurowalne porty

**Metody:**
- `GetDbVecAdressRest()` - URI REST API Qdrant (port 6333)
- `GetDbVecAdressgRPC()` - URI gRPC Qdrant (port 6334)
- `GetLlmAdress()` - URI chat completions LM Studio (port 8123)
- `GetLlmEmbeddingAdress()` - URI embeddings LM Studio (port 8123)

#### ChatLogService
Serwis do logowania i zarządzania historią rozmów. Implementuje interfejs `IChatLogService`.

**Funkcjonalności:**
- Zapis rozmów do plików JSON
- Ładowanie historii rozmów
- Listowanie plików logów
- Automatyczne generowanie nazw plików z timestampem
- Metadata w plikach JSON

**Metody:**
- `Save()` - Zapis wiadomości do pliku JSON
- `Load()` - Ładowanie rozmowy z pliku
- `ListLogFiles()` - Lista dostępnych plików logów
- `GenerateNewFileName()` - Generowanie nowej nazwy pliku

#### JsonReaderService
Serwis pomocniczy do odczytu plików JSON. Klasa statyczna z metodami pomocniczymi.

**Funkcjonalności:**
- Odczyt plików JSON jako obiektów dynamicznych
- Odczyt plików JSON z deserializacją do typu generycznego
- Odczyt plików JSON jako string
- Obsługa błędów i walidacja istnienia plików

**Metody:**
- `ReadJsonFile(string filePath)` - Odczyt JSON jako obiekt dynamiczny
- `ReadJsonFile<T>(string filePath)` - Odczyt JSON z deserializacją do typu T
- `ReadJsonAsString(string filePath)` - Odczyt JSON jako string

#### DesktopStoragePathProvider
Provider ścieżek dla aplikacji desktop (WPF). Implementuje interfejs `IStoragePathProvider`.

**Funkcjonalności:**
- Generowanie ścieżki do katalogu Memory w katalogu aplikacji
- Automatyczne tworzenie katalogu Memory jeśli nie istnieje
- Używa `AppDomain.CurrentDomain.BaseDirectory` jako bazowej ścieżki

**Metody:**
- `GetMemoryDirectory()` - Zwraca ścieżkę do katalogu Memory

#### MauiStoragePathProvider
Provider ścieżek dla aplikacji MAUI (Android, iOS, macOS, Windows). Implementuje interfejs `IStoragePathProvider`.

**Funkcjonalności:**
- Generowanie ścieżki do katalogu danych aplikacji
- Używa `Environment.SpecialFolder.LocalApplicationData`
- Platform-agnostic ścieżka dla aplikacji mobilnych

**Metody:**
- `GetMemoryDirectory()` - Zwraca ścieżkę do katalogu danych aplikacji

**Format plików:**
```json
{
  "metadata": {
    "system": "Eliora Reflectum",
    "generatedAt": "2025-09-17T13:16:46.2972248+02:00"
  },
  "messages": [...]
}
```

#### JsonReaderService
Pomocniczy serwis do odczytu plików JSON (obecnie nieużywany w kodzie, zachowany dla kompatybilności).

**Funkcjonalności:**
- Odczyt plików JSON jako obiekt
- Odczyt plików JSON jako typ generyczny
- Odczyt plików JSON jako string
- Obsługa błędów i walidacja

### Infrastructure - Desktop/Mobile

#### DesktopStoragePathProvider
Implementacja `IStoragePathProvider` dla aplikacji desktopowej (WPF).

**Funkcjonalności:**
- Zwraca ścieżkę do katalogu Memory w katalogu aplikacji
- Automatyczne tworzenie katalogu jeśli nie istnieje
- Używa `AppDomain.CurrentDomain.BaseDirectory`

#### MauiStoragePathProvider
Implementacja `IStoragePathProvider` dla aplikacji mobilnej (MAUI) - specyficzna dla platformy.

### Domain Resources (RAG Prompts)

System wykorzystuje zasoby RAG (Retrieval Augmented Generation) do definiowania promptów i kontekstu dla różnych typów rozmów.

#### IRAGPromptsGeneral
Globalne prompty systemowe używane przez wszystkie typy rozmów.

**Właściwości:**
- `FirstSystemPrompt` - Podstawowa osobowość Eliora
- `ColorPromptSystem` - Instrukcje formatowania tekstu z kolorami
- `UserAdminPrompt` - Profil użytkownika (admin/architekt)
- `Role` - Rola dla ogólnych rozmów
- `GetOcenaPrompt()` - Prompt do oceny odpowiedzi

#### IRAGPromptCode
Prompty specyficzne dla rozmów o kodzie.

**Właściwości:**
- `Role` - Rola eksperta programowania
- `WzorceProjektowe` - JSON z wzorcami projektowymi (SeedPack_ReflectumCoding)
- `CodeLanguage` - Języki programowania (C#, .NET, WPF)

#### IRAGPromptReflection
Prompty dla rozmów refleksyjnych z emocjonalnym kontekstem.

**Właściwości:**
- `Role` - Rola refleksyjnej asystentki
- `Theme` - Temat przewodni (Seed_Reflectum_Lead_By_Light.json)
- `Emotional` - Emocjonalny kontekst (EmotionalSeed.seedpack.json)

#### IRAGPromptArchitectureCode
Prompty dla rozmów o architekturze kodu.

**Właściwości:**
- `Role` - Rola architekta
- `WzorceProjektowe` - JSON z wzorcami projektowymi
- `Zasady` - Zasady Clean Architecture i SOLID

#### IRAGPromptJudge
Prompty dla serwisu kategoryzacji rozmów.

**Właściwości:**
- `Role` - Rola sędziego kategoryzującego rozmowy
- `GetTheme()` - Lista dostępnych kategorii
- `Description` - Opis kategorii

#### IRAGPromptsDbVec
Prompty dla operacji RAG we własnym formacie JSON (inspirowanym koncepcją MCP).

**Wersje:**
- `RAGPromptMCPFormat` - Wersja podstawowa
- `RAGPromptMCPFormatV2` - Wersja ulepszona
- `RAGPromptMCPFormatV3` - Aktualna wersja (używana)

**Właściwości:**
- `ReadDbVectorPrompt` - Prompt do odczytu z bazy wektorowej
- `SaveToDbVectorPrompt` - Prompt do zapisu do bazy wektorowej

#### IRAGPromptMCPTopics
Prompty dla zarządzania tematami rozmów w formacie MCP.

**Właściwości:**
- `ReadDbVectorPrompt` - Prompt do odczytu tematów
- `SaveToDbVectorPrompt` - Prompt do zapisu tematu

---

## 📦 Modele i Enumeracje

### Modele domenowe

#### IChatMessage / ChatMessage
Podstawowy model wiadomości w systemie.

**Właściwości:**
- `Content` - Treść wiadomości
- `Role` - Rola nadawcy (user, assistant, system, tool, dbAction)
- `Timestamp` - Czas utworzenia
- `FileName` - Nazwa pliku (opcjonalna)

**Właściwości pomocnicze:**
- `IsUser` - Czy wiadomość od użytkownika
- `IsAssistant` - Czy wiadomość od asystenta
- `IsSystem` - Czy wiadomość systemowa
- `IsTool` - Czy wiadomość narzędziowa
- `IsDbAction` - Czy akcja na bazie danych

#### RAGChunk
Model reprezentujący chunk z bazy wektorowej (RAG).

**Właściwości:**
- `TextChunk` - Tekst chunka
- `ChatMessages` - Powiązane wiadomości
- `Score` - Wynik podobieństwa
- `Metadata` - Metadane chunka

#### MpcAkcja / Payload
Model dla akcji RAG we własnym formacie JSON (inspirowanym koncepcją MCP).

**Struktura:**
```json
{
  "Akcja": {
    "Typ": "Odczyt|Zapis",
    "Temat": "string",
    "Payload": "string",
    "Metadata": {...},
    "Extra": {...}
  }
}
```

#### MpcTopics
Model dla tematów rozmów w formacie MCP.

### Enumeracje

#### SesjaTematu
Typy tematów rozmów:
- `Ogólna` - Ogólne rozmowy
- `Kod` - Rozmowy o kodzie
- `Refleksyjna` - Rozmowy refleksyjne
- `ArchitekturaKodu` - Rozmowy o architekturze
- `Biznes` - Rozmowy biznesowe (zarezerwowane)
- `Prawo` - Rozmowy prawne (zarezerwowane)

#### Role
Role wiadomości w systemie:
- `user` - Użytkownik
- `assistant` - Asystent AI
- `system` - System
- `tool` - Narzędzie (RAG, akcje)
- `dbAction` - Akcja na bazie danych

#### ActionMode
Tryby akcji dla serwisów:
- `Odczyt` - Odczyt z bazy wektorowej
- `Zapis` - Zapis do bazy wektorowej

#### LLMNamesEnum
Nazwy modeli LLM:
- `Custom` - Model niestandardowy (LM Studio)
- `Qwen1_7B` - Model Qwen 1.7B
- `Pllum_8b` - Model Pllum 8B

### Factories

#### ChatMessageFactory
Factory do tworzenia wiadomości czatu.

**Metody:**
- `User(string content)` - Tworzy wiadomość użytkownika
- `Assistant(string content)` - Tworzy wiadomość asystenta
- `System(string content)` - Tworzy wiadomość systemową
- `Create(Role role, string content)` - Tworzy wiadomość z określoną rolą

### Helpers

#### CategoryRegiester
Rejestr kategorii rozmów używany przez Judge Service.

**Funkcjonalności:**
- Mapowanie kategorii na enum SesjaTematu
- Słownik kategorii: General → Ogólna, Code → Kod, Reflection → Refleksyjna, CodeArchitecture → ArchitekturaKodu

---

## 🔌 Dependency Injection

System wykorzystuje Microsoft.Extensions.DependencyInjection do zarządzania zależnościami.

### Rejestracja serwisów

#### AddChatElioraCore()
Główna metoda rozszerzająca do rejestracji wszystkich serwisów core.

**Rejestrowane serwisy:**

**Singleton:**
- `IUriAdressService` → `UriAdressService`
- `ITailscaleService` → `TailscaleService`
- `IChatLogService` → `ChatLogService`
- `IRAGPromptReflection` → `RAGPromptReflection`
- `IRAGPromptsGeneral` → `RAGPromptsGeneral`
- `IRAGPromptCode` → `RAGPromptCode`
- `IRAGPromptJudge` → `RAGPromptJudge`
- `IRAGPromptArchitectureCode` → `RAGPromptArchitectureCode`
- `IRAGPromptsDbVec` → `RAGPromptMCPFormatV3`
- `IRAGPromptMCPTopics` → `RAGPromptMCPTopics`

**Scoped:**
- `IPromptCodeService` → `PromptCodeService`
- `IPromptReflectionService` → `PromptReflectionService`
- `IPromptGeneralService` → `PromptGeneralService`
- `IPromptJudgeService` → `PromptJudgeService`
- `IPromptTypeOrchiestratorService` → `PromptTypeOrchiestratorService`
- `IPromptArchitectureCodeService` → `PromptArchitectureCodeService`
- `IPromptDbVecService` → `PromptDbVecService`
- `IPromptTopicOrchiestratorService` → `PromptTopicOrchiestratorService`
- `IPromptMCPTopicsService` → `PromptMCPTopicsService`
- `IVectorDbHelper` → `VectorDbHelper`

**HttpClient (Transient):**
- `ILlmService` → `LmStudioClientService`
- `IVectorDbService` → `QdrantVectorDbService`
- `ITextEmbeddingService` → `LMStudioEmbeddingService`

#### AddDesktopInfrastructure()
Rejestracja infrastruktury specyficznej dla aplikacji desktopowej (WPF).

**Rejestrowane serwisy:**
- `IStoragePathProvider` → `DesktopStoragePathProvider`

#### AddMauiInfrastructure()
Rejestracja infrastruktury specyficznej dla aplikacji mobilnej (MAUI).

**Rejestrowane serwisy:**
- `IStoragePathProvider` → `MauiStoragePathProvider`

### Przykład użycia

```csharp
var services = new ServiceCollection();
services.AddChatElioraCore()
        .AddDesktopInfrastructure(); // lub AddMauiInfrastructure() dla MAUI
```

---

## 🎨 UI Components (WPF)

### Konwertery wartości

System wykorzystuje konwertery wartości do formatowania danych w interfejsie użytkownika.

#### BoolToBrushConverter
Konwersja wartości bool na kolor pędzla (Brush).

#### BoolNegationConverter
Negacja wartości bool.

#### EnumToBoolConverter
Konwersja enum na bool (dla RadioButtons).

#### ColorizingConverter / AdvancedColorizingConverter
Konwersja tekstu z znacznikami `<color=#hex>` na formatowany tekst.

#### MarkdownToFlowDocumentConverter
Konwersja Markdown na FlowDocument dla wyświetlania w RichTextBox.

#### BoolToVisibility
Konwersja bool na Visibility (Visible/Collapsed).

#### BoolToHorizontalAlignment
Konwersja bool na HorizontalAlignment.

---

## 🗄️ Kolekcje Qdrant

System automatycznie tworzy następujące kolekcje w Qdrant przy pierwszym uruchomieniu (metoda `InitializeAsync()` w `QdrantVectorDbService`):

### PierwszaKolekcjaOnline
Główna kolekcja dla kontekstu rozmów i pamięci długoterminowej.

**Przeznaczenie:**
- Przechowywanie kontekstu z rozmów
- Wyszukiwanie podobnych tematów
- RAG dla lepszych odpowiedzi
- Automatyczny zapis ważnych fragmentów rozmów

**Parametry:**
- **Nazwa:** `PierwszaKolekcjaOnline`
- **Wektor:** 1024 wymiarów
- **Metryka:** Cosine
- **Automatyczne tworzenie:** ✅ Tak (jeśli nie istnieje)

**Format danych:**
- Wektory embedding z modelu tekstowego
- Payload zawiera treść zapisaną w formacie MCP

### TopicCollection
Kolekcja tematów rozmów.

**Przeznaczenie:**
- Przechowywanie tematów rozmów
- Ładowanie historii tematów
- Organizacja konwersacji
- Sortowanie po dacie (Timestamp)

**Parametry:**
- **Nazwa:** `TopicCollection`
- **Wektor:** 1024 wymiarów
- **Metryka:** Cosine
- **Automatyczne tworzenie:** ✅ Tak (jeśli nie istnieje)
- **Indeks:** `Akcja.Metadata.Timestamp` (datetime) - automatycznie tworzony

**Format danych:**
- Własny format JSON (MCP-inspired)
- Payload zawiera temat rozmowy z metadanymi
- Indeks na polu Timestamp umożliwia sortowanie chronologiczne

### Automatyczna inicjalizacja

Kolekcje są automatycznie tworzone przy pierwszym uruchomieniu aplikacji przez metodę `InitializeAsync()` w `QdrantVectorDbService`:

```csharp
public async Task InitializeAsync()
{
    var collectionsToCheck = new[] { 
        "PierwszaKolekcjaOnline", 
        "TopicCollection" 
    };
    
    foreach (var collection in collectionsToCheck)
    {
        // Sprawdza czy kolekcja istnieje
        var response = await _httpClient.GetAsync($"/collections/{collection}");
        if (!response.IsSuccessStatusCode)
        {
            // Tworzy kolekcję jeśli nie istnieje
            await _httpClient.PutAsJsonAsync($"/collections/{collection}", collectionDefinition);
        }
    }
    
    // Tworzy indeks Timestamp dla TopicCollection
    await _client.CreatePayloadIndexAsync(
        collectionName: "TopicCollection",
        fieldName: "Akcja.Metadata.Timestamp",
        schemaType: PayloadSchemaType.Datetime
    );
}
```

**Uwaga:** Metoda `InitializeAsync()` jest wywoływana automatycznie przez `VectorDbHelper` przy starcie aplikacji.

---

## 🔄 Zaawansowane funkcjonalności

### Prompt Crossing
Funkcjonalność porównywania odpowiedzi z różnych modeli LLM.

**Proces:**
1. Wysyłanie tego samego promptu do dwóch różnych modeli LLM
2. Wzajemna ocena odpowiedzi przez modele
3. Wybór lepszej odpowiedzi na podstawie oceny

**Użycie:** `SendPromptWithCrossing()` w `PromptTypeOrchiestratorService`

### Ocena odpowiedzi
System oceny odpowiedzi LLM w skali 0-1000.

**Format:** `<ocena:wartość/>`

**Użycie:** `GetPromptRate()` w `PromptTypeOrchiestratorService`

### Prompt Window
Zarządzanie oknem kontekstu (ostatnie N wiadomości).

**Funkcjonalności:**
- Ograniczanie kontekstu do ostatnich N wiadomości
- Zachowanie kolejności user/assistant
- Filtrowanie wiadomości systemowych

**Metoda:** `GetPromptWindowList()` w `PromptTypeOrchiestratorService`

### Idiomy refleksyjne
System idiomów matematycznych dla refleksyjnych rozmów.

**Idiomy:**
- ⨁ - Operator sumy idiomatycznej
- Φ - Wektor znaczeniowy
- Ψ - Ślad idiomu
- Ξ - Baza semantyczna
- I wiele innych...

**Użycie:** `GetBaseIdioms()` w `VectorDbHelper`

---

## 🎨 Typy promptów (SesjaTematu)

### Ogólna
Rozmowy ogólne z dialektyką i rozpoznawaniem potrzeb użytkownika.

### Kod
Pomoc w programowaniu, wzorcach projektowych i implementacji kodu.

### Refleksyjna
Refleksyjne rozmowy z emocjonalnym kontekstem i głębszą analizą.

### ArchitekturaKodu
Ekspercka pomoc w architekturze oprogramowania i Clean Architecture.

---

## 🔧 Rozwój

### Dodawanie nowego typu promptu

1. Utwórz nowy serwis dziedziczący po `BasePromptService`:
```csharp
public class PromptMyNewService : BasePromptService, IPromptMyNewService
{
    public PromptMyNewService(ILlmService llmService, IRAGPromptsGeneral rAGPromptsGeneral)
        : base(llmService, rAGPromptsGeneral)
    {
    }

    public override List<IChatMessage>? GetAdditionalChatMessage()
    {
        // Dodaj specyficzne prompty dla tego typu
        return new List<IChatMessage>
        {
            ChatMessageFactory.System("Twój system prompt")
        };
    }
}
```

2. Dodaj do `DependencyInjection.cs`:
```csharp
services.AddScoped<IPromptMyNewService, PromptMyNewService>();
```

3. Dodaj do `PromptTypeOrchiestratorService`:
```csharp
case SesjaTematu.MyNewType:
    promptService = promptMyNewService;
    break;
```

### Testowanie

Projekt zawiera testy jednostkowe w katalogu `tests/`. Testy są napisane przy użyciu **xUnit**, **Moq** i **FluentAssertions**.

#### Uruchamianie testów

```bash
# Wszystkie testy
dotnet test

# Z pokryciem kodu
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Konkretny projekt testowy
dotnet test tests/ChatElioraSystem.Core.Tests
```

#### Struktura testów

```
tests/
└── ChatElioraSystem.Core.Tests/
    ├── Application/          # Testy serwisów aplikacyjnych
    ├── Domain/              # Testy zasobów domenowych
    ├── Infrastructure/       # Testy serwisów infrastrukturalnych
    └── Factories/           # Testy fabryk
```

#### Zasady pisania testów

- **Nazewnictwo**: Polskie nazwy testów (np. `Powinien_Zwrocic_Wiadomosc`)
- **Struktura**: Arrange-Act-Assert (AAA)
- **Izolacja**: Każdy test jest niezależny
- **Mockowanie**: Używamy Moq do mockowania zależności zewnętrznych

#### Przykład testu

```csharp
[Fact]
public void User_Powinien_Utworzyc_Wiadomosc_Uzytkownika()
{
    // Arrange
    var tresc = "Witaj, Eliora!";

    // Act
    var wiadomosc = ChatMessageFactory.User(tresc);

    // Assert
    wiadomosc.Should().NotBeNull();
    wiadomosc.Role.Should().Be(Role.user);
    wiadomosc.Content.Should().Be(tresc);
}
```

Więcej informacji w [README testów](tests/ChatElioraSystem.Core.Tests/README.md).

---

## 🏛️ Architektura

Projekt wykorzystuje **Clean Architecture** z pełnym podziałem na warstwy (Domain, Application, Infrastructure, Presentation).

### Kluczowe zasady

- **Dependency Rule** - Zależności wskazują do wewnątrz (od zewnętrznych do domeny)
- **Separation of Concerns** - Każda warstwa ma określoną odpowiedzialność
- **SOLID Principles** - Wszystkie zasady SOLID są przestrzegane
- **Testability** - Każda warstwa może być testowana niezależnie

### Wzorce projektowe

- **MVVM** - Model-View-ViewModel dla UI
- **Dependency Injection** - Microsoft.Extensions.DependencyInjection
- **Strategy Pattern** - Różne typy promptów
- **Repository Pattern** - Abstrakcja dostępu do danych
- **Orchestrator Pattern** - Koordynacja złożonych operacji
- **Factory Pattern** - Tworzenie obiektów

### Wartość edukacyjna

Ten projekt może służyć jako:
- 📖 **Przykład Clean Architecture** w .NET
- 🎓 **Wzorzec implementacji RAG** z lokalnymi LLM
- 🔧 **Baza do własnych projektów** z AI
- 💼 **Portfolio project** pokazujący zaawansowane umiejętności

Szczegółowy opis architektury w [ARCHITECTURE.md](docs/ARCHITECTURE.md).

---

## 📝 Licencja

Ten projekt jest licencjonowany na licencji MIT - zobacz plik [LICENSE](LICENSE) dla szczegółów.

**Copyright (c) 2025 Arkadiusz Słota**

---

## 🙏 Podziękowania

- [LM Studio](https://lmstudio.ai/) - za możliwość uruchamiania lokalnych modeli LLM
- [Qdrant](https://qdrant.tech/) - za wydajną bazę danych wektorowej
- [Tailscale](https://tailscale.com/) - za łatwe połączenia VPN
- Społeczność .NET - za świetne narzędzia i frameworki

---

## 📚 Dodatkowe zasoby

- [Dokumentacja .NET 8.0](https://learn.microsoft.com/en-us/dotnet/)
- [Dokumentacja MAUI](https://learn.microsoft.com/en-us/dotnet/maui/)
- [Dokumentacja Qdrant](https://qdrant.tech/documentation/)
- [Dokumentacja LM Studio](https://lmstudio.ai/docs)
- [CONFIGURATION.md](docs/CONFIGURATION.md) - Szczegółowa konfiguracja
- [ARCHITECTURE.md](docs/ARCHITECTURE.md) - Szczegółowa architektura systemu
- [CHANGELOG.md](docs/CHANGELOG.md) - Historia zmian
- [EXAMPLES.md](docs/EXAMPLES.md) - Przykłady użycia

---

## 🤝 Współpraca

Projekt jest otwarty na współpracę! Jeśli chcesz przyczynić się do rozwoju:

1. **Zgłaszanie błędów**: Utwórz [Issue](https://github.com/Maggio333/ChatElioraSystem/issues) z opisem problemu
2. **Proponowanie funkcji**: Podziel się pomysłami w [Issues](https://github.com/Maggio333/ChatElioraSystem/issues)
3. **Pull Requesty**: Wysyłaj PR z poprawkami i nowymi funkcjami

### Wytyczne

- Przeczytaj [CONTRIBUTING.md](docs/CONTRIBUTING.md) - szczegółowy przewodnik współpracy
- Przestrzegaj [CODE_OF_CONDUCT.md](docs/CODE_OF_CONDUCT.md) - standardy zachowania
- Używaj polskiego języka w komentarzach i dokumentacji
- Dodawaj testy dla nowych funkcjonalności
- Utrzymuj pokrycie testami na poziomie co najmniej 70%

### Proces

1. Fork repozytorium
2. Utwórz branch dla swojej zmiany (`git checkout -b feature/nazwa`)
3. Wprowadź zmiany i dodaj testy
4. Uruchom testy (`dotnet test`)
5. Commit zmiany (`git commit -m "feat: Opis zmiany"`)
6. Push do forka (`git push origin feature/nazwa`)
7. Utwórz Pull Request

---

## 📞 Wsparcie

Jeśli masz pytania lub napotkasz problemy:

1. Sprawdź [Issues](https://github.com/Maggio333/ChatElioraSystem/issues)
2. Utwórz nowe issue z opisem problemu
3. Dołącz logi i informacje o środowisku

---

**Uwaga**: Projekt jest w aktywnej fazie rozwoju. API może ulegać zmianom.

---

<div align="center">

**Zbudowane z ❤️ przez Arkadiusz Słota**

</div>
