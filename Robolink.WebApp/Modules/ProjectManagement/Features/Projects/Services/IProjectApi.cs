using Refit;
using Robolink.Shared.DTOs; // Thay bằng namespace DTO của em

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Services
{
        public interface IProjectApi
        {

            // Lấy danh sách project có phân trang và lọc theo PhaseId
            [Get("/api/projects/paged")]
            Task<PagedResult<ProjectDto>> GetProjectsPagedAsync([Query] int startIndex, [Query] int count, [Query] string? searchTerm = null);

            // Lấy chi tiết 1 project theo ID
            [Get("/api/projects/{id}")]
            Task<ProjectDto?> GetByIdAsync(Guid id);
        
            // Tạo mới task
            [Post("/api/projects")]
            Task<ProjectDto> CreateAsync([Body] CreateProjectRequest request);

            // Tạo mới task
            [Post("/api/projects/sample")]
            Task<ProjectDto> QuickCreateAsync();

            // Tạo mới task
            [Put("/api/projects/{id}")]
            Task<ProjectDto> UpdateAsync(Guid id,[Body] UpdateProjectRequest request);
            
            // Xoa task
            [Delete("/api/projects/{id}")]
            Task DeleteAsync(Guid id); // Đổi từ ProjectDto thành bool
    }
}
