using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;
using System.Linq;

namespace DoAnCoSo.Repositories
{
    public class DangKyORepository : IDangKyORepository
    {
        private readonly QLKTXDbContext _context;

        public DangKyORepository(QLKTXDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DangKyO>> GetAllAsync()
        {
            return await _context.DangKyOs.ToListAsync();
        }

        public async Task<DangKyO> GetByIdAsync(string id)
        {
            return await _context.DangKyOs.FindAsync(id);
        }

        public async Task AddAsync(DangKyO entity)
        {
            await _context.DangKyOs.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DangKyO entity)
        {
            _context.DangKyOs.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.DangKyOs.FindAsync(id);
            if (entity != null)
            {
                _context.DangKyOs.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateNextMaDKAsync()
        {
            var lastDangKyO = await _context.DangKyOs
                .OrderByDescending(d => d.MaDK)
                .FirstOrDefaultAsync();

            if (lastDangKyO == null)
            {
                return "001";
            }

            if (int.TryParse(lastDangKyO.MaDK, out int lastNumber))
            {
                return (lastNumber + 1).ToString("D3");
            }

            return "001";
        }

        public async Task<bool> ExistsByMSSVAsync(string mssv, string? excludeMaDK = null)
        {
            if (string.IsNullOrWhiteSpace(mssv))
                return false;

            var query = _context.DangKyOs.Where(d => d.MSSV == mssv);
            
            if (!string.IsNullOrEmpty(excludeMaDK))
            {
                query = query.Where(d => d.MaDK != excludeMaDK);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsBySDTAsync(string sdt, string? excludeMaDK = null)
        {
            if (string.IsNullOrWhiteSpace(sdt))
                return false;

            var query = _context.DangKyOs.Where(d => d.SDT == sdt);
            
            if (!string.IsNullOrEmpty(excludeMaDK))
            {
                query = query.Where(d => d.MaDK != excludeMaDK);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email, string? excludeMaDK = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var query = _context.DangKyOs.Where(d => d.Email == email);
            
            if (!string.IsNullOrEmpty(excludeMaDK))
            {
                query = query.Where(d => d.MaDK != excludeMaDK);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByCCCDAsync(string cccd, string? excludeMaDK = null)
        {
            if (string.IsNullOrWhiteSpace(cccd))
                return false;

            var query = _context.DangKyOs.Where(d => d.CCCD == cccd);
            
            if (!string.IsNullOrEmpty(excludeMaDK))
            {
                query = query.Where(d => d.MaDK != excludeMaDK);
            }

            return await query.AnyAsync();
        }
    }
}
