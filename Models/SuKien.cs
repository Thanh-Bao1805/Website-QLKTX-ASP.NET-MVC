using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
    public class SuKien
    {
        [Key]
        [Display(Name = "Mã sự kiện")]
        public string MaSuKien { get; set; }

        [Required(ErrorMessage = "Tên sự kiện không được để trống")]
        [Display(Name = "Tên sự kiện")]
        [StringLength(255, ErrorMessage = "Tên sự kiện không được vượt quá 255 ký tự")]
        public string TenSuKien { get; set; }

        [Required(ErrorMessage = "Ngày tổ chức không được để trống")]
        [Display(Name = "Ngày tổ chức")]
        [DataType(DataType.DateTime)]
        public DateTime NgayToChuc { get; set; }

        [Required(ErrorMessage = "Địa điểm không được để trống")]
        [Display(Name = "Địa điểm")]
        [StringLength(200, ErrorMessage = "Địa điểm không được vượt quá 200 ký tự")]
        public string DiaDiem { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; }

        [ForeignKey("QuanTriVien")]
        [Display(Name = "Mã quản trị viên")]
        public string? MaQTV { get; set; }

        [Display(Name = "Quản trị viên")]
        public QuanTriVien? QuanTriVien { get; set; }
    }
}