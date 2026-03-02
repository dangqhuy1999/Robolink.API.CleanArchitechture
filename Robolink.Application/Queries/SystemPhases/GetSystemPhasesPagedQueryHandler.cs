using MediatR;
using Robolink.Application.Queries.Projects;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;
using Robolink.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Robolink.Application.Queries.SystemPhases
{ 
    public class GetSystemPhasesPagedQueryHandler : IRequestHandler<GetSystemPhasesPagedQuery, PagedResult<SystemPhaseDto>>
    {
        // 1. Đổi từ Generic sang Repo chuyên biệt
        private readonly IGenericRepository<SystemPhase> _systemPhaseRepo;

        // Lúc này em thậm chí KHÔNG CẦN IMapper ở Handler nữa 
        // vì Repo đã làm hộ việc đó thông qua ProjectTo rồi!
        public GetSystemPhasesPagedQueryHandler(IGenericRepository<SystemPhase> systemPhaseRepo)
        {
            _systemPhaseRepo = systemPhaseRepo;
        }

        public async Task<PagedResult<SystemPhaseDto>> Handle(GetSystemPhasesPagedQuery request, CancellationToken cancellationToken)
        {
            // 1. Tạo một bộ lọc (Predicate) mặc định là null
            Expression<Func<SystemPhase, bool>>? predicate = null;
            /*
             2. Nếu người dùng có nhập searchTerm thì mới nạp logic tìm kiếm vào
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower();
                predicate = x => x.Name.ToLower().Contains(term)
                              || x.ProjectCode.ToLower().Contains(term);
            }*/

            // 3. Gọi hàm "thần thánh" của Repo, ném thêm cái predicate vào
            return await _systemPhaseRepo.GetPagedProjectedAsync<SystemPhaseDto>(
                request.StartIndex,
                request.Count,
                predicate // 👈 Nếu searchTerm rỗng, cái này là null, nó chạy y hệt bản cũ!
            );
        }
    }
}
