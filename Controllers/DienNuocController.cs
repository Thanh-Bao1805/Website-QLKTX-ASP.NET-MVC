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
    public class DienNuocController : Controller
    {
        private readonly IDienNuocRepository _dienNuocRepository;
        private readonly IPhongRepository _phongRepository;

        public DienNuocController(IDienNuocRepository dienNuocRepository, IPhongRepository phongRepository)
        {
            _dienNuocRepository = dienNuocRepository ?? throw new ArgumentNullException(nameof(dienNuocRepository));
            _phongRepository = phongRepository ?? throw new ArgumentNullException(nameof(phongRepository));
        }

        // Hiển thị danh sách điện nước
        public async Task<IActionResult> Index()
        {
            try
            {
                var dienNuocs = await _dienNuocRepository.GetAllAsync();
                return View(dienNuocs);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải danh sách điện nước: " + ex.Message;
                return View(Enumerable.Empty<DienNuoc>());
            }
        }

        // Hiển thị form thêm mới (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            try
            {
                await LoadDropdownDataAsync();

                // Tạo model mới với giá trị mặc định
                var dienNuoc = new DienNuoc
                {
                    NgayGhi = DateTime.Today,
                    DonGiaDien = 3500, // Giá điện mặc định
                    DonGiaNuoc = 10000 // Giá nước mặc định
                };

                return View(dienNuoc);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form thêm: " + ex.Message;
                return View(new DienNuoc());
            }
        }

        // Xử lý thêm mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(DienNuoc dienNuoc)
        {
            try
            {
                // TỰ ĐỘNG SINH MÃ ĐIỆN NƯỚC
                dienNuoc.MaDN = await _dienNuocRepository.GenerateNextMaDNAsync();

                if (ModelState.IsValid)
                {
                    // Tính tổng tiền
                    dienNuoc.TongTien = (dienNuoc.SoDien * dienNuoc.DonGiaDien) + (dienNuoc.SoNuoc * dienNuoc.DonGiaNuoc);

                    await _dienNuocRepository.AddAsync(dienNuoc);
                    TempData["Success"] = $"Thêm dữ liệu điện nước thành công. Mã: {dienNuoc.MaDN}";
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
                return View(dienNuoc);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi thêm dữ liệu: " + ex.Message;
                await LoadDropdownDataAsync();
                return View(dienNuoc);
            }
        }

        // API để lấy mã điện nước tự động
        [HttpGet]
        public async Task<JsonResult> GenerateMaDN()
        {
            try
            {
                string generatedCode = await _dienNuocRepository.GenerateNextMaDNAsync();
                return Json(new { success = true, maDN = generatedCode });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // Hiển thị chi tiết
        public async Task<IActionResult> Display(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã điện nước không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var dienNuoc = await _dienNuocRepository.GetByIdAsync(id);
                if (dienNuoc == null)
                {
                    TempData["Error"] = "Không tìm thấy dữ liệu điện nước";
                    return RedirectToAction(nameof(Index));
                }

                return View(dienNuoc);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải thông tin: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Hiển thị form chỉnh sửa (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã điện nước không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var dienNuoc = await _dienNuocRepository.GetByIdAsync(id);
                if (dienNuoc == null)
                {
                    TempData["Error"] = "Không tìm thấy dữ liệu điện nước";
                    return RedirectToAction(nameof(Index));
                }

                await LoadDropdownDataAsync();
                return View(dienNuoc);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form cập nhật: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Xử lý cập nhật (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        // Xử lý cập nhật (POST)
        [HttpPost]
        
        public async Task<IActionResult> Update(DienNuoc dienNuoc)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadDropdownDataAsync();
                    return View(dienNuoc);
                }

                // Kiểm tra sự tồn tại trước khi cập nhật (sử dụng NoTracking)
                var existingDienNuoc = await _dienNuocRepository.GetByIdAsync(dienNuoc.MaDN);
                if (existingDienNuoc == null)
                {
                    TempData["Error"] = "Không tìm thấy dữ liệu điện nước để cập nhật";
                    await LoadDropdownDataAsync();
                    return View(dienNuoc);
                }

                // Tính tổng tiền
                dienNuoc.TongTien = (dienNuoc.SoDien * dienNuoc.DonGiaDien) + (dienNuoc.SoNuoc * dienNuoc.DonGiaNuoc);

                // CẬP NHẬT với xử lý lỗi tracking
                await _dienNuocRepository.UpdateAsync(dienNuoc);
                TempData["Success"] = $"Cập nhật dữ liệu điện nước {dienNuoc.MaDN} thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi chi tiết hơn
                System.Diagnostics.Debug.WriteLine($"Lỗi cập nhật: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                TempData["Error"] = "Lỗi khi cập nhật dữ liệu: " + ex.Message;
                await LoadDropdownDataAsync();
                return View(dienNuoc);
            }
        }

        // Hiển thị form xác nhận xóa (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã điện nước không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var dienNuoc = await _dienNuocRepository.GetByIdAsync(id);
                if (dienNuoc == null)
                {
                    TempData["Error"] = "Không tìm thấy dữ liệu điện nước";
                    return RedirectToAction(nameof(Index));
                }

                return View(dienNuoc);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form xóa: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Xử lý xóa (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã điện nước không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra sự tồn tại trước khi xóa
                var existingDienNuoc = await _dienNuocRepository.GetByIdAsync(id);
                if (existingDienNuoc == null)
                {
                    TempData["Error"] = "Không tìm thấy dữ liệu điện nước để xóa";
                    return RedirectToAction(nameof(Index));
                }

                await _dienNuocRepository.DeleteAsync(id);
                TempData["Success"] = $"Xóa dữ liệu điện nước {id} thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa dữ liệu: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // ------------- Helpers -------------
        private async Task LoadDropdownDataAsync()
        {
            try
            {
                var phongList = await _phongRepository.GetAllAsync();
                ViewBag.Phongs = new SelectList(phongList, "MaPhong", "MaPhong");
            }
            catch (Exception ex)
            {
                ViewBag.Phongs = new SelectList(Enumerable.Empty<SelectListItem>());
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tải dropdown: {ex.Message}");
            }
        }
    }
}