using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class LichTrucNhanVienController : Controller
    {
        private readonly ILichTrucNhanVienRepository _lichTrucRepository;
        private readonly INhanVienRepository _nhanVienRepository;

        public LichTrucNhanVienController(
            ILichTrucNhanVienRepository lichTrucRepository,
            INhanVienRepository nhanVienRepository)
        {
            _lichTrucRepository = lichTrucRepository ?? throw new ArgumentNullException(nameof(lichTrucRepository));
            _nhanVienRepository = nhanVienRepository ?? throw new ArgumentNullException(nameof(nhanVienRepository));
        }

        // Hiển thị danh sách lịch trực
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _lichTrucRepository.GetAllAsync();
                return View(list);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải danh sách lịch trực: " + ex.Message;
                return View(Enumerable.Empty<LichTrucNhanVien>());
            }
        }

        // Hiển thị form thêm lịch trực (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            try
            {
                await LoadDropdownDataAsync();

                // Tạo model mới với giá trị mặc định
                var lichTruc = new LichTrucNhanVien
                {
                    NgayTruc = DateTime.Today,
                    CaTruc = "Sáng"
                };

                return View(lichTruc);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form thêm lịch trực: " + ex.Message;
                return View(new LichTrucNhanVien());
            }
        }

        // Xử lý thêm lịch trực (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(LichTrucNhanVien lichTruc)
        {
            try
            {
                // ĐẢM BẢO LUÔN SINH MÃ TRƯỚC KHI VALIDATION
                if (string.IsNullOrEmpty(lichTruc.MaLT))
                {
                    lichTruc.MaLT = await _lichTrucRepository.GenerateNextMaLTAsync();
                }

                // Fallback nếu vẫn không có mã
                if (string.IsNullOrEmpty(lichTruc.MaLT))
                {
                    lichTruc.MaLT = $"LT{DateTime.Now:yyyyMMddHHmmss}";
                }

                if (ModelState.IsValid)
                {
                    await _lichTrucRepository.AddAsync(lichTruc);
                    TempData["Success"] = $"Thêm lịch trực thành công. Mã lịch trực: {lichTruc.MaLT}";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Hiển thị lỗi validation
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        TempData["Error"] = error.ErrorMessage;
                        break;
                    }
                }

                await LoadDropdownDataAsync();
                return View(lichTruc);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi thêm lịch trực: " + ex.Message;
                await LoadDropdownDataAsync();
                return View(lichTruc);
            }
        }

        // API để lấy mã lịch trực tự động
        [HttpGet]
        public async Task<JsonResult> GenerateMaLT()
        {
            try
            {
                string generatedCode = await _lichTrucRepository.GenerateNextMaLTAsync();
                return Json(new { success = true, maLT = generatedCode });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // Hiển thị chi tiết lịch trực
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Display(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã lịch trực không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var lichTruc = await _lichTrucRepository.GetByIdAsync(id);
                if (lichTruc == null)
                {
                    TempData["Error"] = "Không tìm thấy lịch trực";
                    return RedirectToAction(nameof(Index));
                }

                return View(lichTruc);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải thông tin lịch trực: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Hiển thị form cập nhật lịch trực (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã lịch trực không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var lichTruc = await _lichTrucRepository.GetByIdAsync(id);
                if (lichTruc == null)
                {
                    TempData["Error"] = "Không tìm thấy lịch trực";
                    return RedirectToAction(nameof(Index));
                }

                await LoadDropdownDataAsync();
                return View(lichTruc);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form cập nhật: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Xử lý cập nhật lịch trực (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(LichTrucNhanVien lichTruc)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadDropdownDataAsync();
                    return View(lichTruc);
                }

                // Kiểm tra sự tồn tại trước khi cập nhật
                var existingLichTruc = await _lichTrucRepository.GetByIdAsync(lichTruc.MaLT);
                if (existingLichTruc == null)
                {
                    TempData["Error"] = "Không tìm thấy lịch trực để cập nhật";
                    await LoadDropdownDataAsync();
                    return View(lichTruc);
                }

                await _lichTrucRepository.UpdateAsync(lichTruc);
                TempData["Success"] = $"Cập nhật lịch trực {lichTruc.MaLT} thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật lịch trực: " + ex.Message;
                await LoadDropdownDataAsync();
                return View(lichTruc);
            }
        }

        // Hiển thị form xác nhận xóa lịch trực (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã lịch trực không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var lichTruc = await _lichTrucRepository.GetByIdAsync(id);
                if (lichTruc == null)
                {
                    TempData["Error"] = "Không tìm thấy lịch trực";
                    return RedirectToAction(nameof(Index));
                }

                return View(lichTruc);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form xóa: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Xử lý xóa lịch trực (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã lịch trực không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra sự tồn tại trước khi xóa
                var existingLichTruc = await _lichTrucRepository.GetByIdAsync(id);
                if (existingLichTruc == null)
                {
                    TempData["Error"] = "Không tìm thấy lịch trực để xóa";
                    return RedirectToAction(nameof(Index));
                }

                await _lichTrucRepository.DeleteAsync(id);
                TempData["Success"] = $"Xóa lịch trực {id} thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa lịch trực: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // ------------------ Private Helpers ------------------
        private async Task LoadDropdownDataAsync()
        {
            try
            {
                var nhanViens = await _nhanVienRepository.GetAllAsync();
                ViewBag.NhanViens = new SelectList(nhanViens, "MaNV", "HoTen");
            }
            catch (Exception ex)
            {
                ViewBag.NhanViens = new SelectList(Enumerable.Empty<SelectListItem>());
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tải dropdown: {ex.Message}");
            }
        }
    }
}