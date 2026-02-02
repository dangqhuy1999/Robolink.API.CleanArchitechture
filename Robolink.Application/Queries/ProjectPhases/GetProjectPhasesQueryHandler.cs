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
            System.Diagnostics.Debug.WriteLine($"🔍 GetProjectPhasesQueryHandler: Loading phases for project {request.ProjectId}");
            
            try
            {
                var configs = await _phaseConfigRepo.GetByProjectIdAsync(request.ProjectId);
                
                System.Diagnostics.Debug.WriteLine($"✅ Repository returned {configs?.Count() ?? 0} phase configs");
                
                if (configs == null || !configs.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ No phases found for project {request.ProjectId}");
                    return new List<ProjectPhaseConfigDto>();
                }

                var dtos = new List<ProjectPhaseConfigDto>();
                foreach (var config in configs)
                {
                    System.Diagnostics.Debug.WriteLine($"   Processing phase: {config.Id}, SystemPhaseId: {config.SystemPhaseId}, Sequence: {config.Sequence}");
                    
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
                
                System.Diagnostics.Debug.WriteLine($"✅ Returning {dtos.Count} DTOs");
                return dtos.OrderBy(d => d.Sequence);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error in GetProjectPhasesQueryHandler: {ex}");
                throw;
            }
        }
    }
}