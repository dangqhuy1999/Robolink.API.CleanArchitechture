using MediatR;
using Robolink.Application.DTOs;

namespace Robolink.Application.Queries.Projects
{
    public class GetAllProjectsQuery : IRequest<IEnumerable<ProjectDto>>
    {
    }
}