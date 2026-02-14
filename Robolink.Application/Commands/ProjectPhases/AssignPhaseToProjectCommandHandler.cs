using MediatR;
using AutoMapper;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.ProjectPhases
{
    public class AssignPhaseToProjectCommandHandler : IRequestHandler<AssignPhaseToProjectCommand, ProjectPhaseConfigDto>
    {
        private readonly IProjectSystemPhaseConfigRepository _configRepo;
        private readonly IGenericRepository<Project> _projectRepo;
        private readonly IGenericRepository<SystemPhase> _phaseRepo;
        private readonly IMapper _mapper;

        public AssignPhaseToProjectCommandHandler(
            IProjectSystemPhaseConfigRepository configRepo,
            IGenericRepository<Project> projectRepo,
            IGenericRepository<SystemPhase> phaseRepo,
            IMapper mapper)
        {
            _configRepo = configRepo;
            _projectRepo = projectRepo;
            _phaseRepo = phaseRepo;
            _mapper = mapper;
        }

        public async Task<ProjectPhaseConfigDto> Handle(AssignPhaseToProjectCommand request, CancellationToken cancellationToken)
        {
            // Validate project exists
            var project = await _projectRepo.GetByIdAsync(request.ProjectId);
            if (project == null)
                throw new InvalidOperationException("Project not found");

            // Validate phase exists
            var phase = await _phaseRepo.GetByIdAsync(request.SystemPhaseId);
            if (phase == null)
                throw new InvalidOperationException("System Phase not found");

            // Check if already assigned
            if (await _configRepo.ProjectHasPhaseAsync(request.ProjectId, request.SystemPhaseId))
                throw new InvalidOperationException("This phase is already assigned to the project");

            // Get next sequence
            var nextSequence = await _configRepo.GetNextSequenceAsync(request.ProjectId);

            // Create new config
            var config = new ProjectSystemPhaseConfig
            {
                Id = Guid.NewGuid(),
                ProjectId = request.ProjectId,
                SystemPhaseId = request.SystemPhaseId,
                CustomPhaseName = request.CustomPhaseName,
                Sequence = nextSequence,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow,
                // ✅ THÊM DÒNG NÀY ĐỂ HẾT LỖI COMPILER
                RowVersion = Array.Empty<byte>()
            };

            await _configRepo.AddAsync(config);
            await _configRepo.SaveChangesAsync();

            // Return mapped DTO
            config.SystemPhase = phase; // Attach phase for mapping
            return new ProjectPhaseConfigDto
            {
                Id = config.Id,
                ProjectId = config.ProjectId,
                SystemPhaseId = config.SystemPhaseId,
                SystemPhase = _mapper.Map<SystemPhaseDto>(phase),
                CustomPhaseName = config.CustomPhaseName,
                Sequence = config.Sequence,
                IsEnabled = config.IsEnabled,
                TaskCount = 0,
                Tasks = new List<PhaseTaskDto>()
            };
        }
    }
}