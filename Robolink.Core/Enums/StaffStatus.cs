using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Core.Enums
{
    public enum StaffStatus
    {
        Active,    // Đang làm việc, được phép đăng nhập
        Inactive,  // Tạm nghỉ (thai sản, ốm đau), không cho đăng nhập nhưng vẫn giữ data
        Terminated // Đã nghỉ việc hẳn (Xóa mềm)
    }
}
