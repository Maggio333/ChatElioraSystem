using ChatElioraSystem.Core.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Infrastructure.Desktop
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDesktopInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IStoragePathProvider, DesktopStoragePathProvider>();
            return services;
        }
    }
}
