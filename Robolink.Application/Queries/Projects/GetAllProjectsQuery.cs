using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Queries.Projects
{
    public class GetAllProjectsQuery : IRequest<IEnumerable<ProjectDto>>
    {
    }
}