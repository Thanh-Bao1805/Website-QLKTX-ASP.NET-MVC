using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
    public class HopDong
    {
        [Key]
        [Required(ErrorMessage = "Mã hợp đồng là bắt buộc")]
        [StringLength(10)]
        [Display(Name = "Mã hợp đồng")]
        public string MaHopDong { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        [Display(Name = "Ngày kết thúc")]
        public DateTime NgayKetThuc { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; }

        [Required(ErrorMessage = "Sinh viên là bắt buộc")]
        [ForeignKey("SinhVien")]
        [Display(Name = "Mã sinh viên")]
        public string? MaSV { get; set; }
        public SinhVien? SinhVien { get; set; }

        [Required(ErrorMessage = "Phòng là bắt buộc")]
        [ForeignKey("Phong")]
        [Display(Name = "Mã phòng")]
        public string? MaPhong { get; set; }
        public Phong? Phong { get; set; }

        [Required(ErrorMessage = "Nhân viên là bắt buộc")]
        [ForeignKey("NhanVien")]
        [Display(Name = "Mã nhân viên")]
        public string? MaNV { get; set; }
        public NhanVien? NhanVien { get; set; }
    }
}