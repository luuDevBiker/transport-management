# Website Quản Lý Doanh Nghiệp Vận Tải - Vật Tư Xây Dựng

Ứng dụng quản lý **xe tải, tài xế, khách hàng, công nợ, chuyến hàng** và **báo cáo** cho doanh nghiệp vận tải chuyên vật tư xây dựng.

## 🚀 Công nghệ sử dụng

- **Frontend:** ReactJS + TypeScript + Ant Design
- **Backend:** ASP.NET Core 8.0 Web API
- **Database:** PostgreSQL 15
- **Triển khai:** Docker Compose

## 📋 Yêu cầu hệ thống

- Docker và Docker Compose
- .NET 8.0 SDK (nếu chạy local)
- Node.js 18+ (nếu chạy frontend local)

## 🏗️ Cấu trúc dự án

```
Transport/
├── src/
│   ├── Transport.Api/          # Web API
│   ├── Transport.Application/  # Business logic, DTOs, Services
│   ├── Transport.Domain/        # Domain models
│   └── Transport.Infrastructure/# Data access, DbContext
├── client/                      # React frontend
├── docker-compose.yml           # Docker Compose configuration
└── README.md
```

## 🚀 Cách khởi chạy

### Sử dụng Docker Compose (Khuyến nghị)

1. **Clone dự án:**
```bash
git clone <repo-url>
cd Transport
```

2. **Build và chạy:**
```bash
docker compose up --build
```

3. **Truy cập ứng dụng:**
- Frontend: http://localhost:3000
- API Swagger: http://localhost:5000/swagger

### Chạy local (Development)

#### Backend

1. **Cài đặt PostgreSQL** và tạo database `transportdb`

2. **Cập nhật connection string** trong `src/Transport.Api/appsettings.json`

3. **Chạy API:**
```bash
cd src/Transport.Api
dotnet run
```

#### Frontend

1. **Cài đặt dependencies:**
```bash
cd client
npm install
```

2. **Chạy development server:**
```bash
npm start
```

3. **Tạo file `.env` trong thư mục `client`:**
```
REACT_APP_API_URL=http://localhost:5000
```

## 🔐 Đăng nhập

**Tài khoản mặc định:**
- Email: `admin@example.com`
- Password: `Admin@123`
- Role: Admin

**Các role khác:**
- Dispatcher: `dispatcher@example.com` / `Dispatcher@123`
- Accountant: (có thể tạo thêm)
- Driver: (có thể tạo thêm)

## 📚 API Endpoints

### Authentication
- `POST /api/auth/login` - Đăng nhập
- `POST /api/auth/refresh` - Refresh token

### Customers
- `GET /api/customers` - Lấy danh sách khách hàng
- `GET /api/customers/{id}` - Lấy chi tiết khách hàng
- `POST /api/customers` - Tạo khách hàng mới
- `PUT /api/customers/{id}` - Cập nhật khách hàng
- `DELETE /api/customers/{id}` - Xóa khách hàng

### Trucks
- `GET /api/trucks` - Lấy danh sách xe tải
- `GET /api/trucks/{id}` - Lấy chi tiết xe tải
- `POST /api/trucks` - Tạo xe tải mới
- `PUT /api/trucks/{id}` - Cập nhật xe tải
- `DELETE /api/trucks/{id}` - Xóa xe tải

### Drivers
- `GET /api/drivers` - Lấy danh sách tài xế
- `GET /api/drivers/{id}` - Lấy chi tiết tài xế
- `POST /api/drivers` - Tạo tài xế mới
- `PUT /api/drivers/{id}` - Cập nhật tài xế
- `DELETE /api/drivers/{id}` - Xóa tài xế

### Trips
- `GET /api/trips` - Lấy danh sách chuyến hàng
- `GET /api/trips/{id}` - Lấy chi tiết chuyến hàng
- `POST /api/trips` - Tạo chuyến hàng mới
- `PUT /api/trips/{id}` - Cập nhật chuyến hàng
- `PATCH /api/trips/{id}/status` - Cập nhật trạng thái chuyến hàng
- `DELETE /api/trips/{id}` - Xóa chuyến hàng

### Invoices
- `GET /api/invoices` - Lấy danh sách hóa đơn
- `GET /api/invoices/{id}` - Lấy chi tiết hóa đơn
- `GET /api/invoices/customer/{customerId}` - Lấy hóa đơn theo khách hàng
- `POST /api/invoices` - Tạo hóa đơn mới
- `PUT /api/invoices/{id}` - Cập nhật hóa đơn
- `DELETE /api/invoices/{id}` - Xóa hóa đơn

### Payments
- `GET /api/payments` - Lấy danh sách thanh toán
- `GET /api/payments/{id}` - Lấy chi tiết thanh toán
- `GET /api/payments/invoice/{invoiceId}` - Lấy thanh toán theo hóa đơn
- `POST /api/payments` - Tạo thanh toán mới
- `DELETE /api/payments/{id}` - Xóa thanh toán

### Reports
- `GET /api/reports/revenue?fromDate=&toDate=` - Báo cáo doanh thu
- `GET /api/reports/debt` - Báo cáo công nợ
- `GET /api/reports/trip-status` - Báo cáo trạng thái chuyến hàng

## 🗄️ Database Schema

### Các bảng chính:
- `users` - Người dùng hệ thống
- `customers` - Khách hàng
- `trucks` - Xe tải
- `drivers` - Tài xế
- `trips` - Chuyến hàng
- `invoices` - Hóa đơn
- `payments` - Thanh toán

## 🔒 Phân quyền

- **Admin:** Toàn quyền truy cập
- **Accountant:** Quản lý hóa đơn, thanh toán, báo cáo
- **Dispatcher:** Quản lý xe tải, tài xế, khách hàng, chuyến hàng
- **Driver:** Xem và cập nhật trạng thái chuyến hàng

## 📝 Seed Data

Khi khởi động lần đầu, hệ thống sẽ tự động tạo dữ liệu mẫu:
- 1 admin user
- 1 dispatcher user
- 2 khách hàng
- 3 xe tải
- 3 tài xế
- 3 chuyến hàng

## 🐳 Docker

### Build và chạy:
```bash
docker compose up --build
```

### Xem logs:
```bash
docker compose logs -f api
docker compose logs -f client
```

### Dừng và xóa:
```bash
docker compose down
docker compose down -v  # Xóa cả volumes
```

## 🛠️ Development

### Backend
- Sử dụng Entity Framework Core với PostgreSQL
- JWT Authentication với refresh token
- AutoMapper cho mapping DTOs
- Serilog cho logging

### Frontend
- React Router cho routing
- Axios cho API calls
- Ant Design cho UI components
- TypeScript cho type safety

## 📄 License

MIT License

## 👥 Contributors

- [Your Name]

## 📞 Support

Nếu có vấn đề, vui lòng tạo issue trên GitHub repository.

