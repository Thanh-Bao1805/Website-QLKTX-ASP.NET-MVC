using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class SinhVienRepository : ISinhVienRepository
    {
        private readonly QLKTXDbContext _context;

        public SinhVienRepository(QLKTXDbContext context)
        {
            _context = context;
        }

		public async Task<IEnumerable<SinhVien>> GetAllAsync()
		{
			return await _context.SinhViens.ToListAsync();
		}

		public async Task<SinhVien> GetByIdAsync(string id)
        {
            return await _context.SinhViens.FindAsync(id);
        }

		public async Task AddAsync(SinhVien entity)
		{
			_context.SinhViens.Add(entity); // Thêm sinh viên vào DbSet
			await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
		}

		public async Task UpdateAsync(SinhVien entity)
        {
            _context.SinhViens.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.SinhViens.FindAsync(id);
            if (entity != null)
            {
                _context.SinhViens.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
