using MediatR;
using Robolink.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Application.Queries.PhaseTasks
{
    // Trong tầng Application
    public class GetPhaseTasksPagedQuery : IRequest<PagedResult<PhaseTaskDto>>
    {
        public int StartIndex { get; } 
        public int Count { get; } 
        public Guid PhaseId { get; }  // Thêm ID này vào
        public string? SearchTerm { get; set; } // 👈 Thêm Property này

        public GetPhaseTasksPagedQuery(int startIndex, int count, Guid phaseId, string? searchTerm  )
        {
            StartIndex = startIndex;
            Count = count;
            PhaseId = phaseId; // 👈 Gán giá trị
            SearchTerm = searchTerm;
        }
    }
}
