// Trong file DoAnCoSo/Models/DangKyO.cs
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DoAnCoSo.Models
{
    public class DangKyO
    {
        [Key]
        public string MaDK { get; set; }
        public string MSSV { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string CCCD { get; set; }
        public string Lop { get; set; }
        public string Khoa { get; set; }
        public string DiaChiThuongTru { get; set; }
        public DateTime NgayBatDauO { get; set; }
        public DateTime NgayKetThucO { get; set; }

        [ForeignKey("Phong")]
        public string? MaPhong { get; set; }
        public Phong? Phong { get; set; }

        // Thêm các trường mới để liên kết với User
        public string? UserId { get; set; } // Liên kết với Identity User

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}