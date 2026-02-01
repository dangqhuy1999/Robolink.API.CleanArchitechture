
using Robolink.Application;

namespace Robolink.WebApp
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services)
        {
            services.AddApplicationDI()
                .AddInfrastructureDI();

            return services;
        }
    }
}
