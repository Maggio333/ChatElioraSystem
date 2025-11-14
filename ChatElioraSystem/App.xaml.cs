using ChatElioraSystem.Core;
using ChatElioraSystem.Core.Infrastructure.Desktop;
using ChatElioraSystem.Presentation.Converters;
using ChatElioraSystem.Presentation.ViewModels;
using ChatElioraSystem.Presentation.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace ChatElioraSystem
{


    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost Host { get; private set; } = null!;

        public App()
        {
            InitializeComponent();

            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);

                })
                .Build();   

            //var converter = 
            //Application.Current.Resources["BoolToBrushConverter"] = _host.Services.GetRequiredService<BooleanToBrushConverter>(); ;

            ConfigureConverters(new BoolToBrushConverter());
            ConfigureConverters(new BoolNegationConverter(), "NegateBool");
            ConfigureConverters(new EnumToBoolConverter());

            // Otwieramy pierwsze okno – pobieramy je z DI 
            var mainWindow = Host.Services.GetRequiredService<ChatWindow2>();

            //Host.Services.GetRequiredService<IThemeService>().Apply(this);

            //VectorDbTest();

            MainWindow = mainWindow;
            MainWindow.Show();


        }

        private void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<IChatViewModel, ChatViewModel>();
            //services.AddTransient<ChatWindow>();
            services.AddTransient<ChatWindow2>();
            services.AddSingleton<BoolToBrushConverter>();
            services.AddSingleton<BoolNegationConverter>();
            services.AddSingleton<EnumToBoolConverter>();

            services.AddChatElioraCore().AddDesktopInfrastructure();

        }

        //private async void VectorDbTest()
        //{
        //    using var scope = Host.Services.CreateScope();
        //    var tester = scope.ServiceProvider.GetRequiredService<VectorDbTester>();
        //    await tester.TestAsync();
        //}


        private void ConfigureConverters<T>(T converter, string name) where T : class => Application.Current.Resources[name] = Host.Services.GetRequiredService<T>();       
        private void ConfigureConverters<T>(T converter) where T : class
        {
            var type = converter.GetType();
            var name = type.Name;
            ConfigureConverters(converter, name);
        }

        //private void ConfigureServices(IServiceCollection services)
        //{

        //    services.AddScoped<ILlmService, LmStudioClientService>();
        //    //services.AddScoped<ILlmService, LmStudioClientStreamService>();

        //    services.AddScoped<IJudgeCategoryService, JudgeCategoryService>();

        //    services.AddHttpClient<IVectorDbService, QdrantVectorDbService>();            

        //    services.AddHttpClient<ITextEmbeddingService, LMStudioEmbeddingService>();
        //    services.AddSingleton<IChatLogService, ChatLogService>();

        //    services.AddScoped<IPromptCodeService, PromptCodeService>();
        //    services.AddScoped<IPromptReflectionService, PromptReflectionService>();
        //    services.AddScoped<IPromptGeneralService, PromptGeneralService>();
        //    services.AddScoped<IPromptJudgeService, PromptJudgeService>();
        //    services.AddScoped<IPromptTypeOrchiestratorService, PromptTypeOrchiestratorService>();
        //    services.AddScoped<IPromptArchitectureCodeService, PromptArchitectureCodeService>();
        //    services.AddScoped<IPromptDbVecService, PromptDbVecService>();


        //    services.AddSingleton<IRAGPromptReflection, RAGPromptReflection>();
        //    services.AddSingleton<IRAGPromptsGeneral, RAGPromptsGeneral>();
        //    services.AddSingleton<IRAGPromptCode, RAGPromptCode>();
        //    services.AddSingleton<IRAGPromptJudge, RAGPromptJudge>();
        //    services.AddSingleton<IRAGPromptArchitectureCode, RAGPromptArchitectureCode>();
        //    services.AddSingleton<BoolToHorizontalAlignment>();

        //    //services.AddSingleton<IBgTaskQueueService, BgTaskQueueService>();

        //    services.AddScoped<IRAGPromptsDbVec, RAGPromptMCPFormat>();
        //    //services.AddScoped<IRAGPromptsDbVec, RAGPromptsDbVec>();


        //    services.AddScoped<IVectorDbHelper, VectorDbHelper>();


        //    services.AddSingleton<IChatViewModel, ChatViewModel>();
        //    services.AddTransient<ChatWindow>();
        //    services.AddTransient<ChatWindow2>();

        //    //services.AddSingleton<IThemeService, ThemeService>();

        //    //services.AddSingleton<ReflectumVerticalScrollBar>
        //    services.AddSingleton<BoolToHorizontalAlignment>();
        //    services.AddSingleton<BoolToBrushConverter>();
        //    services.AddSingleton<BoolNegationConverter>();
        //    services.AddSingleton<EnumToBoolConverter>();
        //    services.AddSingleton<BoolToVisibility>();


        //    //testy
        //    services.AddTransient<VectorDbHelper>();
        //    //testy

        //}
    }
}
