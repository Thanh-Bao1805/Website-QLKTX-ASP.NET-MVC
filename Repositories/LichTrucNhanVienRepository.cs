using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class LichTrucNhanVienRepository : ILichTrucNhanVienRepository
    {
        private readonly QLKTXDbContext _context;

        public LichTrucNhanVienRepository(QLKTXDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<LichTrucNhanVien>> GetAllAsync()
        {
            return await _context.LichTrucNhanViens
                .Include(lt => lt.NhanVien)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LichTrucNhanVien> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return await _context.LichTrucNhanViens
                .Include(lt => lt.NhanVien)
                .AsNoTracking()
                .FirstOrDefaultAsync(lt => lt.MaLT == id);
        }

        public async Task AddAsync(LichTrucNhanVien entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Đảm bảo mã không bị null
            if (string.IsNullOrEmpty(entity.MaLT))
            {
                entity.MaLT = await GenerateNextMaLTAsync();
            }

            await _context.LichTrucNhanViens.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LichTrucNhanVien entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Detach entity cũ nếu có
            var local = _context.Set<LichTrucNhanVien>()
                .Local
                .FirstOrDefault(entry => entry.MaLT.Equals(entity.MaLT));

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            var entity = await _context.LichTrucNhanViens.FindAsync(id);
            if (entity != null)
            {
                _context.LichTrucNhanViens.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateNextMaLTAsync()
        {
            try
            {
                // Lấy mã lịch trực cuối cùng
                var lastLichTruc = await _context.LichTrucNhanViens
                    .Where(lt => lt.MaLT.StartsWith("LT"))
                    .OrderByDescending(lt => lt.MaLT)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (lastLichTruc == null)
                {
                    return "LT001";
                }

                // Tách số từ mã cuối cùng
                string lastCode = lastLichTruc.MaLT;
                if (lastCode.Length >= 3)
                {
                    string numberPart = lastCode.Substring(2); // Bỏ "LT"

                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        int nextNumber = lastNumber + 1;
                        return $"LT{nextNumber:D3}"; // Định dạng 3 chữ số
                    }
                }

                // Fallback - tìm mã tiếp theo nếu parse lỗi
                var allCodes = await _context.LichTrucNhanViens
                    .Where(lt => lt.MaLT.StartsWith("LT"))
                    .Select(lt => lt.MaLT)
                    .AsNoTracking()
                    .ToListAsync();

                if (!allCodes.Any())
                {
                    return "LT001";
                }

                var maxNumber = allCodes
                    .Where(code => code.Length >= 3)
                    .Select(code => {
                        if (int.TryParse(code.Substring(2), out int num))
                            return num;
                        return 0;
                    })
                    .Max();

                return $"LT{(maxNumber + 1):D3}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi sinh mã lịch trực: {ex.Message}");

                // Fallback cuối cùng - sử dụng timestamp
                return $"LT{DateTime.Now:HHmmss}";
            }
        }
    }
    }
