# CẨM NANG VỀ ROWVERSION: CƠ CHẾ CHỐNG GHI ĐÈ DỮ LIỆU (CONCURRENCY)

Trong một hệ thống có nhiều người dùng cùng lúc như Robolink, việc hai người cùng sửa một bản ghi là chuyện thường xuyên xảy ra. `RowVersion` chính là "trọng tài" để phân xử ai được phép lưu dữ liệu.

---

## 1. VẤN ĐỀ: "NGƯỜI ĐẾN SAU XÓA SẠCH CÔNG SỨC NGƯỜI TRƯỚC"

Hãy tưởng tượng kịch bản tại dự án Robolink khi **KHÔNG CÓ** RowVersion:

1. **Anh Quản lý (User A)** mở trang chỉnh sửa Dự án X. Anh thấy ngân sách đang là **100 triệu**. Anh ngồi tính toán để sửa lên **150 triệu**.
2. **Chị Kế toán (User B)** cũng mở đúng dự án đó. Chị cũng thấy **100 triệu**. Chị sửa tên dự án và nhấn **LƯU** ngay lập tức. Hệ thống ghi nhận dữ liệu của chị B.
3. **Anh Quản lý (User A)** sau 5 phút suy nghĩ, nhấn **LƯU** mức 150 triệu.

**Hậu quả:** Hệ thống ghi đè con số 150 triệu lên toàn bộ những gì chị Kế toán vừa sửa. Chị B sẽ rất bực mình vì "rõ ràng mình vừa sửa xong mà giờ mất tiêu". Đây gọi là lỗi **Lost Update** (Mất dữ liệu cập nhật).

---

## 2. GIẢI PHÁP: ROWVERSION (CƠ CHẾ TEM NIÊM PHONG)

`RowVersion` (hay còn gọi là Concurrency Token) hoạt động như một cái **Tem niêm phong tự động thay đổi**.



### Cách vận hành trong thực tế:
- **Lúc xem:** Khi bạn mở dữ liệu, hệ thống gửi cho bạn dữ liệu kèm theo một mã định danh (ví dụ: `V1`).
- **Lúc lưu:** Khi bạn nhấn nút Lưu, hệ thống sẽ hỏi: *"Cái mã của bạn còn khớp với mã hiện tại trong kho không?"*
    - **NẾU KHỚP:** Cho phép lưu và **tự động đổi mã** sang `V2`.
    - **NẾU KHÔNG KHỚP:** (Nghĩa là có ai đó đã nhanh tay lưu trước bạn và đổi mã sang `V2` rồi). Hệ thống sẽ chặn bạn lại và báo lỗi.

---

## 3. Ý NGHĨA TRONG DỰ ÁN ROBOLINK

Trong hệ thống của chúng ta, `RowVersion` bảo vệ các điểm nhạy cảm sau:

| Đối tượng | Rủi ro nếu thiếu RowVersion |
| :--- | :--- |
| **Project Budget** | Sai lệch tiền bạc giữa kế toán và quản lý dự án. |
| **PhaseTask Status** | Một người vừa bấm "Hoàn thành" thì người khác lại bấm "Hủy" do nhìn thấy dữ liệu cũ. |
| **WorkLog Quantity** | Tránh việc hai Robot cùng cập nhật sản lượng vào một mục tiêu dẫn đến số liệu bị ảo. |

---

## 4. QUY TRÌNH XỬ LÝ LỖI (DÀNH CHO LẬP TRÌNH VIÊN)

Khi `RowVersion` phát hiện xung đột, Entity Framework sẽ ném ra ngoại lệ: `DbUpdateConcurrencyException`.

**Cách xử lý chuyên nghiệp (Chuẩn DDD/CQRS):**
1. **Báo lỗi cho người dùng:** "Dữ liệu bạn đang sửa đã bị thay đổi bởi người khác."
2. **Hỗ trợ người dùng:** Hiển thị dữ liệu mới nhất mà người kia vừa sửa để người dùng so sánh.
3. **Lựa chọn:** Yêu cầu người dùng nhập lại hoặc thực hiện "Merge" (trộn) dữ liệu thủ công.



---

## 5. LƯU Ý KỸ THUẬT (DÀNH CHO TẦNG INFRASTRUCTURE & FRONTEND)

- **SQL Server:** Kiểu dữ liệu thực tế là `rowversion`. Nó tự tăng, chúng ta không thể tự sửa bằng tay.
- **Frontend (Web/App):** Phải luôn mang theo trường `RowVersion` trong các yêu cầu gửi đi (DTO/Request). Nếu thiếu, cơ chế này sẽ bị vô hiệu hóa.

---
*Tài liệu này giải thích lý do tại sao hệ thống Robolink lại cực kỳ an toàn về mặt dữ liệu, ngay cả khi có hàng trăm người cùng thao tác một lúc.*