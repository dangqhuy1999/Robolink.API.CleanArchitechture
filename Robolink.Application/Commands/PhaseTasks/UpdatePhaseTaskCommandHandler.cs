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
            var task = await _taskRepo.GetByIdAsync(request.Id);
            if (task == null)
                throw new InvalidOperationException("Phase task not found");

            // ✅ Update ALL properties with proper null handling
            task.Name = request.Name ?? task.Name;
            task.Description = request.Description ?? task.Description;
            task.AssignedStaffId = request.AssignedStaffId ?? task.AssignedStaffId;  // ✅ FIX: Line 34
            task.DueDate = request.DueDate ?? task.DueDate;                          // ✅ FIX: Line 35
            task.Status = request.Status ?? task.Status;                            // ✅ FIX: Line 36
            task.Priority = request.Priority ?? task.Priority;                      // ✅ FIX: Line 37
            task.EstimatedHours = request.EstimatedHours ?? task.EstimatedHours;   // ✅ FIX: Line 38
            task.ParentPhaseTaskId = request.ParentPhaseTaskId;
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