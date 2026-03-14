using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
    public class Phong
    {
        [Key]
        [StringLength(10)]
        [Required(ErrorMessage = "Mã phòng là bắt buộc")]
        [Display(Name = "Mã phòng")]
        public string MaPhong { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số giường là bắt buộc")]
        [Range(1, 20, ErrorMessage = "Số giường phải từ 1 đến 20")]
        [Display(Name = "Số giường")]
        public int SoGiuong { get; set; }

        [Required(ErrorMessage = "Số sinh viên tối đa là bắt buộc")]
        [Range(1, 20, ErrorMessage = "Số sinh viên tối đa phải từ 1 đến 20")]
        [Display(Name = "Số SV tối đa")]
        public int SoSinhVienToiDa { get; set; }

        [Required(ErrorMessage = "Số sinh viên hiện tại là bắt buộc")]
        [Range(0, 20, ErrorMessage = "Số sinh viên hiện tại phải từ 0 đến 20")]
        [Display(Name = "Số SV hiện tại")]
        public int SoSinhVienHienTai { get; set; }

        [Required(ErrorMessage = "Loại phòng là bắt buộc")]
        [Display(Name = "Loại phòng")]
        public string LoaiPhong { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giới tính phòng là bắt buộc")]
        [Display(Name = "Giới tính phòng")]
        public string GioiTinhPhong { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá phòng là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phòng phải lớn hơn 0")]
        [Display(Name = "Giá phòng")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal GiaPhong { get; set; }

        [Display(Name = "Hình ảnh")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Tiền cọc là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Tiền cọc phải lớn hơn 0")]
        [Display(Name = "Tiền cọc")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TienCoc { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tòa nhà là bắt buộc")]
        [ForeignKey("Toa")]
        [Display(Name = "Tòa nhà")]
        public string? MaToaID { get; set; }
        public Toa? Toa { get; set; }

        public ICollection<HopDong>? HopDongs { get; set; }
        public ICollection<ChiTietThietBi>? ChiTietThietBis { get; set; }
        public ICollection<SuCoBaoTri>? SuCoBaoTris { get; set; }
        public ICollection<DangKyO>? DangKyOs { get; set; }
        public ICollection<DienNuoc>? DienNuocs { get; set; }

        // Thuộc tính tính toán
        [NotMapped]
        [Display(Name = "Số chỗ trống")]
        public int SoChoTrong => SoSinhVienToiDa - SoSinhVienHienTai;

        [NotMapped]
        [Display(Name = "Tỷ lệ lấp đầy")]
        public decimal TyLeLapDay => SoSinhVienToiDa > 0 ? (decimal)SoSinhVienHienTai / SoSinhVienToiDa * 100 : 0;
    }
}