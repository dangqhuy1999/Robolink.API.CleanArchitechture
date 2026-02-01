using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Core.Common
{
    public abstract class EntityBase
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Tự tạo Id mới khi khởi tạo
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        // [THÊM MỚI] 
        public bool IsDeleted { get; set; } = false;
    }
}
