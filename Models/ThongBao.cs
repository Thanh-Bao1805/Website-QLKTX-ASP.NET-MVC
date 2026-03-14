using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
    public class ThongBao
    {
        [Key]
        [Display(Name = "Mã thông báo")]
        public string MaTB { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [Display(Name = "Tiêu đề")]
        [StringLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự")]
        public string TieuDe { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; }

        [Required(ErrorMessage = "Ngày gửi không được để trống")]
        [Display(Name = "Ngày gửi")]
        [DataType(DataType.DateTime)]
        public DateTime NgayGui { get; set; }

        [Required(ErrorMessage = "Đối tượng không được để trống")]
        [Display(Name = "Đối tượng")]
        [StringLength(100, ErrorMessage = "Đối tượng không được vượt quá 100 ký tự")]
        public string DoiTuong { get; set; }

        [ForeignKey("QuanTriVien")]
        [Display(Name = "Mã quản trị viên")]
        public string? MaQTV { get; set; }

        [Display(Name = "Quản trị viên")]
        public QuanTriVien? QuanTriVien { get; set; }
    }
}