using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
	public class Toa
	{
		[Key]
		public string MaToa { get; set; }
		public string TenToa { get; set; }
		public int SoTang { get; set; }
		public string DiaChi { get; set; }
		public string LoaiSuDung { get; set; }

		public ICollection<Phong>? Phongs { get; set; }
	}
}