using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Robolink.Core.Interfaces;
using Robolink.Infrastructure.Data;
using Robolink.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services)
        {
            // 1. Đăng ký Factory (Dùng cho triệu dòng/Virtualize/Đa luồng)
            services.AddDbContextFactory<AppDBContext>(options =>
            {
                options.UseSqlServer("Data Source=DESKTOP-2JBAIOH\\SQLEXPRESS;Initial Catalog=RoboLinkDb;User ID=sa;Password=Aa@12345;TrustServerCertificate=True;MultipleActiveResultSets=true");
            });

            // 2. Đăng ký Scoped Context (Để sửa lỗi CS1503 - giúp các Repo cũ vẫn chạy được)
            // Dòng này cực kỳ quan trọng: Nó lấy một Context từ Factory để phục vụ các Constructor cũ
            services.AddScoped(p => p.GetRequiredService<IDbContextFactory<AppDBContext>>().CreateDbContext());

            // 3. Đăng ký các Repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IProjectSystemPhaseConfigRepository, ProjectSystemPhaseConfigRepository>();
            services.AddScoped<IWorkLogRepository, WorkLogRepository>();

            return services;
        }
    }
}
