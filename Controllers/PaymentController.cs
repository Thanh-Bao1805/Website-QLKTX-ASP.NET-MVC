using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace DoAnCoSo.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IHoaDonRepository _hoaDonRepository;
        private readonly IDienNuocRepository _dienNuocRepository;
        private readonly IDangKyDichVuRepository _dangKyDichVuRepository;

        public PaymentController(
            IHoaDonRepository hoaDonRepository,
            IDienNuocRepository dienNuocRepository,
            IDangKyDichVuRepository dangKyDichVuRepository)
        {
            _hoaDonRepository = hoaDonRepository;
            _dienNuocRepository = dienNuocRepository;
            _dangKyDichVuRepository = dangKyDichVuRepository;
        }

        public async Task<IActionResult> OrderDetail(string id)
        {
            var hoaDon = await _hoaDonRepository.GetByIdAsync(id);
            if (hoaDon == null) return NotFound();

            // Load DienNuoc và DangKyDichVu thủ công để đảm bảo dữ liệu được load đầy đủ
            if (!string.IsNullOrEmpty(hoaDon.MaDN))
            {
                hoaDon.DienNuoc = await _dienNuocRepository.GetByIdAsync(hoaDon.MaDN);
            }

            if (!string.IsNullOrEmpty(hoaDon.MaDKDV))
            {
                hoaDon.DangKyDichVu = await _dangKyDichVuRepository.GetByIdAsync(hoaDon.MaDKDV);
            }

            // Set additional view data if needed
            ViewBag.TenKhachHang = hoaDon.SinhVien?.HoTen;
            ViewBag.SoDienThoai = hoaDon.SinhVien?.SDT;
            
            // Debug info
            ViewBag.DebugInfo = $"LoaiChiPhi: {hoaDon.LoaiChiPhi}, MaDN: {hoaDon.MaDN}, MaDKDV: {hoaDon.MaDKDV}, " +
                               $"DienNuoc: {(hoaDon.DienNuoc != null ? "có" : "null")}, " +
                               $"DangKyDichVu: {(hoaDon.DangKyDichVu != null ? "có" : "null")}, " +
                               $"HoaDonChiTiets: {(hoaDon.HoaDonChiTiets != null && hoaDon.HoaDonChiTiets.Any() ? hoaDon.HoaDonChiTiets.Count().ToString() : "null")}";

            return View(hoaDon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayConfirmed(string id)
        {
            var hoaDon = await _hoaDonRepository.GetByIdAsync(id);
            if (hoaDon == null) return NotFound();

            // Only update if not already paid
            if (hoaDon.TrangThai != "Đã thanh toán")
            {
                hoaDon.TrangThai = "Đã thanh toán";
                try
                {
                    await _hoaDonRepository.UpdateAsync(hoaDon);
                    TempData["Success"] = "Thanh toán hóa đơn thành công!";
                    return RedirectToAction("PaymentSuccess", new { id = hoaDon.MaHoaDon });
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Lỗi khi cập nhật trạng thái thanh toán: {ex.Message}";
                    return RedirectToAction("OrderDetail", new { id = hoaDon.MaHoaDon });
                }
            }
            else
            {
                TempData["Info"] = "Hóa đơn này đã được thanh toán trước đó.";
                return RedirectToAction("OrderDetail", new { id = hoaDon.MaHoaDon });
            }
        }

        public async Task<IActionResult> PaymentSuccess(string id)
        {
            var hoaDon = await _hoaDonRepository.GetByIdAsync(id);
            if (hoaDon == null) return NotFound();
            return View(hoaDon);
        }
    }
} 