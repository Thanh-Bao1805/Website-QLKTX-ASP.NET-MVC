using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class ViPhamController : Controller
    {
        private readonly IViPhamRepository _viPhamRepository;
        private readonly ISinhVienRepository _sinhVienRepository;
        private readonly INoiQuyRepository _noiQuyRepository;

        public ViPhamController(
            IViPhamRepository viPhamRepository,
            ISinhVienRepository sinhVienRepository,
            INoiQuyRepository noiQuyRepository)
        {
            _viPhamRepository = viPhamRepository;
            _sinhVienRepository = sinhVienRepository;
            _noiQuyRepository = noiQuyRepository;
        }

        public async Task<IActionResult> Index()
        {
            var viPhams = await _viPhamRepository.GetAllAsync();
            return View(viPhams);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            await LoadDropdownDataAsync();

            // Tạo model mới với mã tự sinh để hiển thị
            var newId = await _viPhamRepository.GenerateNewIdAsync();
            var model = new ViPham
            {
                MaViPham = newId, // Hiển thị mã sẽ được tạo
                NgayViPham = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(ViPham viPham)
        {
            // XÓA VALIDATION ERROR CHO MaViPham VÌ NÓ SẼ ĐƯỢC TỰ SINH
            ModelState.Remove("MaViPham");

            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync();
                return View(viPham);
            }

            try
            {
                // Repository sẽ tự sinh mã nếu MaViPham là null hoặc empty
                viPham.MaViPham = null;

                await _viPhamRepository.AddAsync(viPham);
                TempData["Success"] = "Thêm vi phạm thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm vi phạm: " + ex.Message);
                await LoadDropdownDataAsync();
                return View(viPham);
            }
        }

        public async Task<IActionResult> Display(string id)
        {
            var viPham = await _viPhamRepository.GetByIdAsync(id);
            if (viPham == null) return NotFound();
            return View(viPham);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id)
        {
            var viPham = await _viPhamRepository.GetByIdAsync(id);
            if (viPham == null) return NotFound();

            await LoadDropdownDataAsync();
            return View(viPham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(ViPham viPham)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync();
                return View(viPham);
            }

            try
            {
                await _viPhamRepository.UpdateAsync(viPham);
                TempData["Success"] = "Cập nhật vi phạm thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật vi phạm: " + ex.Message);
                await LoadDropdownDataAsync();
                return View(viPham);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var viPham = await _viPhamRepository.GetByIdAsync(id);
            if (viPham == null) return NotFound();
            return View(viPham);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _viPhamRepository.DeleteAsync(id);
                TempData["Success"] = "Xóa vi phạm thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi xóa vi phạm: " + ex.Message);
                var viPham = await _viPhamRepository.GetByIdAsync(id);
                return View("Delete", viPham);
            }
        }

        private async Task LoadDropdownDataAsync()
        {
            var sinhViens = await _sinhVienRepository.GetAllAsync();
            var noiQuys = await _noiQuyRepository.GetAllAsync();

            ViewBag.SinhViens = new SelectList(sinhViens, "MaSV", "HoTen");
            ViewBag.NoiQuys = new SelectList(noiQuys, "MaNoiQuy", "NoiDung");
        }
    }
}