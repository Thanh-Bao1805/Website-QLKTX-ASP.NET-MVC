using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface ISinhVienRepository
    {
        Task<IEnumerable<SinhVien>> GetAllAsync();
        Task<SinhVien> GetByIdAsync(string id);
        Task AddAsync(SinhVien entity);
        Task UpdateAsync(SinhVien entity);
        Task DeleteAsync(string id);
    }
}
