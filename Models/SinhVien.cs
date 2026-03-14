using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnCoSo.Models
{
	public class SinhVien
	{
		[Key]
		public string MaSV { get; set; }
		public string HoTen { get; set; }
		public DateTime NgaySinh { get; set; }
		public string GioiTinh { get; set; }
		public string SDT { get; set; }
		public string Email { get; set; }
		public string CCCD { get; set; }
		public string Lop { get; set; }
		public string Khoa { get; set; }
		public string DiaChiThuongTru { get; set; }
		public DateTime NgayNhapKTX { get; set; }
	

		public ICollection<HopDong>? HopDongs { get; set; }
		public ICollection<HoaDon>? HoaDons { get; set; }
		public ICollection<DangKyDichVu>? DangKyDichVus { get; set; }
		public ICollection<PhanHoi>? PhanHois { get; set; }
		public ICollection<ViPham>? ViPhams { get; set; }
	}
}
