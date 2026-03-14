using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
    public class SuCoBaoTri
    {
        [Key]
        [Required(ErrorMessage = "Mã sự cố là bắt buộc")]
        [StringLength(10)]
        public string MaSuCo { get; set; }

        [Required(ErrorMessage = "Ngày phát hiện là bắt buộc")]
        [Display(Name = "Ngày phát hiện")]
        public DateTime NgayPhatHien { get; set; }

        [Required(ErrorMessage = "Tình trạng là bắt buộc")]
        [StringLength(50)]
        [Display(Name = "Tình trạng")]
        public string TinhTrang { get; set; }

        [Required(ErrorMessage = "Phòng là bắt buộc")]
        [ForeignKey("Phong")]
        [Display(Name = "Mã phòng")]
        public string? MaPhong { get; set; }
        public Phong? Phong { get; set; }

        [ForeignKey("ThietBi")]
        [Display(Name = "Mã thiết bị")]
        public string? MaThietBi { get; set; }
        public ThietBi? ThietBi { get; set; }
    }
}