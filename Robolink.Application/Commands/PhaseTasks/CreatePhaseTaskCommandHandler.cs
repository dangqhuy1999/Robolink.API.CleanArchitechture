using AutoMapper;
using MediatR;
using Robolink.Application.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Enums;
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
            // ✅ Validate phase config exists
            var phaseConfig = await _phaseConfigRepo.GetByIdAsync(request.Request.ProjectSystemPhaseConfigId);
            if (phaseConfig == null)
                throw new InvalidOperationException("Phase configuration not found");

            // ✅ Create task with ALL properties
            var task = new PhaseTask
            {
                Name = request.Request.Name,
                ProjectId = request.Request.ProjectId,
                ProjectSystemPhaseConfigId = request.Request.ProjectSystemPhaseConfigId,
                Description = request.Request.Description,
                AssignedStaffId = request.Request.AssignedStaffId,
                DueDate = request.Request.DueDate,
                Status = Task_Status.Pending,
                Priority = request.Request.Priority,              // ✅ ADDED
                EstimatedHours = request.Request.EstimatedHours,  // ✅ ADDED
                ParentPhaseTaskId = request.Request.ParentPhaseTaskId,  // ✅ ADDED
                CreatedBy = request.CreatedBy ?? "System",
                RowVersion = Array.Empty<byte>()  // ✅ Initialize for concurrency
            };

            // ✅ Save
            await _taskRepo.AddAsync(task);
            await _taskRepo.SaveChangesAsync();

            // ✅ Return DTO with mapping
            var dto = _mapper.Map<PhaseTaskDto>(task);
            dto.PhaseName = phaseConfig.CustomPhaseName ?? phaseConfig.SystemPhase?.Name;
            // AssignedStaffName nếu cần, có thể load từ Staff table

            return dto;
        }
    }
}