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
        private readonly IMapper _mapper;

        public GetAllClientsQueryHandler(IGenericRepository<Client> clientRepo, IMapper mapper)
        {
            _clientRepo = clientRepo;
            _mapper = mapper;
        }

        public async Task<PagedResult<ClientDto>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
        {
            // 1. Lấy danh sách thực thể từ Repo
            var clients = await _clientRepo.GetAllAsync();

            // 2. Map sang danh sách DTO
            var clientDtos = _mapper.Map<List<ClientDto>>(clients);

            // 3. Đóng gói vào PagedResult để khớp với Controller và Interface
            return new PagedResult<ClientDto>
            {
                Items = clientDtos,
                TotalCount = clientDtos.Count, // Đếm tổng số lượng
                                               // Nếu có PageSize/PageIndex trong request thì bỏ vào đây, 
                                               // còn không thì để mặc định hoặc lấy theo list.Count
            };
        }
    }
}