using DoAnCoSo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoAnCoSo.Repositories
{
    public interface IPhanHoiRepository
    {
        Task<IEnumerable<PhanHoi>> GetAllAsync();
        Task<PhanHoi> GetByIdAsync(string id);
        Task AddAsync(PhanHoi entity);
        Task UpdateAsync(PhanHoi entity);
        Task DeleteAsync(string id);
        Task<IEnumerable<PhanHoi>> GetMessagesBetweenAsync(string user1, string user2);
        Task<IEnumerable<string>> GetUsersChattedWithAdminAsync();
    }
}