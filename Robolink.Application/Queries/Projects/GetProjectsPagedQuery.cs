using MediatR;
using Robolink.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Application.Queries.Projects
{
    // Trong tầng Application
    public class GetProjectsPagedQuery : IRequest<PagedResult<ProjectDto>>
    {
        public int StartIndex { get; set; }
        public int Count { get; set; }
        public string? SearchTerm { get; set; } // 👈 Thêm Property này

        public GetProjectsPagedQuery(int startIndex, int count, string? searchTerm = null)
        {
            StartIndex = startIndex;
            Count = count;
            SearchTerm = searchTerm; // 👈 Gán giá trị
        }
    }
}
