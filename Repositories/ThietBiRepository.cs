using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class ThietBiRepository : IThietBiRepository
    {
        private readonly QLKTXDbContext _context;

        public ThietBiRepository(QLKTXDbContext context)
        {
            _context = context;
        }
        //Them phần tạo id /////////////////////////////////////////////////
        public async Task<string> GenerateNewIdAsync()
        {
            // Lấy mã lớn nhất hiện có
            var lastItem = await _context.ThietBis
                .OrderByDescending(t => t.MaThietBi)
                .FirstOrDefaultAsync();

            if (lastItem == null)
                return "TB001";

            // Tách phần số ra
            string numberPart = lastItem.MaThietBi.Substring(2);
            int number = int.Parse(numberPart);

            // Tăng 1
            number++;

            // Trả về dạng TB + số có 3 chữ số
            return "TB" + number.ToString("D3");
        }
        //
        public async Task<IEnumerable<ThietBi>> GetAllAsync()
        {
            return await _context.ThietBis.ToListAsync();
        }

        public async Task<ThietBi> GetByIdAsync(string id)
        {
            return await _context.ThietBis.FindAsync(id);
        }

        public async Task AddAsync(ThietBi entity)
        {
            await _context.ThietBis.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ThietBi entity)
        {
            _context.ThietBis.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.ThietBis.FindAsync(id);
            if (entity != null)
            {
                _context.ThietBis.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
