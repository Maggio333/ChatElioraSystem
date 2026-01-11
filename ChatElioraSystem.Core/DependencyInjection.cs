using ChatElioraSystem.Core.Application_.Models;
using ChatElioraSystem.Core.Application_.Services;
using ChatElioraSystem.Core.Domain.Resources;
using ChatElioraSystem.Core.Domain.Services;
using ChatElioraSystem.Core.Infrastructure.Services;
using ChatElioraSystem.Core.Infrastructure.VectorDataBase.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChatElioraSystem.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddChatElioraCore(this IServiceCollection services)
        {
            // Core services — Application, Domain, Infrastructure
            services.AddSingleton<IUriAdressService, UriAdressService>();
            services.AddSingleton<ITailscaleService, TailscaleService>();

            services.AddHttpClient<ILlmService, LmStudioClientService>();

            services.AddHttpClient<IVectorDbService, QdrantVectorDbService>();

            services.AddHttpClient<ITextEmbeddingService, LMStudioEmbeddingService>();

            //services.AddScoped<IChatViewConfigService, ChatViewConfigService>();

            //services.AddHttpClient<IVectorDbService, QdrantVectorDbService>();
            //services.AddHttpClient<ITextEmbeddingService, LMStudioEmbeddingService>();


            services.AddSingleton<IChatLogService, ChatLogService>();

            services.AddScoped<IPromptCodeService, PromptCodeService>();
            services.AddScoped<IPromptReflectionService, PromptReflectionService>();
            services.AddScoped<IPromptGeneralService, PromptGeneralService>();
            services.AddScoped<IPromptJudgeService, PromptJudgeService>();
            services.AddScoped<IPromptTypeOrchiestratorService, PromptTypeOrchiestratorService>();
            services.AddScoped<IPromptArchitectureCodeService, PromptArchitectureCodeService>();
            services.AddScoped<IPromptDbVecService, PromptDbVecService>();
            services.AddScoped<IPromptTopicOrchiestratorService, PromptTopicOrchiestratorService>();
            services.AddScoped<IPromptMCPTopicsService, PromptMCPTopicsService>();

            
            services.AddSingleton<IRAGPromptReflection, RAGPromptReflection>();
            services.AddSingleton<IRAGPromptsGeneral, RAGPromptsGeneral>();
            services.AddSingleton<IRAGPromptCode, RAGPromptCode>();
            services.AddSingleton<IRAGPromptJudge, RAGPromptJudge>();
            services.AddSingleton<IRAGPromptArchitectureCode, RAGPromptArchitectureCode>();
            //services.AddScoped<IRAGPromptsDbVec, RAGPromptsDbVec>();
            //services.AddScoped<IRAGPromptsDbVec, RAGPromptMCPFormat>();
            //services.AddSingleton<IRAGPromptsDbVec, RAGPromptMCPFormatV2>();
            services.AddSingleton<IRAGPromptsDbVec, RAGPromptMCPFormatV3>();
            services.AddSingleton<IRAGPromptMCPTopics, RAGPromptMCPTopics>();

            services.AddSingleton<ICategoryRegiester, CategoryRegister>();

            //services.AddScoped<IJudgeCategoryService, JudgeCategoryService>();


            services.AddScoped<IVectorDbHelper, VectorDbHelper>();

            return services;
        }
    }
}