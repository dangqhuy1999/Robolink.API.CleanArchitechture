using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Commands.Projects
{
    public class UpdateProjectCommand : IRequest<ProjectDto>
    {
        public Guid Id { get; set; } // ID để riêng bên ngoài để dễ dùng trong URL API
        public UpdateProjectRequest Request { get; set; } = null!;
        public string? UpdatedBy { get; set; }
    }
}