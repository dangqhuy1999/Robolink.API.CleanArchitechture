using Refit;
using Robolink.Shared.DTOs; // Thay bằng namespace DTO của em

namespace Robolink.Shared.Interfaces.API.SystemPhases
{
    public interface ISystemPhaseApi
    {
        // Lấy danh sách project có phân trang và lọc theo PhaseId
        [Get("/api/systemphases/paged")]
        Task<PagedResult<SystemPhaseDto>> GetSystemPhasesPagedAsync([Query] int startIndex, [Query] int count, [Query] string? searchTerm = null);

        // Lấy tất cả System Phases (mặc định chỉ lấy cái Active để chọn)
        [Get("/api/systemphases")]
        Task<IEnumerable<SystemPhaseDto>> GetAllAsync([Query] bool onlyActive = true);

        // 🚀 Vũ khí mới cho trang Detail/Edit
        [Get("/api/systemphases/{id}")]
        Task<SystemPhaseDto?> GetByIdAsync(Guid id);

        [Post("/api/systemphases")]
        Task<SystemPhaseDto> CreateAsync([Body] CreateSystemPhaseRequest request);

        [Put("/api/systemphases/{id}")]
        Task<SystemPhaseDto> UpdateAsync(Guid id, [Body] UpdateSystemPhaseRequest request);
        // Xoa task
        [Delete("/api/systemphases/{id}")]
        Task DeleteAsync(Guid id); // Đổi từ ProjectDto thành bool
    }
}
