using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnCoSo.Models
{
    public class QuanTriVien
    {
        [Key]
        public string MaQTV { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên không quá 100 ký tự")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]  // 🔹 CÓ THỂ GÂY LỖI
        [RegularExpression(@"^(0[0-9]{9,10})$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có 10-11 số")]  // 🔹 KIỂM TRA REGEX NÀY
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Số điện thoại phải có 10-11 số")]
        public string SDT { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không quá 100 ký tự")]
        public string Email { get; set; }

        public ICollection<ThongBao>? ThongBaos { get; set; }
        public ICollection<SuKien>? SuKiens { get; set; }
        public ICollection<PhieuDuyet>? PhieuDuyets { get; set; }
    }
}