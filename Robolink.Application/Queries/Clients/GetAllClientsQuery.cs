using MediatR;
using Robolink.Shared.DTOs;
using System.Collections.Generic;

namespace Robolink.Application.Queries.Clients
{
    public class GetAllClientsQuery : IRequest<PagedResult<ClientDto>> // Đổi IEnumerable thành PagedResult
    {
    }
}