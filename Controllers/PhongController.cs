using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace DoAnCoSo.Controllers
{
    public class PhongController : Controller
    {
        private readonly IPhongRepository _phongRepository;
        private readonly IToaRepository _toaRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PhongController(
            IPhongRepository phongRepository,
            IToaRepository toaRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _phongRepository = phongRepository ?? throw new ArgumentNullException(nameof(phongRepository));
            _toaRepository = toaRepository ?? throw new ArgumentNullException(nameof(toaRepository));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        private int? ResolveCapacityFromLoaiPhong(string? loaiPhong)
        {
            if (string.IsNullOrWhiteSpace(loaiPhong))
                return null;

            // Lấy chữ số đầu tiên trong chuỗi loại phòng (VD: "Phòng 4 người" -> 4)
            foreach (var ch in loaiPhong)
            {
                if (char.IsDigit(ch))
                {
                    return int.Parse(ch.ToString());
                }
            }
            return null;
        }

        private (decimal? giaPhong, decimal? tienCoc) ResolvePriceAndDeposit(string? loaiPhong)
        {
            var soNguoi = ResolveCapacityFromLoaiPhong(loaiPhong);
            if (!soNguoi.HasValue) return (null, null);

            decimal gia = soNguoi switch
            {
                2 => 500_000m,
                4 => 3_800_000m,
                6 => 2_800_000m,
                8 => 200_000m,
                _ => 0m
            };

            if (gia <= 0) return (null, null);

            decimal tienCoc = gia * 3; // Tiền cọc = 3 tháng
            return (gia, tienCoc);
        }

        // Hiển thị danh sách phòng
        public async Task<IActionResult> Index()
        {
            try
            {
                var phongs = await _phongRepository.GetAllAsync();
                return View(phongs);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải danh sách phòng: " + ex.Message;
                return View(new List<Phong>());
            }
        }

        // Hiển thị form thêm phòng (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            try
            {
                await LoadDropdownDataAsync();
                var phong = new Phong();
                return View(phong);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form thêm phòng: " + ex.Message;
                return View(new Phong());
            }
        }

        // Xử lý thêm phòng (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(Phong phong, IFormFile imageUrl)
        {
            try
            {
                var soToiDaFromLoai = ResolveCapacityFromLoaiPhong(phong.LoaiPhong);
                if (soToiDaFromLoai.HasValue)
                {
                    phong.SoSinhVienToiDa = soToiDaFromLoai.Value;
                }
                else
                {
                    ModelState.AddModelError(nameof(Phong.LoaiPhong), "Không xác định được số người tối đa từ loại phòng.");
                }

                var (giaPhong, tienCoc) = ResolvePriceAndDeposit(phong.LoaiPhong);
                if (giaPhong.HasValue && tienCoc.HasValue)
                {
                    phong.GiaPhong = giaPhong.Value;
                    phong.TienCoc = tienCoc.Value;
                }
                else
                {
                    ModelState.AddModelError(nameof(Phong.LoaiPhong), "Không xác định được giá phòng mặc định cho loại phòng này.");
                }

                if (phong.SoSinhVienHienTai > phong.SoSinhVienToiDa)
                {
                    ModelState.AddModelError(nameof(Phong.SoSinhVienHienTai), "Số SV hiện tại không được vượt quá số tối đa.");
                }

                // DEBUG: Kiểm tra dữ liệu nhận được
                Console.WriteLine($"MaPhong nhận được: {phong.MaPhong}");
                Console.WriteLine($"MaToaID nhận được: {phong.MaToaID}");

                if (ModelState.IsValid)
                {
                    // KIỂM TRA MÃ PHÒNG ĐÃ ĐƯỢC GỬI TỪ VIEW CHƯA
                    if (string.IsNullOrEmpty(phong.MaPhong))
                    {
                        // Nếu chưa có mã phòng, sinh mã mới
                        if (!string.IsNullOrEmpty(phong.MaToaID))
                        {
                            phong.MaPhong = await _phongRepository.GenerateNextRoomCodeAsync(phong.MaToaID);
                        }
                        else
                        {
                            ModelState.AddModelError("MaToaID", "Vui lòng chọn tòa nhà");
                            await LoadDropdownDataAsync();
                            TempData["Error"] = "Vui lòng chọn tòa nhà";
                            return View(phong);
                        }
                    }

                    // Xử lý upload ảnh
                    if (imageUrl != null && imageUrl.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "rooms");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageUrl.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageUrl.CopyToAsync(fileStream);
                        }

                        phong.ImageUrl = "/images/rooms/" + uniqueFileName;
                    }

                    // THÊM PHÒNG VÀO DATABASE
                    await _phongRepository.AddAsync(phong);

                    TempData["Success"] = $"Thêm phòng thành công. Mã phòng: {phong.MaPhong}";
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
                return View(phong);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi thêm phòng: " + ex.Message;
                await LoadDropdownDataAsync();
                return View(phong);
            }
        }

        // API để sinh mã phòng real-time
        [HttpGet]
        public async Task<JsonResult> GenerateRoomCode(string maToa)
        {
            try
            {
                if (string.IsNullOrEmpty(maToa))
                {
                    return Json(new { success = false, message = "Mã tòa không hợp lệ" });
                }

                string generatedCode = await _phongRepository.GenerateNextRoomCodeAsync(maToa);
                return Json(new { success = true, roomCode = generatedCode });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // Hiển thị chi tiết phòng
        public async Task<IActionResult> Display(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var phong = await _phongRepository.GetByIdAsync(id);
                if (phong == null) return NotFound();
                return View(phong);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải thông tin phòng: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Hiển thị form cập nhật phòng (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var phong = await _phongRepository.GetByIdAsync(id);
                if (phong == null) return NotFound();

                await LoadDropdownDataAsync();
                return View(phong);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form cập nhật: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Xử lý cập nhật phòng (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([Bind("MaPhong,SoGiuong,SoSinhVienToiDa,SoSinhVienHienTai,LoaiPhong,GioiTinhPhong,GiaPhong,TienCoc,TrangThai,MaToaID,ImageUrl")] Phong phong, IFormFile? imageFile)
        {
            try
            {
                var soToiDaFromLoai = ResolveCapacityFromLoaiPhong(phong.LoaiPhong);
                if (soToiDaFromLoai.HasValue)
                {
                    phong.SoSinhVienToiDa = soToiDaFromLoai.Value;
                }
                else
                {
                    ModelState.AddModelError(nameof(Phong.LoaiPhong), "Không xác định được số người tối đa từ loại phòng.");
                }

                var (giaPhong, tienCoc) = ResolvePriceAndDeposit(phong.LoaiPhong);
                if (giaPhong.HasValue && tienCoc.HasValue)
                {
                    phong.GiaPhong = giaPhong.Value;
                    phong.TienCoc = tienCoc.Value;
                }
                else
                {
                    ModelState.AddModelError(nameof(Phong.LoaiPhong), "Không xác định được giá phòng mặc định cho loại phòng này.");
                }

                if (phong.SoSinhVienHienTai > phong.SoSinhVienToiDa)
                {
                    ModelState.AddModelError(nameof(Phong.SoSinhVienHienTai), "Số SV hiện tại không được vượt quá số tối đa.");
                }

                if (!ModelState.IsValid)
                {
                    await LoadDropdownDataAsync();
                    return View(phong);
                }

                var existingPhong = await _phongRepository.GetByIdAsync(phong.MaPhong);
                if (existingPhong == null)
                    return NotFound();

                // Lưu lại đường dẫn ảnh cũ
                string oldImageUrl = existingPhong.ImageUrl ?? string.Empty;

                if (imageFile != null && imageFile.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(oldImageUrl))
                    {
                        string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                            oldImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    // Upload new image
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "rooms");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    phong.ImageUrl = "/images/rooms/" + uniqueFileName;
                }
                else
                {
                    // Keep existing image if no new image is uploaded
                    phong.ImageUrl = oldImageUrl;
                }

                await _phongRepository.UpdateAsync(phong);
                TempData["Success"] = "Cập nhật phòng thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật phòng: " + ex.Message);
                await LoadDropdownDataAsync();
                return View(phong);
            }
        }

        // Hiển thị form xác nhận xóa phòng (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var phong = await _phongRepository.GetByIdAsync(id);
                if (phong == null) return NotFound();
                return View(phong);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải form xóa: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Xử lý xóa phòng (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var phong = await _phongRepository.GetByIdAsync(id);
                if (phong != null)
                {
                    if (!string.IsNullOrEmpty(phong.ImageUrl))
                    {
                        string imagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                            phong.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                            System.IO.File.Delete(imagePath);
                    }

                    await _phongRepository.DeleteAsync(id);
                    TempData["Success"] = "Xóa phòng thành công.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa phòng: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // ------------------ Private Helper ------------------

        private async Task LoadDropdownDataAsync()
        {
            try
            {
                var toas = await _toaRepository.GetAllAsync();
                ViewBag.ToaList = new SelectList(toas, "MaToa", "TenToa");
            }
            catch (Exception ex)
            {
                ViewBag.ToaList = new SelectList(new List<SelectListItem>());
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tải dropdown: {ex.Message}");
            }
        }
    }
}
