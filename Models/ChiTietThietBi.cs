using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
	public class ChiTietThietBi
	{
		[Key, Column(Order = 0)]
		public string? MaPhong { get; set; }

		[Key, Column(Order = 1)]
		public string? MaThietBi { get; set; }

		public int SoLuong { get; set; }

		[ForeignKey("MaPhong")]
		public Phong? Phong { get; set; }

		[ForeignKey("MaThietBi")]
		public ThietBi? ThietBi { get; set; }
	}
}
