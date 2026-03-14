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
    public class SuCoBaoTriController : Controller
    {
        private readonly ISuCoBaoTriRepository _suCoBaoTriRepository;
        private readonly IPhongRepository _phongRepository;
        private readonly IThietBiRepository _thietBiRepository;

        public SuCoBaoTriController(
            ISuCoBaoTriRepository suCoBaoTriRepository,
            IPhongRepository phongRepository,
            IThietBiRepository thietBiRepository)
        {
            _suCoBaoTriRepository = suCoBaoTriRepository ?? throw new ArgumentNullException(nameof(suCoBaoTriRepository));
            _phongRepository = phongRepository ?? throw new ArgumentNullException(nameof(phongRepository));
            _thietBiRepository = thietBiRepository ?? throw new ArgumentNullException(nameof(thietBiRepository));
        }

        // Hiển thị danh sách sự cố
        public async Task<IActionResult> Index(string searchString, string maPhong, string maThietBi, string tinhTrang, DateTime? tuNgay)
        {
            try
            {
                var suCos = await _suCoBaoTriRepository.GetAllAsync();

                // Lọc theo từ khóa tìm kiếm
                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    suCos = suCos.Where(s =>
                        s.MaSuCo.ToLower().Contains(searchString) ||
                        s.Phong?.MaPhong.ToLower().Contains(searchString) == true ||
                        s.ThietBi?.TenThietBi.ToLower().Contains(searchString) == true
                    );
                }

                // Lọc theo phòng
                if (!string.IsNullOrEmpty(maPhong))
                {
                    suCos = suCos.Where(s => s.MaPhong == maPhong);
                }

                // Lọc theo thiết bị
                if (!string.IsNullOrEmpty(maThietBi))
                {
                    suCos = suCos.Where(s => s.MaThietBi == maThietBi);
                }

                // Lọc theo tình trạng
                if (!string.IsNullOrEmpty(tinhTrang))
                {
                    suCos = suCos.Where(s => s.TinhTrang == tinhTrang);
                }

                // Lọc theo ngày
                if (tuNgay.HasValue)
                {
                    suCos = suCos.Where(s => s.NgayPhatHien >= tuNgay.Value);
                }

                // Lưu các giá trị tìm kiếm vào ViewBag để giữ lại trên form
                ViewBag.SearchString = searchString;
                ViewBag.MaPhong = maPhong;
                ViewBag.MaThietBi = maThietBi;
                ViewBag.TinhTrang = tinhTrang;
                ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");

                // Load dropdown data
                await LoadDropdownDataAsync();

                return View(suCos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải danh sách sự cố: " + ex.Message;
                return View(Enumerable.Empty<SuCoBaoTri>());
            }
        }

        // Hiển thị form thêm sự cố (GET)
        [Authorize(Roles = "Admin,SinhVien,NhanVien")]
        public async Task<IActionResult> Add()
        {
            try
            {
                await LoadDropdownDataAsync();

                // Tạo model mới với mã tự động
                var suCo = new SuCoBaoTri
                {
                    NgayPhatHien = DateTime.Today,
                    TinhTrang = "Chờ xử lý"
                };

                return View(suCo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form thêm sự cố: " + ex.Message;
                return View(new SuCoBaoTri());
            }
        }

        // Xử lý thêm sự cố (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SinhVien,NhanVien")]
        public async Task<IActionResult> Add(SuCoBaoTri suCo)
        {
            try
            {
                // TỰ ĐỘNG SINH MÃ SỰ CỐ
                suCo.MaSuCo = await _suCoBaoTriRepository.GenerateNextMaSuCoAsync();

                if (ModelState.IsValid)
                {
                    await _suCoBaoTriRepository.AddAsync(suCo);
                    TempData["Success"] = $"Thêm sự cố thành công. Mã sự cố: {suCo.MaSuCo}";
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
                return View(suCo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi thêm sự cố: " + ex.Message;
                await LoadDropdownDataAsync();
                return View(suCo);
            }
        }

        // API để lấy mã sự cố tự động
        [HttpGet]
        public async Task<JsonResult> GenerateMaSuCo()
        {
            try
            {
                string generatedCode = await _suCoBaoTriRepository.GenerateNextMaSuCoAsync();
                return Json(new { success = true, maSuCo = generatedCode });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // Hiển thị chi tiết sự cố
        public async Task<IActionResult> Display(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã sự cố không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var suCo = await _suCoBaoTriRepository.GetByIdAsync(id);
                if (suCo == null)
                {
                    TempData["Error"] = "Không tìm thấy sự cố";
                    return RedirectToAction(nameof(Index));
                }

                return View(suCo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải thông tin sự cố: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Hiển thị form cập nhật sự cố (GET)
        [Authorize(Roles = "Admin,SinhVien,NhanVien")]
        public async Task<IActionResult> Update(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã sự cố không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var suCo = await _suCoBaoTriRepository.GetByIdAsync(id);
                if (suCo == null)
                {
                    TempData["Error"] = "Không tìm thấy sự cố";
                    return RedirectToAction(nameof(Index));
                }

                await LoadDropdownDataAsync();
                return View(suCo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form cập nhật: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Xử lý cập nhật sự cố (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SinhVien,NhanVien")]
        public async Task<IActionResult> Update(SuCoBaoTri suCo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadDropdownDataAsync();
                    return View(suCo);
                }

                // Kiểm tra sự tồn tại của sự cố trước khi cập nhật
                var existingSuCo = await _suCoBaoTriRepository.GetByIdAsync(suCo.MaSuCo);
                if (existingSuCo == null)
                {
                    TempData["Error"] = "Không tìm thấy sự cố để cập nhật";
                    await LoadDropdownDataAsync();
                    return View(suCo);
                }

                await _suCoBaoTriRepository.UpdateAsync(suCo);
                TempData["Success"] = $"Cập nhật sự cố {suCo.MaSuCo} thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật sự cố: " + ex.Message;
                await LoadDropdownDataAsync();
                return View(suCo);
            }
        }

        // Hiển thị form xác nhận xóa sự cố (GET)
        [Authorize(Roles = "Admin,SinhVien,NhanVien")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã sự cố không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var suCo = await _suCoBaoTriRepository.GetByIdAsync(id);
                if (suCo == null)
                {
                    TempData["Error"] = "Không tìm thấy sự cố";
                    return RedirectToAction(nameof(Index));
                }

                return View(suCo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form xóa: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Xử lý xóa sự cố (POST) - SỬA TÊN ACTION
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SinhVien,NhanVien")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã sự cố không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra sự tồn tại trước khi xóa
                var existingSuCo = await _suCoBaoTriRepository.GetByIdAsync(id);
                if (existingSuCo == null)
                {
                    TempData["Error"] = "Không tìm thấy sự cố để xóa";
                    return RedirectToAction(nameof(Index));
                }

                await _suCoBaoTriRepository.DeleteAsync(id);
                TempData["Success"] = $"Xóa sự cố {id} thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa sự cố: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // ------------------ Private Helpers ------------------

        private async Task LoadDropdownDataAsync()
        {
            try
            {
                var phongList = await _phongRepository.GetAllAsync();
                var thietBiList = await _thietBiRepository.GetAllAsync();

                ViewBag.Phongs = new SelectList(phongList, "MaPhong", "MaPhong");
                ViewBag.ThietBis = new SelectList(thietBiList, "MaThietBi", "TenThietBi");
            }
            catch (Exception ex)
            {
                ViewBag.Phongs = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.ThietBis = new SelectList(Enumerable.Empty<SelectListItem>());
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tải dropdown: {ex.Message}");
            }
        }
    }
}