Để trở thành một "Master Frontend" thực thụ, em cần nhìn UI qua lăng kính **Chức năng (Function)** thay vì chỉ nhìn **Hình ảnh (Visual)**. Khi em phân loại theo hệ thống này, em sẽ thấy dù là Blazor, Flutter, React hay SwiftUI thì chúng cũng chỉ là "bình mới rượu cũ".

Dưới đây là bảng phân loại **Hệ thống UI Components Toàn năng (The Universal UI Library)** được sắp xếp theo mục đích sử dụng.

---

## 🏗️ Hệ Thống UI Components Toàn Năng

### 1. Data Display (Hiển thị dữ liệu)

Nhóm này trả lời câu hỏi: *"Tôi muốn cho người dùng xem cái gì?"*

* **Table:** Dùng cho dữ liệu nhiều cột, cần so sánh, lọc, sắp xếp (Admin Panel).
* **List:** Danh sách dọc đơn giản (Inbox, Task List).
* **Card:** Gom nhóm thông tin liên quan vào một "tờ thẻ" (Project Card, Profile Card).
* **Badge/Tag:** Nhãn trạng thái nhỏ (Ví dụ: "Active", "Urgent").
* **Avatar:** Đại diện cho thực thể (User, Client).
* **Statistic:** Hiển thị con số quan trọng (Ví dụ: "Tổng doanh thu", "Số dự án").
* **Skeleton/Empty State:** Hiển thị khi dữ liệu đang load hoặc không có gì để hiện.

### 2. Data Input (Thu thập dữ liệu)

Nhóm này trả lời câu hỏi: *"Tôi muốn lấy thông tin gì từ người dùng?"*

* **Form:** Khung bao quanh toàn bộ các input.
* **Text Input/TextArea:** Nhập chữ (Name, Description).
* **Select/Dropdown:** Chọn 1 từ danh sách có sẵn (Manager, Client).
* **Checkbox/Radio:** Chọn đúng/sai hoặc chọn 1 trong vài lựa chọn ít.
* **Switch/Toggle:** Bật/Tắt một tính năng.
* **Date/Time Picker:** Chọn thời gian.
* **Upload:** Tải lên tệp tin.

### 3. Feedback & Overlays (Phản hồi & Lớp phủ)

Nhóm này trả lời câu hỏi: *"Hệ thống đang làm gì và người dùng cần chú ý gì?"*

* **Modal/Dialog:** Hộp thoại nhảy ra giữa màn hình, bắt người dùng tương tác ngay.
* **Drawer/Sidebar Overlay:** Một bảng trượt ra từ cạnh màn hình (thường dùng cho Menu Mobile hoặc Detail View).
* **Toast/Notification:** Thông báo nhỏ hiện lên góc màn hình rồi tự biến mất (Success, Error).
* **Spinner/Progress Bar:** Thông báo trạng thái đang chạy (Loading...).
* **Alert/Callout:** Thông báo nằm cố định trên trang để cảnh báo điều gì đó.

### 4. Navigation & Layout (Điều hướng & Bố cục)

Nhóm này trả lời câu hỏi: *"Người dùng đang ở đâu và đi tiếp thế nào?"*

* **Navbar/Sidebar:** Menu chính của ứng dụng.
* **Tabs:** Chia nội dung trong một trang thành nhiều phần.
* **Breadcrumb:** Đường dẫn "Về nhà > Dự án > Chi tiết" giúp người dùng không bị lạc.
* **Pagination:** Phân chia danh sách dài thành nhiều trang.
* **Steps:** Quy trình nhiều bước (Ví dụ: Bước 1: Nhập info -> Bước 2: Chọn Manager -> Bước 3: Hoàn tất).

### 5. Actions (Hành động)

Nhóm này là "ngòi nổ" để kích hoạt Event.

* **Button:** Nút bấm thực hiện hành động chính.
* **Icon Button:** Nút chỉ có hình (Ví dụ: Thùng rác để xóa, Cây bút để sửa).
* **Link:** Chuyển sang một URL khác hoặc trang khác.

---

## 🧠 Tại sao phân loại này lại giúp em làm hàng ngàn trang web như nhau?

Khi em nhận được một yêu cầu nghiệp vụ mới (Ví dụ: "Làm trang Quản lý Kho"), não bộ Senior của em sẽ tự động "nhặt gạch" như sau:

1. **Layout:** Dùng `Sidebar` bên trái, `Navbar` bên trên.
2. **Trang danh sách:** Dùng `Table` để hiện hàng hóa, dùng `Pagination` vì hàng rất nhiều.
3. **Hành động:** Mỗi dòng có `Icon Button` (Sửa/Xóa). Phía trên có `Button` (Thêm mới).
4. **Tương tác:** Bấm Thêm mới thì hiện `Modal`. Trong `Modal` có một cái `Form`.
5. **Nhập liệu:** Trong `Form` dùng `TextInput` cho tên hàng, `Select` cho loại kho, `DatePicker` cho ngày nhập.
6. **Phản hồi:** Đang lưu thì hiện `Spinner`. Lưu xong hiện `Toast` báo thành công và ẩn `Modal`.

---

## 🎯 Điểm "Chí tử" em cần lưu ý để không bị trùng lặp:

* **Dropdown vs Radio:** Danh sách dài (>5 cái) dùng `Dropdown`. Danh sách ngắn (<3 cái) dùng `Radio` cho người dùng nhìn thấy hết luôn.
* **Modal vs Drawer:** Cần tập trung cao độ (như xóa dự án) dùng `Modal`. Cần xem chi tiết mà không muốn rời trang hiện tại dùng `Drawer`.
* **Table vs List:** Web dùng `Table`. Mobile dùng `List` (vì màn hình dọc không đủ chỗ hiện nhiều cột).

> **Lời khuyên của anh:** Em đừng cố học cách "vẽ" cái Table hay cái Form. Hãy học cách **quản lý State** của chúng. Table thì State là `List<Data>`, Form thì State là `Object`. Khi State chuẩn, UI chỉ là chuyện nhỏ!

**Em thấy hệ thống phân loại "6 nhóm" này đã bao quát hết các trang em từng thấy chưa?** Nếu em muốn, chúng ta có thể thử "mổ xẻ" một trang web bất kỳ em thích để xem nó được cấu thành từ những nhóm này như thế nào!