using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class SuKienRepository : ISuKienRepository
    {
        private readonly QLKTXDbContext _context;

        public SuKienRepository(QLKTXDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SuKien>> GetAllAsync()
        {
            return await _context.SuKiens
                                 .Include(sk => sk.QuanTriVien)
                                 .OrderByDescending(sk => sk.NgayToChuc)
                                 .ToListAsync();
        }

        public async Task<SuKien> GetByIdAsync(string id)
        {
            return await _context.SuKiens
                                 .Include(sk => sk.QuanTriVien)
                                 .FirstOrDefaultAsync(sk => sk.MaSuKien == id);
        }

        public async Task AddAsync(SuKien entity)
        {
            // TỰ SINH MÃ SỰ KIỆN
            if (string.IsNullOrEmpty(entity.MaSuKien))
            {
                entity.MaSuKien = await GenerateNewIdAsync();
            }

            // TRÁNH LỖI NULL - gán giá trị mặc định
            entity.TenSuKien ??= "Sự kiện không có tên";
            entity.DiaDiem ??= "Chưa có địa điểm";
            entity.NoiDung ??= "Không có nội dung";

            // Đảm bảo NgayToChuc có giá trị
            if (entity.NgayToChuc == default)
            {
                entity.NgayToChuc = DateTime.Now;
            }

            await _context.SuKiens.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SuKien entity)
        {
            // TRÁNH LỖI NULL
            entity.TenSuKien ??= "Sự kiện không có tên";
            entity.DiaDiem ??= "Chưa có địa điểm";
            entity.NoiDung ??= "Không có nội dung";

            _context.SuKiens.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.SuKiens.FindAsync(id);
            if (entity != null)
            {
                _context.SuKiens.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        // PHƯƠNG THỨC TỰ SINH MÃ SỰ KIỆN
        public async Task<string> GenerateNewIdAsync()
        {
            var lastSK = await _context.SuKiens
                .OrderByDescending(sk => sk.MaSuKien)
                .FirstOrDefaultAsync();

            if (lastSK == null)
            {
                return "SK001";
            }

            // Lấy số từ mã cuối cùng (ví dụ: SK001 -> 001)
            var lastNumberString = new string(lastSK.MaSuKien.Where(char.IsDigit).ToArray());
            if (int.TryParse(lastNumberString, out int lastNumber))
            {
                return $"SK{(lastNumber + 1):D3}";
            }

            return "SK001";
        }
    }
}