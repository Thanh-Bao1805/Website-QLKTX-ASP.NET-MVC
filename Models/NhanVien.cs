using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
	public class NhanVien
	{
		[Key]
		public string MaNV { get; set; }
		public string HoTen { get; set; }
		public string SDT { get; set; }
		

		[ForeignKey("ChucVu")]
		public string? MaChucVu { get; set; }
		public ChucVu? chucVu { get; set; }

		public ICollection<LichTrucNhanVien>? LichTrucs { get; set; }
		public ICollection<HopDong>? HopDongs { get; set; }
		public ICollection<HoaDon>? HoaDons { get; set; }
	}

}
