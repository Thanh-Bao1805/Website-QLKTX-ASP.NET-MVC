// DoAnCoSo/Repositories/IHopDongRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;
using Microsoft.EntityFrameworkCore;
namespace DoAnCoSo.Repositories
{
    public interface IHopDongRepository
    {
        Task<IEnumerable<HopDong>> GetAllAsync();
        Task<IEnumerable<HopDong>> GetBySinhVienIdAsync(string maSV); // Thêm phương thức này
        Task<HopDong> GetByIdAsync(string id);
        Task AddAsync(HopDong entity);
        Task UpdateAsync(HopDong entity);
        Task DeleteAsync(string id);
        Task<string> GenerateNextMaHopDongAsync();
    }
}