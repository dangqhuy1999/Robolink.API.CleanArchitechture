using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Core.Common
{
    // Kế thừa lại EntityBase để tránh viết lặp lại Id và Audit
    public abstract class EntityRootBase : EntityBase
    {
        // Có thể thêm các thuộc tính riêng cho Aggregate Root ở đây 
        // Ví dụ: Version để xử lý tranh chấp dữ liệu (Concurrency)
        public required byte[] RowVersion { get; set; }
    }
}
