using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IThietBiRepository
    {
        Task<IEnumerable<ThietBi>> GetAllAsync();
        Task<ThietBi> GetByIdAsync(string id);
        Task AddAsync(ThietBi entity);
        Task UpdateAsync(ThietBi entity);
        Task DeleteAsync(string id);

        // Thêm hàm sinh ID
        Task<string> GenerateNewIdAsync();
    }
}
