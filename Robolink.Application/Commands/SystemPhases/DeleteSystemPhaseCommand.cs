using MediatR;

namespace Robolink.Application.Commands.SystemPhases
{
    public record DeleteSystemPhaseCommand(
        Guid SystemPhaseId,
        bool HardDelete = false  // ✅ NEW: Default to soft delete
    ) : IRequest<bool>;
}