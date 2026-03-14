using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IChucVuRepository
    {
        Task<IEnumerable<ChucVu>> GetAllAsync();
        Task<ChucVu> GetByIdAsync(string id);
        Task AddAsync(ChucVu entity);
        Task UpdateAsync(ChucVu entity);
        Task DeleteAsync(string id);
    }
}
