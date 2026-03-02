using MediatR;
using AutoMapper;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.ProjectPhases
{
    public class AssignPhaseToProjectCommandHandler : IRequestHandler<AssignPhaseToProjectCommand, ProjectPhaseConfigDto>
    {
        private readonly IGenericRepository<ProjectSystemPhaseConfig> _configRepo;
        private readonly IGenericRepository<Project> _projectRepo;
        private readonly IGenericRepository<SystemPhase> _phaseRepo;

        public AssignPhaseToProjectCommandHandler(
            IGenericRepository<ProjectSystemPhaseConfig> configRepo,
            IGenericRepository<Project> projectRepo,
            IGenericRepository<SystemPhase> phaseRepo)
        {
            _configRepo = configRepo;
            _projectRepo = projectRepo;
            _phaseRepo = phaseRepo;
        }

        public async Task<ProjectPhaseConfigDto> Handle(AssignPhaseToProjectCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate tồn tại (Chỉ dùng AnyAsync để SQL chạy cực nhanh, không load cả Entity)
            if (!await _projectRepo.AnyAsync(p => p.Id == request.ProjectId))
                throw new InvalidOperationException("Project not found");

            if (!await _phaseRepo.AnyAsync(p => p.Id == request.SystemPhaseId))
                throw new InvalidOperationException("System Phase not found");

            // 2. Check trùng (Dùng AnyAsync của Generic Repo thay cho hàm đặc thù)
            var alreadyExists = await _configRepo.AnyAsync(x =>
                x.ProjectId == request.ProjectId && x.SystemPhaseId == request.SystemPhaseId);

            if (alreadyExists)
                throw new InvalidOperationException("This phase is already assigned to the project");

            // 3. Tính Sequence (Dùng Count ngay trên Generic Repo)
            var currentCount = await _configRepo.CountAsync(x => x.ProjectId == request.ProjectId);
            var nextSequence = currentCount + 1;

            // 4. Create Entity
            var config = new ProjectSystemPhaseConfig
            {
                Id = Guid.NewGuid(),
                ProjectId = request.ProjectId,
                SystemPhaseId = request.SystemPhaseId,
                CustomPhaseName = request.CustomPhaseName,
                Sequence = nextSequence,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow,
                // ✅ Gán mảng rỗng để thỏa mãn điều kiện 'required'
                RowVersion = Array.Empty<byte>()
            };

            await _configRepo.AddAsync(config);
            await _configRepo.SaveChangesAsync();

            // 5. ĂN TIỀN: Dùng vũ khí hạng nặng để trả về DTO hoàn hảo
            // Không gán tay, không IMapper ở đây. Repo tự Join và tự Map.
            return await _configRepo.GetProjectedByIdAsync<ProjectPhaseConfigDto>(config.Id)
                   ?? throw new Exception("Error mapping new phase");
        }
    }

}