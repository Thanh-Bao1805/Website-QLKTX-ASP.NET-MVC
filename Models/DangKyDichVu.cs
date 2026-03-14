using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
	public class DangKyDichVu
	{
		[Key]
		public string MaDKDV { get; set; }
		public DateTime NgayDangKy { get; set; }
		public int SoLuong { get; set; }
	
		[Column(TypeName = "decimal(18,2)")]
		public decimal TongTien { get; set; }

		[ForeignKey("SinhVien")]
		public string? MaSV { get; set; }
		public SinhVien? SinhVien { get; set; }

		[ForeignKey("DichVu")]
		public string? MaDV { get; set; }
		public DichVu? DichVu { get; set; }

		public HoaDon? HoaDon { get; set; }
	}
}