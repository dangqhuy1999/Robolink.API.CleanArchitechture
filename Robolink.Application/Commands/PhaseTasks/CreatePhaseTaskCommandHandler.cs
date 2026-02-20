using AutoMapper;
using MediatR;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Shared.Enums;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.PhaseTasks
{
    public class CreatePhaseTaskCommandHandler : IRequestHandler<CreatePhaseTaskCommand, PhaseTaskDto>
    {
        private readonly IGenericRepository<PhaseTask> _taskRepo;
        private readonly IProjectSystemPhaseConfigRepository _phaseConfigRepo;
        private readonly IMapper _mapper;

        public CreatePhaseTaskCommandHandler(
            IGenericRepository<PhaseTask> taskRepo,
            IProjectSystemPhaseConfigRepository phaseConfigRepo,
            IMapper mapper)
        {
            _taskRepo = taskRepo;
            _phaseConfigRepo = phaseConfigRepo;
            _mapper = mapper;
        }

        public async Task<PhaseTaskDto> Handle(CreatePhaseTaskCommand request, CancellationToken cancellationToken)
        {
            // 1. Kiểm tra config
            var phaseConfig = await _phaseConfigRepo.GetByIdAsync(request.Request.ProjectSystemPhaseConfigId);
            if (phaseConfig == null) throw new InvalidOperationException("Phase configuration not found");

            // 2. DÙNG MAPPER ĐỂ TẠO ENTITY (Cực gọn!)
            var task = _mapper.Map<PhaseTask>(request.Request);

            // Gán nốt những thông tin thuộc về "Hệ thống" mà Request không có
            task.CreatedBy = request.CreatedBy ?? "System";

            // 3. Lưu vào DB
            await _taskRepo.AddAsync(task);

            // 4. Map ngược lại DTO để trả về
            var dto = _mapper.Map<PhaseTaskDto>(task);
            dto.PhaseName = phaseConfig.CustomPhaseName ?? phaseConfig.SystemPhase?.Name;

            return dto;
        }
    }
}