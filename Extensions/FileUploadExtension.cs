using Microsoft.AspNetCore.Http;

namespace DoAnCoSo.Extensions
{
    public static class FileUploadExtension
    {
        public static async Task<string> SaveImageAsync(this IFormFile file, string webRootPath)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var uploadsFolder = Path.Combine(webRootPath, "uploads", "dichvu");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Tạo tên file ngẫu nhiên để tránh trùng lặp
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Trả về đường dẫn tương đối để lưu vào database
            return Path.Combine("uploads", "dichvu", uniqueFileName);
        }
    }
} 