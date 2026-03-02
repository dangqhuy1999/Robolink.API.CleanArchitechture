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
    public class GetAllStaffQueryHandler : IRequestHandler<GetAllStaffQuery, PagedResult<StaffDto>>
    {
        private readonly IGenericRepository<Robolink.Core.Entities.Staff> _staffRepo;

        public GetAllStaffQueryHandler(
            IGenericRepository<Robolink.Core.Entities.Staff> staffRepo)
        {
            _staffRepo = staffRepo;
        }

        public async Task<PagedResult<StaffDto>> Handle(GetAllStaffQuery request, CancellationToken cancellationToken)
        {
            // Không cần gán tay, không cần gọi _mapper.Map ở đây nữa!
            return await _staffRepo.GetPagedProjectedAsync<StaffDto>(request.StartIndex, request.Count);
        }
    }
}