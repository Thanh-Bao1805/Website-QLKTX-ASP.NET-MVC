using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DoAnCoSo.Models
{
	public class ChucVu
	{
		[Key]
		public string MaChucVu { get; set; } // Khóa chính, có thể đặt tên khác nếu bạn muốn

		public string TenChucVu { get; set; } // Hiển thị tên chức vụ, nếu bạn cần

		// Các constant đại diện cho role
		public const string Role_NhanVien = "NhanVien";
		public const string Role_Admin = "Admin";
		public const string Role_SinhVien = "SinhVien";

		// Quan hệ 1-n với TaiKhoan
	
		public ApplicationUser ApplicationUser { get; set; } // ✔ Navigation chính xác
		public ICollection<NhanVien>? NhanViens { get; set; }
	
	}
}
