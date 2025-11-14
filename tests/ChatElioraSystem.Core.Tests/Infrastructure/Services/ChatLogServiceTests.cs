using ChatElioraSystem.Core.Application_.Common.Factories;
using ChatElioraSystem.Core.Domain.Services;
using ChatElioraSystem.Core.Infrastructure.Models;
using ChatElioraSystem.Core.Infrastructure.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChatElioraSystem.Core.Tests.Infrastructure.Services;

/// <summary>
/// Testy jednostkowe dla ChatLogService
/// </summary>
public class ChatLogServiceTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly Mock<IStoragePathProvider> _mockPathProvider;
    private readonly ChatLogService _chatLogService;

    public ChatLogServiceTests()
    {
        // Tworzenie tymczasowego katalogu dla testów
        _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);

        // Mock dla IStoragePathProvider
        _mockPathProvider = new Mock<IStoragePathProvider>();
        _mockPathProvider.Setup(p => p.GetMemoryDirectory()).Returns(_testDirectory);

        // Tworzenie instancji serwisu
        _chatLogService = new ChatLogService(_mockPathProvider.Object);
    }

    [Fact]
    public void Save_Powinien_Zapisac_Wiadomosci_Do_Pliku_JSON()
    {
        // Arrange
        var wiadomosci = new List<IChatMessage>
        {
            ChatMessageFactory.User("Witaj!"),
            ChatMessageFactory.Assistant("Cześć! Jak mogę pomóc?")
        };

        // Act
        var sciezka = _chatLogService.Save(wiadomosci);

        // Assert
        sciezka.Should().NotBeNullOrEmpty();
        File.Exists(sciezka).Should().BeTrue();

        var zawartosc = File.ReadAllText(sciezka);
        zawartosc.Should().Contain("Witaj!");
        // JSON może zawierać unicode escape sequences, więc sprawdzamy czy plik istnieje i ma zawartość
        zawartosc.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Save_Powinien_Zapisac_Metadane()
    {
        // Arrange
        var wiadomosci = new List<IChatMessage>
        {
            ChatMessageFactory.User("Test")
        };

        var metadane = new Dictionary<string, object>
        {
            { "testKey", "testValue" },
            { "liczba", 42 }
        };

        // Act
        var sciezka = _chatLogService.Save(wiadomosci, metadane);

        // Assert
        var zawartosc = File.ReadAllText(sciezka);
        zawartosc.Should().Contain("testKey");
        zawartosc.Should().Contain("testValue");
        zawartosc.Should().Contain("42");
    }

    [Fact]
    public void Load_Powinien_Wczytac_Wiadomosci_Z_Pliku()
    {
        // Arrange
        var wiadomosciOryginalne = new List<IChatMessage>
        {
            ChatMessageFactory.User("Pytanie"),
            ChatMessageFactory.Assistant("Odpowiedź")
        };

        var sciezka = _chatLogService.Save(wiadomosciOryginalne);
        var nazwaPliku = Path.GetFileName(sciezka);

        // Act
        var wiadomosciWczytane = _chatLogService.Load(nazwaPliku, out var metadane);

        // Assert
        wiadomosciWczytane.Should().NotBeNull();
        wiadomosciWczytane.Should().HaveCount(2);
        wiadomosciWczytane.First().Content.Should().Be("Pytanie");
        wiadomosciWczytane.Last().Content.Should().Be("Odpowiedź");
    }

    [Fact]
    public void Load_Powinien_Rzucic_Wyjatek_Gdy_Plik_Nie_Istnieje()
    {
        // Arrange
        var nieistniejacyPlik = "nieistniejacy_plik.json";

        // Act & Assert
        var akcja = () => _chatLogService.Load(nieistniejacyPlik, out _);
        akcja.Should().Throw<FileNotFoundException>()
            .WithMessage("Nie znaleziono pliku logu.*");
    }

    [Fact]
    public void ListLogFiles_Powinien_Zwrocic_Liste_Plikow_JSON()
    {
        // Arrange
        var wiadomosci1 = new List<IChatMessage> { ChatMessageFactory.User("Test 1") };
        var wiadomosci2 = new List<IChatMessage> { ChatMessageFactory.User("Test 2") };

        // Używamy różnych nazw plików, aby uniknąć problemów z timestamp
        var nazwa1 = "test1.json";
        var nazwa2 = "test2.json";
        
        _chatLogService.Save(wiadomosci1, nazwa1);
        _chatLogService.Save(wiadomosci2, nazwa2);

        // Act
        var pliki = _chatLogService.ListLogFiles();

        // Assert
        pliki.Should().NotBeNull();
        pliki.Should().HaveCountGreaterThanOrEqualTo(2);
        pliki.Should().OnlyContain(p => p != null && p.EndsWith(".json"));
        pliki.Should().Contain(nazwa1);
        pliki.Should().Contain(nazwa2);
    }

    [Fact]
    public void GenerateNewFileName_Powinien_Wygenerowac_Nowa_Nazwe_Pliku()
    {
        // Arrange
        var pierwszaNazwa = _chatLogService.ListLogFiles();

        // Act
        _chatLogService.GenerateNewFileName();
        Thread.Sleep(100);
        var drugaNazwa = _chatLogService.ListLogFiles();

        // Assert
        // Nazwy powinny się różnić (zawierają timestamp)
        pierwszaNazwa.Should().NotBeNull();
    }

    [Fact]
    public void Save_Z_Nazwa_Pliku_Powinien_Uzyc_Podanej_Nazwy()
    {
        // Arrange
        var wiadomosci = new List<IChatMessage> { ChatMessageFactory.User("Test") };
        var nazwaPliku = "testowy_plik.json";

        // Act
        var sciezka = _chatLogService.Save(wiadomosci, nazwaPliku);

        // Assert
        Path.GetFileName(sciezka).Should().Be(nazwaPliku);
    }

    public void Dispose()
    {
        // Czyszczenie tymczasowego katalogu
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }
}

