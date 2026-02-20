namespace Robolink.Shared.DTOs.Pagination
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new(); // Danh sách dữ liệu
        public int TotalCount { get; set; }        // Tổng số bản ghi trong DB
        public int StartIndex { get; set; }
        public int Count { get; set; }
    }
}
