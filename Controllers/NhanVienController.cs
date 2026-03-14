using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace DoAnCoSo.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly INhanVienRepository _nhanVienRepository;
        private readonly IChucVuRepository _chucVuRepository;

        public NhanVienController(INhanVienRepository nhanVienRepository, IChucVuRepository chucVuRepository)
        {
            _nhanVienRepository = nhanVienRepository;
            _chucVuRepository = chucVuRepository;
        }

        public async Task<IActionResult> Index()
        {
            var danhSachNhanVien = await _nhanVienRepository.GetAllAsync();
            return View(danhSachNhanVien);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var chucVus = await _chucVuRepository.GetAllAsync();
            ViewBag.ChucVus = new SelectList(chucVus, "MaChucVu", "TenChucVu");

            // 🔹 LẤY MÃ TỰ ĐỘNG VÀ TRUYỀN QUA VIEWBAG
            ViewBag.MaNV = await _nhanVienRepository.GenerateNextMaNVAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(NhanVien nhanVien)
        {
            if (!ModelState.IsValid)
            {
                var chucVus = await _chucVuRepository.GetAllAsync();
                ViewBag.ChucVus = new SelectList(chucVus, "MaChucVu", "TenChucVu");
                ViewBag.MaNV = await _nhanVienRepository.GenerateNextMaNVAsync();
                return View(nhanVien);
            }

            try
            {
                // 🔹 NẾU ĐỂ TRỐNG MÃ, REPOSITORY SẼ TỰ SINH
                // Hoặc kiểm tra trùng mã nếu người dùng nhập
                if (!string.IsNullOrEmpty(nhanVien.MaNV))
                {
                    var existingNhanVien = await _nhanVienRepository.GetByIdAsync(nhanVien.MaNV);
                    if (existingNhanVien != null)
                    {
                        ModelState.AddModelError("MaNV", "Mã nhân viên đã tồn tại.");
                        var chucVus = await _chucVuRepository.GetAllAsync();
                        ViewBag.ChucVus = new SelectList(chucVus, "MaChucVu", "TenChucVu");
                        ViewBag.MaNV = await _nhanVienRepository.GenerateNextMaNVAsync();
                        return View(nhanVien);
                    }
                }

                // Thêm nhân viên mới
                await _nhanVienRepository.AddAsync(nhanVien);

                TempData["SuccessMessage"] = "Thêm nhân viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra khi thêm nhân viên: {ex.Message}");
                var chucVus = await _chucVuRepository.GetAllAsync();
                ViewBag.ChucVus = new SelectList(chucVus, "MaChucVu", "TenChucVu");
                ViewBag.MaNV = await _nhanVienRepository.GenerateNextMaNVAsync();
                return View(nhanVien);
            }
        }

        public async Task<IActionResult> Display(string id)
        {
            var nhanVien = await _nhanVienRepository.GetByIdAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id)
        {
            var nhanVien = await _nhanVienRepository.GetByIdAsync(id);
            if (nhanVien == null) return NotFound();

            var chucVus = await _chucVuRepository.GetAllAsync();
            ViewBag.ChucVus = new SelectList(chucVus, "MaChucVu", "TenChucVu");
            return View(nhanVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, NhanVien nhanVien)
        {
            if (id != nhanVien.MaNV)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var chucVus = await _chucVuRepository.GetAllAsync();
                ViewBag.ChucVus = new SelectList(chucVus, "MaChucVu", "TenChucVu");
                return View(nhanVien);
            }

            try
            {
                await _nhanVienRepository.UpdateAsync(nhanVien);
                TempData["SuccessMessage"] = "Cập nhật thông tin nhân viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra khi cập nhật nhân viên: {ex.Message}");
                var chucVus = await _chucVuRepository.GetAllAsync();
                ViewBag.ChucVus = new SelectList(chucVus, "MaChucVu", "TenChucVu");
                return View(nhanVien);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var nhanVien = await _nhanVienRepository.GetByIdAsync(id);
            if (nhanVien == null) return NotFound();
            return View(nhanVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _nhanVienRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = "Xóa nhân viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra khi xóa nhân viên: {ex.Message}");
                var nhanVien = await _nhanVienRepository.GetByIdAsync(id);
                return View(nhanVien);
            }
        }
    }
}