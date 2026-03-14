using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IHoaDonRepository
    {
        Task<IEnumerable<HoaDon>> GetAllAsync();
        Task<HoaDon> GetByIdAsync(string id);
        Task<HoaDon> GetByMaDNAsync(string maDN);
        Task<HoaDon> GetByMaDKDVAsync(string maDKDV);
        Task<string> GetNextMaHoaDonAsync();
        Task AddAsync(HoaDon entity);
        Task UpdateAsync(HoaDon entity);
        Task DeleteAsync(string id);
    }
}
