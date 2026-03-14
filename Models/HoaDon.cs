using DoAnCoSo.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections.Generic;

namespace DoAnCoSo.Models;
public class HoaDon
{
	[Key]
	public string MaHoaDon { get; set; }
	public DateTime NgayLap { get; set; }
	public string LoaiChiPhi { get; set; }
	[DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
	public decimal TongTien { get; set; }
	public string? TrangThai { get; set; }

	[ForeignKey("SinhVien")]
	public string? MaSV { get; set; }
	public SinhVien? SinhVien { get; set; }

	[ForeignKey("NhanVien")]
	public string? MaNV { get; set; }
	public NhanVien? NhanVien { get; set; }

	[ForeignKey("DangKyDichVu")]
	public string? MaDKDV { get; set; }
	public DangKyDichVu? DangKyDichVu { get; set; }

	[ForeignKey("DienNuoc")]
	public string? MaDN { get; set; }
	public DienNuoc? DienNuoc { get; set; }

	public ICollection<HoaDonChiTiet>? HoaDonChiTiets { get; set; }

}
