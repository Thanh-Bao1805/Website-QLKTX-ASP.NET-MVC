// File: Controllers/ThongKeController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace DoAnCoSo.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly QLKTXDbContext _context;

        public ThongKeController(QLKTXDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = await GetThongKeDataAsync();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log lỗi ở đây
                Console.WriteLine($"Lỗi khi lấy dữ liệu thống kê: {ex.Message}");
                return View(new ThongKeViewModel());
            }
        }

        private async Task<ThongKeViewModel> GetThongKeDataAsync()
        {
            var viewModel = new ThongKeViewModel();

            // 1. THỐNG KÊ HÓA ĐƠN (từ bảng HoaDon)
            viewModel.TongSoHoaDon = await _context.HoaDons.CountAsync();
            viewModel.SoHoaDonDaThanhToan = await _context.HoaDons
                .Where(h => h.TrangThai == "Đã thanh toán")
                .CountAsync();
            viewModel.SoHoaDonChuaThanhToan = await _context.HoaDons
                .Where(h => h.TrangThai != "Đã thanh toán")
                .CountAsync();

            // 2. THÔNG SỐ ĐIỆN NƯỚC (từ bảng DienNuoc)
            var dienNuocStats = await GetDienNuocStatisticsAsync();
            viewModel.TienDienTrungBinh = dienNuocStats.TienDienTrungBinh;
            viewModel.TienNuocTrungBinh = dienNuocStats.TienNuocTrungBinh;
            viewModel.TongTrungBinh = dienNuocStats.TongTrungBinh;
            viewModel.CaoNhat = dienNuocStats.CaoNhat;

            // 3. THỐNG KÊ VI PHẠM (từ bảng ViPham và SinhVien)
            var viPhamStats = await GetViPhamStatisticsAsync();
            viewModel.TongSoSinhVien = viPhamStats.TongSoSinhVien;
            viewModel.SoSinhVienKhongViPham = viPhamStats.SoSinhVienKhongViPham;
            viewModel.SoSinhVienViPham1Lan = viPhamStats.SoSinhVienViPham1Lan;
            viewModel.SoSinhVienViPham2Lan = viPhamStats.SoSinhVienViPham2Lan;
            viewModel.SoSinhVienViPham3Lan = viPhamStats.SoSinhVienViPham3Lan;

            // 4. THỐNG KÊ KHOA (từ bảng SinhVien)
            viewModel.ThongKeSinhVienTheoKhoa = await _context.SinhViens
                .Where(sv => !string.IsNullOrEmpty(sv.Khoa))
                .GroupBy(sv => sv.Khoa)
                .Select(g => new { Khoa = g.Key, SoLuong = g.Count() })
                .ToDictionaryAsync(x => x.Khoa, x => x.SoLuong);

            // 5. THỐNG KÊ HỢP ĐỒNG (từ bảng HopDong)
            var hopDongStats = await GetHopDongStatisticsAsync();
            viewModel.TongSoHopDong = hopDongStats.TongSoHopDong;
            viewModel.SoSinhVienConHopDong = hopDongStats.SoSinhVienConHopDong;
            viewModel.SoSinhVienDaHetHopDong = hopDongStats.SoSinhVienDaHetHopDong;

            // 6. THỐNG KÊ PHÒNG (từ bảng Phong)
            var phongStats = await GetPhongStatisticsAsync();
            viewModel.TongSoToa = await _context.Toas.CountAsync();
            viewModel.TongSoPhong = phongStats.TongSoPhong;
            viewModel.SoPhongDaSuDung = phongStats.SoPhongDaSuDung;
            viewModel.SoPhongChuaSuDung = phongStats.SoPhongChuaSuDung;
            viewModel.SoPhongDaDuSinhVien = phongStats.SoPhongDaDuSinhVien;
            viewModel.SoPhongCoDoBong = phongStats.SoPhongCoDoBong;

            // 7. THÔNG SỐ KHÁC
            viewModel.TongSoVatChat = await _context.ThietBis.CountAsync();
            viewModel.TongSoDichVu = await _context.DichVus.CountAsync();
            viewModel.SoOmar = await _context.PhanHois.CountAsync();

            // 8. DỮ LIỆU BIỂU ĐỒ CHI PHÍ THEO THÁNG
            viewModel.ChiPhiTheoThang = await GetChiPhiTheoThangAsync(DateTime.Now.Year);

            return viewModel;
        }

        private async Task<DienNuocStatistics> GetDienNuocStatisticsAsync()
        {
            var dienNuocList = await _context.DienNuocs.ToListAsync();

            var dienNuocWithMoney = dienNuocList.Select(d => new
            {
                TienDien = d.SoDien * d.DonGiaDien,          // Số điện × Đơn giá điện
                TienNuoc = d.SoNuoc * d.DonGiaNuoc,          // Số nước × Đơn giá nước
                TongTien = d.TongTien                        // Tổng tiền từ DB
            }).ToList();

            return new DienNuocStatistics
            {
                TienDienTrungBinh = dienNuocWithMoney.Average(x => (decimal?)x.TienDien),
                TienNuocTrungBinh = dienNuocWithMoney.Average(x => (decimal?)x.TienNuoc),
                TongTrungBinh = dienNuocWithMoney.Average(x => (decimal?)x.TongTien),
                CaoNhat = dienNuocWithMoney.Max(x => (decimal?)x.TongTien)
            };
        }

        private async Task<ViPhamStatistics> GetViPhamStatisticsAsync()
        {
            var tongSoSinhVien = await _context.SinhViens.CountAsync();

            // Đếm số lần vi phạm cho từng sinh viên
            var viPhamCounts = await _context.SinhViens
                .Select(sv => new
                {
                    MaSV = sv.MaSV,
                    SoLanViPham = _context.ViPhams.Count(vp => vp.MaSV == sv.MaSV)
                })
                .ToListAsync();

            return new ViPhamStatistics
            {
                TongSoSinhVien = tongSoSinhVien,
                SoSinhVienKhongViPham = viPhamCounts.Count(x => x.SoLanViPham == 0),
                SoSinhVienViPham1Lan = viPhamCounts.Count(x => x.SoLanViPham == 1),
                SoSinhVienViPham2Lan = viPhamCounts.Count(x => x.SoLanViPham == 2),
                SoSinhVienViPham3Lan = viPhamCounts.Count(x => x.SoLanViPham >= 3)
            };
        }

        private async Task<HopDongStatistics> GetHopDongStatisticsAsync()
        {
            var now = DateTime.Now;

            var hopDongs = await _context.HopDongs
                .Include(hd => hd.SinhVien)
                .ToListAsync();

            var sinhVienConHopDong = hopDongs
                .Where(hd => hd.TrangThai == "Đang hoạt động" && hd.NgayKetThuc >= now)
                .Select(hd => hd.MaSV)
                .Distinct()
                .Count();

            var sinhVienDaHetHopDong = hopDongs
                .Where(hd => hd.TrangThai == "Đã hết hạn" || hd.NgayKetThuc < now)
                .Select(hd => hd.MaSV)
                .Distinct()
                .Count();

            return new HopDongStatistics
            {
                TongSoHopDong = hopDongs.Count,
                SoSinhVienConHopDong = sinhVienConHopDong,
                SoSinhVienDaHetHopDong = sinhVienDaHetHopDong
            };
        }

        private async Task<PhongStatistics> GetPhongStatisticsAsync()
        {
            var phongs = await _context.Phongs.ToListAsync();

            var soPhongDaSuDung = phongs.Count(p =>
                p.TrangThai == "Đã thuê" ||
                p.SoSinhVienHienTai > 0);

            var soPhongChuaSuDung = phongs.Count(p =>
                p.TrangThai == "Còn trống" ||
                p.SoSinhVienHienTai == 0);

            var soPhongDaDuSinhVien = phongs.Count(p =>
                p.SoSinhVienHienTai >= p.SoSinhVienToiDa);

            var soPhongCoDoBong = phongs.Count(p =>
                p.SoSinhVienHienTai < p.SoSinhVienToiDa);

            return new PhongStatistics
            {
                TongSoPhong = phongs.Count,
                SoPhongDaSuDung = soPhongDaSuDung,
                SoPhongChuaSuDung = soPhongChuaSuDung,
                SoPhongDaDuSinhVien = soPhongDaDuSinhVien,
                SoPhongCoDoBong = soPhongCoDoBong
            };
        }


        /// <summary>
        /// ///////////////////////////////////////////////////
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CheckDienNuocData()
        {
            try
            {
                // Kiểm tra các năm có dữ liệu
                var yearsWithData = await _context.DienNuocs
                    .Select(d => d.NgayGhi.Year)
                    .Distinct()
                    .OrderBy(y => y)
                    .ToListAsync();

                // Đếm số bản ghi theo năm
                var yearStats = await _context.DienNuocs
                    .GroupBy(d => d.NgayGhi.Year)
                    .Select(g => new
                    {
                        Nam = g.Key,
                        SoBanGhi = g.Count(),
                        TongTien = g.Sum(d => d.TongTien)
                    })
                    .OrderBy(x => x.Nam)
                    .ToListAsync();

                var result = $"<h3>KIỂM TRA DỮ LIỆU ĐIỆN NƯỚC</h3>";
                result += $"<p>Tổng số bản ghi: {await _context.DienNuocs.CountAsync()}</p>";
                result += $"<p>Các năm có dữ liệu: {string.Join(", ", yearsWithData)}</p>";
                result += "<table border='1'><tr><th>Năm</th><th>Số bản ghi</th><th>Tổng tiền</th></tr>";

                foreach (var stat in yearStats)
                {
                    result += $"<tr><td>{stat.Nam}</td><td>{stat.SoBanGhi}</td><td>{stat.TongTien:N0} VNĐ</td></tr>";
                }

                result += "</table>";

                // Kiểm tra dữ liệu mẫu
                var sampleData = await _context.DienNuocs
                    .OrderByDescending(d => d.NgayGhi)
                    .Take(5)
                    .Select(d => new
                    {
                        d.MaDN,
                        d.NgayGhi,
                        d.SoDien,
                        d.SoNuoc,
                        d.TongTien
                    })
                    .ToListAsync();

                result += "<h4>5 bản ghi mới nhất:</h4>";
                result += "<table border='1'><tr><th>Mã</th><th>Ngày</th><th>Điện</th><th>Nước</th><th>Tổng</th></tr>";

                foreach (var item in sampleData)
                {
                    result += $"<tr><td>{item.MaDN}</td><td>{item.NgayGhi:dd/MM/yyyy}</td><td>{item.SoDien}</td><td>{item.SoNuoc}</td><td>{item.TongTien:N0}</td></tr>";
                }

                result += "</table>";

                return Content(result, "text/html");
            }
            catch (Exception ex)
            {
                return Content($"Lỗi: {ex.Message}");
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetChiPhiByYear(int year)
        {
            try
            {
                Console.WriteLine($"====== ĐANG LẤY DỮ LIỆU NĂM {year} ======");

                // 1. LẤY DỮ LIỆU ĐIỆN NƯỚC CỦA NĂM NÀY
                var dienNuocNam = await _context.DienNuocs
                    .Where(d => d.NgayGhi.Year == year)
                    .ToListAsync();

                Console.WriteLine($"Tìm thấy {dienNuocNam.Count} bản ghi điện nước năm {year}");

                // 2. TÍNH TOÁN THỐNG KÊ CHO 4 Ô SỐ
                var statistics = new
                {
                    TienDienTrungBinh = 0m,
                    TienNuocTrungBinh = 0m,
                    TongTrungBinh = 0m,
                    CaoNhat = 0m,
                    SoLuongHoaDon = 0,
                    TongTienNam = 0m
                };

                if (dienNuocNam.Any())
                {
                    // Tính tiền điện, tiền nước cho từng bản ghi
                    var calculations = dienNuocNam.Select(d => new
                    {
                        TienDien = d.SoDien * d.DonGiaDien,
                        TienNuoc = d.SoNuoc * d.DonGiaNuoc,
                        TongTien = d.TongTien
                    }).ToList();

                    statistics = new
                    {
                        TienDienTrungBinh = Math.Round(calculations.Average(c => c.TienDien), 0),
                        TienNuocTrungBinh = Math.Round(calculations.Average(c => c.TienNuoc), 0),
                        TongTrungBinh = Math.Round(calculations.Average(c => c.TongTien), 0),
                        CaoNhat = calculations.Max(c => c.TongTien),
                        SoLuongHoaDon = dienNuocNam.Count,
                        TongTienNam = calculations.Sum(c => c.TongTien)
                    };

                    Console.WriteLine($"Thống kê năm {year}:");
                    Console.WriteLine($"- Tiền điện TB: {statistics.TienDienTrungBinh:N0}");
                    Console.WriteLine($"- Tiền nước TB: {statistics.TienNuocTrungBinh:N0}");
                    Console.WriteLine($"- Tổng TB: {statistics.TongTrungBinh:N0}");
                    Console.WriteLine($"- Cao nhất: {statistics.CaoNhat:N0}");
                }

                // 3. LẤY DỮ LIỆU CHO BIỂU ĐỒ THEO THÁNG
                var monthlyData = await _context.DienNuocs
                    .Where(d => d.NgayGhi.Year == year)
                    .GroupBy(d => d.NgayGhi.Month)
                    .Select(g => new
                    {
                        Thang = g.Key,
                        TienDien = g.Sum(d => d.SoDien * d.DonGiaDien),
                        TienNuoc = g.Sum(d => d.SoNuoc * d.DonGiaNuoc),
                        TongTien = g.Sum(d => d.TongTien)
                    })
                    .OrderBy(x => x.Thang)
                    .ToListAsync();

                Console.WriteLine($"Có dữ liệu cho {monthlyData.Count} tháng trong năm {year}");

                // 4. CHUẨN BỊ DỮ LIỆU BIỂU ĐỒ (12 tháng)
                var labels = new List<string>();
                var tienDienData = new List<decimal>();
                var tienNuocData = new List<decimal>();
                var tongTienData = new List<decimal>();

                for (int month = 1; month <= 12; month++)
                {
                    labels.Add($"Tháng {month}");

                    var data = monthlyData.FirstOrDefault(d => d.Thang == month);

                    tienDienData.Add(data?.TienDien ?? 0);
                    tienNuocData.Add(data?.TienNuoc ?? 0);
                    tongTienData.Add(data?.TongTien ?? 0);

                    if (data != null)
                    {
                        Console.WriteLine($"Tháng {month}: Điện={data.TienDien:N0}, Nước={data.TienNuoc:N0}, Tổng={data.TongTien:N0}");
                    }
                }

                return Json(new
                {
                    success = true,
                    year = year,

                    // Dữ liệu biểu đồ
                    labels = labels,
                    tienDienData = tienDienData,
                    tienNuocData = tienNuocData,
                    tongTienData = tongTienData,

                    // Thống kê 4 ô số
                    statistics = statistics,

                    // Thông tin bổ sung
                    message = $"Đã tải dữ liệu năm {year}",
                    timestamp = DateTime.Now.ToString("HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LỖI khi lấy dữ liệu năm {year}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    year = year
                });
            }
        }

        private async Task<ChiPhiTheoThangViewModel> GetChiPhiTheoThangAsync(int year)
        {
            var viewModel = new ChiPhiTheoThangViewModel
            {
                Nam = year
            };

            // Tạo danh sách tháng từ 1 đến 12
            var allMonths = Enumerable.Range(1, 12).ToList();
            viewModel.Labels = allMonths.Select(m => $"Tháng {m}").ToList();

            // Lấy dữ liệu điện nước theo tháng và năm
            var dienNuocData = await _context.DienNuocs
                .Where(d => d.NgayGhi.Year == year)
                .GroupBy(d => d.NgayGhi.Month)
                .Select(g => new
                {
                    Thang = g.Key,
                    TienDien = g.Sum(d => d.SoDien * d.DonGiaDien),
                    TienNuoc = g.Sum(d => d.SoNuoc * d.DonGiaNuoc),
                    TongTien = g.Sum(d => d.TongTien)
                })
                .ToListAsync();

            // Điền dữ liệu cho từng tháng
            foreach (var month in allMonths)
            {
                var data = dienNuocData.FirstOrDefault(d => d.Thang == month);

                viewModel.TienDienData.Add(data?.TienDien ?? 0);
                viewModel.TienNuocData.Add(data?.TienNuoc ?? 0);
                viewModel.TongTienData.Add(data?.TongTien ?? 0);
            }

            return viewModel;
        }

        // Helper classes
        private class DienNuocStatistics
        {
            public decimal? TienDienTrungBinh { get; set; }
            public decimal? TienNuocTrungBinh { get; set; }
            public decimal? TongTrungBinh { get; set; }
            public decimal? CaoNhat { get; set; }
        }

        private class ViPhamStatistics
        {
            public int TongSoSinhVien { get; set; }
            public int SoSinhVienKhongViPham { get; set; }
            public int SoSinhVienViPham1Lan { get; set; }
            public int SoSinhVienViPham2Lan { get; set; }
            public int SoSinhVienViPham3Lan { get; set; }
        }

        private class HopDongStatistics
        {
            public int TongSoHopDong { get; set; }
            public int SoSinhVienConHopDong { get; set; }
            public int SoSinhVienDaHetHopDong { get; set; }
        }

        private class PhongStatistics
        {
            public int TongSoPhong { get; set; }
            public int SoPhongDaSuDung { get; set; }
            public int SoPhongChuaSuDung { get; set; }
            public int SoPhongDaDuSinhVien { get; set; }
            public int SoPhongCoDoBong { get; set; }
        }
    }
}