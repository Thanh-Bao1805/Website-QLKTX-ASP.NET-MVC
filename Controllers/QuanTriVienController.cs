using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class QuanTriVienController : Controller
    {
        private readonly IQuanTriVienRepository _qtvRepository;

        public QuanTriVienController(IQuanTriVienRepository qtvRepository)
        {
            _qtvRepository = qtvRepository;
        }

        // Hiển thị danh sách quản trị viên
        public async Task<IActionResult> Index()
        {
            var list = await _qtvRepository.GetAllAsync();
            return View(list);
        }

        // Hiển thị chi tiết quản trị viên
        public async Task<IActionResult> Display(string id)
        {
            var qtv = await _qtvRepository.GetByIdAsync(id);
            if (qtv == null) return NotFound();
            return View(qtv);
        }

        // Hiển thị form thêm quản trị viên
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            // 🔹 LẤY MÃ TỰ ĐỘNG VÀ TRUYỀN QUA VIEWBAG
            ViewBag.MaQTV = await _qtvRepository.GenerateNextMaQTVAsync();
            return View();
        }

        // Xử lý thêm quản trị viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(QuanTriVien qtv)
        {
            if (!ModelState.IsValid)
            {
                // Giữ lại mã QTV khi có lỗi validation
                ViewBag.MaQTV = await _qtvRepository.GenerateNextMaQTVAsync();
                return View(qtv);
            }

            try
            {
                // 🔹 KHÔNG CẦN SET MaQTV Ở ĐÂY NỮA, REPOSITORY SẼ TỰ SINH
                // Hoặc nếu bạn muốn kiểm tra trùng mã:
                var existingQTV = await _qtvRepository.GetByIdAsync(qtv.MaQTV);
                if (existingQTV != null)
                {
                    ModelState.AddModelError("MaQTV", "Mã quản trị viên đã tồn tại!");
                    ViewBag.MaQTV = await _qtvRepository.GenerateNextMaQTVAsync();
                    return View(qtv);
                }

                await _qtvRepository.AddAsync(qtv);
                TempData["Success"] = "Thêm quản trị viên thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm: " + ex.Message);
                ViewBag.MaQTV = await _qtvRepository.GenerateNextMaQTVAsync();
                return View(qtv);
            }
        }

        // Hiển thị form cập nhật quản trị viên
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id)
        {
            var qtv = await _qtvRepository.GetByIdAsync(id);
            if (qtv == null) return NotFound();
            return View(qtv);
        }

        // Xử lý cập nhật quản trị viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(QuanTriVien qtv)
        {
            if (!ModelState.IsValid)
                return View(qtv);

            try
            {
                await _qtvRepository.UpdateAsync(qtv);
                TempData["Success"] = "Cập nhật thông tin thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
                return View(qtv);
            }
        }

        // Hiển thị form xác nhận xóa quản trị viên
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var qtv = await _qtvRepository.GetByIdAsync(id);
            if (qtv == null) return NotFound();
            return View(qtv);
        }

        // Xử lý xóa quản trị viên
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _qtvRepository.DeleteAsync(id);
                TempData["Success"] = "Xóa quản trị viên thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi xóa: " + ex.Message);
                var qtv = await _qtvRepository.GetByIdAsync(id);
                return View(qtv);
            }
        }
    }
}