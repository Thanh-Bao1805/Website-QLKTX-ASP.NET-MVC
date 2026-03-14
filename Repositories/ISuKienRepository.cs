using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface ISuKienRepository
    {
        Task<IEnumerable<SuKien>> GetAllAsync();
        Task<SuKien> GetByIdAsync(string id);
        Task AddAsync(SuKien entity);
        Task UpdateAsync(SuKien entity);
        Task DeleteAsync(string id);
        Task<string> GenerateNewIdAsync(); // THÊM PHƯƠNG THỨC NÀY
    }
}