
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Robolink.Application.Mappers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace Robolink.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();


            // 2. ✅ AutoMapper: Chỉ cần 1 DÒNG DUY NHẤT để đăng ký tất cả Profile có trong tầng này
            services.AddAutoMapper(assembly);

            // 3. ✅ MediatR & Pipeline Behaviors: Nhóm lại một chỗ cho dễ quản lý
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);

            });

            return services;
        }
    }
}
