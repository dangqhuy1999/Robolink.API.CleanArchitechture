using AutoMapper;
using MediatR;
using Robolink.Application.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Queries.PhaseTasks
{
    public class GetPhaseTaskByIdQueryHandler : IRequestHandler<GetPhaseTaskByIdQuery, PhaseTaskDto>
    {
        private readonly IGenericRepository<PhaseTask> _phaseTaskRepo;
        private readonly IGenericRepository<Robolink.Core.Entities.Staff> _staffRepo;
        private readonly IGenericRepository<ProjectSystemPhaseConfig> _phaseConfigRepo;
        private readonly IMapper _mapper;

        public GetPhaseTaskByIdQueryHandler(
            IGenericRepository<PhaseTask> phaseTaskRepo,
            IGenericRepository<Robolink.Core.Entities.Staff> staffRepo,
            IGenericRepository<ProjectSystemPhaseConfig> phaseConfigRepo,
            IMapper mapper)
        {
            _phaseTaskRepo = phaseTaskRepo;
            _staffRepo = staffRepo;
            _phaseConfigRepo = phaseConfigRepo;
            _mapper = mapper;
        }

        public async Task<PhaseTaskDto> Handle(GetPhaseTaskByIdQuery request, CancellationToken cancellationToken)
        {
            // Get single phase task
            var task = await _phaseTaskRepo.GetByIdAsync(request.PhaseTaskId);
            if (task == null)
                throw new KeyNotFoundException($"Phase task with ID {request.PhaseTaskId} not found");

            var dto = _mapper.Map<PhaseTaskDto>(task);

            // Load related data
            var staff = await _staffRepo.GetByIdAsync(task.AssignedStaffId);
            if (staff != null)
                dto.AssignedStaffName = staff.FullName ?? "Unknown";

            var phaseConfig = await _phaseConfigRepo.GetByIdAsync(task.ProjectSystemPhaseConfigId);
            if (phaseConfig != null)
                dto.PhaseName = phaseConfig.CustomPhaseName ?? phaseConfig.SystemPhase?.Name ?? "Unknown Phase";

            return dto;
        }
    }
}