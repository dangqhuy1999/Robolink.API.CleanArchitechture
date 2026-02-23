using Robolink.Core.Entities;
using Robolink.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Core.Interfaces
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        Task<ProjectDto> GetProjectByIdWithDeepDataAsync(Guid id);
        // ✅ Sửa Project thành ProjectDto ở đây
        Task<(IEnumerable<ProjectDto> Items, int TotalCount)> GetProjectsWithDeepDataAsync(int startIndex, int count);
    }
}
