using MediatR;
using Robolink.Application.DTOs;
using Robolink.Core.Enums;

namespace Robolink.Application.Commands.PhaseTasks
{
    public record UpdatePhaseTaskCommand(
        Guid PhaseTaskId,
        string Description,
        Guid AssignedStaffId,
        DateTime DueDate,
        Task_Status Status,
        int ProcessRate = 0,
        int Priority = 0,  // ✅ NEW
        decimal EstimatedHours = 0m,
        Guid? ParentPhaseTaskId = null,  // ✅ NEW
        string? CreatedBy = null
    ) : IRequest<PhaseTaskDto>;
}