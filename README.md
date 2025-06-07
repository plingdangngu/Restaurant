# 🍽️ Hệ thống Quản lý Nhà hàng

![Trạng thái dự án](https://img.shields.io/badge/Trạng%20thái-Đang%20phát%20triển-yellow)
![Phiên bản](https://img.shields.io/badge/Phiên%20bản-1.0.0-blue)
![Giấy phép](https://img.shields.io/badge/Giấy%20phép-MIT-green)

> Hệ thống quản lý nhà hàng toàn diện với đầy đủ tính năng: quản lý đặt bàn, gọi món, thanh toán, quản lý nhân viên, và nhiều chức năng khác.

## 📋 Mục lục

- [Tổng quan](#tổng-quan)
- [Tính năng chính](#tính-năng-chính)
- [Công nghệ sử dụng](#công-nghệ-sử-dụng)
- [Cấu trúc dự án](#cấu-trúc-dự-án)
- [Vai trò người dùng](#vai-trò-người-dùng)
- [Cài đặt và thiết lập](#cài-đặt-và-thiết-lập)
- [Hướng dẫn sử dụng](#hướng-dẫn-sử-dụng)
- [Tài liệu API](#tài-liệu-api)
- [Đóng góp](#đóng-góp)
- [Giấy phép](#giấy-phép)
- [Liên hệ](#liên-hệ)

## 🌟 Tổng quan

Hệ thống Quản lý Nhà hàng là một ứng dụng web toàn diện được xây dựng trên nền tảng ASP.NET Core MVC, giúp nhà hàng vận hành hiệu quả và nâng cao trải nghiệm khách hàng. Hệ thống cho phép quản lý bàn, đơn hàng, thực đơn, nhân viên và khách hàng một cách dễ dàng và trực quan.

## ✨ Tính năng chính

### 🍴 Quản lý đơn hàng
- Tạo và quản lý đơn hàng
- Thêm/xóa món ăn trong đơn hàng
- Cập nhật trạng thái đơn hàng (Chờ xử lý, Đang xử lý, Hoàn thành, Hủy)
- Quản lý trạng thái từng món ăn (Chờ xử lý, Đang chế biến, Sẵn sàng, Đã phục vụ, Hủy)

### 🪑 Quản lý bàn
- Thêm/sửa/xóa bàn
- Theo dõi trạng thái bàn (Trống, Đã đặt, Đang sử dụng)
- Sắp xếp bàn theo tầng/khu vực

### 🍜 Quản lý thực đơn
- Thêm/sửa/xóa món ăn
- Phân loại món ăn theo danh mục
- Cập nhật tình trạng món ăn (Còn/Hết)
- Quản lý giá và ảnh món ăn

### 💰 Thanh toán
- Xử lý thanh toán đơn hàng
- Tạo hóa đơn tự động
- Xem lịch sử thanh toán

### 👥 Quản lý nhân viên và khách hàng
- Quản lý thông tin nhân viên
- Lưu trữ thông tin khách hàng
- Phân quyền dựa trên vai trò (Admin, Nhân viên, Khách hàng)

### 📊 Báo cáo và thống kê
- Báo cáo doanh thu
- Thống kê đơn hàng
- Phân tích hiệu suất nhà hàng

## 🛠️ Công nghệ sử dụng

- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap 5
- **Backend**: ASP.NET Core MVC (.NET 7)
- **Cơ sở dữ liệu**: SQL Server
- **ORM**: Entity Framework Core
- **Xác thực**: ASP.NET Core Identity
- **Thư viện & Framework**:
  - jQuery
  - Font Awesome
  - DataTables
  - Chart.js (cho biểu đồ thống kê)

## 📁 Cấu trúc dự án

```
RestaurantManagement/
├── Controllers/          # MVC Controllers
├── Models/               # Các model chính của ứng dụng
├── Views/                # Razor Views
│   ├── Home/             # Trang chủ
│   ├── Order/            # Quản lý đơn hàng
│   ├── Menu/             # Quản lý thực đơn
│   ├── Table/            # Quản lý bàn
│   ├── Employee/         # Quản lý nhân viên
│   └── Shared/           # Layout và partial views
├── Data/                 # Cơ sở dữ liệu và migrations
├── Services/             # Các dịch vụ nghiệp vụ
│   ├── Interfaces/       # Định nghĩa interface
│   └── Implementations/  # Thực hiện các service
├── Repositories/         # Repository pattern
│   ├── Interfaces/       # Định nghĩa interface
│   └── Implementations/  # Thực hiện các repository
├── ViewModels/           # DTO và view models
├── wwwroot/              # Tài nguyên tĩnh (CSS, JS, images)
├── Extensions/           # Các tiện ích mở rộng
├── Migrations/           # EF Core migrations
└── Program.cs           # Điểm khởi đầu ứng dụng
```

## 👥 Vai trò người dùng

### 🔑 Admin/Nhân viên
- Toàn quyền CRUD với đơn hàng
- Cập nhật trạng thái đơn hàng và món ăn
- Quản lý bàn, khách hàng, thực đơn
- Xử lý thanh toán và tạo hóa đơn
- Xem báo cáo và thống kê

### 👤 Người dùng thường
- Xem thông tin đơn hàng của mình
- Xem chi tiết món ăn và giá
- Xem trạng thái đơn hàng
- Không thể thêm/sửa/xóa món trong đơn hàng
- Không thể thay đổi trạng thái đơn hàng

## 💻 Cài đặt và thiết lập

### Yêu cầu hệ thống
- .NET 7.0 SDK trở lên
- SQL Server 2019 trở lên
- Visual Studio 2022 hoặc Visual Studio Code

### Các bước cài đặt

1. **Clone dự án**
   ```bash
   git clone https://github.com/your-username/RestaurantManagement.git
   cd RestaurantManagement
   ```

2. **Khôi phục các gói NuGet**
   ```bash
   dotnet restore
   ```

3. **Cài đặt cơ sở dữ liệu**
   - Cập nhật chuỗi kết nối trong `appsettings.json`
   - Chạy các migration:
   ```bash
   dotnet ef database update
   ```

4. **Chạy ứng dụng**
   ```bash
   dotnet run
   ```
   Ứng dụng sẽ khởi chạy tại `https://localhost:5001` và `http://localhost:5000`

5. **Tạo tài khoản admin**
   - Mặc định, hệ thống sẽ tạo tài khoản admin với:
     - Tên đăng nhập: `admin@restaurant.com`
     - Mật khẩu: `Admin123!`

## 📝 Hướng dẫn sử dụng

### Quản lý đơn hàng

1. **Tạo đơn hàng mới**
   - Đăng nhập với quyền Admin/Nhân viên
   - Điều hướng đến "Quản lý đơn hàng" > "Tạo đơn hàng"
   - Chọn bàn, khách hàng và điền thông tin cần thiết
   - Nhấn "Lưu" để tạo đơn hàng

2. **Thêm món vào đơn hàng**
   - Trong màn hình chi tiết đơn hàng, nhấn "Thêm món"
   - Chọn món ăn, số lượng và ghi chú đặc biệt (nếu có)
   - Nhấn "Thêm món" để xác nhận

3. **Cập nhật trạng thái đơn hàng**
   - Trong màn hình chi tiết đơn hàng, sử dụng menu dropdown "Cập nhật"
   - Chọn trạng thái mới (Chờ xử lý, Đang xử lý, Hoàn thành, Hủy)

4. **Thanh toán đơn hàng**
   - Trong màn hình chi tiết đơn hàng, nhấn "Thanh toán"
   - Xác nhận thông tin thanh toán
   - Nhấn "Xác nhận" để hoàn tất

### Quản lý bàn

1. **Thêm bàn mới**
   - Điều hướng đến "Quản lý bàn" > "Thêm bàn"
   - Điền thông tin bàn (tên, tầng, sức chứa)
   - Nhấn "Lưu" để tạo bàn mới

2. **Chỉnh sửa thông tin bàn**
   - Trong danh sách bàn, nhấn "Chỉnh sửa" ở bàn cần thay đổi
   - Cập nhật thông tin
   - Nhấn "Lưu" để xác nhận thay đổi

### Quản lý thực đơn

1. **Thêm món ăn mới**
   - Điều hướng đến "Quản lý thực đơn" > "Thêm món ăn"
   - Điền thông tin món ăn (tên, giá, danh mục, mô tả)
   - Tải lên ảnh món ăn (nếu có)
   - Nhấn "Lưu" để tạo món ăn mới

2. **Cập nhật tình trạng món ăn**
   - Trong danh sách thực đơn, sử dụng công tắc "Còn/Hết" để cập nhật

## 📡 Tài liệu API

Hệ thống cung cấp một số API để tích hợp với các hệ thống khác:

- `GET /api/orders` - Lấy danh sách đơn hàng
- `GET /api/orders/{id}` - Lấy chi tiết đơn hàng
- `POST /api/orders` - Tạo đơn hàng mới
- `PUT /api/orders/{id}` - Cập nhật đơn hàng
- `DELETE /api/orders/{id}` - Xóa đơn hàng

*Chi tiết đầy đủ có thể được tìm thấy trong tài liệu API riêng biệt.*

## 🤝 Đóng góp

Chúng tôi rất hoan nghênh mọi đóng góp để cải thiện hệ thống:

1. Fork dự án
2. Tạo branch mới (`git checkout -b feature/amazing-feature`)
3. Commit thay đổi (`git commit -m 'Add some amazing feature'`)
4. Push lên branch (`git push origin feature/amazing-feature`)
5. Mở Pull Request

## 📄 Giấy phép

Dự án này được phân phối dưới Giấy phép MIT. Xem file `LICENSE` để biết thêm chi tiết.

## 📞 Liên hệ

Tên Của Bạn - [@twitter_handle](https://twitter.com/twitter_handle) - email@example.com

Link dự án: [https://github.com/your-username/RestaurantManagement](https://github.com/your-username/RestaurantManagement)

---

<p align="center">
  <img src="https://your-image-url.com/restaurant-logo.png" alt="Restaurant Management Logo" width="100">
  <br>
  <em>Xây dựng với ❤️ cho ngành ẩm thực</em>
</p> 