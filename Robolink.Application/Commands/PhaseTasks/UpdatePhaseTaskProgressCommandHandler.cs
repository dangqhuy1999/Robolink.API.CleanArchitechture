using MediatR;
using AutoMapper;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Shared.Enums;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.PhaseTasks
{
    public class UpdatePhaseTaskProgressCommandHandler : IRequestHandler<UpdatePhaseTaskProgressCommand, PhaseTaskDto>
    {
        private readonly IGenericRepository<PhaseTask> _taskRepo;
        private readonly IMapper _mapper;

        public UpdatePhaseTaskProgressCommandHandler(
            IGenericRepository<PhaseTask> taskRepo,
            IMapper mapper)
        {
            _taskRepo = taskRepo;
            _mapper = mapper;
        }

        public async Task<PhaseTaskDto> Handle(UpdatePhaseTaskProgressCommand request, CancellationToken ct)
        {
            var task = await _taskRepo.GetByIdAsync(request.PhaseTaskId);
            if (task == null) throw new InvalidOperationException("Phase task not found");

            // 1. Validate (Giữ nguyên vì đây là bảo vệ dữ liệu)
            if (request.ProcessRate is < 0 or > 100)
                throw new InvalidOperationException("ProcessRate must be between 0 and 100");

            // 2. Cập nhật các trường cơ bản
            task.ProcessRate = request.ProcessRate;
            task.Status = (Task_Status)request.Status;
            task.UpdatedAt = DateTime.UtcNow;
            task.Name = task.Name.Trim();

            // 3. Logic xử lý tự động (Business Rules)
            // Chị viết gọn lại bằng toán tử 3 ngôi hoặc if đơn cho dễ nhìn
            if (task.ProcessRate == 100)
            {
                task.CompletedAt ??= DateTime.UtcNow; // Chỉ gán nếu chưa có
                task.Status = Task_Status.Completed;
            }
            else
            {
                task.CompletedAt = null;
            }

            // 4. Lưu
            await _taskRepo.UpdateAsync(task);
            // await _taskRepo.SaveChangesAsync(); // khong can savechange vi trong update da co san roi 

            // 5. Dùng Mapper duy nhất ở bước cuối cùng để trả về DTO
            // Vì trong Profile em đã có logic lấy PhaseName và StaffName rất xịn rồi!
            return _mapper.Map<PhaseTaskDto>(task);
        }
    }
}