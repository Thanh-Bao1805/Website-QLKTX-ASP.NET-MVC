using System.ComponentModel.DataAnnotations;
namespace DoAnCoSo.Models
{
    public class PhanHoi
    {
        [Key]
        public string MaPhanHoi { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string NoiDung { get; set; }

        public DateTime NgayGui { get; set; } = DateTime.Now;

        [Required]
        public string NguoiGui { get; set; }

        [Required]
        public string NguoiNhan { get; set; }

        public bool DaDoc { get; set; } = false;

        // 🔹 THÊM: Loại tin nhắn (text/image)
        public string LoaiTinNhan { get; set; } = "text";

        // 🔹 THÊM: Đường dẫn ảnh (nếu có)
        public string? DuongDanAnh { get; set; }
    }
}