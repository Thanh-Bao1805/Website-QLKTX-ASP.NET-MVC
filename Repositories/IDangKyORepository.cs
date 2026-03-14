using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IDangKyORepository
    {
        Task<IEnumerable<DangKyO>> GetAllAsync();
        Task<DangKyO> GetByIdAsync(string id);
        Task AddAsync(DangKyO entity);
        Task UpdateAsync(DangKyO entity);
        Task DeleteAsync(string id);
        Task<string> GenerateNextMaDKAsync();
        Task<bool> ExistsByMSSVAsync(string mssv, string? excludeMaDK = null);
        Task<bool> ExistsBySDTAsync(string sdt, string? excludeMaDK = null);
        Task<bool> ExistsByEmailAsync(string email, string? excludeMaDK = null);
        Task<bool> ExistsByCCCDAsync(string cccd, string? excludeMaDK = null);
    }
}
