using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class ThietBiController : Controller
    {
        private readonly IThietBiRepository _thietBiRepository;

        public ThietBiController(IThietBiRepository thietBiRepository)
        {
            _thietBiRepository = thietBiRepository;
        }

        public async Task<IActionResult> Index()
        {
            var thietBis = await _thietBiRepository.GetAllAsync();
            return View(thietBis);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(ThietBi thietBi)
        {
            // BỎ CHECK REQUIRED CHO MaThietBi (vì form không gửi lên)
            ModelState.Remove("MaThietBi");

            if (!ModelState.IsValid)
            {
                return View(thietBi);
            }

            try
            {
                // GÁN MÃ SAU KHI VALIDATE
                thietBi.MaThietBi = await _thietBiRepository.GenerateNewIdAsync();

                await _thietBiRepository.AddAsync(thietBi);
                TempData["Success"] = "Thêm thiết bị thành công.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm thiết bị: " + ex.Message);
                return View(thietBi);
            }
        }

        public async Task<IActionResult> Display(string id)
        {
            var thietBi = await _thietBiRepository.GetByIdAsync(id);
            if (thietBi == null) return NotFound();
            return View(thietBi);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id)
        {
            var thietBi = await _thietBiRepository.GetByIdAsync(id);
            if (thietBi == null) return NotFound();
            return View(thietBi);
        }

        // Update POST: nhận id, lấy entity, cập nhật trường cho phép, lưu
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, ThietBi thietBi)
        {
            if (!ModelState.IsValid)
            {
                // cần fetch lại MaThietBi để hiển thị view nếu có lỗi
                var existing = await _thietBiRepository.GetByIdAsync(id);
                if (existing == null) return NotFound();
                // copy để view hiển thị những gì user đã nhập (thietBi chứa các trường bind được)
                ViewBag.MaThietBi = existing.MaThietBi;
                return View(thietBi);
            }

            try
            {
                var existing = await _thietBiRepository.GetByIdAsync(id);
                if (existing == null) return NotFound();

                // CẬP NHẬT từng trường được phép thay đổi
                existing.TenThietBi = thietBi.TenThietBi;
                existing.Loai = thietBi.Loai;
                existing.MoTa = thietBi.MoTa;

                await _thietBiRepository.UpdateAsync(existing);
                TempData["Success"] = "Cập nhật thiết bị thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật thiết bị: " + ex.Message);
                // để hiển thị MaThietBi trong view nếu cần
                ViewBag.MaThietBi = id;
                return View(thietBi);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var thietBi = await _thietBiRepository.GetByIdAsync(id);
            if (thietBi == null) return NotFound();
            return View(thietBi);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _thietBiRepository.DeleteAsync(id);
                TempData["Success"] = "Xóa thiết bị thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi xóa thiết bị: " + ex.Message);
                return View();
            }
        }
    }
}
