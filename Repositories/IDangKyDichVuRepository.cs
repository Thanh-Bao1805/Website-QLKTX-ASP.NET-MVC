using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IDangKyDichVuRepository
    {
        Task<IEnumerable<DangKyDichVu>> GetAllAsync();
        Task<DangKyDichVu> GetByIdAsync(string id);
        Task AddAsync(DangKyDichVu entity);
        Task UpdateAsync(DangKyDichVu entity);
        Task DeleteAsync(string id);
        Task<string> GenerateNextMaDKDVAsync();
    }
}
