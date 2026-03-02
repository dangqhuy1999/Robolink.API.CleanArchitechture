using MediatR;
using Robolink.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Application.Queries.SystemPhases
{
    public class GetSystemPhasesPagedQuery : IRequest<PagedResult<SystemPhaseDto>>
    {
        public int StartIndex { get; set; }
        public int Count { get; set; }
        public string? SearchTerm { get; set; } // 👈 Thêm Property này

        public GetSystemPhasesPagedQuery(int startIndex, int count, string? searchTerm = null)
        {
            StartIndex = startIndex;
            Count = count;
            SearchTerm = searchTerm; // 👈 Gán giá trị
        }
    }
}
