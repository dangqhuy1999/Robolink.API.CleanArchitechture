using MediatR;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Queries.Projects
{
    public class GetAllProjectsQuery : IRequest<PagedResult<ProjectDto>>
    {
        public int StartIndex { get; set; }
        public int Count { get; set; }

        // Constructor để gán giá trị nhanh
        public GetAllProjectsQuery(int startIndex, int count)
        {
            StartIndex = startIndex;
            Count = count;
        }
    }
}