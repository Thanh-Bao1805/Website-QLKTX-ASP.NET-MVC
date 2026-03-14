using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IChiTietThietBiRepository
    {
        Task<IEnumerable<ChiTietThietBi>> GetAllAsync();
        Task<ChiTietThietBi> GetByIdAsync(string id);
        Task AddAsync(ChiTietThietBi entity);
        Task UpdateAsync(ChiTietThietBi entity);
        Task DeleteAsync(string id);
    }
}
