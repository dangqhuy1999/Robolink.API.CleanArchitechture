using Refit;
using Robolink.Shared.DTOs;

namespace Robolink.Shared.Services.API.Staffs

{
    public interface IStaffApi
    {
        // Lấy danh sách staffs có phân trang và lọc theo PhaseId
        [Get("/api/staffs")]
        Task<PagedResult<StaffDto>> GetAllStaffsAsync([Query] int startIndex, [Query] int count);

    }
}
