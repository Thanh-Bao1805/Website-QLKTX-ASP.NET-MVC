using DoAnCoSo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoAnCoSo.Repositories
{
    public class DichVuRepository : IDichVuRepository
    {
        private readonly QLKTXDbContext _context;

        public DichVuRepository(QLKTXDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DichVu>> GetAllAsync()
        {
            return await _context.DichVus.ToListAsync();
        }

        public async Task<DichVu> GetByIdAsync(string id)
        {
            return await _context.DichVus.FindAsync(id);
        }

        public async Task AddAsync(DichVu entity)
        {
            await _context.DichVus.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DichVu entity)
        {
            _context.DichVus.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.DichVus.FindAsync(id);
            if (entity != null)
            {
                _context.DichVus.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
