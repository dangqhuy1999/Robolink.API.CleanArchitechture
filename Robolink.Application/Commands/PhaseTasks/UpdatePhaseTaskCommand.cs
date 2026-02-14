using MediatR;
using Robolink.Shared.DTOs;
using Robolink.Shared.Enums;

namespace Robolink.Application.Commands.PhaseTasks
{
    public record UpdatePhaseTaskCommand : IRequest<PhaseTaskDto>

    {
        public string? Name { get; set; }
        public Guid Id { get; init; }
        public string? Description { get; init; }
        public Guid? AssignedStaffId { get; init; }
        public DateTime? DueDate { get; init; }
        public Task_Status? Status { get; init; }
        public int? Priority { get; init; }
        public decimal? EstimatedHours { get; init; }
        public Guid? ParentPhaseTaskId { get; init; }
        public string? CreatedBy { get; init; }
        public decimal? InternalBudget { get; set; }

        public decimal? CustomerBudget { get; set; }
    }
}