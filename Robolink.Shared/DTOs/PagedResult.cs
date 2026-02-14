using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Shared.DTOs
{
    // T là kiểu dữ liệu linh hoạt (sau này dùng cho StaffDto, ClientDto đều được)
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }

        public PagedResult(IEnumerable<T> items, int totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }
    }
}
