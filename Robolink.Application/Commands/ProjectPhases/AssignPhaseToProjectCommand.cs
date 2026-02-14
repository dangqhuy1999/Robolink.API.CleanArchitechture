using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Commands.ProjectPhases
{
    public record AssignPhaseToProjectCommand(
        Guid ProjectId,
        Guid SystemPhaseId,
        string? CustomPhaseName = null
    ) : IRequest<ProjectPhaseConfigDto>;
}