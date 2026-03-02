using AutoMapper;
using MediatR;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Queries.Clients
{
    public class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, PagedResult<ClientDto>>
    {
        private readonly IGenericRepository<Client> _clientRepo;

        public GetAllClientsQueryHandler(IGenericRepository<Client> clientRepo)
        {
            _clientRepo = clientRepo;
        }

        public async Task<PagedResult<ClientDto>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
        {
            // Không cần gán tay, không cần gọi _mapper.Map ở đây nữa!
            return await _clientRepo.GetPagedProjectedAsync<ClientDto>(request.StartIndex, request.Count);
        }
    }
}