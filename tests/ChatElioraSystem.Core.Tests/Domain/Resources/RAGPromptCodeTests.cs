using ChatElioraSystem.Core.Domain.Resources;
using FluentAssertions;
using Xunit;

namespace ChatElioraSystem.Core.Tests.Domain.Resources;

/// <summary>
/// Testy jednostkowe dla RAGPromptCode
/// </summary>
public class RAGPromptCodeTests
{
    [Fact]
    public void Konstruktor_Powinien_Zainicjalizowac_WzorceProjektowe()
    {
        // Act
        var prompt = new RAGPromptCode();

        // Assert
        prompt.Should().NotBeNull();
        prompt.WzorceProjektowe.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void WzorceProjektowe_Powinien_Zawierac_JSON_Z_Wzorami()
    {
        // Arrange
        var prompt = new RAGPromptCode();

        // Act
        var wzorce = prompt.WzorceProjektowe;

        // Assert
        wzorce.Should().NotBeNullOrEmpty();
        wzorce.Should().Contain("SeedPack_ReflectumCoding");
        wzorce.Should().Contain("version");
        wzorce.Should().Contain("author");
    }

    [Fact]
    public void CodeLanguage_Powinien_Zwrocic_Prawidlowy_Jezyk()
    {
        // Arrange
        var prompt = new RAGPromptCode();

        // Act
        var jezyk = prompt.CodeLanguage;

        // Assert
        jezyk.Should().Be("C#, .Net, WPF");
    }

    [Fact]
    public void Role_Powinien_Zawierac_Opis_Roli()
    {
        // Arrange
        var prompt = new RAGPromptCode();

        // Act
        var rola = prompt.Role;

        // Assert
        rola.Should().NotBeNullOrEmpty();
        rola.Should().Contain("ekspertem w programowaniu");
        rola.Should().Contain("architektur");
    }
}

