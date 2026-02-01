/*
# GIẢI THÍCH VỀ EXPRESSION FUNC (BỘ LỌC THÔNG MINH)

## 1. Định nghĩa dễ hiểu
using System.Linq.Expressions;

-**Expression < Func<T, bool> > **: Là một "Yêu cầu lọc dữ liệu" viết bằng C# nhưng được thực thi bởi Database.
- Nó giúp chúng ta không cần viết quá nhiều hàm tìm kiếm riêng lẻ.

## 2. Cách hoạt động
1. **Viết**: Em viết code C# (dạng Lambda: `x => x...`).
2. **Dịch**: Entity Framework dịch nó sang câu lệnh SQL.
3. **Chạy**: Database lọc dữ liệu và chỉ trả về kết quả khớp yêu cầu.

## 3. Ví dụ trong Robolink
- Lọc nhân viên: `staffRepo.FindAsync(s => s.Email == "admin@robolink.com")`
-Lọc nhiệm vụ trễ hạn: `taskRepo.FindAsync(t => t.Deadline < DateTime.Now)`

# GIẢI THÍCH VỀ UPDATERANGEASYNC (CẬP NHẬT HÀNG LOẠT)

## 1. Định nghĩa
- **UpdateRange**: Cập nhật một danh sách (tập hợp) các đối tượng thay vì một đối tượng đơn lẻ.
- **Async**: Chạy bất đồng bộ, giúp ứng dụng không bị "đơ" trong lúc chờ Database xử lý lượng dữ liệu lớn.

## 2. Ưu điểm vượt trội
- **Hiệu suất (Performance)**: Giảm số lượng chuyến đi (Round-trips) giữa ứng dụng và máy chủ dữ liệu. 
- **Tốc độ**: Cập nhật 1000 dòng bằng `UpdateRange` nhanh hơn nhiều so với việc chạy vòng lặp cập nhật từng dòng.

## 3. Khi nào nên dùng?
- Khi thay đổi trạng thái của nhiều nhiệm vụ cùng lúc.
- Khi cập nhật thông tin chung cho một nhóm nhân viên hoặc dự án.
- Khi đồng bộ hóa dữ liệu từ một nguồn bên ngoài vào hệ thống Robolink.
*/