using MediatR;
using Robolink.Application.DTOs;
using System.Collections.Generic;

namespace Robolink.Application.Queries.Staff
{
    public class GetAllStaffQuery : IRequest<IEnumerable<StaffDto>>
    {
    }
}