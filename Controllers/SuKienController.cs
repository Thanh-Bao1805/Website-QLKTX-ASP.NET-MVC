using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class SuKienController : Controller
    {
        private readonly ISuKienRepository _suKienRepository;
        private readonly IQuanTriVienRepository _qtvRepository;

        public SuKienController(
            ISuKienRepository suKienRepository,
            IQuanTriVienRepository qtvRepository)
        {
            _suKienRepository = suKienRepository;
            _qtvRepository = qtvRepository;
        }

        // Hiển thị danh sách sự kiện
        public async Task<IActionResult> Index()
        {
            var suKiens = await _suKienRepository.GetAllAsync();
            return View(suKiens);
        }

        // Hiển thị form thêm sự kiện
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            await LoadDropdownDataAsync();

            // Tạo model mới với mã tự sinh để hiển thị
            var newId = await _suKienRepository.GenerateNewIdAsync();
            var model = new SuKien
            {
                MaSuKien = newId, // Hiển thị mã sẽ được tạo
                NgayToChuc = DateTime.Now
            };

            return View(model);
        }

        // Xử lý thêm sự kiện
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(SuKien suKien)
        {
            // XÓA VALIDATION ERROR CHO MaSuKien VÌ NÓ SẼ ĐƯỢC TỰ SINH
            ModelState.Remove("MaSuKien");

            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync();
                return View(suKien);
            }

            try
            {
                // Repository sẽ tự sinh mã nếu MaSuKien là null hoặc empty
                suKien.MaSuKien = null;

                await _suKienRepository.AddAsync(suKien);
                TempData["Success"] = "Thêm sự kiện thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm sự kiện: " + ex.Message);
                await LoadDropdownDataAsync();
                return View(suKien);
            }
        }

        // Hiển thị chi tiết sự kiện
        public async Task<IActionResult> Display(string id)
        {
            var suKien = await _suKienRepository.GetByIdAsync(id);
            if (suKien == null) return NotFound();
            return View(suKien);
        }

        // Hiển thị form cập nhật sự kiện
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id)
        {
            var suKien = await _suKienRepository.GetByIdAsync(id);
            if (suKien == null) return NotFound();

            await LoadDropdownDataAsync();
            return View(suKien);
        }

        // Xử lý cập nhật sự kiện
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(SuKien suKien)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync();
                return View(suKien);
            }

            try
            {
                await _suKienRepository.UpdateAsync(suKien);
                TempData["Success"] = "Cập nhật sự kiện thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật sự kiện: " + ex.Message);
                await LoadDropdownDataAsync();
                return View(suKien);
            }
        }

        // Hiển thị form xác nhận xóa sự kiện
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var suKien = await _suKienRepository.GetByIdAsync(id);
            if (suKien == null) return NotFound();
            return View(suKien);
        }

        // Xử lý xóa sự kiện
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _suKienRepository.DeleteAsync(id);
                TempData["Success"] = "Xóa sự kiện thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi xóa sự kiện: " + ex.Message);
                var suKien = await _suKienRepository.GetByIdAsync(id);
                return View("Delete", suKien);
            }
        }

        // ------------------ Private Helpers ------------------
        private async Task LoadDropdownDataAsync()
        {
            var qtvs = await _qtvRepository.GetAllAsync();
            ViewBag.QuanTriViens = new SelectList(qtvs, "MaQTV", "HoTen");
        }
    }
}