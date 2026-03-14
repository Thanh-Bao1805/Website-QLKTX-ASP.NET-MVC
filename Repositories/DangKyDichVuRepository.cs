using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;
using System.Linq;

namespace DoAnCoSo.Repositories
{
    public class DangKyDichVuRepository : IDangKyDichVuRepository
    {
        private readonly QLKTXDbContext _context;

        public DangKyDichVuRepository(QLKTXDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DangKyDichVu>> GetAllAsync()
        {
            return await _context.DangKyDichVus
                .Include(d => d.SinhVien)
                .Include(d => d.DichVu)
                .ToListAsync();
        }

        public async Task<DangKyDichVu> GetByIdAsync(string id)
        {
            return await _context.DangKyDichVus
                .Include(d => d.SinhVien)
                .Include(d => d.DichVu)
                .FirstOrDefaultAsync(d => d.MaDKDV == id);
        }

        public async Task AddAsync(DangKyDichVu entity)
        {
            await _context.DangKyDichVus.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DangKyDichVu entity)
        {
            _context.DangKyDichVus.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.DangKyDichVus.FindAsync(id);
            if (entity != null)
            {
                _context.DangKyDichVus.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateNextMaDKDVAsync()
        {
            var lastDangKy = await _context.DangKyDichVus
                .OrderByDescending(d => d.MaDKDV)
                .FirstOrDefaultAsync();

            if (lastDangKy == null)
            {
                return "001";
            }

            if (int.TryParse(lastDangKy.MaDKDV, out int lastNumber))
            {
                return (lastNumber + 1).ToString("D3");
            }

            return "001";
        }
    }
}
