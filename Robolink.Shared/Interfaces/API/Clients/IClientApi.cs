using Refit;
using Robolink.Shared.DTOs; // Thay bằng namespace DTO của em

namespace Robolink.Shared.Interfaces.API.Clients
{
    public interface IClientApi
    {
        // Lấy danh sách task có phân trang và lọc theo PhaseId
        [Get("/api/clients")]
        Task<PagedResult<ClientDto>> GetAllClientsAsync([Query] int startIndex, [Query] int count);

    }
}
