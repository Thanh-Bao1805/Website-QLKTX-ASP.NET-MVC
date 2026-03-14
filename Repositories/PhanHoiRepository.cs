using DoAnCoSo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCoSo.Repositories
{
    public class PhanHoiRepository : IPhanHoiRepository
    {
        private readonly QLKTXDbContext _context;

        public PhanHoiRepository(QLKTXDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PhanHoi>> GetAllAsync()
        {
            return await _context.PhanHois.ToListAsync();
        }

        public async Task<PhanHoi> GetByIdAsync(string id)
        {
            return await _context.PhanHois.FindAsync(id);
        }

        public async Task AddAsync(PhanHoi entity)
        {
            await _context.PhanHois.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PhanHoi entity)
        {
            _context.PhanHois.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.PhanHois.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<PhanHoi>> GetMessagesBetweenAsync(string user1, string user2)
        {
            return await _context.PhanHois
                .Where(p => (p.NguoiGui == user1 && p.NguoiNhan == user2) ||
                           (p.NguoiGui == user2 && p.NguoiNhan == user1))
                .OrderBy(p => p.NgayGui)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetUsersChattedWithAdminAsync()
        {
            return await _context.PhanHois
                .Where(p => p.NguoiGui != "Admin" && p.NguoiNhan == "Admin")
                .Select(p => p.NguoiGui)
                .Distinct()
                .ToListAsync();
        }
    }
}