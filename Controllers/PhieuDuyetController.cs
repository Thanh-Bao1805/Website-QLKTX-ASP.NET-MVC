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
    [Authorize(Roles = "Admin")]
    public class PhieuDuyetController : Controller
    {
        private readonly IPhieuDuyetRepository _phieuDuyetRepository;
        private readonly IQuanTriVienRepository _qtvRepository;
        private readonly IDangKyORepository _dangKyORepository;

        public PhieuDuyetController(
            IPhieuDuyetRepository phieuDuyetRepository,
            IQuanTriVienRepository qtvRepository,
            IDangKyORepository dangKyORepository)
        {
            _phieuDuyetRepository = phieuDuyetRepository ?? throw new ArgumentNullException(nameof(phieuDuyetRepository));
            _qtvRepository = qtvRepository ?? throw new ArgumentNullException(nameof(qtvRepository));
            _dangKyORepository = dangKyORepository ?? throw new ArgumentNullException(nameof(dangKyORepository));
        }

        // Hiển thị danh sách phiếu duyệt
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _phieuDuyetRepository.GetAllAsync();
                return View(list);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải danh sách phiếu duyệt: " + ex.Message;
                return View(Enumerable.Empty<PhieuDuyet>());
            }
        }

        // Hiển thị form thêm phiếu duyệt (GET)
        public async Task<IActionResult> Add()
        {
            try
            {
                await LoadDropdownDataAsync();

                // Tạo model mới với giá trị mặc định
                var phieu = new PhieuDuyet
                {
                    NgayDuyet = DateTime.Today,
                    TrangThai = "Chờ duyệt"
                };

                return View(phieu);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form thêm phiếu duyệt: " + ex.Message;
                return View(new PhieuDuyet());
            }
        }

        // Xử lý thêm phiếu duyệt (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(PhieuDuyet phieu)
        {
            try
            {
                // TỰ ĐỘNG SINH MÃ PHIẾU
                phieu.MaPhieu = await _phieuDuyetRepository.GenerateNextMaPhieuAsync();

                if (ModelState.IsValid)
                {
                    await _phieuDuyetRepository.AddAsync(phieu);
                    TempData["Success"] = $"Thêm phiếu duyệt thành công. Mã phiếu: {phieu.MaPhieu}";
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
                return View(phieu);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi thêm phiếu duyệt: " + ex.Message;
                await LoadDropdownDataAsync();
                return View(phieu);
            }
        }

        // API để lấy mã phiếu tự động
        [HttpGet]
        public async Task<JsonResult> GenerateMaPhieu()
        {
            try
            {
                string generatedCode = await _phieuDuyetRepository.GenerateNextMaPhieuAsync();
                return Json(new { success = true, maPhieu = generatedCode });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // Hiển thị chi tiết phiếu duyệt
        public async Task<IActionResult> Display(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã phiếu không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var phieu = await _phieuDuyetRepository.GetByIdAsync(id);
                if (phieu == null)
                {
                    TempData["Error"] = "Không tìm thấy phiếu duyệt";
                    return RedirectToAction(nameof(Index));
                }

                return View(phieu);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải thông tin phiếu duyệt: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Hiển thị form cập nhật phiếu duyệt (GET)
        public async Task<IActionResult> Update(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã phiếu không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var phieu = await _phieuDuyetRepository.GetByIdAsync(id);
                if (phieu == null)
                {
                    TempData["Error"] = "Không tìm thấy phiếu duyệt";
                    return RedirectToAction(nameof(Index));
                }

                await LoadDropdownDataAsync();
                return View(phieu);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form cập nhật: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Xử lý cập nhật phiếu duyệt (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PhieuDuyet phieu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadDropdownDataAsync();
                    return View(phieu);
                }

                // Kiểm tra sự tồn tại trước khi cập nhật
                var existingPhieu = await _phieuDuyetRepository.GetByIdAsync(phieu.MaPhieu);
                if (existingPhieu == null)
                {
                    TempData["Error"] = "Không tìm thấy phiếu duyệt để cập nhật";
                    await LoadDropdownDataAsync();
                    return View(phieu);
                }

                await _phieuDuyetRepository.UpdateAsync(phieu);
                TempData["Success"] = $"Cập nhật phiếu duyệt {phieu.MaPhieu} thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật phiếu duyệt: " + ex.Message;
                await LoadDropdownDataAsync();
                return View(phieu);
            }
        }

        // Hiển thị form xác nhận xóa phiếu duyệt (GET)
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã phiếu không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var phieu = await _phieuDuyetRepository.GetByIdAsync(id);
                if (phieu == null)
                {
                    TempData["Error"] = "Không tìm thấy phiếu duyệt";
                    return RedirectToAction(nameof(Index));
                }

                return View(phieu);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form xóa: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Xử lý xóa phiếu duyệt (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã phiếu không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra sự tồn tại trước khi xóa
                var existingPhieu = await _phieuDuyetRepository.GetByIdAsync(id);
                if (existingPhieu == null)
                {
                    TempData["Error"] = "Không tìm thấy phiếu duyệt để xóa";
                    return RedirectToAction(nameof(Index));
                }

                await _phieuDuyetRepository.DeleteAsync(id);
                TempData["Success"] = $"Xóa phiếu duyệt {id} thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa phiếu duyệt: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // ------------------ Private Helpers ------------------

        private async Task LoadDropdownDataAsync()
        {
            try
            {
                var qtvList = await _qtvRepository.GetAllAsync();
                var dkList = await _dangKyORepository.GetAllAsync();

                ViewBag.QuanTriViens = new SelectList(qtvList, "MaQTV", "HoTen");
                ViewBag.DangKyOs = new SelectList(dkList, "MaDK", "MaDK");
            }
            catch (Exception ex)
            {
                ViewBag.QuanTriViens = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.DangKyOs = new SelectList(Enumerable.Empty<SelectListItem>());
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tải dropdown: {ex.Message}");
            }
        }
    }
}