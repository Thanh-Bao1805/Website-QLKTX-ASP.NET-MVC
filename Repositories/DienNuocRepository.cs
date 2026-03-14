using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class DienNuocRepository : IDienNuocRepository
    {
        private readonly QLKTXDbContext _context;

        public DienNuocRepository(QLKTXDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<DienNuoc>> GetAllAsync()
        {
            return await _context.DienNuocs
                .Include(d => d.Phong)
                .AsNoTracking() // THÊM DÒNG NÀY
                .ToListAsync();
        }

        public async Task<DienNuoc> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return await _context.DienNuocs
                .Include(d => d.Phong)
                .AsNoTracking() // THÊM DÒNG NÀY
                .FirstOrDefaultAsync(d => d.MaDN == id);
        }

        public async Task AddAsync(DienNuoc entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Đảm bảo mã không bị null
            if (string.IsNullOrEmpty(entity.MaDN))
            {
                entity.MaDN = await GenerateNextMaDNAsync();
            }

            await _context.DienNuocs.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DienNuoc entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                // CÁCH 1: Detach entity cũ nếu có
                var local = _context.Set<DienNuoc>()
                    .Local
                    .FirstOrDefault(entry => entry.MaDN.Equals(entity.MaDN));

                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

                // CÁCH 2: Attach và mark as modified
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // CÁCH 3: Tải entity từ database và cập nhật giá trị
                var existingEntity = await _context.DienNuocs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.MaDN == entity.MaDN);

                if (existingEntity != null)
                {
                    _context.DienNuocs.Update(entity);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Không tìm thấy dữ liệu điện nước để cập nhật");
                }
            }
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            // Tìm entity với tracking
            var entity = await _context.DienNuocs.FindAsync(id);
            if (entity != null)
            {
                _context.DienNuocs.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateNextMaDNAsync()
        {
            try
            {
                // Lấy mã điện nước cuối cùng
                var lastDienNuoc = await _context.DienNuocs
                    .Where(d => d.MaDN.StartsWith("DN"))
                    .OrderByDescending(d => d.MaDN)
                    .AsNoTracking() // THÊM DÒNG NÀY
                    .FirstOrDefaultAsync();

                if (lastDienNuoc == null)
                {
                    return "DN001";
                }

                // Tách số từ mã cuối cùng
                string lastCode = lastDienNuoc.MaDN;
                if (lastCode.Length >= 3)
                {
                    string numberPart = lastCode.Substring(2); // Bỏ "DN"

                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        int nextNumber = lastNumber + 1;
                        return $"DN{nextNumber:D3}"; // Định dạng 3 chữ số
                    }
                }

                // Fallback
                return "DN001";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi sinh mã điện nước: {ex.Message}");
                return "DN001";
            }
        }
    }
}