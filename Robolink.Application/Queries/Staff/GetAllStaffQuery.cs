using MediatR;
using Robolink.Shared.DTOs;
using System.Collections.Generic;

namespace Robolink.Application.Queries.Staff
{
    public class GetAllStaffQuery : IRequest<PagedResult<StaffDto>>
    {
    }
}