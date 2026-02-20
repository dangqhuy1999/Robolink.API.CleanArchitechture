
using Robolink.Application;

namespace Robolink.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationDI()
                .AddInfrastructureDI(configuration);
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    // Tránh lỗi vòng lặp nếu em có quan hệ cha-con phức tạp
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            return services;
        }
    }
}
