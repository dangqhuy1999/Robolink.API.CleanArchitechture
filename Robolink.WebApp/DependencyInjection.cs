
using Refit;
using Robolink.Application;
using Robolink.Infrastructure;
using Robolink.WebApp.Shared.Services.Clients;
using Robolink.WebApp.Shared.Services.Staffs;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Services;
using Robolink.WebApp.Modules.ProjectManagement.Features.SystemPhases.Services;
using Robolink.WebApp.Modules.ProjectManagement.Features.ProjectPhases.Services;
using Robolink.WebApp.Modules.ProjectManagement.Features.PhaseTasks.Services;
using Robolink.WebApp.Shared.Services.ApiError;
using Robolink.WebApp.Shared.Services.NotificationService;
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
            var apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7120";

            /* 3. Đăng ký các Interface từ tầng Shared
            services.AddRefitClient<IProjectApi>(settings)
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

            services.AddRefitClient<IProjectPhaseApi>(settings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
            */
            services.AddRefitClient<IProjectApi>(settings)
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
            services.AddRefitClient<IProjectPhaseApi>(settings)
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
            services.AddRefitClient<ISystemPhaseApi>(settings)
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
            services.AddRefitClient<IPhaseTaskApi>(settings)
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
            services.AddRefitClient<IClientApi>(settings)
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));
            services.AddRefitClient<IStaffApi>(settings)
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl));

            // Add these BEFORE builder.Build()
            services.AddScoped<IToastNotificationService, ToastNotificationService>();
            services.AddScoped<IApiErrorHandler, ApiErrorHandler>();

            services.AddScoped<IProjectService, ProjectService>();
            return services;
        }
    }
}
