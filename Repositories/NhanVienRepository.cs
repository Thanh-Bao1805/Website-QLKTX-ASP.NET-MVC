using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class NhanVienRepository : INhanVienRepository
    {
        private readonly QLKTXDbContext _context;

        public NhanVienRepository(QLKTXDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NhanVien>> GetAllAsync()
        {
            return await _context.NhanViens
                .Include(nv => nv.chucVu)
                .ToListAsync();
        }

        public async Task<NhanVien> GetByIdAsync(string id)
        {
            return await _context.NhanViens
                .Include(nv => nv.chucVu)
                .FirstOrDefaultAsync(nv => nv.MaNV == id);
        }

        public async Task AddAsync(NhanVien entity)
        {
            // Nếu chưa có MaNV, tự động sinh
            if (string.IsNullOrEmpty(entity.MaNV))
            {
                entity.MaNV = await GenerateNextMaNVAsync();
            }

            await _context.NhanViens.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(NhanVien entity)
        {
            _context.NhanViens.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.NhanViens.FindAsync(id);
            if (entity != null)
            {
                _context.NhanViens.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        // 🔹 THÊM PHƯƠNG THỨC SINH MÃ TỰ ĐỘNG
        public async Task<string> GenerateNextMaNVAsync()
        {
            // Lấy mã cuối cùng trong database
            var lastNV = await _context.NhanViens
                .OrderByDescending(nv => nv.MaNV)
                .FirstOrDefaultAsync();

            if (lastNV == null || string.IsNullOrEmpty(lastNV.MaNV))
            {
                return "NV001"; // Mã đầu tiên
            }

            // Tách số từ mã hiện tại (ví dụ: NV001 -> 001)
            var currentNumberStr = lastNV.MaNV.Substring(2); // Bỏ "NV" đầu
            if (int.TryParse(currentNumberStr, out int currentNumber))
            {
                return $"NV{(currentNumber + 1).ToString("D3")}"; // D3: 001, 002, ...
            }

            return "NV001"; // Fallback
        }
    }
}