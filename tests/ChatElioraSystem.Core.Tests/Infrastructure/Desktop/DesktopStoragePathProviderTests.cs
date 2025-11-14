using ChatElioraSystem.Core.Infrastructure.Desktop;
using FluentAssertions;
using Xunit;

namespace ChatElioraSystem.Core.Tests.Infrastructure.Desktop;

/// <summary>
/// Testy jednostkowe dla DesktopStoragePathProvider
/// </summary>
public class DesktopStoragePathProviderTests
{
    [Fact]
    public void GetMemoryDirectory_Powinien_Zwrocic_Sciezke_Do_Katalogu_Memory()
    {
        // Arrange
        var provider = new DesktopStoragePathProvider();

        // Act
        var sciezka = provider.GetMemoryDirectory();

        // Assert
        sciezka.Should().NotBeNullOrEmpty();
        sciezka.Should().Contain("Memory");
        Directory.Exists(sciezka).Should().BeTrue();
    }

    [Fact]
    public void GetMemoryDirectory_Powinien_Utworzyc_Katalog_Jeśli_Nie_Istnieje()
    {
        // Arrange
        var provider = new DesktopStoragePathProvider();
        var sciezka = provider.GetMemoryDirectory();

        // Act - usuń katalog jeśli istnieje
        if (Directory.Exists(sciezka))
        {
            Directory.Delete(sciezka, recursive: true);
        }

        // Wywołaj ponownie - powinien utworzyć katalog
        sciezka = provider.GetMemoryDirectory();

        // Assert
        Directory.Exists(sciezka).Should().BeTrue();
    }

    [Fact]
    public void GetMemoryDirectory_Powinien_Zwracac_Ta_Sama_Sciezke_Przy_Wielokrotnych_Wywolaniach()
    {
        // Arrange
        var provider = new DesktopStoragePathProvider();

        // Act
        var sciezka1 = provider.GetMemoryDirectory();
        var sciezka2 = provider.GetMemoryDirectory();

        // Assert
        sciezka1.Should().Be(sciezka2);
    }
}

