using MediatR;
using Robolink.Application.Queries.PhaseTasks;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;
using Robolink.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Application.Queries.SystemPhases
{
    public class GetSystemPhaseByIdQueryHandler : IRequestHandler<GetSystemPhaseByIdQuery, SystemPhaseDto?>
    {
        private readonly IGenericRepository<SystemPhase> _systemPhaseRepo;

        // ✅ Chỉ cần duy nhất Repo của chính nó
        public GetSystemPhaseByIdQueryHandler(IGenericRepository<SystemPhase> systemPhaseRepo)
        {
            _systemPhaseRepo = systemPhaseRepo;
        }

        public async Task<SystemPhaseDto?> Handle(GetSystemPhaseByIdQuery request, CancellationToken cancellationToken)
        {
            // ✅ Một dòng code quét sạch tất cả:
            // 1. Tự động Join bảng Staff để lấy AssignedStaffName
            // 2. Tự động Join bảng PhaseConfig & SystemPhase để lấy PhaseName
            // 3. Chỉ tạo duy nhất 1 câu lệnh SQL (tối ưu hiệu năng)
            return await _systemPhaseRepo.GetProjectedByIdAsync<SystemPhaseDto>(request.SystemPhaseId);
        }
    }
}
