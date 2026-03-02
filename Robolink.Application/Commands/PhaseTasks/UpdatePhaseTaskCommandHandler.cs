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
        private readonly IGenericRepository<Staff> _staffRepo; // ✅ Thêm để validate nhân viên
        private readonly IGenericRepository<ProjectSystemPhaseConfig> _phaseConfigRepo; // ✅ Dùng Generic Repo luôn cho đồng bộ
        private readonly IMapper _mapper;

        public UpdatePhaseTaskCommandHandler(
            IGenericRepository<PhaseTask> taskRepo,
            IGenericRepository<Staff> staffRepo,
            IGenericRepository<ProjectSystemPhaseConfig> phaseConfigRepo,
            IMapper mapper)
        {
            _taskRepo = taskRepo;
            _staffRepo = staffRepo;
            _phaseConfigRepo = phaseConfigRepo;
            _mapper = mapper;
        }

        public async Task<PhaseTaskDto> Handle(UpdatePhaseTaskCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy dữ liệu cũ từ DB
            var task = await _taskRepo.GetByIdAsync(request.Id)
                ?? throw new InvalidOperationException("Phase task not found");

            // 2. ✅ Validation: Nếu có đổi nhân viên, phải check nhân viên đó có tồn tại không
            if (request.Request.AssignedStaffId.HasValue && request.Request.AssignedStaffId != Guid.Empty)
            {
                var staff = await _staffRepo.GetByIdAsync(request.Request.AssignedStaffId.Value);
                if (staff == null) throw new InvalidOperationException("Assigned Staff not found");
            }

            // 3. ✅ Validation: Nếu có đổi Phase, check Phase mới
            if (request.Request.ProjectSystemPhaseConfigId.HasValue && request.Request.ProjectSystemPhaseConfigId != Guid.Empty)
            {
                var config = await _phaseConfigRepo.GetByIdAsync(request.Request.ProjectSystemPhaseConfigId.Value);
                if (config == null) throw new InvalidOperationException("Phase configuration not found");
            }

            // 4. ✅ Validation: Tránh vòng lặp Task cha - con
            if (request.Request.ParentPhaseTaskId.HasValue && request.Request.ParentPhaseTaskId != Guid.Empty)
            {
                if (request.Request.ParentPhaseTaskId == task.Id)
                    throw new InvalidOperationException("A task cannot be its own parent");
            }

            // 5. 🚀 MÁY GIẶT AUTOMAPPER: Đè dữ liệu mới lên Entity cũ
            // Những trường null trong Request sẽ KHÔNG đè lên dữ liệu cũ (nhờ Condition trong Profile)
            _mapper.Map(request.Request, task);

            // Xử lý logic xóa Task cha nếu truyền Guid.Empty
            if (request.Request.ParentPhaseTaskId == Guid.Empty)
            {
                task.ParentPhaseTaskId = null;
            }

            task.UpdatedBy = request.UpdatedBy ?? "System";

            // 6. Lưu vào Database
            await _taskRepo.UpdateAsync(task);
            await _taskRepo.SaveChangesAsync(); // Đảm bảo mọi thứ đã vào DB

            // 7. 🚀 CHIÊU CUỐI: Trả về DTO xịn
            // Không cần gán tay PhaseName hay StaffName nữa, GetProjectedByIdAsync sẽ tự JOIN lấy hết!
            return await _taskRepo.GetProjectedByIdAsync<PhaseTaskDto>(task.Id)
                   ?? throw new InvalidOperationException("Failed to retrieve updated task");
        }
    }
}