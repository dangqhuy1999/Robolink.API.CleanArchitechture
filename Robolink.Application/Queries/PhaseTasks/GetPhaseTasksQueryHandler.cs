using AutoMapper;
using MediatR;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Queries.PhaseTasks
{
    public class GetPhaseTasksQueryHandler : IRequestHandler<GetPhaseTasksQuery, IEnumerable<PhaseTaskDto>>
    {
        private readonly IGenericRepository<PhaseTask> _phaseTaskRepo;
        private readonly IGenericRepository<Robolink.Core.Entities.Staff> _staffRepo;
        private readonly IGenericRepository<ProjectSystemPhaseConfig> _phaseConfigRepo;
        private readonly IMapper _mapper;

        public GetPhaseTasksQueryHandler(
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

        public async Task<IEnumerable<PhaseTaskDto>> Handle(GetPhaseTasksQuery request, CancellationToken cancellationToken)
        {
            // Get all phase tasks for the specified phase config
            var tasks = await _phaseTaskRepo.FindAsync(t => t.ProjectSystemPhaseConfigId == request.ProjectSystemPhaseConfigId);
            var dtos = _mapper.Map<List<PhaseTaskDto>>(tasks);

            // Load related data
            var staffIds = tasks.Select(t => t.AssignedStaffId).Distinct();
            var phaseConfigIds = tasks.Select(t => t.ProjectSystemPhaseConfigId).Distinct();

            var staffList = await _staffRepo.FindAsync(s => staffIds.Contains(s.Id));
            var phaseConfigs = await _phaseConfigRepo.FindAsync(pc => phaseConfigIds.Contains(pc.Id));

            foreach (var dto in dtos)
            {
                // Populate assigned staff name
                var staff = staffList.FirstOrDefault(s => s.Id == dto.AssignedStaffId);
                if (staff != null)
                    dto.AssignedStaffName = staff.FullName ?? "Unknown";

                // Populate phase name (from config)
                var phaseConfig = phaseConfigs.FirstOrDefault(pc => pc.Id == dto.ProjectSystemPhaseConfigId);
                if (phaseConfig != null)
                    dto.PhaseName = phaseConfig.CustomPhaseName ?? phaseConfig.SystemPhase?.Name ?? "Unknown Phase";
            }

            return dtos;
        }
    }
}