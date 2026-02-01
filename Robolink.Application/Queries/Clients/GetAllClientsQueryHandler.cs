using AutoMapper;
using MediatR;
using Robolink.Application.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Robolink.Application.Queries.Clients
{
    public class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, IEnumerable<ClientDto>>
    {
        private readonly IGenericRepository<Client> _clientRepo;
        private readonly IMapper _mapper;

        public GetAllClientsQueryHandler(
            IGenericRepository<Client> clientRepo,
            IMapper mapper)
        {
            _clientRepo = clientRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClientDto>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
        {
            var clients = await _clientRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<ClientDto>>(clients);
        }
    }
}