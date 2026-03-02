using MediatR;
using Robolink.Shared.DTOs;
using System.Collections.Generic;

namespace Robolink.Application.Queries.Staff
{
    public class GetAllStaffQuery : IRequest<PagedResult<StaffDto>>
    {
        public int StartIndex { get; set; }
        public int Count { get; set; }

        // Constructor để gán giá trị nhanh
        public GetAllStaffQuery(int startIndex, int count)
        {
            StartIndex = startIndex;
            Count = count;
        }
    }
}