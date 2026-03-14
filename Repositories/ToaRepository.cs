using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class ToaRepository : IToaRepository
    {
        private readonly QLKTXDbContext _context;

        public ToaRepository(QLKTXDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Toa>> GetAllAsync()
        {
            return await _context.Toas.ToListAsync();
        }

        public async Task<Toa> GetByIdAsync(string id)
        {
            return await _context.Toas.FindAsync(id);
        }

        public async Task AddAsync(Toa entity)
        {
            await _context.Toas.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Toa entity)
        {
            _context.Toas.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.Toas.FindAsync(id);
            if (entity != null)
            {
                _context.Toas.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
