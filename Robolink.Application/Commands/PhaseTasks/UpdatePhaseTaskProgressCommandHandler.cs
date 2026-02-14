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

        public async Task<PhaseTaskDto> Handle(UpdatePhaseTaskProgressCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepo.GetByIdAsync(request.PhaseTaskId);
            if (task == null)
                throw new InvalidOperationException("Phase task not found");

            // ✅ VALIDATE: ProcessRate must be 0-100
            if (request.ProcessRate < 0 || request.ProcessRate > 100)
                throw new InvalidOperationException("ProcessRate must be between 0 and 100");

            // ✅ VALIDATE: Status must be valid enum value
            if (!Enum.IsDefined(typeof(Task_Status), request.Status))
                throw new InvalidOperationException($"Invalid task status: {request.Status}");

           task.Name = task.Name.Trim();
            // Update properties
            task.ProcessRate = request.ProcessRate;
            task.Status = (Task_Status)request.Status;
            task.UpdatedAt = DateTime.UtcNow;

            // ✅ AUTO-COMPLETE: If 100%, mark as completed
            if (request.ProcessRate == 100 && task.CompletedAt == null)
            {
                task.CompletedAt = DateTime.UtcNow;
                task.Status = Task_Status.Completed;
            }
            // ✅ AUTO-RESET: If < 100%, clear completed date
            else if (request.ProcessRate < 100 && task.CompletedAt != null)
            {
                task.CompletedAt = null;
            }

            await _taskRepo.UpdateAsync(task);
            await _taskRepo.SaveChangesAsync();

            return _mapper.Map<PhaseTaskDto>(task);
        }
    }
}