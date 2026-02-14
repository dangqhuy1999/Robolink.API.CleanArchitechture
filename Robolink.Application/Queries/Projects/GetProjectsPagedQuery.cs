using MediatR;
using Robolink.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Application.Queries.Projects
{
    // Trong tầng Application
    public class GetPhaseTasksPagedQuery(int startIndex, int count) : IRequest<PagedResult<ProjectDto>>
    {
        // Bắt buộc phải có 2 dòng này để Handler "nhìn thấy" dữ liệu
        public int StartIndex { get; } = startIndex;
        public int Count { get; } = count;
    }
}
