using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Robolink.Core.Interfaces;
using Robolink.Infrastructure.Data;
using Robolink.Infrastructure.Repositories;

namespace Robolink.Application
{
    public static class DependencyInjection
    {
        // ✅ ADD IConfiguration parameter
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. ✅ FIXED: Read connection string from appsettings.json
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 2. Đăng ký Factory (Dùng cho triệu dòng/Virtualize/Đa luồng)
            services.AddDbContextFactory<AppDBContext>(options =>
            {
                // ✅ FIXED: Use connection string from appsettings.json
                // ✅ FIXED: UseNpgsql() with PostgreSQL connection string
                options.UseNpgsql(connectionString);
            });

            // 3. Đăng ký Scoped Context (Để sửa lỗi CS1503)
            services.AddScoped(p => p.GetRequiredService<IDbContextFactory<AppDBContext>>().CreateDbContext());

            // 4. Đăng ký các Repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IProjectSystemPhaseConfigRepository, ProjectSystemPhaseConfigRepository>();
            services.AddScoped<IWorkLogRepository, WorkLogRepository>();


            return services;
        }
    }
}
