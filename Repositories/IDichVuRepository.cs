using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IDichVuRepository
    {
        Task<IEnumerable<DichVu>> GetAllAsync();
        Task<DichVu> GetByIdAsync(string id);
        Task AddAsync(DichVu entity);
        Task UpdateAsync(DichVu entity);
        Task DeleteAsync(string id);
    }
}
