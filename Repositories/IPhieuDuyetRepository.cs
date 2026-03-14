using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IPhieuDuyetRepository
    {
        Task<IEnumerable<PhieuDuyet>> GetAllAsync();
        Task<PhieuDuyet> GetByIdAsync(string id);
        Task AddAsync(PhieuDuyet entity);
        Task UpdateAsync(PhieuDuyet entity);
        Task DeleteAsync(string id);

        // Thêm phương thức sinh mã tự động
        Task<string> GenerateNextMaPhieuAsync();
    }
}