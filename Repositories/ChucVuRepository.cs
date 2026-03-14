using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class ChucVuRepository : IChucVuRepository
    {
        private readonly QLKTXDbContext _context;

        public ChucVuRepository(QLKTXDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChucVu>> GetAllAsync()
        {
            return await _context.ChucVus.ToListAsync();
        }

        public async Task<ChucVu> GetByIdAsync(string id)
        {
            return await _context.ChucVus.FindAsync(id);
        }

        public async Task AddAsync(ChucVu entity)
        {
            await _context.ChucVus.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChucVu entity)
        {
            _context.ChucVus.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.ChucVus.FindAsync(id);
            if (entity != null)
            {
                _context.ChucVus.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
