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
        private readonly IGenericRepository<ProjectSystemPhaseConfig> _phaseConfigRepo;
        private readonly IGenericRepository<Staff> _staffRepo; // ✅ Thêm để Validate nhân viên
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; // ✅ Thêm để đồng bộ Event

        public CreatePhaseTaskCommandHandler(
            IGenericRepository<PhaseTask> taskRepo,
            IGenericRepository<ProjectSystemPhaseConfig> phaseConfigRepo,
            IGenericRepository<Staff> staffRepo,
            IMapper mapper,
            IMediator mediator)
        {
            _taskRepo = taskRepo;
            _phaseConfigRepo = phaseConfigRepo;
            _staffRepo = staffRepo;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<PhaseTaskDto> Handle(CreatePhaseTaskCommand request, CancellationToken cancellationToken)
        {
            // ✅ 1. Validation: Kiểm tra Phase Config
            var phaseConfig = await _phaseConfigRepo.GetByIdAsync(request.Request.ProjectSystemPhaseConfigId);
            if (phaseConfig == null)
                throw new InvalidOperationException("Phase configuration not found");

            // ✅ 2. Validation: Kiểm tra Nhân viên được giao (Nếu có AssignedStaffId)
            if (request.Request.AssignedStaffId != Guid.Empty)
            {
                var staff = await _staffRepo.GetByIdAsync(request.Request.AssignedStaffId);
                if (staff == null)
                    throw new InvalidOperationException("Assigned Staff not found");
            }

            // ✅ 3. Tạo Entity (ID đã được sinh tự động trong Entity Constructor như chị em mình bàn)
            var task = _mapper.Map<PhaseTask>(request.Request);
            task.CreatedBy = request.CreatedBy ?? "System";

            // ✅ 4. Lưu vào Database
            await _taskRepo.AddAsync(task);

            /* ✅ 5. Publish Event (Để làm các việc như: Gửi thông báo cho Staff, Ghi Log...)
            await _mediator.Publish(new PhaseTaskCreatedEvent
            {
                TaskId = task.Id,
                TaskName = task.Name
            }, cancellationToken);
            */

            // ✅ 6. ĂN TIỀN: Dùng Projection để lấy DTO có đầy đủ PhaseName, StaffName
            // Repo sẽ tự JOIN các bảng liên quan dựa trên MappingProfile đã viết
            return await _taskRepo.GetProjectedByIdAsync<PhaseTaskDto>(task.Id)
                   ?? throw new InvalidOperationException("Failed to retrieve updated task after creation");
        }
    }
}