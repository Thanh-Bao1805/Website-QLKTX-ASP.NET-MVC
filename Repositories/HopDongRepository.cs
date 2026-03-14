// DoAnCoSo/Repositories/HopDongRepository.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class HopDongRepository : IHopDongRepository
    {
        private readonly QLKTXDbContext _context;
        private readonly ILogger<HopDongRepository> _logger;

        public HopDongRepository(QLKTXDbContext context, ILogger<HopDongRepository> logger = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }

        public async Task<IEnumerable<HopDong>> GetAllAsync()
        {
            try
            {
                return await _context.HopDongs
                    .Include(h => h.SinhVien)
                    .Include(h => h.Phong)
                    .Include(h => h.NhanVien)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi khi lấy tất cả hợp đồng");
                return new List<HopDong>();
            }
        }

        public async Task<HopDong> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return null;

                return await _context.HopDongs
                    .Include(h => h.SinhVien)
                    .Include(h => h.Phong)
                    .Include(h => h.NhanVien)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(h => h.MaHopDong == id);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi khi lấy hợp đồng theo ID: {Id}", id);
                return null;
            }
        }

        public async Task AddAsync(HopDong entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                // Đảm bảo mã không bị null
                if (string.IsNullOrEmpty(entity.MaHopDong))
                {
                    entity.MaHopDong = await GenerateNextMaHopDongAsync();
                }

                await _context.HopDongs.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi khi thêm hợp đồng");
                throw;
            }
        }

        public async Task UpdateAsync(HopDong entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                // Detach entity cũ nếu có
                var existingEntity = await _context.HopDongs
                    .FirstOrDefaultAsync(h => h.MaHopDong == entity.MaHopDong);

                if (existingEntity != null)
                {
                    _context.Entry(existingEntity).State = EntityState.Detached;
                }

                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi khi cập nhật hợp đồng: {MaHopDong}", entity?.MaHopDong);
                throw;
            }
        }

        public async Task DeleteAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return;

                var entity = await _context.HopDongs.FindAsync(id);
                if (entity != null)
                {
                    _context.HopDongs.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi khi xóa hợp đồng: {Id}", id);
                throw;
            }
        }

        public async Task<string> GenerateNextMaHopDongAsync()
        {
            try
            {
                // Lấy mã hợp đồng cuối cùng
                var lastHopDong = await _context.HopDongs
                    .Where(h => h.MaHopDong.StartsWith("HD"))
                    .OrderByDescending(h => h.MaHopDong)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (lastHopDong == null)
                {
                    return "HD001";
                }

                // Tách số từ mã cuối cùng
                string lastCode = lastHopDong.MaHopDong;
                if (lastCode.Length >= 3)
                {
                    string numberPart = lastCode.Substring(2); // Bỏ "HD"

                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        int nextNumber = lastNumber + 1;
                        return $"HD{nextNumber:D3}"; // Định dạng 3 chữ số
                    }
                }

                // Fallback
                return "HD001";
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi khi sinh mã hợp đồng");
                return "HD001";
            }
        }

        public async Task<IEnumerable<HopDong>> GetBySinhVienIdAsync(string maSV)
        {
            try
            {
                if (string.IsNullOrEmpty(maSV))
                    return new List<HopDong>();

                return await _context.HopDongs
                    .Include(h => h.SinhVien)
                    .Include(h => h.Phong)
                    .Include(h => h.NhanVien)
                    .Where(h => h.MaSV == maSV)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi khi lấy hợp đồng theo sinh viên: {MaSV}", maSV);
                return new List<HopDong>();
            }
        }
    }
}