using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class ChiTietThietBiRepository : IChiTietThietBiRepository
    {
        private readonly QLKTXDbContext _context;

        public ChiTietThietBiRepository(QLKTXDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChiTietThietBi>> GetAllAsync()
        {
            return await _context.ChiTietThietBis.ToListAsync();
        }

        public async Task<ChiTietThietBi> GetByIdAsync(string id)
        {
            return await _context.ChiTietThietBis.FindAsync(id);
        }

        public async Task AddAsync(ChiTietThietBi entity)
        {
            await _context.ChiTietThietBis.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChiTietThietBi entity)
        {
            _context.ChiTietThietBis.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.ChiTietThietBis.FindAsync(id);
            if (entity != null)
            {
                _context.ChiTietThietBis.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
