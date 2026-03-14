// DoAnCoSo/Controllers/HopDongController.cs
using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    public class HopDongController : Controller
    {
        private readonly IHopDongRepository _hopDongRepository;
        private readonly ISinhVienRepository _sinhVienRepository;
        private readonly IPhongRepository _phongRepository;
        private readonly INhanVienRepository _nhanVienRepository;

        public HopDongController(
            IHopDongRepository hopDongRepository,
            ISinhVienRepository sinhVienRepository,
            IPhongRepository phongRepository,
            INhanVienRepository nhanVienRepository)
        {
            _hopDongRepository = hopDongRepository ?? throw new ArgumentNullException(nameof(hopDongRepository));
            _sinhVienRepository = sinhVienRepository ?? throw new ArgumentNullException(nameof(sinhVienRepository));
            _phongRepository = phongRepository ?? throw new ArgumentNullException(nameof(phongRepository));
            _nhanVienRepository = nhanVienRepository ?? throw new ArgumentNullException(nameof(nhanVienRepository));
        }

        // Hiển thị danh sách hợp đồng (PHÂN QUYỀN)
        public async Task<IActionResult> Index()
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                var currentUser = User.Identity;

                if (currentUser == null || !currentUser.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Kiểm tra vai trò của người dùng
                if (User.IsInRole("Admin") || User.IsInRole("NhanVien"))
                {
                    // Admin và nhân viên xem tất cả
                    var hopDongs = await _hopDongRepository.GetAllAsync();
                    return View(hopDongs);
                }
                else if (User.IsInRole("SinhVien"))
                {
                    // Sinh viên chỉ xem hợp đồng của mình
                    var maSV = GetCurrentSinhVienId(); // Lấy mã SV từ thông tin đăng nhập

                    if (string.IsNullOrEmpty(maSV))
                    {
                        TempData["Error"] = "Không tìm thấy thông tin sinh viên";
                        return View(Enumerable.Empty<HopDong>());
                    }

                    var hopDongs = await _hopDongRepository.GetBySinhVienIdAsync(maSV);
                    return View(hopDongs);
                }
                else
                {
                    TempData["Error"] = "Bạn không có quyền truy cập";
                    return RedirectToAction("AccessDenied", "Account");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải danh sách hợp đồng: " + ex.Message;
                return View(Enumerable.Empty<HopDong>());
            }
        }

        // Hiển thị chi tiết hợp đồng (PHÂN QUYỀN)
        public async Task<IActionResult> Display(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã hợp đồng không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var hopDong = await _hopDongRepository.GetByIdAsync(id);
                if (hopDong == null)
                {
                    TempData["Error"] = "Không tìm thấy hợp đồng";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra quyền truy cập
                if (!HasAccessToHopDong(hopDong))
                {
                    TempData["Error"] = "Bạn không có quyền xem hợp đồng này";
                    return RedirectToAction(nameof(Index));
                }

                return View(hopDong);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải thông tin hợp đồng: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Hiển thị form thêm hợp đồng
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Add()
        {
            try
            {
                await LoadDropdownDataAsync();

                // Gợi ý ngày mặc định để nhập nhanh
                return View(new HopDong
                {
                    NgayBatDau = DateTime.Today,
                    NgayKetThuc = DateTime.Today.AddMonths(6),
                    TrangThai = "Đang hoạt động"
                });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form thêm hợp đồng: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Xử lý thêm hợp đồng
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Add(HopDong hopDong)
        {
            try
            {
                if (hopDong.NgayKetThuc <= hopDong.NgayBatDau)
                {
                    ModelState.AddModelError(nameof(HopDong.NgayKetThuc), "Ngày kết thúc phải sau ngày bắt đầu");
                }

                if (!ModelState.IsValid)
                {
                    await LoadDropdownDataAsync();
                    return View(hopDong);
                }

                // Sinh mã nếu người dùng không nhập
                if (string.IsNullOrWhiteSpace(hopDong.MaHopDong))
                {
                    hopDong.MaHopDong = await _hopDongRepository.GenerateNextMaHopDongAsync();
                }

                await _hopDongRepository.AddAsync(hopDong);
                TempData["Success"] = "Thêm hợp đồng thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể thêm hợp đồng: " + ex.Message;
                await LoadDropdownDataAsync();
                return View(hopDong);
            }
        }

        // Hiển thị form cập nhật hợp đồng (PHÂN QUYỀN)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã hợp đồng không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var hopDong = await _hopDongRepository.GetByIdAsync(id);
                if (hopDong == null)
                {
                    TempData["Error"] = "Không tìm thấy hợp đồng";
                    return RedirectToAction(nameof(Index));
                }

                await LoadDropdownDataAsync();
                return View(hopDong);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form cập nhật: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Xử lý cập nhật hợp đồng
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, HopDong hopDong)
        {
            try
            {
                if (id != hopDong.MaHopDong)
                {
                    ModelState.AddModelError(string.Empty, "Mã hợp đồng không khớp");
                }

                if (hopDong.NgayKetThuc <= hopDong.NgayBatDau)
                {
                    ModelState.AddModelError(nameof(HopDong.NgayKetThuc), "Ngày kết thúc phải sau ngày bắt đầu");
                }

                if (!ModelState.IsValid)
                {
                    await LoadDropdownDataAsync();
                    return View(hopDong);
                }

                await _hopDongRepository.UpdateAsync(hopDong);
                TempData["Success"] = "Cập nhật hợp đồng thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể cập nhật hợp đồng: " + ex.Message;
                await LoadDropdownDataAsync();
                return View(hopDong);
            }
        }

        // Hiển thị xác nhận xóa
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Mã hợp đồng không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            var hopDong = await _hopDongRepository.GetByIdAsync(id);
            if (hopDong == null)
            {
                TempData["Error"] = "Không tìm thấy hợp đồng";
                return RedirectToAction(nameof(Index));
            }

            return View(hopDong);
        }

        // Xử lý xóa
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Mã hợp đồng không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                await _hopDongRepository.DeleteAsync(id);
                TempData["Success"] = "Đã xóa hợp đồng";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể xóa hợp đồng: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // API sinh mã hợp đồng tự động cho form Add
        [HttpGet]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> GenerateMaHopDong()
        {
            try
            {
                var ma = await _hopDongRepository.GenerateNextMaHopDongAsync();
                return Json(new { success = true, maHopDong = ma });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Không thể sinh mã: " + ex.Message });
            }
        }

        // ------------------ Private Helpers ------------------

        private async Task LoadDropdownDataAsync()
        {
            try
            {
                var sinhViens = await _sinhVienRepository.GetAllAsync();
                var phongList = await _phongRepository.GetAllAsync();
                var nhanViens = await _nhanVienRepository.GetAllAsync();

                ViewBag.SinhViens = new SelectList(sinhViens, "MaSV", "HoTen");
                ViewBag.Phongs = new SelectList(phongList, "MaPhong", "MaPhong");
                ViewBag.NhanViens = new SelectList(nhanViens, "MaNV", "HoTen");
            }
            catch (Exception ex)
            {
                ViewBag.SinhViens = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.Phongs = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.NhanViens = new SelectList(Enumerable.Empty<SelectListItem>());
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tải dropdown: {ex.Message}");
            }
        }

        // Phương thức lấy mã sinh viên hiện tại
        private string GetCurrentSinhVienId()
        {
            // Cách 1: Lấy từ Claim (nếu bạn lưu MaSV trong Claims khi đăng nhập)
            var maSVClaim = User.FindFirst("MaSV");
            if (maSVClaim != null)
            {
                return maSVClaim.Value;
            }

            // Cách 2: Lấy từ UserName nếu bạn dùng MaSV làm username
            if (User.Identity.Name.StartsWith("SV"))
            {
                return User.Identity.Name;
            }

            // Cách 3: Lấy từ email hoặc các thông tin khác
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            if (emailClaim != null)
            {
                // Cần thêm logic để lấy MaSV từ email
                // Ví dụ: gọi repository để tìm SV theo email
            }

            return null;
        }

        // Phương thức kiểm tra quyền truy cập hợp đồng
        private bool HasAccessToHopDong(HopDong hopDong)
        {
            if (hopDong == null) return false;

            // Admin và nhân viên có quyền xem tất cả
            if (User.IsInRole("Admin") || User.IsInRole("NhanVien"))
            {
                return true;
            }

            // Sinh viên chỉ xem được hợp đồng của mình
            if (User.IsInRole("SinhVien"))
            {
                var currentMaSV = GetCurrentSinhVienId();
                return hopDong.MaSV == currentMaSV;
            }

            return false;
        }
    }
}