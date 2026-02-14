using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Commands.PhaseTasks
{
    public class CreatePhaseTaskCommand : IRequest<PhaseTaskDto>
    {
        public CreatePhaseTaskRequest Request { get; set; } = null!;
        public string? CreatedBy { get; set; }
    }
}