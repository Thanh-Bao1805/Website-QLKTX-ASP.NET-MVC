// File: Models/ViewModels/ThongKeViewModel.cs
using System.Collections.Generic;
using System.Linq;

namespace DoAnCoSo.Models.ViewModels
{
    public class ThongKeViewModel
    {
        // 1. THỐNG KÊ HÓA ĐƠN
        public int TongSoHoaDon { get; set; }
        public int SoHoaDonDaThanhToan { get; set; }
        public int SoHoaDonChuaThanhToan { get; set; }

        // 2. THÔNG SỐ ĐIỆN NƯỚC/TỔNG TIỀN
        public decimal? TienDienTrungBinh { get; set; }
        public decimal? TienNuocTrungBinh { get; set; }
        public decimal? TongTrungBinh { get; set; }
        public decimal? CaoNhat { get; set; }

        // 3. THỐNG KÊ VI PHẠM
        public int TongSoSinhVien { get; set; }
        public int SoSinhVienKhongViPham { get; set; }
        public int SoSinhVienViPham1Lan { get; set; }
        public int SoSinhVienViPham2Lan { get; set; }
        public int SoSinhVienViPham3Lan { get; set; }

        // 4. THỐNG KÊ KHOA
        public Dictionary<string, int>? ThongKeSinhVienTheoKhoa { get; set; }

        // 5. THỐNG KÊ HỢP ĐỒNG & PHÒNG
        public int SoSinhVienConHopDong { get; set; }
        public int SoSinhVienDaHetHopDong { get; set; }
        public int TongSoHopDong { get; set; }

        public int TongSoToa { get; set; }
        public int TongSoPhong { get; set; }
        public int SoPhongDaSuDung { get; set; }
        public int SoPhongChuaSuDung { get; set; }
        public int SoPhongDaDuSinhVien { get; set; }
        public int SoPhongCoDoBong { get; set; }

        // 6. THÔNG SỐ KHÁC
        public int TongSoVatChat { get; set; }
        public int TongSoDichVu { get; set; }
        public int SoOmar { get; set; }

        // 7. DỮ LIỆU BIỂU ĐỒ CHI PHÍ THEO THÁNG
        public ChiPhiTheoThangViewModel ChiPhiTheoThang { get; set; } = new ChiPhiTheoThangViewModel();
    }

    public class ChiPhiTheoThangViewModel
    {
        public int Nam { get; set; }
        public List<string> Labels { get; set; } = new List<string>();
        public List<decimal> TienDienData { get; set; } = new List<decimal>();
        public List<decimal> TienNuocData { get; set; } = new List<decimal>();
        public List<decimal> TongTienData { get; set; } = new List<decimal>();
    }
}