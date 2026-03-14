using DoAnCoSo.Models.ViewModels;
using System.Threading.Tasks;

namespace DoAnCoSo.Repositories
{
    public interface IThongKeRepository
    {
        Task<ThongKeViewModel> GetThongKeTongQuanAsync();
        Task<ChiPhiTheoThangViewModel> GetChiPhiTheoThangAsync(int? year = null);
    }
}