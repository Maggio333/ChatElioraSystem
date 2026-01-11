using ChatElioraSystem.Core.Application_.Enums;
using ChatElioraSystem.Core.Application_.Models;
using ChatElioraSystem.Core.Application_.Services;
using ChatElioraSystem.Core.Domain.Models;
using ChatElioraSystem.Core.Domain.Resources;
using ChatElioraSystem.Core.Domain.Services;
using ChatElioraSystem.Core.Infrastructure.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChatElioraSystem.Core.Tests.Application.Services;

/// <summary>
/// Testy jednostkowe dla PromptTypeOrchiestratorService
/// </summary>
public class PromptTypeOrchiestratorServiceTests
{
    private readonly Mock<IPromptGeneralService> _mockPromptGeneralService;
    private readonly Mock<IPromptCodeService> _mockPromptCodeService;
    private readonly Mock<IPromptReflectionService> _mockPromptReflectionService;
    private readonly Mock<IPromptJudgeService> _mockPromptJudgeService;
    private readonly Mock<IPromptArchitectureCodeService> _mockPromptArchitectureCodeService;
    private readonly Mock<IPromptDbVecService> _mockPromptDbVecService;
    private readonly Mock<IPromptTopicOrchiestratorService> _mockPromptTopicOrchiestratorService;
    private readonly Mock<ICategoryRegiester> _mockCategoryRegister;
    private readonly PromptTypeOrchiestratorService _service;

    public PromptTypeOrchiestratorServiceTests()
    {
        _mockPromptGeneralService = new Mock<IPromptGeneralService>();
        _mockPromptCodeService = new Mock<IPromptCodeService>();
        _mockPromptReflectionService = new Mock<IPromptReflectionService>();
        _mockPromptJudgeService = new Mock<IPromptJudgeService>();
        _mockPromptArchitectureCodeService = new Mock<IPromptArchitectureCodeService>();
        _mockPromptDbVecService = new Mock<IPromptDbVecService>();
        _mockPromptTopicOrchiestratorService = new Mock<IPromptTopicOrchiestratorService>();
        _mockCategoryRegister = new Mock<ICategoryRegiester>();

        // Setup mock CategoryRegister
        _mockCategoryRegister.Setup(x => x.Categories).Returns(new List<CategoryModel>());
        _mockCategoryRegister.Setup(x => x.GetCategoriesList()).Returns(new List<string>());

        _service = new PromptTypeOrchiestratorService(
            _mockPromptGeneralService.Object,
            _mockPromptCodeService.Object,
            _mockPromptReflectionService.Object,
            _mockPromptJudgeService.Object,
            _mockPromptArchitectureCodeService.Object,
            _mockPromptDbVecService.Object,
            _mockPromptTopicOrchiestratorService.Object,
            _mockCategoryRegister.Object
        );
    }

    [Fact]
    public void IsSaveToDbVector_Powinien_Miec_Domyslna_Wartosc_False()
    {
        // Assert
        _service.IsSaveToDbVector.Should().BeFalse();
    }

    [Fact]
    public void IsSaveToDbVector_Powinien_Pozwolic_Na_Ustawienie_Wartosci()
    {
        // Act
        _service.IsSaveToDbVector = true;

        // Assert
        _service.IsSaveToDbVector.Should().BeTrue();
    }

    [Fact]
    public async Task GetBaseIdioms_Powinien_Zwrocic_Wiadomosc_Systemowa()
    {
        // Arrange
        var oczekiwanyIdiom = "Test idiom";
        _mockPromptDbVecService
            .Setup(s => s.GetBaseIdioms())
            .ReturnsAsync(oczekiwanyIdiom);

        // Act
        var wynik = await _service.GetBaseIdioms();

        // Assert
        wynik.Should().NotBeNull();
        wynik.Content.Should().Be(oczekiwanyIdiom);
        wynik.Role.Should().Be(Role.system);
    }

    [Fact]
    public async Task LoadCurrentTopic_Powinien_Wywolac_PromptTopicOrchiestratorService()
    {
        // Arrange
        var oczekiwanyTemat = "Testowy temat";
        _mockPromptTopicOrchiestratorService
            .Setup(s => s.GetLastTopics())
            .ReturnsAsync(oczekiwanyTemat);

        // Act
        var wynik = await _service.LoadCurrentTopic();

        // Assert
        wynik.Should().Be(oczekiwanyTemat);
        _mockPromptTopicOrchiestratorService.Verify(s => s.GetLastTopics(), Times.Once);
    }
}

