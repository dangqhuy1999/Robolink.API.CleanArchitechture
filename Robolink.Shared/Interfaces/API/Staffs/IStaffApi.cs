using Refit;
using Robolink.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Shared.Interfaces.API.Staffs
{
    public interface IStaffApi
    {
        // Lấy danh sách staffs có phân trang và lọc theo PhaseId
        [Get("/api/staffs")]
        Task<PagedResult<StaffDto>> GetAllStaffsAsync();

    }
}
