using AutoMapper;
using AutoMapper.QueryableExtensions; // <--- Cực kỳ quan trọng, thiếu cái này là không dùng được ProjectTo đâu
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;
using Robolink.Infrastructure.Data;
using Robolink.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Infrastructure.Repositories
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        private readonly AutoMapper.IConfigurationProvider _configurationProvider;

        public ProjectRepository(
            IDbContextFactory<AppDBContext> factory,
            IMapper mapper) // Inject thêm Mapper vào đây
            : base(factory)
        {
            _configurationProvider = mapper.ConfigurationProvider;
        }

        public async Task<(IEnumerable<ProjectDto> Items, int TotalCount)> GetProjectsWithDeepDataAsync(int startIndex, int count)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var query = context.Projects
                .Where(p => p.ParentProjectId == null && !p.IsDeleted); // Chỉ lấy Cha

            var totalCount = await query.CountAsync(); // Đếm xem có bao nhiêu ông Cha

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(startIndex).Take(count)
                .ProjectTo<ProjectDto>(_configurationProvider) // "Vũ khí" tự nạp luôn đám Con vào trong Cha
                .ToListAsync();

            return (items, totalCount);
        }

        // Trong ProjectRepository (Implementation)
        public async Task<ProjectDto> GetProjectByIdWithDeepDataAsync(Guid id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Projects
                .AsNoTracking()
                .Where(p => p.Id == id)
                .ProjectTo<ProjectDto>(_configurationProvider) // Tự động nạp đủ con cháu, ClientName...
                .FirstOrDefaultAsync();
        }
    }
}
