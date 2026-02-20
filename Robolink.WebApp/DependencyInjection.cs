
using Refit;
using Robolink.Application;
using Robolink.Infrastructure;
using Robolink.Shared.Interfaces.API.PhaseTasks;
using Robolink.Shared.Interfaces.API.Clients;
using Robolink.Shared.Interfaces.API.Staffs;
namespace Robolink.WebApp
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationDI()
                .AddInfrastructureDI(configuration);

            // CHỈNH THÊM: Cấu hình Serializer để dùng NewtonsoftJson đã cài
            var settings = new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer()
            };

            // Lấy BaseUrl từ cấu hình (để sau này deploy không phải sửa code)
            var apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001";

            /* 3. Đăng ký các Interface từ tầng Shared
            services.AddRefitClient<IProjectApi>(settings)
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

            services.AddRefitClient<IProjectPhaseApi>(settings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
            */
            services.AddRefitClient<IPhaseTaskApi>(settings)
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
            services.AddRefitClient<IClientApi>(settings)
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
            services.AddRefitClient<IStaffApi>(settings)
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));


            return services;
        }
    }
}
