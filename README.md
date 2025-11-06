# Website Quản Lý Doanh Nghiệp Vận Tải - Vật Tư Xây Dựng

Hệ thống quản lý doanh nghiệp vận tải và vật tư xây dựng với đầy đủ tính năng quản lý xe tải, tài xế, khách hàng, chuyến hàng, hóa đơn, thanh toán và báo cáo.

## 🚀 Tính năng chính

- **Quản lý Xe Tải**: Quản lý thông tin xe, lịch bảo trì, trạng thái
- **Quản lý Tài Xế**: Quản lý thông tin tài xế, bằng lái, hiệu suất
- **Quản lý Khách Hàng**: Quản lý thông tin khách hàng, công nợ
- **Quản lý Chuyến Hàng**: Quản lý chuyến hàng, tuyến đường, trạng thái
- **Quản lý Hóa Đơn**: Quản lý hóa đơn, thanh toán, công nợ
- **Dashboard Tổng Quan**: Thống kê tổng quan với charts và visualizations
- **Báo Cáo Chi Tiết**: Báo cáo doanh thu, chuyến hàng, xe tải, tài xế, khách hàng với bộ lọc

## 🛠️ Công nghệ sử dụng

### Backend
- **.NET Core 8.0** - Web API
- **Entity Framework Core** - ORM
- **PostgreSQL** - Database
- **AutoMapper** - Object mapping
- **JWT Authentication** - Xác thực và phân quyền
- **Serilog** - Logging
- **Swagger** - API Documentation

### Frontend
- **React 19** - UI Framework
- **TypeScript** - Type safety
- **Ant Design** - UI Components
- **Recharts** - Data visualization
- **Axios** - HTTP client
- **React Router DOM** - Routing

### Deployment
- **Docker** - Containerization
- **Docker Compose** - Orchestration

## 📋 Yêu cầu hệ thống

- Docker Desktop
- Git

## 🚀 Cài đặt và chạy

### 1. Clone repository

```bash
git clone <repository-url>
cd Transport
```

### 2. Chạy với Docker Compose

```bash
docker compose up --build -d
```

### 3. Truy cập ứng dụng

- **Frontend**: http://localhost:3000
- **API Swagger**: http://localhost:5000/swagger
- **Database**: localhost:5432

### 4. Đăng nhập

- **Email**: `admin@example.com`
- **Password**: `Admin@123`

## 📊 Cấu trúc dự án

```
Transport/
├── src/
│   ├── Transport.Api/          # API Layer
│   ├── Transport.Application/   # Application Layer
│   ├── Transport.Domain/        # Domain Layer
│   └── Transport.Infrastructure/# Infrastructure Layer
├── client/                      # React Frontend
├── docker-compose.yml          # Docker Compose config
└── README.md                   # Documentation
```

## 🔐 Authentication & Authorization

- **JWT Token** - Xác thực người dùng
- **Refresh Token** - Làm mới token
- **Role-based Authorization** - Phân quyền theo vai trò:
  - Admin: Toàn quyền
  - Dispatcher: Quản lý chuyến hàng
  - Accountant: Quản lý hóa đơn và thanh toán

## 📈 Báo cáo

### Dashboard
- Tổng quan: Customers, Trucks, Drivers, Active Trips
- Doanh thu: Today, This Week, This Month, This Year với growth rate
- Chuyến hàng: Status breakdown với pie chart
- Công nợ: Total debt, overdue debt với progress bar
- Recent trips và top customers

### Báo cáo chi tiết
- **Doanh thu**: Theo period, customer, trip với trend analysis
- **Chuyến hàng**: Theo status, period, truck, driver, customer
- **Công nợ**: Chi tiết theo khách hàng với overdue tracking
- **Xe tải**: Utilization, maintenance schedule, performance
- **Tài xế**: Performance, license expiry tracking
- **Khách hàng**: Details, revenue, debt analysis

## 🗄️ Database Schema

- **Users**: Người dùng hệ thống
- **Customers**: Khách hàng
- **Trucks**: Xe tải
- **Drivers**: Tài xế
- **Trips**: Chuyến hàng
- **Invoices**: Hóa đơn
- **Payments**: Thanh toán

## 🔧 Các lệnh hữu ích

### Docker Compose

```bash
# Xem logs
docker compose logs -f api
docker compose logs -f client
docker compose logs -f db

# Dừng containers
docker compose down

# Khởi động lại
docker compose up -d

# Rebuild
docker compose up --build -d

# Xem trạng thái
docker compose ps
```

### Database

```bash
# Kết nối database
docker compose exec db psql -U transport -d transportdb

# Backup database
docker compose exec db pg_dump -U transport transportdb > backup.sql

# Restore database
docker compose exec -T db psql -U transport transportdb < backup.sql
```

## 📝 Seed Data

Hệ thống tự động tạo seed data khi khởi động lần đầu:
- 3 users (Admin, Dispatcher, Accountant)
- 100 customers
- 100 trucks
- 100 drivers
- 100 trips
- 100 invoices
- 100 payments

## 🐛 Troubleshooting

### Lỗi kết nối database
```bash
# Kiểm tra database health
docker compose ps db

# Restart database
docker compose restart db
```

### Lỗi build frontend
```bash
# Xóa node_modules và rebuild
cd client
rm -rf node_modules
npm install
npm run build
```

### Lỗi build backend
```bash
# Clean và rebuild
cd src/Transport.Api
dotnet clean
dotnet build
```

## 📄 License

MIT License

## 👥 Contributors

- Initial development

## 📞 Liên hệ

Nếu có vấn đề hoặc câu hỏi, vui lòng tạo issue trên GitHub.
