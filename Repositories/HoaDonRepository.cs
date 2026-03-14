using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class HoaDonRepository : IHoaDonRepository
    {
        private readonly QLKTXDbContext _context;

        public HoaDonRepository(QLKTXDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HoaDon>> GetAllAsync()
        {
            return await _context.HoaDons
                .Include(h => h.SinhVien)
                .Include(h => h.NhanVien)
                .ToListAsync();
        }

        public async Task<HoaDon> GetByIdAsync(string id)
        {
            return await _context.HoaDons
                .Include(h => h.SinhVien)
                .Include(h => h.NhanVien)
                .Include(h => h.HoaDonChiTiets)
                .Include(h => h.DienNuoc)
                    .ThenInclude(dn => dn.Phong)
                .Include(h => h.DangKyDichVu)
                    .ThenInclude(dkdv => dkdv.DichVu)
                .FirstOrDefaultAsync(h => h.MaHoaDon == id);
        }

        public async Task<HoaDon> GetByMaDNAsync(string maDN)
        {
            return await _context.HoaDons.FirstOrDefaultAsync(h => h.MaDN == maDN);
        }

        public async Task<HoaDon> GetByMaDKDVAsync(string maDKDV)
        {
            return await _context.HoaDons.FirstOrDefaultAsync(h => h.MaDKDV == maDKDV);
        }

        public async Task<string> GetNextMaHoaDonAsync()
        {
            return await GenerateNextMaHoaDonAsync();
        }

        public async Task AddAsync(HoaDon entity)
        {
            // Tự động sinh MaHoaDon khi lập hóa đơn (tăng dần và không trùng)
            // Nếu mã đã được cung cấp và hợp lệ, kiểm tra xem có trùng không
            if (string.IsNullOrEmpty(entity.MaHoaDon) || 
                !entity.MaHoaDon.StartsWith("HD") || 
                await GetByIdAsync(entity.MaHoaDon) != null)
            {
                // Nếu mã không hợp lệ hoặc đã tồn tại, tạo mã mới
                entity.MaHoaDon = await GenerateNextMaHoaDonAsync();
            }
            
            await _context.HoaDons.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        private async Task<string> GenerateNextMaHoaDonAsync()
        {
            // Lấy tất cả các MaHoaDon hiện có
            var existingMaHoaDons = await _context.HoaDons
                .Select(h => h.MaHoaDon)
                .ToListAsync();

            int nextNumber = 1;

            if (existingMaHoaDons.Any())
            {
                // Tìm số lớn nhất trong các MaHoaDon hiện có
                var numbers = existingMaHoaDons
                    .Where(m => m != null && m.StartsWith("HD") && m.Length > 2)
                    .Select(m =>
                    {
                        var numberPart = m.Substring(2); // Bỏ qua "HD"
                        if (int.TryParse(numberPart, out int num))
                            return num;
                        return 0;
                    })
                    .Where(n => n > 0)
                    .ToList();

                if (numbers.Any())
                {
                    nextNumber = numbers.Max() + 1;
                }
            }

            // Format: HD001, HD002, ...
            return $"HD{nextNumber:D3}";
        }

        public async Task UpdateAsync(HoaDon entity)
        {
            _context.HoaDons.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.HoaDons.FindAsync(id);
            if (entity != null)
            {
                _context.HoaDons.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
