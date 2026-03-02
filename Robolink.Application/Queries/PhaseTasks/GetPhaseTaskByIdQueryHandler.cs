using AutoMapper;
using MediatR;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Queries.PhaseTasks
{
    public class GetPhaseTaskByIdQueryHandler : IRequestHandler<GetPhaseTaskByIdQuery, PhaseTaskDto?>
    {
        private readonly IGenericRepository<PhaseTask> _phaseTaskRepo;

        // ✅ Chỉ cần duy nhất Repo của chính nó
        public GetPhaseTaskByIdQueryHandler(IGenericRepository<PhaseTask> phaseTaskRepo)
        {
            _phaseTaskRepo = phaseTaskRepo;
        }

        public async Task<PhaseTaskDto?> Handle(GetPhaseTaskByIdQuery request, CancellationToken cancellationToken)
        {
            // ✅ Một dòng code quét sạch tất cả:
            // 1. Tự động Join bảng Staff để lấy AssignedStaffName
            // 2. Tự động Join bảng PhaseConfig & SystemPhase để lấy PhaseName
            // 3. Chỉ tạo duy nhất 1 câu lệnh SQL (tối ưu hiệu năng)
            return await _phaseTaskRepo.GetProjectedByIdAsync<PhaseTaskDto>(request.PhaseTaskId);
        }
    }
}