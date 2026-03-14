using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IThongBaoRepository
    {
        Task<IEnumerable<ThongBao>> GetAllAsync();
        Task<ThongBao> GetByIdAsync(string id);
        Task AddAsync(ThongBao entity);
        Task UpdateAsync(ThongBao entity);
        Task DeleteAsync(string id);
        Task<string> GenerateNewIdAsync(); // THÊM PHƯƠNG THỨC NÀY
    }
}