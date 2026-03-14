using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IQuanTriVienRepository
    {
        Task<IEnumerable<QuanTriVien>> GetAllAsync();
        Task<QuanTriVien> GetByIdAsync(string id);
        Task AddAsync(QuanTriVien entity);
        Task UpdateAsync(QuanTriVien entity);
        Task DeleteAsync(string id);

        // ?? THÊM PH??NG TH?C SINH MÃ
        Task<string> GenerateNextMaQTVAsync();
    }
}