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

            // ✅ Create task
            var task = new PhaseTask
            {
                ProjectId = request.Request.ProjectId,
                ProjectSystemPhaseConfigId = request.Request.ProjectSystemPhaseConfigId,
                Description = request.Request.Description,
                AssignedStaffId = request.Request.AssignedStaffId,
                DueDate = request.Request.DueDate,
                Status = Task_Status.Pending,
                CreatedBy = request.CreatedBy
            };

            // ✅ Save
            await _taskRepo.AddAsync(task);
            await _taskRepo.SaveChangesAsync();

            // ✅ Return DTO
            var dto = _mapper.Map<PhaseTaskDto>(task);
            dto.PhaseName = phaseConfig.CustomPhaseName ?? phaseConfig.SystemPhase?.Name;

            return dto;
        }
    }
}