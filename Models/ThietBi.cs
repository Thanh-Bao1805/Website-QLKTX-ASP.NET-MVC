using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DoAnCoSo.Models
{
    public class ThietBi
    {
        [Key]
        [BindNever]    // IMPORTANT: ngăn MVC bind giá trị từ form
        public string MaThietBi { get; set; }

        [Required]
        [StringLength(200)]
        public string TenThietBi { get; set; }

        public string Loai { get; set; }

        public string MoTa { get; set; }

        public ICollection<ChiTietThietBi>? ChiTietThietBis { get; set; }
        public ICollection<SuCoBaoTri>? SuCoBaoTris { get; set; }
    }
}
