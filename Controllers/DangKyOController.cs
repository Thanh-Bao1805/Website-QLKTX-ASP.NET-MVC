using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DoAnCoSo.Controllers
{
    public class DangKyOController : Controller
    {
        private readonly IDangKyORepository _dangKyORepository;
        private readonly IPhongRepository _phongRepository;
        private readonly QLKTXDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DangKyOController(
            IDangKyORepository dangKyORepository,
            IPhongRepository phongRepository,
            QLKTXDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _dangKyORepository = dangKyORepository;
            _phongRepository = phongRepository;
            _context = context;
            _userManager = userManager;
        }

        // GET: DangKyO
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return Challenge(); // Redirect to login if user not found

            var currentUserId = currentUser.Id;
            if (string.IsNullOrEmpty(currentUserId))
                return Challenge();

            // Kiểm tra role của user
            var isAdmin = await _userManager.IsInRoleAsync(currentUser!, "Admin");
            var isNhanVien = await _userManager.IsInRoleAsync(currentUser!, "NhanVien");

            if (isAdmin || isNhanVien)
            {
                // Admin và nhân viên xem tất cả
                var danhSachDangKyO = await _context.DangKyOs
                    .Include(d => d.Phong!)
                    .ThenInclude(p => p!.Toa)
                    .Include(d => d.User) // Include thông tin user
                    .ToListAsync();
                return View(danhSachDangKyO);
            }
            else
            {
                // Sinh viên chỉ xem của mình
                var danhSachDangKyO = await _context.DangKyOs
                    .Where(d => d.UserId == currentUserId)
                    .Include(d => d.Phong!)
                    .ThenInclude(p => p!.Toa)
                    .ToListAsync();
                return View(danhSachDangKyO);
            }
        }

        // GET: DangKyO/Add
        [AllowAnonymous] // Cho phép đăng ký mà không cần đăng nhập
        public async Task<IActionResult> Add()
        {
            var phongList = await _phongRepository.GetAllAsync();
            ViewBag.PhongList = new SelectList(phongList, "MaPhong", "MaPhong");
            ViewBag.MaDK = await _dangKyORepository.GenerateNextMaDKAsync();
            return View();
        }

        // GET: DangKyO/AddWithRoom/PA001 - THÊM LẠI ACTION NÀY
        [AllowAnonymous] // Cho phép đăng ký mà không cần đăng nhập
        public async Task<IActionResult> AddWithRoom(string maPhong)
        {
            if (string.IsNullOrEmpty(maPhong))
            {
                return RedirectToAction("Index", "Home");
            }

            var phong = await _phongRepository.GetByIdAsync(maPhong);
            if (phong == null)
            {
                return NotFound();
            }

            ViewBag.MaPhong = maPhong;
            ViewBag.MaDK = await _dangKyORepository.GenerateNextMaDKAsync();

            // Lấy danh sách phòng cho dropdown (trong trường hợp muốn đổi phòng)
            var phongList = await _phongRepository.GetAllAsync();
            ViewBag.PhongList = new SelectList(phongList, "MaPhong", "MaPhong");

            return View("Add");
        }

        // POST: DangKyO/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous] // Cho phép đăng ký mà không cần đăng nhập
        public async Task<IActionResult> Add(DangKyO dangKyO)
        {
            // Kiểm tra ô xác nhận (không thay đổi model)
            var daXacNhan = Request.Form["DaXacNhan"].FirstOrDefault();
            bool daTick = string.Equals(daXacNhan, "true", StringComparison.OrdinalIgnoreCase) ||
                          string.Equals(daXacNhan, "on", StringComparison.OrdinalIgnoreCase);

            if (!daTick)
            {
                ModelState.AddModelError("DaXacNhan", "Bạn cần xác nhận đã đọc kỹ và chịu trách nhiệm về thông tin đã khai.");
            }

            // Lưu lại trạng thái checkbox để view hiển thị lại
            ViewBag.DaXacNhanChecked = daTick;

            if (string.IsNullOrEmpty(dangKyO.MaPhong))
            {
                ModelState.AddModelError(nameof(DangKyO.MaPhong), "Vui lòng chọn phòng.");
            }

            // Kiểm tra độ dài và định dạng SDT (10 số)
            var cleanedSdt = (dangKyO.SDT ?? string.Empty).Trim();
            dangKyO.SDT = cleanedSdt;
            if (string.IsNullOrWhiteSpace(cleanedSdt) ||
                cleanedSdt.Length != 10 ||
                !cleanedSdt.All(char.IsDigit))
            {
                ModelState.AddModelError(nameof(DangKyO.SDT), "Số điện thoại phải gồm đúng 10 chữ số.");
            }

            // Kiểm tra độ dài và định dạng CCCD (12 số)
            var cleanedCccd = (dangKyO.CCCD ?? string.Empty).Trim();
            dangKyO.CCCD = cleanedCccd;
            if (string.IsNullOrWhiteSpace(cleanedCccd) ||
                cleanedCccd.Length != 12 ||
                !cleanedCccd.All(char.IsDigit))
            {
                ModelState.AddModelError(nameof(DangKyO.CCCD), "CCCD phải gồm đúng 12 chữ số.");
            }

            if (!ModelState.IsValid)
            {
                var phongList = await _phongRepository.GetAllAsync();
                ViewBag.PhongList = new SelectList(phongList, "MaPhong", "MaPhong");
                ViewBag.MaDK = await _dangKyORepository.GenerateNextMaDKAsync();
                // Giữ lại MaPhong nếu đã được set từ AddWithRoom
                if (!string.IsNullOrEmpty(dangKyO.MaPhong))
                {
                    ViewBag.MaPhong = dangKyO.MaPhong;
                }
                return View(dangKyO);
            }

            // Kiểm tra trùng lặp MSSV
            if (!string.IsNullOrWhiteSpace(dangKyO.MSSV) &&
                await _dangKyORepository.ExistsByMSSVAsync(dangKyO.MSSV))
            {
                ModelState.AddModelError(nameof(DangKyO.MSSV), "MSSV đã tồn tại trong hệ thống. Vui lòng nhập MSSV khác.");
            }

            // Kiểm tra trùng lặp SDT
            if (!string.IsNullOrWhiteSpace(dangKyO.SDT) &&
                await _dangKyORepository.ExistsBySDTAsync(dangKyO.SDT))
            {
                ModelState.AddModelError(nameof(DangKyO.SDT), "Số điện thoại đã tồn tại trong hệ thống. Vui lòng nhập số điện thoại khác.");
            }

            // Kiểm tra trùng lặp Email
            if (!string.IsNullOrWhiteSpace(dangKyO.Email) &&
                await _dangKyORepository.ExistsByEmailAsync(dangKyO.Email))
            {
                ModelState.AddModelError(nameof(DangKyO.Email), "Email đã tồn tại trong hệ thống. Vui lòng nhập email khác.");
            }

            // Kiểm tra trùng lặp CCCD
            if (!string.IsNullOrWhiteSpace(dangKyO.CCCD) &&
                await _dangKyORepository.ExistsByCCCDAsync(dangKyO.CCCD))
            {
                ModelState.AddModelError(nameof(DangKyO.CCCD), "CCCD đã tồn tại trong hệ thống. Vui lòng nhập CCCD khác.");
            }

            // Nếu có lỗi trùng lặp, trả về view với thông báo lỗi
            if (!ModelState.IsValid)
            {
                var phongList = await _phongRepository.GetAllAsync();
                ViewBag.PhongList = new SelectList(phongList, "MaPhong", "MaPhong");
                ViewBag.MaDK = await _dangKyORepository.GenerateNextMaDKAsync();
                // Giữ lại MaPhong nếu đã được set từ AddWithRoom
                if (!string.IsNullOrEmpty(dangKyO.MaPhong))
                {
                    ViewBag.MaPhong = dangKyO.MaPhong;
                }
                return View(dangKyO);
            }

            try
            {
                var maPhongDangKy = dangKyO.MaPhong!;

                // Lấy thông tin user hiện tại
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    // Gán UserId cho đăng ký
                    dangKyO.UserId = currentUser.Id;
                }

                var existing = await _dangKyORepository.GetByIdAsync(dangKyO.MaDK);
                if (existing != null)
                {
                    ModelState.AddModelError("", "Mã đăng ký đã tồn tại.");
                    var phongList = await _phongRepository.GetAllAsync();
                    ViewBag.PhongList = new SelectList(phongList, "MaPhong", "MaPhong");
                    ViewBag.MaDK = await _dangKyORepository.GenerateNextMaDKAsync();
                    // Giữ lại MaPhong nếu đã được set từ AddWithRoom
                    if (!string.IsNullOrEmpty(dangKyO.MaPhong))
                    {
                        ViewBag.MaPhong = dangKyO.MaPhong;
                    }
                    return View(dangKyO);
                }

                // Lấy thông tin phòng
                var phong = await _phongRepository.GetByIdAsync(maPhongDangKy);
                if (phong == null)
                {
                    ModelState.AddModelError(nameof(DangKyO.MaPhong), "Không tìm thấy phòng.");
                    var phongList = await _phongRepository.GetAllAsync();
                    ViewBag.PhongList = new SelectList(phongList, "MaPhong", "MaPhong");
                    ViewBag.MaDK = await _dangKyORepository.GenerateNextMaDKAsync();
                    // Giữ lại MaPhong nếu đã được set từ AddWithRoom
                    if (!string.IsNullOrEmpty(dangKyO.MaPhong))
                    {
                        ViewBag.MaPhong = dangKyO.MaPhong;
                    }
                    return View(dangKyO);
                }

                // Ràng buộc giới tính: sinh viên chỉ được đăng ký phòng cùng giới tính
                if (!string.IsNullOrWhiteSpace(dangKyO.GioiTinh) &&
                    !string.IsNullOrWhiteSpace(phong.GioiTinhPhong) &&
                    !string.Equals(dangKyO.GioiTinh.Trim(), phong.GioiTinhPhong.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(nameof(DangKyO.MaPhong),
                        $"Giới tính phòng ({phong.GioiTinhPhong}) không phù hợp với giới tính sinh viên ({dangKyO.GioiTinh}). Vui lòng chọn phòng phù hợp.");

                    var phongList = await _phongRepository.GetAllAsync();
                    ViewBag.PhongList = new SelectList(phongList, "MaPhong", "MaPhong");
                    ViewBag.MaDK = await _dangKyORepository.GenerateNextMaDKAsync();
                    if (!string.IsNullOrEmpty(dangKyO.MaPhong))
                    {
                        ViewBag.MaPhong = dangKyO.MaPhong;
                    }
                    return View(dangKyO);
                }

                // Kiểm tra số lượng sinh viên trong phòng
                if (phong.SoSinhVienHienTai >= phong.SoSinhVienToiDa)
                {
                    ModelState.AddModelError("", "Phòng đã đầy, không thể đăng ký thêm.");
                    var phongList = await _phongRepository.GetAllAsync();
                    ViewBag.PhongList = new SelectList(phongList, "MaPhong", "MaPhong");
                    ViewBag.MaDK = await _dangKyORepository.GenerateNextMaDKAsync();
                    // Giữ lại MaPhong nếu đã được set từ AddWithRoom
                    if (!string.IsNullOrEmpty(dangKyO.MaPhong))
                    {
                        ViewBag.MaPhong = dangKyO.MaPhong;
                    }
                    return View(dangKyO);
                }

                // Tăng số sinh viên hiện tại
                phong.SoSinhVienHienTai += 1;

                // Cập nhật trạng thái phòng
                if (phong.SoSinhVienHienTai == phong.SoSinhVienToiDa)
                {
                    phong.TrangThai = "Đầy";
                }
                else if (phong.SoSinhVienHienTai < phong.SoSinhVienToiDa && phong.SoSinhVienHienTai > 0)
                {
                    phong.TrangThai = "Còn chỗ";
                }
                else
                {
                    phong.TrangThai = "Trống";
                }

                // Lưu đăng ký và cập nhật thông tin phòng
                await _dangKyORepository.AddAsync(dangKyO);
                await _phongRepository.UpdateAsync(phong);

                // Nếu chưa đăng nhập, redirect về trang chủ, nếu đã đăng nhập thì redirect về danh sách đăng ký
                if (User.Identity?.IsAuthenticated == true)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["SuccessMessage"] = "Đăng ký ở thành công! Vui lòng đăng nhập để xem thông tin đăng ký của bạn.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra khi thêm đăng ký: {ex.Message}");
                var phongList = await _phongRepository.GetAllAsync();
                ViewBag.PhongList = new SelectList(phongList, "MaPhong", "MaPhong");
                ViewBag.MaDK = await _dangKyORepository.GenerateNextMaDKAsync();
                // Giữ lại MaPhong nếu đã được set từ AddWithRoom
                if (!string.IsNullOrEmpty(dangKyO.MaPhong))
                {
                    ViewBag.MaPhong = dangKyO.MaPhong;
                }
                return View(dangKyO);
            }
        }

        // GET: DangKyO/Display/5
        [Authorize]
        public async Task<IActionResult> Display(string id)
        {
            if (id == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var dangKyO = await _context.DangKyOs
                .Include(d => d.Phong)
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.MaDK == id);

            if (dangKyO == null) return NotFound();

            // Kiểm tra quyền xem
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var isNhanVien = await _userManager.IsInRoleAsync(currentUser, "NhanVien");

            // Chỉ cho phép xem nếu là admin/nhân viên HOẶC là sinh viên sở hữu phiếu này
            if (!isAdmin && !isNhanVien && dangKyO.UserId != currentUser.Id)
            {
                return Forbid(); // Hoặc RedirectToAction("AccessDenied", "Account")
            }

            return View(dangKyO);
        }

        // GET: DangKyO/Update/5
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Update(string id)
        {
            if (id == null) return NotFound();

            var dangKyO = await _context.DangKyOs
                .Include(d => d.Phong)
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.MaDK == id);

            if (dangKyO == null) return NotFound();

            var phongList = await _phongRepository.GetAllAsync();
            ViewBag.PhongList = new SelectList(phongList, "MaPhong", "MaPhong", dangKyO.MaPhong);
            return View(dangKyO);
        }

        // POST: DangKyO/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Update(string id, DangKyO dangKyO)
        {
            if (id != dangKyO.MaDK) return NotFound();

            if (!ModelState.IsValid)
            {
                var phongList = await _phongRepository.GetAllAsync();
                ViewBag.PhongList = new SelectList(phongList, "MaPhong", "MaPhong", dangKyO.MaPhong);
                return View(dangKyO);
            }

            // Kiểm tra trùng lặp MSSV (trừ bản ghi hiện tại)
            if (await _dangKyORepository.ExistsByMSSVAsync(dangKyO.MSSV, dangKyO.MaDK))
            {
                ModelState.AddModelError(nameof(DangKyO.MSSV), "MSSV đã tồn tại trong hệ thống. Vui lòng nhập MSSV khác.");
            }

            // Kiểm tra trùng lặp SDT (trừ bản ghi hiện tại)
            if (await _dangKyORepository.ExistsBySDTAsync(dangKyO.SDT, dangKyO.MaDK))
            {
                ModelState.AddModelError(nameof(DangKyO.SDT), "Số điện thoại đã tồn tại trong hệ thống. Vui lòng nhập số điện thoại khác.");
            }

            // Kiểm tra trùng lặp Email (trừ bản ghi hiện tại)
            if (await _dangKyORepository.ExistsByEmailAsync(dangKyO.Email, dangKyO.MaDK))
            {
                ModelState.AddModelError(nameof(DangKyO.Email), "Email đã tồn tại trong hệ thống. Vui lòng nhập email khác.");
            }

            // Kiểm tra trùng lặp CCCD (trừ bản ghi hiện tại)
            if (await _dangKyORepository.ExistsByCCCDAsync(dangKyO.CCCD, dangKyO.MaDK))
            {
                ModelState.AddModelError(nameof(DangKyO.CCCD), "CCCD đã tồn tại trong hệ thống. Vui lòng nhập CCCD khác.");
            }

            // Nếu có lỗi trùng lặp, trả về view với thông báo lỗi
            if (!ModelState.IsValid)
            {
                var phongList = await _phongRepository.GetAllAsync();
                ViewBag.PhongList = new SelectList(phongList, "MaPhong", "MaPhong", dangKyO.MaPhong);
                return View(dangKyO);
            }

            try
            {
                // Lấy thông tin đăng ký cũ và theo dõi nó
                var oldDangKyO = await _context.DangKyOs
                    .Include(d => d.Phong)
                    .AsNoTracking()  // Prevent tracking of the old entity
                    .FirstOrDefaultAsync(d => d.MaDK == id);

                if (oldDangKyO == null)
                {
                    return NotFound();
                }

                // Nếu phòng thay đổi
                if (oldDangKyO.MaPhong != dangKyO.MaPhong)
                {
                    if (string.IsNullOrEmpty(oldDangKyO.MaPhong) || string.IsNullOrEmpty(dangKyO.MaPhong))
                    {
                        ModelState.AddModelError("", "Mã phòng không hợp lệ.");
                        var phongList = await _phongRepository.GetAllAsync();
                        ViewBag.PhongList = new SelectList(phongList, "MaPhong", "MaPhong", dangKyO.MaPhong);
                        return View(dangKyO);
                    }

                    // Giảm số sinh viên ở phòng cũ
                    var oldPhong = await _phongRepository.GetByIdAsync(oldDangKyO.MaPhong);
                    if (oldPhong != null)
                    {
                        oldPhong.SoSinhVienHienTai -= 1;
                        if (oldPhong.SoSinhVienHienTai == 0)
                            oldPhong.TrangThai = "Trống";
                        else if (oldPhong.SoSinhVienHienTai < oldPhong.SoSinhVienToiDa)
                            oldPhong.TrangThai = "Còn chỗ";
                        await _phongRepository.UpdateAsync(oldPhong);
                    }

                    // Tăng số sinh viên ở phòng mới
                    var newPhong = await _phongRepository.GetByIdAsync(dangKyO.MaPhong);
                    if (newPhong != null)
                    {
                        if (newPhong.SoSinhVienHienTai >= newPhong.SoSinhVienToiDa)
                        {
                            ModelState.AddModelError("", "Phòng mới đã đầy, không thể chuyển đến.");
                            var phongList = await _phongRepository.GetAllAsync();
                            ViewBag.PhongList = new SelectList(phongList, "MaPhong", "MaPhong", dangKyO.MaPhong);
                            return View(dangKyO);
                        }

                        newPhong.SoSinhVienHienTai += 1;
                        if (newPhong.SoSinhVienHienTai == newPhong.SoSinhVienToiDa)
                            newPhong.TrangThai = "Đầy";
                        else
                            newPhong.TrangThai = "Còn chỗ";
                        await _phongRepository.UpdateAsync(newPhong);
                    }
                }

                // Update the entity
                _context.Entry(dangKyO).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra khi cập nhật đăng ký: {ex.Message}");
                var phongList = await _phongRepository.GetAllAsync();
                ViewBag.PhongList = new SelectList(phongList, "MaPhong", "MaPhong", dangKyO.MaPhong);
                return View(dangKyO);
            }
        }

        // GET: DangKyO/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var dangKyO = await _context.DangKyOs
                .Include(d => d.Phong)
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.MaDK == id);

            if (dangKyO == null) return NotFound();

            return View(dangKyO);
        }

        // POST: DangKyO/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var dangKyO = await _context.DangKyOs
                    .Include(d => d.Phong)
                    .FirstOrDefaultAsync(d => d.MaDK == id);

                if (dangKyO != null)
                {
                    // Giảm số sinh viên trong phòng
                    if (!string.IsNullOrEmpty(dangKyO.MaPhong))
                    {
                        var phong = await _phongRepository.GetByIdAsync(dangKyO.MaPhong);
                        if (phong != null)
                        {
                            phong.SoSinhVienHienTai -= 1;
                            if (phong.SoSinhVienHienTai == 0)
                                phong.TrangThai = "Trống";
                            else if (phong.SoSinhVienHienTai < phong.SoSinhVienToiDa)
                                phong.TrangThai = "Còn chỗ";
                            await _phongRepository.UpdateAsync(phong);
                        }
                    }

                    await _dangKyORepository.DeleteAsync(id);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra khi xóa đăng ký: {ex.Message}");
                var dangKyO = await _dangKyORepository.GetByIdAsync(id);
                return View(dangKyO);
            }
        }
    }
}