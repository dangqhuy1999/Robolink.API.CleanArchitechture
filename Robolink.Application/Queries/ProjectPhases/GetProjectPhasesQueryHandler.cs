using MediatR;
using AutoMapper;
using Robolink.Application.DTOs;
using Robolink.Core.Interfaces;
using Robolink.Core.Entities;

namespace Robolink.Application.Queries.ProjectPhases
{
    public class GetProjectPhasesQueryHandler : IRequestHandler<GetProjectPhasesQuery, IEnumerable<ProjectPhaseConfigDto>>
    {
        private readonly IProjectSystemPhaseConfigRepository _phaseConfigRepo;
        private readonly IMapper _mapper;

        public GetProjectPhasesQueryHandler(
            IProjectSystemPhaseConfigRepository phaseConfigRepo,
            IMapper mapper)
        {
            _phaseConfigRepo = phaseConfigRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProjectPhaseConfigDto>> Handle(GetProjectPhasesQuery request, CancellationToken cancellationToken)
        {
            var configs = await _phaseConfigRepo.GetByProjectIdAsync(request.ProjectId);
            
            var dtos = new List<ProjectPhaseConfigDto>();
            foreach (var config in configs)
            {
                var dto = new ProjectPhaseConfigDto
                {
                    Id = config.Id,
                    ProjectId = config.ProjectId,
                    SystemPhaseId = config.SystemPhaseId,
                    SystemPhase = _mapper.Map<SystemPhaseDto>(config.SystemPhase),
                    CustomPhaseName = config.CustomPhaseName,
                    Sequence = config.Sequence,
                    IsEnabled = config.IsEnabled,
                    TaskCount = config.PhaseTasks?.Count ?? 0,
                    Tasks = _mapper.Map<List<PhaseTaskDto>>(config.PhaseTasks ?? new List<PhaseTask>())
                };
                dtos.Add(dto);
            }
            
            return dtos.OrderBy(d => d.Sequence);
        }
    }
}