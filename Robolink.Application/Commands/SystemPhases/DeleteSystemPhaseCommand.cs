using MediatR;

namespace Robolink.Application.Commands.SystemPhases
{
    public class DeleteSystemPhaseCommand : IRequest<bool>
    {
        public Guid PhaseId { get; set; }
        public bool HardDelete { get; set; } = false; // false = soft delete

        public DeleteSystemPhaseCommand(Guid phaseId, bool hardDelete = false)
        {
            PhaseId = phaseId;
            HardDelete = hardDelete;
        }
    }
}