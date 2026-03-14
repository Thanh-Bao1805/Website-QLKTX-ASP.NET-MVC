using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IViPhamRepository
    {
        Task<IEnumerable<ViPham>> GetAllAsync();
        Task<ViPham> GetByIdAsync(string id);
        Task AddAsync(ViPham entity);
        Task UpdateAsync(ViPham entity);
        Task DeleteAsync(string id);
        Task<string> GenerateNewIdAsync(); // THÊM PHƯƠNG THỨC NÀY
    }
}