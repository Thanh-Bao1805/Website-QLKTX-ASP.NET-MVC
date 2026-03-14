using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
	public class DichVu
	{
		[Key]
		public string MaDV { get; set; }
		public string TenDichVu { get; set; }
		public decimal DonGia { get; set; }
		public string MoTa { get; set; }
		public string? HinhAnh { get; set; }

		public ICollection<DangKyDichVu>? DangKyDichVus { get; set; }
	}
}
