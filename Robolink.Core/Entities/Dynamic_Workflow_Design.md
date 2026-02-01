# TÀI LIỆU THIẾT KẾ: HỆ THỐNG QUY TRÌNH ĐỘNG (DYNAMIC WORKFLOW)

**Mục tiêu:** Xây dựng một hệ thống không bị "cứng hóa" (Hard-code) vào quy trình sản xuất Robot, mà có thể biến hình để phục vụ cho Tuyển dụng (HR), Bán hàng (Sales) hoặc bất kỳ quy trình nào khác mà **không cần sửa code**.

---

## 1. TRIẾT LÝ THIẾT KẾ: "THỰC ĐƠN VÀ GỌI MÓN"

Thay vì viết cứng quy trình vào phần mềm, chúng ta tách nó ra làm hai phần:

### A. Kho thực đơn (Master Data - `SystemPhase`)
- **Định nghĩa:** Đây là nơi chứa tất cả các bước có thể xảy ra trên đời này mà công ty có thể làm.
- **Ví dụ:** Thiết kế, Cắt CNC, Phỏng vấn, Ký hợp đồng, Giao hàng...
- **Đặc điểm:** Thêm mới thoải mái, càng nhiều càng tốt.

### B. Tờ gọi món (Project Config - `ProjectSystemPhaseConfig`)
- **Định nghĩa:** Khi có một dự án cụ thể, chúng ta chỉ "nhặt" ra những bước cần thiết từ kho thực đơn.
- **Đặc điểm:** Mỗi dự án có một tờ gọi món riêng biệt. Dự án này không ảnh hưởng dự án kia.

---

## 2. KỊCH BẢN CHỨNG MINH SỰ LINH HOẠT

Dưới đây là 3 tình huống thực tế chứng minh hệ thống này "bất tử" trước sự thay đổi của nghiệp vụ.

### ✅ Tình huống 1: Chuyển từ làm Robot sang làm HR (Tuyển dụng)
Công ty mở rộng mảng nhân sự. Quy trình hoàn toàn khác biệt.

* **Cách xử lý:**
    1.  **Vào Kho thực đơn (`SystemPhase`):** Thêm các bước mới: "Sàng lọc hồ sơ", "Phỏng vấn vòng 1", "Offer lương".
    2.  **Tạo Dự án mới:** Tên là "Tuyển dụng Fresher 2026".
    3.  **Cấu hình (`Config`):** Nối dự án này với 3 bước vừa tạo ở trên.
* **Kết quả:** Hệ thống chạy ngay lập tức. Nhân viên HR vào nhập liệu bình thường. Không cần viết lại phần mềm.

### ✅ Tình huống 2: Khách hàng yêu cầu thêm "Bước F" (Kỳ quặc)
Khách hàng yêu cầu trước khi giao hàng phải có bước "Cúng tổ nghề" hoặc "Kiểm tra phong thủy".

* **Cách xử lý:**
    1.  **Vào Kho thực đơn:** Thêm bước "Other_Check" (Kiểm tra khác).
    2.  **Vào Cấu hình dự án:** Chọn bước "Other_Check" đó.
    3.  **Đổi tên hiển thị (`CustomPhaseName`):** Sửa thành **"Cúng tổ nghề"**.
* **Kết quả:** Trên màn hình nhân viên hiện chữ "Cúng tổ nghề". Dữ liệu vẫn được lưu trữ chuẩn chỉnh.

### ✅ Tình huống 3: Rút gọn quy trình
Dự án gấp, Sếp muốn bỏ qua bước "Sơn tĩnh điện".

* **Cách xử lý:** Vào cấu hình dự án đó, xóa dòng liên kết với bước "Sơn tĩnh điện".
* **Kết quả:** Quy trình của dự án đó lập tức ngắn lại, nhân viên không còn thấy bước đó nữa.

---

## 3. SO SÁNH: CÁCH CŨ vs. CÁCH MỚI (ROBOLINK)

| Tiêu chí | Cách cũ (Hard-code Enum) | Cách mới (Dynamic Database) |
| :--- | :--- | :--- |
| **Khi thêm bước mới** | Phải sửa Code C#, Build lại, Deploy lại Server (Tốn công, rủi ro). | Chỉ cần mở Admin, bấm "Thêm mới" vào Database (30 giây). |
| **Khi đổi tên bước** | Sửa Code (ảnh hưởng toàn bộ hệ thống). | Sửa trong Cấu hình dự án (chỉ ảnh hưởng 1 dự án đó). |
| **Khả năng mở rộng** | Chỉ làm được đúng 1 ngành nghề. | **Làm được mọi ngành nghề (Đa hệ).** |
| **Dừng hệ thống?** | Có (để update phiên bản mới). | **Không (Update sống - Live Update).** |

---

## 4. KẾT LUẬN

Hệ thống Robolink được thiết kế không phải để quản lý Robot, mà là để **quản lý quy trình**.
> *"Bất kể bạn làm gì, miễn là có Quy trình (Bước 1 -> Bước 2 -> Bước 3), hệ thống này đều cân được tất cả."*