using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface ISuCoBaoTriRepository
    {
        Task<IEnumerable<SuCoBaoTri>> GetAllAsync();
        Task<SuCoBaoTri> GetByIdAsync(string id);
        Task AddAsync(SuCoBaoTri entity);
        Task UpdateAsync(SuCoBaoTri entity);
        Task DeleteAsync(string id);
        // Thêm phương thức sinh mã tự động
        Task<string> GenerateNextMaSuCoAsync();
    }
}
