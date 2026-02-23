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
        private readonly IMapper _mapper;

        public GetAllStaffQueryHandler(
            IGenericRepository<Robolink.Core.Entities.Staff> staffRepo,
            IMapper mapper)
        {
            _staffRepo = staffRepo;
            _mapper = mapper;
        }

        public async Task<PagedResult<StaffDto>> Handle(GetAllStaffQuery request, CancellationToken cancellationToken)
        {
            var staff = await _staffRepo.GetAllAsync();
            // 2. Map sang danh sách DTO
            var staffDtos = _mapper.Map<List<StaffDto>>(staff);

            // 3. Đóng gói vào PagedResult để khớp với Controller và Interface
            return new PagedResult<StaffDto>
            {
                Items = staffDtos,
                TotalCount = staffDtos.Count, // Đếm tổng số lượng
                                               // Nếu có PageSize/PageIndex trong request thì bỏ vào đây, 
                                               // còn không thì để mặc định hoặc lấy theo list.Count
            };

            
        }
    }
}