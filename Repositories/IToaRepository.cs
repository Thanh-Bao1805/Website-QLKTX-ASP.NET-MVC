using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IToaRepository
    {
        Task<IEnumerable<Toa>> GetAllAsync();
        Task<Toa> GetByIdAsync(string id);
        Task AddAsync(Toa entity);
        Task UpdateAsync(Toa entity);
        Task DeleteAsync(string id);
    }
}
