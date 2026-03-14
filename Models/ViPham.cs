using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
    public class ViPham
    {
        [Key]
        [Display(Name = "Mã vi phạm")]
        public string MaViPham { get; set; }

        [Required(ErrorMessage = "Ngày vi phạm không được để trống")]
        [Display(Name = "Ngày vi phạm")]
        [DataType(DataType.DateTime)]
        public DateTime NgayViPham { get; set; }

        [Required(ErrorMessage = "Sinh viên không được để trống")]
        [ForeignKey("SinhVien")]
        [Display(Name = "Mã sinh viên")]
        public string? MaSV { get; set; }

        [Display(Name = "Sinh viên")]
        public SinhVien? SinhVien { get; set; }

        [Required(ErrorMessage = "Nội quy không được để trống")]
        [ForeignKey("NoiQuy")]
        [Display(Name = "Mã nội quy")]
        public string? MaNoiQuy { get; set; }

        [Display(Name = "Nội quy")]
        public NoiQuy? NoiQuy { get; set; }
    }
}