using ChatElioraSystem.Core;
using ChatElioraSystem.Core.Infrastructure.Maui;
using ChatElioraSystemMobile.ViewModels;

namespace ChatElioraSystemMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddChatElioraCore().AddMauiInfrastructure();

            builder.Services.AddSingleton<IChatViewModel, ChatViewModel>();
            builder.Services.AddSingleton<MainPage>();


            // 💡 Rejestracja serwisów

            //// Jeżeli masz serwisy takie jak:
            //builder.Services.AddSingleton<IPromptTypeOrchiestratorService, DummyPromptService>();
            //builder.Services.AddSingleton<IChatViewConfigService, DummyViewConfig>();
            //builder.Services.AddSingleton<IChatLogService, DummyChatLogService>();


            return builder.Build();
        }
    }
}

