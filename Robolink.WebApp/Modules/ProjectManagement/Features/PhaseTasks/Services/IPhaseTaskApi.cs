using Refit;
using Robolink.Shared.DTOs; // Thay bằng namespace DTO của em

namespace Robolink.WebApp.Modules.ProjectManagement.Features.PhaseTasks.Services
{
    public interface IPhaseTaskApi
    {
        // Lấy danh sách task có phân trang và lọc theo PhaseId
        [Get("/api/phasetasks/paged")]
        Task<PagedResult<PhaseTaskDto>> GetPhaseTasksPagedAsync([Query]  int startIndex, [Query] int count, [Query] Guid phaseId, [Query] string? searchTerm = null);
        // Lấy chi tiết 1 task theo ID
        [Get("/api/phasetasks/{id}")]
        Task<PhaseTaskDto> GetByIdAsync(Guid id);
        
        // Tạo mới task
        [Post("/api/phasetasks")]
        Task<PhaseTaskDto> CreateAsync([Body] CreatePhaseTaskRequest request);

        // Tạo mới task
        [Post("/api/phasetasks/sample")]
        Task<PhaseTaskDto> QuickCreateAsync( Guid ProjectId, Guid ProjectSystemPhaseConfigId);

        // Tạo mới task
        [Put("/api/phasetasks/{id}")]
        Task<PhaseTaskDto> UpdateAsync(Guid id,[Body] UpdatePhaseTaskRequest request);
            
        // Xoa task
        [Delete("/api/phasetasks/{id}")]
        Task DeleteAsync(Guid id); // Đổi từ PhaseTaskDto thành bool
    }
}
