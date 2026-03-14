using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IDienNuocRepository
    {
        Task<IEnumerable<DienNuoc>> GetAllAsync();
        Task<DienNuoc> GetByIdAsync(string id);
        Task AddAsync(DienNuoc entity);
        Task UpdateAsync(DienNuoc entity);
        Task DeleteAsync(string id);

        // Thêm phương thức sinh mã tự động
        Task<string> GenerateNextMaDNAsync();
    }
}