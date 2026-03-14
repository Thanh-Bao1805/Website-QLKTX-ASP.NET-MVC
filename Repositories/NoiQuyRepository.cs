using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class NoiQuyRepository : INoiQuyRepository
    {
        private readonly QLKTXDbContext _context;

        public NoiQuyRepository(QLKTXDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NoiQuy>> GetAllAsync()
        {
            return await _context.NoiQuys.ToListAsync();
        }

        public async Task<NoiQuy> GetByIdAsync(string id)
        {
            return await _context.NoiQuys.FindAsync(id);
        }

        public async Task AddAsync(NoiQuy entity)
        {
            await _context.NoiQuys.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(NoiQuy entity)
        {
            _context.NoiQuys.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.NoiQuys.FindAsync(id);
            if (entity != null)
            {
                _context.NoiQuys.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
