<<<<<<< HEAD
# Room Rental Management System

Ứng dụng quản lý phòng trọ gồm **Next.js 15 + Tailwind CSS**, **ASP.NET Core 10 Web API**, PostgreSQL, Entity Framework Core và JWT. Backend tự chạy migration khi khởi động và thêm dữ liệu mẫu lần đầu.

## Chức năng

- Admin: quản lý phòng, khách thuê, hợp đồng, hóa đơn, thanh toán, yêu cầu sửa chữa và dashboard.
- Tenant: xem hợp đồng/hóa đơn của chính mình, gửi yêu cầu sửa chữa có ảnh và theo dõi trạng thái.
- Hóa đơn chụp lại mọi đơn giá, chỉ số cũ/mới và các loại phí tại lúc lập; tổng tiền được tính ở backend.
- API kiểm soát role bằng JWT, đồng thời lọc dữ liệu Tenant bằng quan hệ tài khoản - khách thuê.

## Cấu trúc

```text
room-rental-management/
  backend/RoomRental.Api/     # ASP.NET Core API, EF entities, services, controllers
  backend/database/init.sql   # SQL khởi tạo PostgreSQL
  frontend/                   # Next.js App Router + Tailwind
  docker-compose.yml          # PostgreSQL 16
```

## Chạy ứng dụng

1. Khởi động PostgreSQL từ thư mục gốc dự án:

```powershell
docker compose up -d
```

2. Mở terminal thứ nhất, chạy API. Chuỗi kết nối mặc định nằm ở `backend/RoomRental.Api/appsettings.json`; thay đổi nó nếu PostgreSQL của bạn dùng thông tin khác. Khi chạy Development mà chưa có PostgreSQL, API tự dùng SQLite cục bộ (`room-rental-demo.db`) để demo ngay.

```powershell
cd backend/RoomRental.Api
dotnet restore
dotnet run --urls http://localhost:5075
```

Khi chạy lần đầu, `Database.MigrateAsync()` tạo schema và `SeedData` thêm các tài khoản minh họa. Swagger ở `http://localhost:5075/swagger`.

3. Mở terminal thứ hai, tạo file cấu hình frontend rồi chạy Next.js:

```powershell
cd frontend
Copy-Item .env.local.example .env.local
npm install
npm run dev
```

Truy cập `http://localhost:3000`.

## Tài khoản mẫu

| Vai trò | Email | Mật khẩu |
| --- | --- | --- |
| Admin | `admin@nhatro.local` | `Admin@123` |
| Tenant | `tenant@nhatro.local` | `Tenant@123` |

## Migration EF Core thủ công

API đã tự áp dụng migration khi khởi động. Khi thay đổi model trong quá trình phát triển, cài công cụ và tạo migration mới:

```powershell
dotnet tool install --global dotnet-ef
cd backend/RoomRental.Api
dotnet ef migrations add AddYourChange
dotnet ef database update
```

Để triển khai thực tế, hãy chuyển JWT Key, thông tin PostgreSQL và URL frontend sang biến môi trường/secret manager; không dùng các thông tin development trong `appsettings.json`.
=======
# Du-an-nhom---He-thong-quan-ly-phong-tro
Hệ thống quản lý phòng trọ - Bài tập lớn
>>>>>>> 54f2ba9b8631e6e8f6f4373aaaea4eccdab5160b
