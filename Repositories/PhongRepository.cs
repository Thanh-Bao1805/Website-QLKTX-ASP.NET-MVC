using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class PhongRepository : IPhongRepository
    {
        private readonly QLKTXDbContext _context;

        public PhongRepository(QLKTXDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Phong>> GetAllAsync()
        {
            return await _context.Phongs
                .Include(p => p.Toa)
                .ToListAsync();
        }

        public async Task<Phong> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return await _context.Phongs
                .Include(p => p.Toa)
                .FirstOrDefaultAsync(p => p.MaPhong == id);
        }

        public async Task AddAsync(Phong entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _context.Phongs.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Phong entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingEntity = await _context.Phongs.FindAsync(entity.MaPhong);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            var entity = await _context.Phongs.FindAsync(id);
            if (entity != null)
            {
                _context.Phongs.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateNextRoomCodeAsync(string maToa)
        {
            if (string.IsNullOrEmpty(maToa))
                return "PXXX001"; // Fallback code

            try
            {
                // Kiểm tra xem có phòng nào trong tòa nhà này không
                var roomsInBuilding = await _context.Phongs
                    .Where(p => p.MaToaID == maToa && p.MaPhong.StartsWith($"P{maToa}"))
                    .ToListAsync();

                if (roomsInBuilding == null || !roomsInBuilding.Any())
                {
                    return $"P{maToa}001";
                }

                // Lấy mã phòng cuối cùng
                var lastRoom = roomsInBuilding
                    .OrderByDescending(p => p.MaPhong)
                    .FirstOrDefault();

                if (lastRoom == null)
                {
                    return $"P{maToa}001";
                }

                // Tách số từ mã phòng
                string lastCode = lastRoom.MaPhong;
                if (lastCode.Length > (1 + maToa.Length))
                {
                    string numberPart = lastCode.Substring(1 + maToa.Length);

                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        int nextNumber = lastNumber + 1;
                        return $"P{maToa}{nextNumber:D3}";
                    }
                }

                // Fallback nếu có lỗi
                return $"P{maToa}001";
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về mã mặc định
                System.Diagnostics.Debug.WriteLine($"Lỗi khi sinh mã phòng: {ex.Message}");
                return $"P{maToa}001";
            }
        }
    }
}