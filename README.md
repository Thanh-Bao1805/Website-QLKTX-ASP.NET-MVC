HỆ THỐNG QUẢN LÝ KÝ TÚC XÁ SINH VIÊN
Giới thiệu
Hệ thống Quản lý Ký túc xá Sinh viên là một ứng dụng web được xây dựng nhằm hỗ trợ quản lý 
các hoạt động trong ký túc xá của trường đại học. Hệ thống giúp tự động hóa và đơn giản hóa 
quá trình quản lý sinh viên, phòng ở, hợp đồng và hóa đơn, giúp cán bộ quản lý ký túc xá 
làm việc hiệu quả hơn.

Ứng dụng được phát triển dưới dạng website trên nền tảng ASP.NET MVC sử dụng ngôn ngữ C# 
và cơ sở dữ liệu SQL Server.
Mục tiêu của hệ thống
- Hỗ trợ quản lý thông tin sinh viên đang ở ký túc xá
- Quản lý phòng ở và tình trạng phòng
- Quản lý hợp đồng thuê phòng
- Quản lý hóa đơn và các dịch vụ trong ký túc xá
- Tăng hiệu quả quản lý và giảm thao tác thủ công
Chức năng chính
1. Quản lý sinh viên
- Thêm, sửa, xóa thông tin sinh viên
- Xem danh sách sinh viên trong ký túc xá
- Quản lý sinh viên theo phòng

2. Quản lý phòng
- Quản lý danh sách phòng
- Theo dõi số lượng sinh viên trong phòng
- Quản lý trạng thái phòng

3. Quản lý hợp đồng
- Tạo hợp đồng ở ký túc xá
- Gia hạn hoặc kết thúc hợp đồng
- Quản lý thông tin hợp đồng của sinh viên

4. Quản lý hóa đơn
- Tạo hóa đơn cho sinh viên
- Quản lý chi phí ký túc xá
- Theo dõi trạng thái thanh toán

5. Quản lý dịch vụ
- Quản lý các dịch vụ trong ký túc xá
- Tính chi phí dịch vụ
Công nghệ sử dụng
- ASP.NET MVC: Framework phát triển ứng dụng web
- C#: Ngôn ngữ lập trình chính
- SQL Server: Hệ quản trị cơ sở dữ liệu
- HTML / CSS: Thiết kế giao diện
- JavaScript: Xử lý tương tác phía client
- Bootstrap: Thiết kế giao diện responsive
- jQuery: Thư viện hỗ trợ JavaScript
- Git: Quản lý phiên bản
- GitHub: Lưu trữ mã nguồn
Cấu trúc thư mục chính
Controllers   : Xử lý logic nghiệp vụ
Models        : Các lớp dữ liệu (Entity)
Views         : Giao diện người dùng (Razor Views)
wwwroot       : Tài nguyên tĩnh (CSS, JS, Images)
Data          : Cấu hình và kết nối cơ sở dữ liệu
appsettings.json : File cấu hình hệ thống
Hướng dẫn cài đặt và chạy project
Bước 1: Clone project từ GitHub
git clone https://github.com/Thanh-Bao1805/Website-QLKTX-ASP.NET-MVC.git

Bước 2: Mở project bằng Visual Studio

Bước 3: Cấu hình cơ sở dữ liệu
Cập nhật connection string trong file appsettings.json

Bước 4: Chạy chương trình
Nhấn Ctrl + F5 hoặc chọn Run trong Visual Studio
Cách sử dụng hệ thống
Sau khi chạy chương trình, quản trị viên có thể:
- Quản lý sinh viên trong ký túc xá
- Phân bổ sinh viên vào phòng
- Tạo và quản lý hợp đồng
- Tạo và quản lý hóa đơn
- Quản lý các dịch vụ trong ký túc xá
Định hướng phát triển trong tương lai
- Thêm chức năng thanh toán online
- Tích hợp hệ thống thông báo
- Phát triển phiên bản mobile
- Bổ sung báo cáo và thống kê
Tác giả
Nguyễn Thanh Bảo
GitHub: https://github.com/Thanh-Bao1805
Bản quyền
Dự án này được thực hiện với mục đích học tập và nghiên cứu.
