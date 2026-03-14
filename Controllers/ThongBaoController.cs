using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class ThongBaoController : Controller
    {
        private readonly IThongBaoRepository _thongBaoRepository;
        private readonly IQuanTriVienRepository _quanTriVienRepository;

        public ThongBaoController(
            IThongBaoRepository thongBaoRepository,
            IQuanTriVienRepository quanTriVienRepository)
        {
            _thongBaoRepository = thongBaoRepository;
            _quanTriVienRepository = quanTriVienRepository;
        }

        // Hiển thị danh sách thông báo
        public async Task<IActionResult> Index()
        {
            var thongBaos = await _thongBaoRepository.GetAllAsync();
            return View(thongBaos);
        }

        // Hiển thị chi tiết thông báo
        public async Task<IActionResult> Display(string id)
        {
            var tb = await _thongBaoRepository.GetByIdAsync(id);
            if (tb == null) return NotFound();
            return View(tb);
        }

        // Hiển thị form thêm thông báo
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            await LoadDropdownDataAsync();

            // Tạo model mới với mã tự sinh để hiển thị
            var newId = await _thongBaoRepository.GenerateNewIdAsync();
            var model = new ThongBao
            {
                MaTB = newId, // Hiển thị mã sẽ được tạo
                NgayGui = DateTime.Now
            };

            return View(model);
        }

        // Xử lý thêm thông báo
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(ThongBao tb)
        {
            // XÓA VALIDATION ERROR CHO MaTB VÌ NÓ SẼ ĐƯỢC TỰ SINH
            ModelState.Remove("MaTB");

            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync();
                return View(tb);
            }

            try
            {
                // Repository sẽ tự sinh mã nếu MaTB là null hoặc empty
                tb.MaTB = null; // Để repository tự sinh mã mới

                await _thongBaoRepository.AddAsync(tb);
                TempData["Success"] = "Thêm thông báo thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm thông báo: " + ex.Message);
                await LoadDropdownDataAsync();
                return View(tb);
            }
        }

        // Hiển thị form cập nhật thông báo
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id)
        {
            var tb = await _thongBaoRepository.GetByIdAsync(id);
            if (tb == null) return NotFound();

            await LoadDropdownDataAsync();
            return View(tb);
        }

        // Xử lý cập nhật thông báo
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(ThongBao tb)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync();
                return View(tb);
            }

            try
            {
                await _thongBaoRepository.UpdateAsync(tb);
                TempData["Success"] = "Cập nhật thông báo thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật thông báo: " + ex.Message);
                await LoadDropdownDataAsync();
                return View(tb);
            }
        }

        // Hiển thị form xác nhận xóa
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var tb = await _thongBaoRepository.GetByIdAsync(id);
            if (tb == null) return NotFound();
            return View(tb);
        }

        // Xử lý xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _thongBaoRepository.DeleteAsync(id);
                TempData["Success"] = "Xóa thông báo thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi xóa thông báo: " + ex.Message);
                var tb = await _thongBaoRepository.GetByIdAsync(id);
                return View("Delete", tb);
            }
        }

        // ------------------ Private Helpers ------------------

        private async Task LoadDropdownDataAsync()
        {
            // Load danh sách quản trị viên
            var qtvs = await _quanTriVienRepository.GetAllAsync();
            ViewBag.QuanTriViens = new SelectList(qtvs, "MaQTV", "HoTen");

            // Danh sách đối tượng
            var doiTuongList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Tất cả sinh viên", Text = "Tất cả sinh viên" },
                new SelectListItem { Value = "Sinh viên KTX", Text = "Sinh viên KTX" },
                new SelectListItem { Value = "Sinh viên năm 1", Text = "Sinh viên năm 1" },
                new SelectListItem { Value = "Sinh viên năm 2", Text = "Sinh viên năm 2" },
                new SelectListItem { Value = "Sinh viên năm 3", Text = "Sinh viên năm 3" },
                new SelectListItem { Value = "Sinh viên năm 4", Text = "Sinh viên năm 4" },
                new SelectListItem { Value = "Khu A", Text = "Khu A" },
                new SelectListItem { Value = "Khu B", Text = "Khu B" },
                new SelectListItem { Value = "Khu C", Text = "Khu C" },
                new SelectListItem { Value = "Tầng 1", Text = "Tầng 1" },
                new SelectListItem { Value = "Tầng 2", Text = "Tầng 2" },
                new SelectListItem { Value = "Tầng 3", Text = "Tầng 3" }
            };

            ViewBag.DoiTuongs = new SelectList(doiTuongList, "Value", "Text");
        }
    }
}