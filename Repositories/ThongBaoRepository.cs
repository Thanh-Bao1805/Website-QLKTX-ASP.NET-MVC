using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class ThongBaoRepository : IThongBaoRepository
    {
        private readonly QLKTXDbContext _context;

        public ThongBaoRepository(QLKTXDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ThongBao>> GetAllAsync()
        {
            return await _context.ThongBaos
                                 .Include(tb => tb.QuanTriVien)
                                 .OrderByDescending(tb => tb.NgayGui)
                                 .ToListAsync();
        }

        public async Task<ThongBao> GetByIdAsync(string id)
        {
            return await _context.ThongBaos
                                 .Include(tb => tb.QuanTriVien)
                                 .FirstOrDefaultAsync(tb => tb.MaTB == id);
        }

        public async Task AddAsync(ThongBao entity)
        {
            // TỰ SINH MÃ THÔNG BÁO
            if (string.IsNullOrEmpty(entity.MaTB))
            {
                entity.MaTB = await GenerateNewIdAsync();
            }

            // TRÁNH LỖI NULL - gán giá trị mặc định
            entity.TieuDe ??= "Không có tiêu đề";
            entity.NoiDung ??= "Không có nội dung";
            entity.DoiTuong ??= "Tất cả";

            // Đảm bảo NgayGui có giá trị
            if (entity.NgayGui == default)
            {
                entity.NgayGui = DateTime.Now;
            }

            // TRÁNH LỖI KHÔNG LƯU
            await _context.ThongBaos.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ThongBao entity)
        {
            // TRÁNH LỖI NULL
            entity.TieuDe ??= "Không có tiêu đề";
            entity.NoiDung ??= "Không có nội dung";
            entity.DoiTuong ??= "Tất cả";

            _context.ThongBaos.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.ThongBaos.FindAsync(id);
            if (entity != null)
            {
                _context.ThongBaos.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        // PHƯƠNG THỨC TỰ SINH MÃ THÔNG BÁO
        public async Task<string> GenerateNewIdAsync()
        {
            var lastTB = await _context.ThongBaos
                .OrderByDescending(tb => tb.MaTB)
                .FirstOrDefaultAsync();

            if (lastTB == null)
            {
                return "TB001";
            }

            // Lấy số từ mã cuối cùng (ví dụ: TB001 -> 001)
            var lastNumberString = new string(lastTB.MaTB.Where(char.IsDigit).ToArray());
            if (int.TryParse(lastNumberString, out int lastNumber))
            {
                return $"TB{(lastNumber + 1):D3}";
            }

            return "TB001";
        }
    }
}