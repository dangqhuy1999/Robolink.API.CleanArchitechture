using Microsoft.Extensions.DependencyInjection;
using Robolink.Application.Mappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            // ✅ MediatR (CQRS)
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            // ✅ AutoMapper
            services.AddAutoMapper(typeof(ProjectMappingProfile));
            services.AddAutoMapper(typeof(ProjectPhaseMappingProfile));
            services.AddAutoMapper(typeof(SystemPhaseMappingProfile));
            services.AddAutoMapper(typeof(PhaseTaskMappingProfile));
            // another way to register all profiles in the assembly
            services.AddAutoMapper(typeof(ClientStaffMappingProfile).Assembly);

            return services;
        }
    }
}
