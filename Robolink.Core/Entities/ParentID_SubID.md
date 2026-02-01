# HƯỚNG DẪN VỀ PHÂN CẤP DỮ LIỆU (CHA - CON)

Để hệ thống gọn nhẹ và dễ dùng, chúng ta chỉ áp dụng "Cha - Con" cho những gì thực sự cần chia nhỏ:

| Bảng | Có ParentId không? | Tại sao? |
| :--- | :--- | :--- |
| **PhaseTask** | **CÓ** | Để chia nhỏ một việc lớn thành nhiều việc con dễ làm hơn. |
| **Project** | **TÙY** | Chỉ khi dự án quá lớn, cần chia thành nhiều dự án nhỏ độc lập. |
| **Staff** | **KHÔNG** | Đã có quản lý theo từng đầu việc cụ thể. |
| **WorkLog** | **KHÔNG** | Đây là kết quả thực tế, không cần chia nhỏ thêm. |
| **Client** | **KHÔNG** | Để quản lý danh sách khách hàng đơn giản, rõ ràng. |