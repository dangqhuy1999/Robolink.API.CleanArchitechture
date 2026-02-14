using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Shared.Enums
{
    public enum LogStatus
    {
        // Nhóm thành công
        Success,      // Hoạt động diễn ra tốt (Robot gắp đúng, Sale chốt đơn, HR phỏng vấn xong)
        // Nhóm cảnh báo/Lỗi
        Warning,      // Có vấn đề nhỏ (Robot nóng, Khách hàng phân vân, Ứng viên đến muộn)
        Error,        // Thất bại hoàn toàn (Robot hỏng, Khách hủy đơn, Ứng viên bỏ thi)
        // Nhóm tiến độ
        Processing,   // Đang diễn ra (Dùng cho các bản ghi kéo dài thời gian)
        // Nhóm hệ thống
        SystemLog     // Các bản ghi tự động của hệ thống (Bảo trì, Update)
    }
}
