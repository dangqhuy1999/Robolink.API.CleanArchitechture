using MediatR;
using Robolink.Application.Commands.Projects;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.SystemPhases
{
    public class DeleteSystemPhaseCommandHandler : IRequestHandler<DeleteSystemPhaseCommand, bool>
    {
        private readonly IGenericRepository<SystemPhase> _systemPhaseRepo;
        private readonly IGenericRepository<ProjectSystemPhaseConfig> _configRepo;

        public DeleteSystemPhaseCommandHandler(
            IGenericRepository<SystemPhase> systemPhaseRepo,
            IGenericRepository<ProjectSystemPhaseConfig> configRepo) // Inject thêm Repo config
        {
            _systemPhaseRepo = systemPhaseRepo;
            _configRepo = configRepo;
        }

        public async Task<bool> Handle(DeleteSystemPhaseCommand request, CancellationToken cancellationToken)
        {
            // 1. Kiểm tra tồn tại
            var phase = await _systemPhaseRepo.GetByIdAsync(request.PhaseId);
            if (phase == null)
                throw new InvalidOperationException("System Phase không tồn tại.");

            // 2. 🛡️ KIỂM TRA SỬ DỤNG (Quan trọng nhất)
            // Thay vì GetBySystemPhaseIdAsync rồi Count (nặng), ta dùng AnyAsync (nhẹ)
            bool isInUse = await _configRepo.AnyAsync(c => c.SystemPhaseId == request.PhaseId);

            if (isInUse)
            {
                // Nếu muốn hiện số lượng dự án đang dùng, em có thể CountAsync
                var usageCount = await _configRepo.CountAsync(c => c.SystemPhaseId == request.PhaseId);
                throw new InvalidOperationException(
                    $"Không thể xóa giai đoạn hệ thống này vì đang được sử dụng bởi {usageCount} dự án. " +
                    "Vui lòng gỡ bỏ giai đoạn này khỏi các dự án trước khi xóa.");
            }

            // 3. Thực hiện xóa
            try
            {
                if (request.HardDelete)
                    await _systemPhaseRepo.DeleteAsync(request.PhaseId);
                else
                    await _systemPhaseRepo.SoftDeleteAsync(request.PhaseId);

                await _systemPhaseRepo.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi xóa giai đoạn hệ thống: {ex.Message}");
            }
        }
    }
}