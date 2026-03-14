// File: Repositories/ThongKeRepository.cs
// Thêm phương thức mới để lấy dữ liệu chi phí theo tháng

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;

namespace DoAnCoSo.Repositories
{
    public class ThongKeRepository : IThongKeRepository
    {
        private readonly QLKTXDbContext _context;

        public ThongKeRepository(QLKTXDbContext context)
        {
            _context = context;
        }

        // Phương thức hiện tại
        public async Task<ThongKeViewModel> GetThongKeTongQuanAsync()
        {
            var viewModel = new ThongKeViewModel();

            // ... các phần khác giữ nguyên ...

            // 2. THÔNG SỐ ĐIỆN NƯỚC/TỔNG TIỀN
            var queryDienNuoc = _context.DienNuocs.AsQueryable();

            if (await queryDienNuoc.AnyAsync())
            {
                var tongSoDienNuoc = await queryDienNuoc.CountAsync();
                var tongTienDien = await queryDienNuoc.SumAsync(dn => dn.DonGiaDien * dn.SoDien);
                var tongTienNuoc = await queryDienNuoc.SumAsync(dn => dn.DonGiaNuoc * dn.SoNuoc);
                var tongTien = await queryDienNuoc.SumAsync(dn => dn.TongTien);

                viewModel.TienDienTrungBinh = tongTienDien / tongSoDienNuoc;
                viewModel.TienNuocTrungBinh = tongTienNuoc / tongSoDienNuoc;
                viewModel.TongTrungBinh = tongTien / tongSoDienNuoc;
                viewModel.CaoNhat = await queryDienNuoc.MaxAsync(dn => dn.TongTien);
            }
            else
            {
                viewModel.TienDienTrungBinh = 0;
                viewModel.TienNuocTrungBinh = 0;
                viewModel.TongTrungBinh = 0;
                viewModel.CaoNhat = 0;
            }

            // ... các phần khác ...

            return viewModel;
        }

        // THÊM PHƯƠNG THỨC MỚI: Lấy chi phí theo tháng
        public async Task<ChiPhiTheoThangViewModel> GetChiPhiTheoThangAsync(int? year = null)
        {
            var currentYear = year ?? DateTime.Now.Year;
            var startDate = new DateTime(currentYear, 1, 1);
            var endDate = new DateTime(currentYear, 12, 31);

            var chiPhiData = await _context.DienNuocs
                .Where(dn => dn.NgayGhi >= startDate && dn.NgayGhi <= endDate)
                .GroupBy(dn => new { dn.NgayGhi.Year, dn.NgayGhi.Month })
                .Select(g => new
                {
                    Thang = g.Key.Month,
                    TienDien = g.Sum(dn => dn.DonGiaDien * dn.SoDien),
                    TienNuoc = g.Sum(dn => dn.DonGiaNuoc * dn.SoNuoc),
                    TongTien = g.Sum(dn => dn.TongTien)
                })
                .OrderBy(x => x.Thang)
                .ToListAsync();

            // Khởi tạo mảng 12 tháng
            var viewModel = new ChiPhiTheoThangViewModel
            {
                Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" },
                TienDienData = new decimal[12],
                TienNuocData = new decimal[12],
                TongTienData = new decimal[12],
                Nam = currentYear
            };

            // Gán dữ liệu thực
            foreach (var item in chiPhiData)
            {
                if (item.Thang >= 1 && item.Thang <= 12)
                {
                    viewModel.TienDienData[item.Thang - 1] = item.TienDien;
                    viewModel.TienNuocData[item.Thang - 1] = item.TienNuoc;
                    viewModel.TongTienData[item.Thang - 1] = item.TongTien;
                }
            }

            return viewModel;
        }
    }

    // THÊM VIEWMODEL MỚI cho chi phí theo tháng
    public class ChiPhiTheoThangViewModel
    {
        public string[] Labels { get; set; }
        public decimal[] TienDienData { get; set; }
        public decimal[] TienNuocData { get; set; }
        public decimal[] TongTienData { get; set; }
        public int Nam { get; set; }
    }
}