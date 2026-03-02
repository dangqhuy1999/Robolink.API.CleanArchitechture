using MediatR;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.ProjectPhases
{
    public class DeleteProjectPhaseCommandHandler : IRequestHandler<DeleteProjectPhaseCommand, bool>
    {
        private readonly IGenericRepository<ProjectSystemPhaseConfig> _configRepo;
        private readonly IGenericRepository<PhaseTask> _taskRepo;

        public DeleteProjectPhaseCommandHandler(
            IGenericRepository<ProjectSystemPhaseConfig> configRepo,
            IGenericRepository<PhaseTask> taskRepo)
        {
            _configRepo = configRepo;
            _taskRepo = taskRepo;
        }

        public async Task<bool> Handle(DeleteProjectPhaseCommand request, CancellationToken cancellationToken)
        {
            // 1. Kiểm tra tồn tại
            var config = await _configRepo.GetByIdAsync(request.PhaseConfigId);
            if (config == null) return false;

            // Dùng AnyAsync của Generic Repo cực nhanh
            var hasTasks = await _taskRepo.AnyAsync(t => t.ProjectSystemPhaseConfigId == request.PhaseConfigId);
            if (hasTasks)
                throw new InvalidOperationException("Giai đoạn này đã có công việc, không thể thu hồi!");

            // 3. THU HỒI (Soft Delete): Đánh dấu IsDeleted = true
            // Hàm này của em đã tự SaveChanges bên trong rồi đúng không?
            await _configRepo.SoftDeleteAsync(request.PhaseConfigId);

            return true;
        }
    }
}