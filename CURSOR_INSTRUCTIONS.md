# CURSOR_INSTRUCTIONS.md
Website Quản Lý Doanh Nghiệp Vận Tải - Vật Tư Xây Dựng

## 1. Tổng quan
Ứng dụng giúp quản lý **xe tải, tài xế, khách hàng, công nợ, chuyến hàng** và **báo cáo** cho doanh nghiệp vận tải chuyên vật tư xây dựng.  
Triển khai **local bằng Docker**, sử dụng **ReactJS + Ant Design** cho giao diện, **.NET Core Web API** cho backend, và **PostgreSQL** cho cơ sở dữ liệu.

---

## 2. Kiến trúc hệ thống

```
[ReactJS + AntD] <--> [ASP.NET Core Web API] <--> [PostgreSQL]
                                   |
                                   +--> [Worker / Hangfire] (tùy chọn)
```

- **Frontend:** ReactJS + Ant Design
- **Backend:** ASP.NET Core 8.0 Web API
- **Database:** PostgreSQL
- **Triển khai:** Docker Compose

---

## 3. Chức năng chính
1. **Quản lý xe tải:** thêm, sửa, xóa, lịch bảo dưỡng, trạng thái xe.
2. **Quản lý tài xế:** hồ sơ, bằng lái, phân công xe.
3. **Quản lý khách hàng:** thông tin, địa chỉ, công nợ.
4. **Quản lý chuyến hàng:** phân công xe, tài xế, lịch trình.
5. **Quản lý công nợ:** hóa đơn, thanh toán, đối soát.
6. **Báo cáo:** doanh thu, công nợ, trạng thái vận hành.
7. **Xác thực:** JWT Token, phân quyền (Admin, Kế toán, Điều phối, Tài xế).

---

## 4. Cơ sở dữ liệu (PostgreSQL)

### Các bảng chính
- **customers**
- **trucks**
- **drivers**
- **trips**
- **invoices**
- **payments**
- **audit_logs (tùy chọn)**

Sử dụng `uuid` làm khóa chính và `gen_random_uuid()` từ extension `pgcrypto`.

---

## 5. Backend (.NET Core)

**Công nghệ:**
- .NET 8 Web API
- Entity Framework Core (Npgsql)
- AutoMapper, FluentValidation, Serilog, Swagger

**Cấu trúc thư mục:**
```
/src
  /Transport.Api
  /Transport.Application
  /Transport.Domain
  /Transport.Infrastructure
```

**Các controller chính:**
- `AuthController`
- `CustomersController`
- `TrucksController`
- `DriversController`
- `TripsController`
- `InvoicesController`
- `ReportsController`

**Auth:** JWT với refresh token, role-based authorization.

**Swagger:** `/swagger`

---

## 6. Frontend (ReactJS + Ant Design)

**Cấu trúc thư mục:**
```
/client
  /src
    /pages
    /api
    /components
    /hooks
    /stores
```

**Các trang chính:**
- Trang đăng nhập
- Quản lý xe tải
- Quản lý tài xế
- Quản lý khách hàng
- Quản lý chuyến hàng
- Quản lý hóa đơn & thanh toán
- Báo cáo

**Thư viện:** axios, react-router-dom, antd, @ant-design/pro-table

---

## 7. Docker & Triển khai

### docker-compose.yml
```yaml
version: '3.8'
services:
  db:
    image: postgres:15
    environment:
      POSTGRES_USER: transport
      POSTGRES_PASSWORD: transportpwd
      POSTGRES_DB: transportdb
    volumes:
      - pgdata:/var/lib/postgresql/data
    ports:
      - "5432:5432"

  api:
    build:
      context: .
      dockerfile: ./src/Transport.Api/Dockerfile
    environment:
      - ConnectionStrings__Default=Host=db;Port=5432;Database=transportdb;Username=transport;Password=transportpwd
    depends_on:
      - db
    ports:
      - "5000:5000"

  client:
    build:
      context: ./client
      dockerfile: ./client/Dockerfile
    ports:
      - "3000:80"
    depends_on:
      - api

volumes:
  pgdata:
```

**Chạy dự án:**
```bash
docker compose up --build
```

API chạy tại `http://localhost:5000/swagger`  
Frontend chạy tại `http://localhost:3000`

---

## 8. Seed Data & Kiểm thử

**Dữ liệu mẫu:**
- 1 admin user: `admin@example.com / Admin@123`
- 2 khách hàng, 3 xe tải, 3 tài xế, 3 chuyến hàng mẫu.

**Kiểm thử:**
- CRUD đầy đủ các module.
- Tạo chuyến hàng -> sinh hóa đơn -> thanh toán -> kiểm tra báo cáo công nợ.

---

## 9. Tính năng mở rộng đề xuất
- Theo dõi GPS xe tải (realtime tracking).
- Lịch bảo dưỡng tự động.
- Gợi ý tài xế & xe gần nhất (smart dispatch).
- Ứng dụng mobile cho tài xế (React Native).
- Tích hợp xuất hóa đơn sang phần mềm kế toán.
- Gửi email/SMS thông báo chuyến.

---

## 10. Checklist cho Cursor
1. Tạo cấu trúc backend và frontend như mô tả.  
2. Kết nối PostgreSQL, chạy migration & seed data.  
3. Thực thi CRUD cho toàn bộ thực thể.  
4. Tạo UI với React + antd tương ứng mỗi module.  
5. Tạo docker-compose.yml & Dockerfile cho API, Client.  
6. Build và chạy thử toàn hệ thống local.  
7. Cung cấp README + OpenAPI hoặc Postman collection.

---

## 11. Cách khởi chạy (developer local)

```bash
# Clone dự án
git clone <repo>
cd <repo>

# Build và chạy
docker compose up --build

# Kiểm tra
docker compose logs -f api
```

---

## 12. Hoàn tất
Khi Cursor hoàn thành:
- API hoạt động tại `http://localhost:5000/swagger`
- Frontend hiển thị dashboard tại `http://localhost:3000`
- Dữ liệu mẫu hiển thị đầy đủ.

---

**Tệp này là hướng dẫn chuẩn để Cursor tự động xây dựng và triển khai hệ thống.**
