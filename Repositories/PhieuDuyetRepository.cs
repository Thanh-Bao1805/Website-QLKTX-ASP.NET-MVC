using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class PhieuDuyetRepository : IPhieuDuyetRepository
    {
        private readonly QLKTXDbContext _context;

        public PhieuDuyetRepository(QLKTXDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<PhieuDuyet>> GetAllAsync()
        {
            return await _context.PhieuDuyets
                .Include(p => p.DangKyO)
                .Include(p => p.QuanTriVien)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PhieuDuyet> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return await _context.PhieuDuyets
                .Include(p => p.DangKyO)
                .Include(p => p.QuanTriVien)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.MaPhieu == id);
        }

        public async Task AddAsync(PhieuDuyet entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Đảm bảo mã không bị null
            if (string.IsNullOrEmpty(entity.MaPhieu))
            {
                entity.MaPhieu = await GenerateNextMaPhieuAsync();
            }

            await _context.PhieuDuyets.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PhieuDuyet entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Detach entity cũ nếu có
            var local = _context.Set<PhieuDuyet>()
                .Local
                .FirstOrDefault(entry => entry.MaPhieu.Equals(entity.MaPhieu));

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            var entity = await _context.PhieuDuyets.FindAsync(id);
            if (entity != null)
            {
                _context.PhieuDuyets.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateNextMaPhieuAsync()
        {
            try
            {
                // Lấy mã phiếu cuối cùng
                var lastPhieu = await _context.PhieuDuyets
                    .Where(p => p.MaPhieu.StartsWith("PD"))
                    .OrderByDescending(p => p.MaPhieu)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (lastPhieu == null)
                {
                    return "PD001";
                }

                // Tách số từ mã cuối cùng
                string lastCode = lastPhieu.MaPhieu;
                if (lastCode.Length >= 3)
                {
                    string numberPart = lastCode.Substring(2); // Bỏ "PD"

                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        int nextNumber = lastNumber + 1;
                        return $"PD{nextNumber:D3}"; // Định dạng 3 chữ số
                    }
                }

                // Fallback
                return "PD001";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi sinh mã phiếu: {ex.Message}");
                return "PD001";
            }
        }
    }
}