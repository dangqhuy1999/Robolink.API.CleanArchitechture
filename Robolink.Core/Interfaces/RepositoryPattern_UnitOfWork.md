/*
# CHIẾN LƯỢC QUẢN LÝ CRUD CHO HỆ THỐNG NHIỀU ĐỐI TƯỢNG

## 1. Vấn đề
Hệ thống Robolink có rất nhiều bảng (Staff, Client, Project, PhaseTask, WorkLog, Config...). Nếu viết CRUD riêng lẻ sẽ dẫn đến:
- Code bị lặp lại (Boilerplate code).
- Khó bảo trì (Sửa 1 chỗ phải sửa 10 chỗ).

## 2. Giải pháp: Generic Repository
Chúng ta xây dựng một "Bộ điều khiển tổng" có khả năng làm việc với mọi loại Entity (Đối tượng).

## 3. Quy trình thực hiện
1. **Định nghĩa BaseEntity:** Tất cả các bảng đều phải có các trường chung (Id, CreatedAt, IsDeleted).
2. **Xây dựng Generic Repository:** Viết các hàm Thêm/Sửa/Xóa/Lấy một lần duy nhất bằng cách sử dụng kiểu dữ liệu `<T>`.
3. **Đăng ký Dependency Injection:** - `builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));`

## 4. Lợi ích cho người vận hành
- **Tốc độ:** Phát triển thêm bảng mới cực nhanh.
- **An toàn:** Tất cả các bảng đều được áp dụng chung cơ chế `Soft Delete` (không xóa thật) và `Global Query Filter` mà em đã cấu hình trong DBContext.
# GIẢI THÍCH TỪ KHÓA VIRTUAL TRONG REPOSITORY

## 1. Định nghĩa đơn giản
- **Virtual**: Nghĩa là "Mặc định là thế này, nhưng có thể thay đổi". 
- Nó tạo ra sự linh hoạt để các bảng khó (như Project, PhaseTask) có thể tự viết lại cách lấy dữ liệu riêng mà không làm ảnh hưởng đến các bảng dễ (như Staff, Client).

## 2. Tại sao phải dùng?
- **Tính kế thừa**: Giúp code gọn gàng. Cái gì giống nhau thì dùng chung ở `Generic`, cái gì đặc thù thì mới viết riêng.
- **Khả năng mở rộng**: Sau này nếu em muốn bảng `WorkLog` khi lấy ra phải tự động tính toán gì đó, em chỉ cần `override` lại hàm của `Generic` là xong.

## 3. Quy tắc nhớ nhanh
- **Cha dùng `virtual`**: Để "mở cửa" cho phép con sửa.
- **Con dùng `override`**: Để thực hiện việc sửa đổi đó.

1. Tại sao em cần Unit of Work?
Hãy tưởng tượng một quy trình thực tế trong dự án Robolink khi em tạo một Dự án mới:

Bước 1: Thêm thông tin vào bảng Project.

Bước 2: Thêm các công đoạn vào bảng ProjectSystemPhaseConfig.

Bước 3: Tạo các nhiệm vụ đầu tiên vào bảng PhaseTask.

Nếu không có Unit of Work: Chẳng may khi đang làm đến Bước 3 thì server mất điện. Kết quả là bảng Project đã có dữ liệu, nhưng PhaseTask thì không. Dữ liệu của em bị "rác" và thiếu đồng nhất.

Nếu có Unit of Work: Mọi thay đổi ở Bước 1, 2, 3 chỉ được giữ ở "bộ nhớ tạm". Khi em gọi lệnh SaveChange() của Unit of Work, nó sẽ đẩy tất cả vào Database trong một giao dịch duy nhất (Transaction). Nếu một bước lỗi, nó sẽ Rollback (hủy bỏ) toàn bộ, trả Database về trạng thái sạch ban đầu.
*/