using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface INoiQuyRepository
    {
        Task<IEnumerable<NoiQuy>> GetAllAsync();
        Task<NoiQuy> GetByIdAsync(string id);
        Task AddAsync(NoiQuy entity);
        Task UpdateAsync(NoiQuy entity);
        Task DeleteAsync(string id);
    }
}
