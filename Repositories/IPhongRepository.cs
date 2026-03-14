using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IPhongRepository
    {
        Task<IEnumerable<Phong>> GetAllAsync();
        Task<Phong> GetByIdAsync(string id);
        Task AddAsync(Phong entity);
        Task UpdateAsync(Phong entity);
        Task DeleteAsync(string id);
        // Thêm phương thức mới
        Task<string> GenerateNextRoomCodeAsync(string maToa);
    }
}
