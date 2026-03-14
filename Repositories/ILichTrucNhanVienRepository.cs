using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface ILichTrucNhanVienRepository
    {
        Task<IEnumerable<LichTrucNhanVien>> GetAllAsync();
        Task<LichTrucNhanVien> GetByIdAsync(string id);
        Task AddAsync(LichTrucNhanVien entity);
        Task UpdateAsync(LichTrucNhanVien entity);
        Task DeleteAsync(string id);

        // Thêm phương thức sinh mã tự động
        Task<string> GenerateNextMaLTAsync();
    }
}