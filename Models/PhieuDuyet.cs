using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
    public class PhieuDuyet
    {
        [Key]
        [Required(ErrorMessage = "Mã phiếu là bắt buộc")]
        [StringLength(10)]
        [Display(Name = "Mã phiếu")]
        public string MaPhieu { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; }

        [Required(ErrorMessage = "Ngày duyệt là bắt buộc")]
        [Display(Name = "Ngày duyệt")]
        public DateTime NgayDuyet { get; set; }

        [Required(ErrorMessage = "Quản trị viên là bắt buộc")]
        [ForeignKey("QuanTriVien")]
        [Display(Name = "Mã QTV")]
        public string? MaQTV { get; set; }
        public QuanTriVien? QuanTriVien { get; set; }

        [Required(ErrorMessage = "Đăng ký ở là bắt buộc")]
        [ForeignKey("DangKyO")]
        [Display(Name = "Mã đăng ký")]
        public string? MaDK { get; set; }
        public DangKyO? DangKyO { get; set; }
    }
}