using ChatElioraSystem.Core.Application_.Common.Factories;
using ChatElioraSystem.Core.Infrastructure.Models;
using FluentAssertions;
using Xunit;

namespace ChatElioraSystem.Core.Tests.Factories;

/// <summary>
/// Testy jednostkowe dla ChatMessageFactory
/// </summary>
public class ChatMessageFactoryTests
{
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
        wiadomosc.Timestamp.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Assistant_Powinien_Utworzyc_Wiadomosc_Asystenta()
    {
        // Arrange
        var tresc = "Witaj! Jak mogę pomóc?";

        // Act
        var wiadomosc = ChatMessageFactory.Assistant(tresc);

        // Assert
        wiadomosc.Should().NotBeNull();
        wiadomosc.Role.Should().Be(Role.assistant);
        wiadomosc.Content.Should().Be(tresc);
    }

    [Fact]
    public void System_Powinien_Utworzyc_Wiadomosc_Systemowa()
    {
        // Arrange
        var tresc = "To jest wiadomość systemowa";

        // Act
        var wiadomosc = ChatMessageFactory.System(tresc);

        // Assert
        wiadomosc.Should().NotBeNull();
        wiadomosc.Role.Should().Be(Role.system);
        wiadomosc.Content.Should().Be(tresc);
    }

    [Fact]
    public void Create_Powinien_Utworzyc_Wiadomosc_Z_Podanymi_Parametrami()
    {
        // Arrange
        var rola = Role.user;
        var tresc = "Testowa wiadomość";

        // Act
        var wiadomosc = ChatMessageFactory.Create(rola, tresc);

        // Assert
        wiadomosc.Should().NotBeNull();
        wiadomosc.Role.Should().Be(rola);
        wiadomosc.Content.Should().Be(tresc);
    }

    [Fact]
    public void Collection_Powinien_Utworzyc_Kolekcje_Wiadomosci_W_Prawidlowej_Kolejnosci()
    {
        // Arrange
        var trescUzytkownika = "Pytanie użytkownika";
        var trescAsystenta = "Odpowiedź asystenta";
        var trescSystemowa = "Instrukcja systemowa";

        // Act
        var kolekcja = ChatMessageFactory.Collection<List<IChatMessage>>(
            trescUzytkownika, 
            trescAsystenta, 
            trescSystemowa
        );

        // Assert
        kolekcja.Should().NotBeNull();
        kolekcja.Should().HaveCount(3);
        
        kolekcja.ElementAt(0).Role.Should().Be(Role.system);
        kolekcja.ElementAt(0).Content.Should().Be(trescSystemowa);
        
        kolekcja.ElementAt(1).Role.Should().Be(Role.assistant);
        kolekcja.ElementAt(1).Content.Should().Be(trescAsystenta);
        
        kolekcja.ElementAt(2).Role.Should().Be(Role.user);
        kolekcja.ElementAt(2).Content.Should().Be(trescUzytkownika);
    }
}

