using Microsoft.Extensions.DependencyInjection;
using Weikio.ApiFramework.Abstractions.DependencyInjection;
using Weikio.ApiFramework.SDK;

namespace Weikio.ApiFramework.Plugins.Epassi
{
    public static class ServiceExtensions
    {
        public static IApiFrameworkBuilder AddEpassi(this IApiFrameworkBuilder builder, string endpoint = null, EpassiOptions configuration = null)
        {
            builder.Services.AddEpassi(endpoint, configuration);

            return builder;
        }

        public static IServiceCollection AddEpassi(this IServiceCollection services, string endpoint = null, EpassiOptions configuration = null)
        {
            services.RegisterPlugin(endpoint, configuration);

            return services;
        }
    }
}
