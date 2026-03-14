using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class SuCoBaoTriRepository : ISuCoBaoTriRepository
    {
        private readonly QLKTXDbContext _context;

        public SuCoBaoTriRepository(QLKTXDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<SuCoBaoTri>> GetAllAsync()
        {
            return await _context.SuCoBaoTris
               .Include(p => p.Phong)
               .Include(p => p.ThietBi)
               .ToListAsync();
        }

        public async Task<SuCoBaoTri> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return await _context.SuCoBaoTris
                .Include(p => p.Phong)
                .Include(p => p.ThietBi)
                .FirstOrDefaultAsync(p => p.MaSuCo == id);
        }

        public async Task AddAsync(SuCoBaoTri entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Đảm bảo mã sự cố không bị null
            if (string.IsNullOrEmpty(entity.MaSuCo))
            {
                entity.MaSuCo = await GenerateNextMaSuCoAsync();
            }

            await _context.SuCoBaoTris.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SuCoBaoTri entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.SuCoBaoTris.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            var entity = await _context.SuCoBaoTris.FindAsync(id);
            if (entity != null)
            {
                _context.SuCoBaoTris.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateNextMaSuCoAsync()
        {
            try
            {
                // Lấy mã sự cố cuối cùng
                var lastSuCo = await _context.SuCoBaoTris
                    .Where(s => s.MaSuCo.StartsWith("SC"))
                    .OrderByDescending(s => s.MaSuCo)
                    .FirstOrDefaultAsync();

                if (lastSuCo == null)
                {
                    return "SC001";
                }

                // Tách số từ mã cuối cùng
                string lastCode = lastSuCo.MaSuCo;
                if (lastCode.Length >= 3)
                {
                    string numberPart = lastCode.Substring(2); // Bỏ "SC"

                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        int nextNumber = lastNumber + 1;
                        return $"SC{nextNumber:D3}"; // Định dạng 3 chữ số
                    }
                }

                // Fallback
                return "SC001";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi sinh mã sự cố: {ex.Message}");
                return "SC001";
            }
        }
    }
}