using Refit;
using Robolink.Shared.DTOs; // Thay bằng namespace DTO của em

namespace Robolink.Shared.Interfaces.API.PhaseTasks
{
    public interface IPhaseTaskApi
    {
        // Lấy danh sách task có phân trang và lọc theo PhaseId
        [Get("/api/phasetasks/paged")]
        Task<PagedResult<PhaseTaskDto>> GetPhaseTasksAsync(int startIndex, int count, Guid phaseId);
        
        // Lấy chi tiết 1 task theo ID
        [Get("/api/phasetasks/{id}")]
        Task<PhaseTaskDto> GetByIdAsync(Guid id);
        
        // Tạo mới task
        [Post("/api/phasetasks")]
        Task<PhaseTaskDto> CreateAsync([Body] CreatePhaseTaskRequest request);

        // Tạo mới task
        [Put("/api/phasetasks/{id}")]
        Task<PhaseTaskDto> UpdateAsync(Guid id,[Body] UpdatePhaseTaskRequest request);
    }
}
