# Robolink Clean Architecture - README

Dưới đây là README cho dự án Robolink (Blazor WebApp + API) theo cấu trúc Clean Architecture 5 tầng và mô tả flow request/response như yêu cầu.

[![Build](https://github.com/jasontaylordev/CleanArchitecture/actions/workflows/build.yml/badge.svg)](https://github.com/jasontaylordev/CleanArchitecture/actions/workflows/build.yml)

## Mục lục
- Giới thiệu nhanh
- Flow request (từ UI tới DB)
- Flow response (mô tả chi tiết các chặng)
- Cách chạy (Dev & Production)
- Cấu trúc dự án (tổng quan 2 tầng: API + WebApp và 5 tầng Clean Architecture)
- Database
- Deploy
- API Documentation
- Technologies
- Hỗ trợ & License

---

## Giới thiệu
Dự án này sử dụng mô hình Clean Architecture với 5 tầng (Presentation, Application, Domain, Infrastructure, Shared/Common). Frontend là Blazor WebApp (Server/WasM tuỳ cấu hình), backend là ASP.NET Core API. 

Mục tiêu: tách biệt rõ ràng trách nhiệm, dễ test, dễ mở rộng.

---

## Flow request (tóm tắt)

User Input -> ViewModel -> [Mapper 1] -> DTO -> API -> Handler -> [Mapper 2] -> Entity -> Repo -> DB

> Lưu ý: Cách viết Mapper1/Mapper2 tùy vị trí (Backend/Frontend). Mapper 1 thường là AutoMapper ở backend (Entity -> DTO hoặc ProjectTo). Mapper 2 là mapper ở frontend (DTO -> ViewModel).

---

## Flow Response (chi tiết)
```
* **Chặng 1 (Kho bãi - DB):** Request từ UI gửi xuống yêu cầu "Cho tôi xem danh sách Project ở trang 1". Entity Framework (EF Core) trong Repository sẽ bắt đầu chuẩn bị chui vào Database để lấy hàng (`Entity`).
* **Chặng 2 (Tối ưu hóa bằng `.ProjectTo()`):** Đây là **Mapper 1** (ở Backend).
* *Nếu không có `.ProjectTo()`:* EF Core sẽ `SELECT *` (lấy toàn bộ các cột, kể cả dữ liệu mật, password, file đính kèm...) đưa lên RAM rồi mới dùng Mapper để cắt gọt thành `DTO`. -> Rất tốn RAM và chậm!
* *Có `.ProjectTo<DTO>()`:* AutoMapper sẽ can thiệp thẳng vào câu lệnh SQL. Nó bảo DB là: *"Ê, thằng DTO này chỉ cần Id, Name và Status thôi, chỉ `SELECT` 3 cột đó cho tao"*. Kết quả là DB trả về thẳng `DTO` luôn, bỏ qua bước trung gian là `Entity`. Cực kỳ tối ưu!


* **Chặng 3 (Vận chuyển - API):** `Handler` nhận được cục `DTO` xịn xò, gọn gàng. Giao cho `API` để nén thành định dạng JSON và bắn qua mạng Internet trả về cho WebApp.
* **Chặng 4 (Trang điểm UI - Mapper 2):** Blazor WebApp nhận được cục JSON, giải mã lại thành `DTO`. Lúc này, **Mapper 2** (ở Frontend) xuất hiện. Nó có nhiệm vụ "trang điểm" cho dữ liệu bằng cách map `DTO` sang `ViewModel`.
* *Tại sao phải làm thế?* Vì `DTO` chỉ chứa dữ liệu "thuần chay" (Id, Name). Nhưng UI thì cần thêm các biến trạng thái như `IsLoading`, `IsSelected` (để check box), `ShowDropdown`... Các thuộc tính này chỉ có ở `ViewModel`. Sau khi map xong, dữ liệu được đổ vào file `.razor` để vẽ lên bảng.
```

---

## Cách chạy (Development)
Mở 2 terminal/terminal tabs vì giải pháp gồm 2 app chính: API và WebApp.

1. Cài đặt & Build

```bash
# 1. Restore
dotnet restore

# 2. (Tùy chọn) build solution
dotnet build
```

2. Chạy API + WebApp (cách nhanh)

```bash
# Chạy API
dotnet run --project Robolink.API/Robolink.API.csproj

# Chạy WebApp (Blazor)
dotnet run --project Robolink.WebApp/Robolink.WebApp.csproj
```

- Mở trình duyệt vào `https://localhost:5001` (WebApp) và `https://localhost:5000` (API) hoặc theo URL console hiển thị.
- Nếu dùng Visual Studio / VS Code, set startup projects: `Robolink.API` và `Robolink.WebApp` cùng lúc (multi-startup) hoặc debug từng app.

---

## Cách chạy (Production)
Chuẩn bị build release, publish và deploy vào môi trường hosting (Linux/nginx, Windows/IIS, Azure App Service, Docker, ...).

1. Publish API

```bash
dotnet publish Robolink.API/Robolink.API.csproj -c Release -o ./publish/api
```

2. Publish WebApp

```bash
dotnet publish Robolink.WebApp/Robolink.WebApp.csproj -c Release -o ./publish/webapp
```

3. Triển khai
- Linux/nginx: dùng reverse proxy (Kestrel chạy ứng dụng, nginx proxy tới Kestrel). Tham khảo cấu hình systemd + nginx.
- Windows/IIS: tạo website trỏ tới thư mục publish và cấu hình Application Pool (No Managed Code cho .NET Core hosting bundle).
- Azure: dùng Azure App Service hoặc Azure Container Instances / AKS nếu container hoá.

4. Docker (tùy chọn)
- Tạo Dockerfile cho API & WebApp, build image và push lên registry, deploy bằng Docker Compose/Swarm/Kubernetes.

---

## Cấu trúc dự án (tổng quan)
- src/
  - Robolink.API -> ASP.NET Core Web API (Presentation: Controllers)
  - Robolink.WebApp -> Blazor WebApp (Presentation: Razor pages & components)
  - Application -> Use cases, Handlers, DTOs, Interfaces
  - Domain -> Entities, ValueObjects, Domain Services
  - Infrastructure -> EF Core, Repositories, DataContext, Migrations
  - Shared / Common -> DTOs, Constants, Helpers

5 tầng (Clean Architecture) ứng dụng:
- Presentation: Controllers / Blazor Components
- Application: Handlers (UseCases), DTOs, Interfaces, Mappers
- Domain: Entities, Domain Rules
- Infrastructure: Repositories, EF Core, Persistence
- Shared: DTOs/Constants/Utilities

---

## Database
- Mặc định dự án sử dụng SQLite trong dev (hoặc Postgres/SQL Server theo cấu hình). Kiểm tra `appsettings.Development.json` và `appsettings.Production.json`.
- Khi khởi chạy lần đầu, dự án có thể chạy migration / seed (ApplicationDbContextInitialiser) để tạo và seed dữ liệu mẫu.

---

## Deploy
- Hướng dẫn nhanh: publish từng app (API + WebApp) rồi deploy. Dùng reverse proxy cho WebApp nếu cần.
- Khuyến nghị: dùng CI/CD (GitHub Actions) để build, test, publish và deploy.

---

## API Documentation
OpenAPI/Swagger có thể được bật trên API. Sau khi chạy API, mở `https://{api-host}/swagger` hoặc xem `wwwroot/openapi/v1.json` nếu dự án ghi spec ra file.

---

## Technologies
- .NET 10, ASP.NET Core 10
- Blazor (WebApp)
- Entity Framework Core
- AutoMapper
- MediatR (Use case / Handler pattern)
- Refit (có client API calls)

---

## Support
Nếu gặp lỗi khi chạy, kiểm tra:
- Connection string trong `appsettings.*.json`
- Các migration đã được áp dụng hay chưa
- Ports bị chiếm bởi process khác

---

## License
MIT

