using MediatR;
using Robolink.Shared.DTOs;
using Robolink.Shared.Enums;

namespace Robolink.Application.Commands.PhaseTasks
{
    public class UpdatePhaseTaskCommand : IRequest<PhaseTaskDto>
    {
        public Guid Id { get; set; } // ID để riêng bên ngoài để dễ dùng trong URL API
        public UpdatePhaseTaskRequest Request { get; set; } = null!;
        public string? UpdatedBy { get; set; }
    }
}