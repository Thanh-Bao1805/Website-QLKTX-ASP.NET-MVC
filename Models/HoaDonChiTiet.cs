using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnCoSo.Models
{
    public class HoaDonChiTiet
    {
        [Key]
        public int MaChiTietHoaDon { get; set; }

        [ForeignKey("HoaDon")]
        public string MaHoaDon { get; set; } // Foreign key to HoaDon
        public HoaDon HoaDon { get; set; } // Navigation property to HoaDon

        public string TenThe { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal GiamGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
} 