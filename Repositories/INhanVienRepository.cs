using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface INhanVienRepository
    {
        Task<IEnumerable<NhanVien>> GetAllAsync();
        Task<NhanVien> GetByIdAsync(string id);
        Task AddAsync(NhanVien entity);
        Task UpdateAsync(NhanVien entity);
        Task DeleteAsync(string id);

        // ?? THÊM PH??NG TH?C SINH MÃ
        Task<string> GenerateNextMaNVAsync();
    }
}