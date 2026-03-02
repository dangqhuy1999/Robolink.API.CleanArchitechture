using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Queries.SystemPhases
{
    // Mặc định là chỉ lấy cái đang dùng (true), 
    // nếu Admin muốn xem hết thì truyền false vào.
    public record GetAllSystemPhasesQuery(bool OnlyActive = true) : IRequest<IEnumerable<SystemPhaseDto>>;
}