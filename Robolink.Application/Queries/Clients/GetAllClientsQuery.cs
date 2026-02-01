using MediatR;
using Robolink.Application.DTOs;
using System.Collections.Generic;

namespace Robolink.Application.Queries.Clients
{
    public class GetAllClientsQuery : IRequest<IEnumerable<ClientDto>>
    {
    }
}