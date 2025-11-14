using ChatElioraSystem.Core.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChatElioraSystem.Core.Infrastructure.Maui
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMauiInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IStoragePathProvider, MauiStoragePathProvider>();
            return services;
        }
    }
}
