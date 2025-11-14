# Testy jednostkowe - ChatElioraSystem

Ten katalog zawiera testy jednostkowe dla projektu ChatElioraSystem.Core.

## Struktura testów

```
ChatElioraSystem.Core.Tests/
├── Application/
│   └── Services/          # Testy serwisów warstwy aplikacyjnej
├── Domain/
│   └── Resources/         # Testy zasobów domenowych
├── Infrastructure/
│   ├── Desktop/           # Testy infrastruktury desktopowej
│   └── Services/          # Testy serwisów infrastrukturalnych
└── Factories/             # Testy fabryk
```

## Uruchamianie testów

### Visual Studio
1. Otwórz Test Explorer (Test → Test Explorer)
2. Kliknij "Run All Tests"

### .NET CLI
```bash
dotnet test
```

### Z pokryciem kodu
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Używane biblioteki

- **xUnit** - Framework testowy
- **Moq** - Mockowanie zależności
- **FluentAssertions** - Asercje czytelne w języku naturalnym

## Zasady pisania testów

1. **Nazewnictwo**: Używamy polskich nazw dla testów (np. `Powinien_Zwrocic_Wiadomosc`)
2. **Struktura**: Arrange-Act-Assert (AAA)
3. **Izolacja**: Każdy test powinien być niezależny
4. **Mockowanie**: Używamy Moq do mockowania zależności zewnętrznych
5. **Czytelność**: Testy powinny być łatwe do zrozumienia

## Przykład testu

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

## Pokrycie kodu

Dążymy do pokrycia testami:
- ✅ Kluczowych serwisów (80%+)
- ✅ Fabryk i helperów (90%+)
- ✅ Logiki biznesowej (70%+)

## Dodawanie nowych testów

1. Utwórz plik testowy w odpowiednim katalogu
2. Nazwij klasę testową: `{KlasaDoTestowania}Tests`
3. Użyj atrybutu `[Fact]` dla testów jednostkowych
4. Użyj `[Theory]` i `[InlineData]` dla testów parametryzowanych
5. Mockuj zależności zewnętrzne (pliki, sieć, bazy danych)

