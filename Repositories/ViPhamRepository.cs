using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class ViPhamRepository : IViPhamRepository
    {
        private readonly QLKTXDbContext _context;

        public ViPhamRepository(QLKTXDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ViPham>> GetAllAsync()
        {
            return await _context.ViPhams
                .Include(vp => vp.SinhVien)
                .Include(vp => vp.NoiQuy)
                .OrderByDescending(vp => vp.NgayViPham)
                .ToListAsync();
        }

        public async Task<ViPham> GetByIdAsync(string id)
        {
            return await _context.ViPhams
                .Include(vp => vp.SinhVien)
                .Include(vp => vp.NoiQuy)
                .FirstOrDefaultAsync(vp => vp.MaViPham == id);
        }

        public async Task AddAsync(ViPham entity)
        {
            // TỰ SINH MÃ VI PHẠM
            if (string.IsNullOrEmpty(entity.MaViPham))
            {
                entity.MaViPham = await GenerateNewIdAsync();
            }

            // TRÁNH LỖI NULL - gán giá trị mặc định
            entity.MaSV ??= "Unknown";
            entity.MaNoiQuy ??= "Unknown";

            // Đảm bảo NgayViPham có giá trị
            if (entity.NgayViPham == default)
            {
                entity.NgayViPham = DateTime.Now;
            }

            await _context.ViPhams.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ViPham entity)
        {
            // TRÁNH LỖI NULL
            entity.MaSV ??= "Unknown";
            entity.MaNoiQuy ??= "Unknown";

            _context.ViPhams.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.ViPhams.FindAsync(id);
            if (entity != null)
            {
                _context.ViPhams.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        // PHƯƠNG THỨC TỰ SINH MÃ VI PHẠM
        public async Task<string> GenerateNewIdAsync()
        {
            var lastVP = await _context.ViPhams
                .OrderByDescending(vp => vp.MaViPham)
                .FirstOrDefaultAsync();

            if (lastVP == null)
            {
                return "VP001";
            }

            // Lấy số từ mã cuối cùng (ví dụ: VP001 -> 001)
            var lastNumberString = new string(lastVP.MaViPham.Where(char.IsDigit).ToArray());
            if (int.TryParse(lastNumberString, out int lastNumber))
            {
                return $"VP{(lastNumber + 1):D3}";
            }

            return "VP001";
        }
    }
}