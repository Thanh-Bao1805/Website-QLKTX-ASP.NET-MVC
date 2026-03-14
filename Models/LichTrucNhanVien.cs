using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
    public class LichTrucNhanVien
    {
        [Key]
        [Required(ErrorMessage = "Mã lịch trực là bắt buộc")]
        [StringLength(10)]
        [Display(Name = "Mã lịch trực")]
        public string MaLT { get; set; }

        [Required(ErrorMessage = "Ngày trực là bắt buộc")]
        [Display(Name = "Ngày trực")]
        public DateTime NgayTruc { get; set; }

        [Required(ErrorMessage = "Ca trực là bắt buộc")]
        [StringLength(50)]
        [Display(Name = "Ca trực")]
        public string CaTruc { get; set; }

        [Required(ErrorMessage = "Nhân viên là bắt buộc")]
        [ForeignKey("NhanVien")]
        [Display(Name = "Mã nhân viên")]
        public string? MaNV { get; set; }
        public NhanVien? NhanVien { get; set; }
    }
}