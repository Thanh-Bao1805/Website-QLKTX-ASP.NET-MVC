using Microsoft.AspNetCore.SignalR;
using DoAnCoSo.Repositories;
using DoAnCoSo.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DoAnCoSo.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IPhanHoiRepository _phanHoiRepository;

        public ChatHub(IPhanHoiRepository phanHoiRepository)
        {
            _phanHoiRepository = phanHoiRepository;
        }

        public async Task SendMessage(string nguoiGui, string nguoiNhan, string noiDung, string loaiTinNhan = "text", string? base64Image = null)
        {
            try
            {
                Console.WriteLine($"📨 Nhận tin nhắn từ {nguoiGui} đến {nguoiNhan} - Loại: {loaiTinNhan}");
                Console.WriteLine($"📊 Độ dài base64Image: {base64Image?.Length ?? 0}");
                Console.WriteLine($"📊 Độ dài nội dung: {noiDung?.Length ?? 0}");

                string? duongDanAnh = null;

                // 🔹 XỬ LÝ ẢNH - THÊM TIMEOUT
                if (loaiTinNhan == "image" && !string.IsNullOrEmpty(base64Image))
                {
                    Console.WriteLine("🖼️ Bắt đầu xử lý ảnh...");

                    duongDanAnh = await LuuAnhBase64(base64Image);

                    if (string.IsNullOrEmpty(duongDanAnh))
                    {
                        Console.WriteLine("❌ Không thể lưu ảnh, gửi tin nhắn text thay thế");
                        await Clients.Caller.SendAsync("ReceiveError", "Không thể gửi ảnh. Ảnh có thể quá lớn hoặc định dạng không hỗ trợ.");
                        return;
                    }
                    else
                    {
                        Console.WriteLine($"✅ Ảnh đã lưu: {duongDanAnh}");
                    }
                }

                var phanHoi = new PhanHoi
                {
                    NoiDung = noiDung,
                    NguoiGui = nguoiGui,
                    NguoiNhan = nguoiNhan,
                    LoaiTinNhan = loaiTinNhan,
                    DuongDanAnh = duongDanAnh
                };

                await _phanHoiRepository.AddAsync(phanHoi);
                Console.WriteLine("💾 Đã lưu vào database");

                // Gửi real-time
                await Clients.All.SendAsync("ReceiveMessage",
                    phanHoi.NguoiGui,
                    phanHoi.NoiDung,
                    phanHoi.NgayGui.ToString("HH:mm dd/MM/yyyy"),
                    phanHoi.LoaiTinNhan,
                    phanHoi.DuongDanAnh);

                Console.WriteLine("✅ Đã gửi tin nhắn real-time");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ LỖI GỬI TIN NHẮN: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");

                // Gửi lỗi chi tiết hơn
                var errorMsg = ex.Message.Contains("too large") ? "Ảnh quá lớn" : $"Lỗi: {ex.Message}";
                await Clients.Caller.SendAsync("ReceiveError", errorMsg);
            }
        }

        private async Task<string?> LuuAnhBase64(string base64Image)
        {
            try
            {
                Console.WriteLine($"🖼️ Bắt đầu xử lý ảnh - Độ dài base64: {base64Image?.Length}");

                // Kiểm tra base64 string
                if (string.IsNullOrEmpty(base64Image) || base64Image.Length < 100)
                {
                    Console.WriteLine("❌ Base64 image không hợp lệ");
                    return null;
                }

                // Tách phần base64 - FIX CHO ẢNH LỚN
                string base64Data;
                string header = "";

                if (base64Image.Contains(','))
                {
                    var parts = base64Image.Split(',');
                    header = parts[0];
                    base64Data = parts[1];
                    Console.WriteLine($"📊 Header: {header}");
                }
                else
                {
                    base64Data = base64Image;
                    Console.WriteLine("📊 Không có header, sử dụng toàn bộ base64");
                }

                Console.WriteLine($"📊 Độ dài base64 data: {base64Data.Length}");

                // 🔥 FIX: Xử lý padding cho base64 string lớn
                if (base64Data.Length % 4 != 0)
                {
                    int padding = 4 - (base64Data.Length % 4);
                    base64Data = base64Data.PadRight(base64Data.Length + padding, '=');
                    Console.WriteLine($"📊 Đã thêm padding: {padding} ký tự");
                }

                // Kiểm tra kích thước base64 trước khi convert
                if (base64Data.Length > 15 * 1024 * 1024) // ~10MB ảnh thực tế
                {
                    Console.WriteLine("❌ Base64 quá lớn (>15MB)");
                    return null;
                }

                // Convert base64 to bytes với buffer lớn hơn
                byte[] bytes;
                try
                {
                    bytes = Convert.FromBase64String(base64Data);
                    Console.WriteLine($"✅ Convert base64 thành công: {bytes.Length} bytes");
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"❌ Lỗi convert base64: {ex.Message}");

                    // 🔥 THỬ FIX: Loại bỏ các ký tự không hợp lệ
                    base64Data = base64Data.Trim();
                    base64Data = base64Data.Replace(" ", "+");

                    if (base64Data.Length % 4 != 0)
                    {
                        base64Data = base64Data.PadRight(base64Data.Length + (4 - base64Data.Length % 4) % 4, '=');
                    }

                    try
                    {
                        bytes = Convert.FromBase64String(base64Data);
                        Console.WriteLine($"✅ Convert base64 thành công sau khi fix: {bytes.Length} bytes");
                    }
                    catch (FormatException ex2)
                    {
                        Console.WriteLine($"❌ Lỗi convert base64 lần 2: {ex2.Message}");
                        return null;
                    }
                }

                // Tăng giới hạn kích thước ảnh lên 10MB
                if (bytes.Length > 10 * 1024 * 1024)
                {
                    Console.WriteLine($"❌ Ảnh quá lớn: {bytes.Length} bytes (max: 10MB)");
                    return null;
                }

                // Kiểm tra magic number
                if (!IsValidImage(bytes))
                {
                    Console.WriteLine("❌ File không phải là ảnh hợp lệ");
                    return null;
                }

                // Nhận diện định dạng từ magic number thay vì header (chính xác hơn)
                string fileExtension = GetImageExtensionFromBytes(bytes);
                Console.WriteLine($"📊 Định dạng ảnh thực tế: {fileExtension}");

                // Tạo tên file
                var fileName = $"chat_{Guid.NewGuid()}{fileExtension}";
                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var filePath = Path.Combine(webRootPath, "images", "chat", fileName);

                Console.WriteLine($"📁 Đường dẫn file: {filePath}");

                // Đảm bảo thư mục tồn tại
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Console.WriteLine($"📁 Tạo thư mục: {directory}");
                    Directory.CreateDirectory(directory);
                }

                // Lưu file với buffer lớn
                await File.WriteAllBytesAsync(filePath, bytes);
                Console.WriteLine($"💾 Đã lưu file thành công: {fileName} ({bytes.Length} bytes)");

                return $"/images/chat/{fileName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ LỖI LƯU ẢNH: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        // 🔥 HÀM NHẬN DIỆN ĐỊNH DẠNG TỪ MAGIC NUMBER (CHÍNH XÁC HƠN)
        private string GetImageExtensionFromBytes(byte[] bytes)
        {
            if (bytes.Length < 8)
            {
                Console.WriteLine("⚠️ File quá nhỏ để nhận diện, mặc định .jpg");
                return ".jpg";
            }

            // PNG
            if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                return ".png";

            // JPEG
            if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                return ".jpg";

            // GIF
            if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
                return ".gif";

            // BMP
            if (bytes[0] == 0x42 && bytes[1] == 0x4D)
                return ".bmp";

            // WEBP
            if (bytes.Length >= 12 &&
                bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 &&
                bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50)
                return ".webp";

            Console.WriteLine("⚠️ Không nhận diện được định dạng từ bytes, mặc định dùng .jpg");
            return ".jpg";
        }

        // 🔥 HÀM NHẬN DIỆN ĐỊNH DẠNG ẢNH TỪ HEADER (DỰ PHÒNG)
        private string GetImageExtensionFromHeader(string header)
        {
            var lowerHeader = header.ToLower();

            if (lowerHeader.Contains("jpeg") || lowerHeader.Contains("jpg"))
                return ".jpg";
            else if (lowerHeader.Contains("png"))
                return ".png";
            else if (lowerHeader.Contains("gif"))
                return ".gif";
            else if (lowerHeader.Contains("webp"))
                return ".webp";
            else if (lowerHeader.Contains("bmp"))
                return ".bmp";
            else if (lowerHeader.Contains("svg"))
                return ".svg";
            else
            {
                Console.WriteLine($"⚠️ Không nhận diện được định dạng từ header, mặc định dùng .jpg. Header: {header}");
                return ".jpg";
            }
        }

        // 🔥 HÀM KIỂM TRA MAGIC NUMBER CỦA ẢNH (CẬP NHẬT)
        private bool IsValidImage(byte[] bytes)
        {
            if (bytes.Length < 8)
            {
                Console.WriteLine("⚠️ File quá nhỏ, không thể kiểm tra magic number");
                return false;
            }

            // PNG
            if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
            {
                Console.WriteLine("✅ Định dạng PNG hợp lệ");
                return true;
            }

            // JPEG
            if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            {
                Console.WriteLine("✅ Định dạng JPEG hợp lệ");
                return true;
            }

            // GIF
            if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
            {
                Console.WriteLine("✅ Định dạng GIF hợp lệ");
                return true;
            }

            // BMP
            if (bytes[0] == 0x42 && bytes[1] == 0x4D)
            {
                Console.WriteLine("✅ Định dạng BMP hợp lệ");
                return true;
            }

            // WEBP
            if (bytes.Length >= 12 &&
                bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 &&
                bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50)
            {
                Console.WriteLine("✅ Định dạng WEBP hợp lệ");
                return true;
            }

            Console.WriteLine("❌ Không nhận diện được magic number của ảnh - KHÔNG cho phép lưu");
            return false;
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"🔗 Client kết nối: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                Console.WriteLine($"🔌 Client ngắt kết nối: {Context.ConnectionId} - Lỗi: {exception.Message}");
            }
            else
            {
                Console.WriteLine($"🔌 Client ngắt kết nối: {Context.ConnectionId}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}