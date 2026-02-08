using MediatR;
using Robolink.Application.DTOs;
using Robolink.Core.Enums;

namespace Robolink.Application.Commands.PhaseTasks
{
    public record UpdatePhaseTaskCommand : IRequest<PhaseTaskDto>

    {
        public Guid Id { get; init; }
        public string? Description { get; init; }
        public Guid AssignedStaffId { get; init; }
        public DateTime DueDate { get; init; }
        public Task_Status Status { get; init; }
        public int Priority { get; init; }
        public decimal EstimatedHours { get; init; }
        public Guid? ParentPhaseTaskId { get; init; }
        public string? CreatedBy { get; init; }
    }
}