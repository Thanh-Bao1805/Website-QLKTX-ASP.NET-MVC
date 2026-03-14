using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class QuanTriVienRepository : IQuanTriVienRepository
    {
        private readonly QLKTXDbContext _context;

        public QuanTriVienRepository(QLKTXDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QuanTriVien>> GetAllAsync()
        {
            return await _context.QuanTriViens.ToListAsync();
        }

        public async Task<QuanTriVien> GetByIdAsync(string id)
        {
            return await _context.QuanTriViens.FindAsync(id);
        }

        public async Task AddAsync(QuanTriVien entity)
        {
            // Nếu chưa có MaQTV, tự động sinh
            if (string.IsNullOrEmpty(entity.MaQTV))
            {
                entity.MaQTV = await GenerateNextMaQTVAsync();
            }

            await _context.QuanTriViens.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(QuanTriVien entity)
        {
            _context.QuanTriViens.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.QuanTriViens.FindAsync(id);
            if (entity != null)
            {
                _context.QuanTriViens.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        // 🔹 THÊM PHƯƠNG THỨC SINH MÃ TỰ ĐỘNG
        public async Task<string> GenerateNextMaQTVAsync()
        {
            // Lấy mã cuối cùng trong database
            var lastQTV = await _context.QuanTriViens
                .OrderByDescending(q => q.MaQTV)
                .FirstOrDefaultAsync();

            if (lastQTV == null || string.IsNullOrEmpty(lastQTV.MaQTV))
            {
                return "QTV001"; // Mã đầu tiên
            }

            // Tách số từ mã hiện tại (ví dụ: QTV001 -> 001)
            var currentNumberStr = lastQTV.MaQTV.Substring(3); // Bỏ "QTV" đầu
            if (int.TryParse(currentNumberStr, out int currentNumber))
            {
                return $"QTV{(currentNumber + 1).ToString("D3")}"; // D3: 001, 002, ...
            }

            return "QTV001"; // Fallback
        }
    }
}