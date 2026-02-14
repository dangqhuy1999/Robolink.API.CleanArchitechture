using AutoMapper;
using MediatR;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;
using System.Collections.Generic;
using System.Threading; 
using System.Threading.Tasks;

namespace Robolink.Application.Queries.Staff
{
    public class GetAllStaffQueryHandler : IRequestHandler<GetAllStaffQuery, IEnumerable<StaffDto>>
    {
        private readonly IGenericRepository<Robolink.Core.Entities.Staff> _staffRepo;
        private readonly IMapper _mapper;

        public GetAllStaffQueryHandler(
            IGenericRepository<Robolink.Core.Entities.Staff> staffRepo,
            IMapper mapper)
        {
            _staffRepo = staffRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StaffDto>> Handle(GetAllStaffQuery request, CancellationToken cancellationToken)
        {
            var staff = await _staffRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<StaffDto>>(staff);
        }
    }
}