using MediatR;
using Robolink.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Application.Queries.PhaseTasks
{
    // Trong tầng Application
    public class GetPhaseTasksPagedQuery(int startIndex, int count, Guid phaseId) : IRequest<PagedResult<PhaseTaskDto>>
    {
        public int StartIndex { get; } = startIndex;
        public int Count { get; } = count;
        public Guid PhaseId { get; } = phaseId; // Thêm ID này vào
    }
}
