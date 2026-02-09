using AutoMapper;
using MediatR;
using Robolink.Application.DTOs;
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
            var task = await _taskRepo.GetByIdAsync(request.Id);
            if (task == null)
                throw new InvalidOperationException("Phase task not found");

            // ✅ Update ALL properties (not just Description, AssignedStaffId, DueDate)
            task.Name = request.Name ?? task.Name;
            task.Description = request.Description ?? string.Empty;
            task.AssignedStaffId = request.AssignedStaffId;
            task.DueDate = request.DueDate;
            task.Status = request.Status;              // ✅ NEW
            task.Priority = request.Priority;          // ✅ NEW
            task.EstimatedHours = request.EstimatedHours;  // ✅ NEW
            task.ParentPhaseTaskId = request.ParentPhaseTaskId;  // ✅ NEW
            task.UpdatedAt = DateTime.UtcNow;
            task.UpdatedBy = request.CreatedBy ?? "System";

            await _taskRepo.UpdateAsync(task);
            await _taskRepo.SaveChangesAsync();

            var phaseConfig = await _phaseConfigRepo.GetByIdAsync(task.ProjectSystemPhaseConfigId);
            var dto = _mapper.Map<PhaseTaskDto>(task);
            if (phaseConfig != null)
                dto.PhaseName = phaseConfig.CustomPhaseName ?? phaseConfig.SystemPhase?.Name;

            return dto;
        }
    }
}