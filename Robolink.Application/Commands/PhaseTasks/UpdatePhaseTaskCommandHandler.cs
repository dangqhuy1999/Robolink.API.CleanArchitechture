using AutoMapper;
using MediatR;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.PhaseTasks
{
    public class UpdatePhaseTaskCommandHandler : IRequestHandler<UpdatePhaseTaskCommand, PhaseTaskDto>
    {
        private readonly IGenericRepository<PhaseTask> _taskRepo;
        private readonly IProjectSystemPhaseConfigRepository _phaseConfigRepo;
        private readonly IMapper _mapper;

        public UpdatePhaseTaskCommandHandler(
            IGenericRepository<PhaseTask> taskRepo,
            IProjectSystemPhaseConfigRepository phaseConfigRepo,
            IMapper mapper)
        {
            _taskRepo = taskRepo;
            _phaseConfigRepo = phaseConfigRepo;
            _mapper = mapper;
        }

        public async Task<PhaseTaskDto> Handle(UpdatePhaseTaskCommand request, CancellationToken cancellationToken)
        {
            // 1. Tìm task cũ từ DB
            var task = await _taskRepo.GetByIdAsync(request.Id);
            if (task == null)
                throw new InvalidOperationException("Phase task not found");

            // 2. DÙNG MAPPER ĐỂ ĐÈ DỮ LIỆU (Thay thế cho 10 dòng gán tay của em)
            // Nó sẽ tự động check: Nếu request.Request.Name null thì nó KHÔNG đè lên task.Name
            // nhờ cái .Condition((src, dest, srcMember) => srcMember != null) em đã viết.
            _mapper.Map(request.Request, task);

            task.UpdatedBy = request.UpdatedBy ?? "System";

            // 4. Lưu
            await _taskRepo.UpdateAsync(task);

            // 5. Lấy thông tin PhaseConfig để trả về DTO đầy đủ
            var phaseConfig = await _phaseConfigRepo.GetByIdAsync(task.ProjectSystemPhaseConfigId);
            var dto = _mapper.Map<PhaseTaskDto>(task);

            if (phaseConfig != null)
                dto.PhaseName = phaseConfig.CustomPhaseName ?? phaseConfig.SystemPhase?.Name;

            return dto;
        }
    }
}