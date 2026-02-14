using MediatR;
using AutoMapper;
using Robolink.Shared.DTOs;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.ProjectPhases
{
    public class UpdateProjectPhaseConfigCommandHandler : IRequestHandler<UpdateProjectPhaseConfigCommand, ProjectPhaseConfigDto>
    {
        private readonly IProjectSystemPhaseConfigRepository _configRepo;
        private readonly IMapper _mapper;

        public UpdateProjectPhaseConfigCommandHandler(
            IProjectSystemPhaseConfigRepository configRepo,
            IMapper mapper)
        {
            _configRepo = configRepo;
            _mapper = mapper;
        }

        public async Task<ProjectPhaseConfigDto> Handle(UpdateProjectPhaseConfigCommand request, CancellationToken cancellationToken)
        {
            var config = await _configRepo.GetByIdAsync(request.PhaseConfigId);
            if (config == null)
                throw new InvalidOperationException("Phase config not found");

            config.CustomPhaseName = request.CustomPhaseName;
            config.Sequence = request.Sequence;
            config.IsEnabled = request.IsEnabled;
            config.UpdatedAt = DateTime.UtcNow;

            await _configRepo.UpdateAsync(config);
            await _configRepo.SaveChangesAsync();

            return new ProjectPhaseConfigDto
            {
                Id = config.Id,
                ProjectId = config.ProjectId,
                SystemPhaseId = config.SystemPhaseId,
                SystemPhase = _mapper.Map<SystemPhaseDto>(config.SystemPhase),
                CustomPhaseName = config.CustomPhaseName,
                Sequence = config.Sequence,
                IsEnabled = config.IsEnabled,
                TaskCount = config.PhaseTasks?.Count ?? 0,
                Tasks = _mapper.Map<List<PhaseTaskDto>>(config.PhaseTasks ?? new List<Robolink.Core.Entities.PhaseTask>())
            };
        }
    }
}