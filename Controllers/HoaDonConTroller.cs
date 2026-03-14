using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text;
using static iTextSharp.text.pdf.BaseFont;

namespace DoAnCoSo.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly IHoaDonRepository _hoaDonRepository;
        private readonly ISinhVienRepository _sinhVienRepository;
        private readonly INhanVienRepository _nhanVienRepository;
        private readonly IDangKyDichVuRepository _dangKyDichVuRepository;
        private readonly IDienNuocRepository _dienNuocRepository;

        public HoaDonController(
            IHoaDonRepository hoaDonRepository,
            ISinhVienRepository sinhVienRepository,
            INhanVienRepository nhanVienRepository,
            IDangKyDichVuRepository dangKyDichVuRepository,
            IDienNuocRepository dienNuocRepository)
        {
            _hoaDonRepository = hoaDonRepository;
            _sinhVienRepository = sinhVienRepository;
            _nhanVienRepository = nhanVienRepository;
            _dangKyDichVuRepository = dangKyDichVuRepository;
            _dienNuocRepository = dienNuocRepository;
        }

        public async Task<IActionResult> Index(string searchString, string loaiChiPhi, string trangThai, DateTime? tuNgay, DateTime? denNgay)
        {
            var hoaDons = await _hoaDonRepository.GetAllAsync();

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                hoaDons = hoaDons.Where(h => 
                    h.MaHoaDon.ToLower().Contains(searchString) ||
                    h.SinhVien?.HoTen.ToLower().Contains(searchString) == true ||
                    h.SinhVien?.MaSV.ToLower().Contains(searchString) == true ||
                    h.NhanVien?.HoTen.ToLower().Contains(searchString) == true ||
                    h.NhanVien?.MaNV.ToLower().Contains(searchString) == true
                );
            }

            // Lọc theo loại chi phí
            if (!string.IsNullOrEmpty(loaiChiPhi))
            {
                hoaDons = hoaDons.Where(h => h.LoaiChiPhi == loaiChiPhi);
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(trangThai))
            {
                hoaDons = hoaDons.Where(h => h.TrangThai == trangThai);
            }

            // Lọc theo khoảng thời gian
            if (tuNgay.HasValue)
            {
                hoaDons = hoaDons.Where(h => h.NgayLap >= tuNgay.Value);
            }
            if (denNgay.HasValue)
            {
                hoaDons = hoaDons.Where(h => h.NgayLap <= denNgay.Value.AddDays(1).AddSeconds(-1));
            }

            // Lưu các giá trị tìm kiếm vào ViewBag để giữ lại trên form
            ViewBag.SearchString = searchString;
            ViewBag.LoaiChiPhi = loaiChiPhi;
            ViewBag.TrangThai = trangThai;
            ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay?.ToString("yyyy-MM-dd");

            return View(hoaDons);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            await LoadDropdownDataAsync();
            // Lấy mã hóa đơn tiếp theo để hiển thị
            var nextMaHoaDon = await _hoaDonRepository.GetNextMaHoaDonAsync();
            ViewBag.NextMaHoaDon = nextMaHoaDon;
            
            // Khởi tạo model với mã hóa đơn để hiển thị trong form
            var hoaDon = new HoaDon
            {
                MaHoaDon = nextMaHoaDon
            };
            
            return View(hoaDon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddConfirmed(HoaDon hoaDon)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync();
                return View("Add", hoaDon);
            }

            try
            {
                // Validate required fields based on invoice type
                if (hoaDon.LoaiChiPhi == "HoaDonDienNuoc")
                {
                    if (string.IsNullOrEmpty(hoaDon.MaDN))
                    {
                        ModelState.AddModelError("MaDN", "Vui lòng chọn mã điện nước.");
                        await LoadDropdownDataAsync();
                        return View("Add", hoaDon);
                    }

                    // Check if DienNuoc exists and is not already associated with another invoice
                    var dienNuoc = await _dienNuocRepository.GetByIdAsync(hoaDon.MaDN);
                    if (dienNuoc == null)
                    {
                        ModelState.AddModelError("MaDN", "Mã điện nước không tồn tại.");
                        await LoadDropdownDataAsync();
                        return View("Add", hoaDon);
                    }

                    var existingDienNuocInvoice = await _hoaDonRepository.GetByMaDNAsync(hoaDon.MaDN);
                    if (existingDienNuocInvoice != null)
                    {
                        ModelState.AddModelError("MaDN", "Mã điện nước này đã được sử dụng cho một hóa đơn khác.");
                        await LoadDropdownDataAsync();
                        return View("Add", hoaDon);
                    }

                    hoaDon.TongTien = dienNuoc.TongTien;
                }
                else if (hoaDon.LoaiChiPhi == "HoaDonDichVu")
                {
                    if (string.IsNullOrEmpty(hoaDon.MaDKDV))
                    {
                        ModelState.AddModelError("MaDKDV", "Vui lòng chọn mã đăng ký dịch vụ.");
                        await LoadDropdownDataAsync();
                        return View("Add", hoaDon);
                    }

                    // Check if DangKyDichVu exists and is not already associated with another invoice
                    var dangKyDichVu = await _dangKyDichVuRepository.GetByIdAsync(hoaDon.MaDKDV);
                    if (dangKyDichVu == null)
                    {
                        ModelState.AddModelError("MaDKDV", "Mã đăng ký dịch vụ không tồn tại.");
                        await LoadDropdownDataAsync();
                        return View("Add", hoaDon);
                    }

                    var existingDKDVInvoice = await _hoaDonRepository.GetByMaDKDVAsync(hoaDon.MaDKDV);
                    if (existingDKDVInvoice != null)
                    {
                        ModelState.AddModelError("MaDKDV", "Mã đăng ký dịch vụ này đã được sử dụng cho một hóa đơn khác.");
                        await LoadDropdownDataAsync();
                        return View("Add", hoaDon);
                    }

                    hoaDon.TongTien = dangKyDichVu.TongTien;
                }

                // Ensure total amount is set
                if (hoaDon.TongTien <= 0)
                {
                    ModelState.AddModelError("TongTien", "Tổng tiền phải lớn hơn 0.");
                    await LoadDropdownDataAsync();
                    return View("Add", hoaDon);
                }

                await _hoaDonRepository.AddAsync(hoaDon);
                TempData["Success"] = "Thêm hóa đơn thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm hóa đơn: " + ex.Message);
                await LoadDropdownDataAsync();
                return View("Add", hoaDon);
            }
        }

        public async Task<IActionResult> Display(string id)
        {
            var hoaDon = await _hoaDonRepository.GetByIdAsync(id);
            if (hoaDon == null) return NotFound();
            return View(hoaDon);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id)
        {
            var hoaDon = await _hoaDonRepository.GetByIdAsync(id);
            if (hoaDon == null) return NotFound();

            await LoadDropdownDataAsync();
            return View(hoaDon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(HoaDon hoaDon)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync();
                return View(hoaDon);
            }

            try
            {
                // Get the existing invoice
                var existingHoaDon = await _hoaDonRepository.GetByIdAsync(hoaDon.MaHoaDon);
                if (existingHoaDon == null)
                {
                    return NotFound();
                }

                // Validate based on invoice type
                if (hoaDon.LoaiChiPhi == "HoaDonDienNuoc")
                {
                    if (string.IsNullOrEmpty(hoaDon.MaDN))
                    {
                        ModelState.AddModelError("MaDN", "Vui lòng chọn mã điện nước.");
                        await LoadDropdownDataAsync();
                        return View(hoaDon);
                    }

                    // Check if DienNuoc exists
                    var dienNuoc = await _dienNuocRepository.GetByIdAsync(hoaDon.MaDN);
                    if (dienNuoc == null)
                    {
                        ModelState.AddModelError("MaDN", "Mã điện nước không tồn tại.");
                        await LoadDropdownDataAsync();
                        return View(hoaDon);
                    }

                    // Check if DienNuoc is already used by another invoice
                    var existingDienNuocInvoice = await _hoaDonRepository.GetByMaDNAsync(hoaDon.MaDN);
                    if (existingDienNuocInvoice != null && existingDienNuocInvoice.MaHoaDon != hoaDon.MaHoaDon)
                    {
                        ModelState.AddModelError("MaDN", "Mã điện nước này đã được sử dụng cho một hóa đơn khác.");
                        await LoadDropdownDataAsync();
                        return View(hoaDon);
                    }

                    // Update invoice with utility information
                    existingHoaDon.MaDN = hoaDon.MaDN;
                    existingHoaDon.MaDKDV = null;
                    existingHoaDon.TongTien = dienNuoc.TongTien;
                }
                else if (hoaDon.LoaiChiPhi == "HoaDonDichVu")
                {
                    if (string.IsNullOrEmpty(hoaDon.MaDKDV))
                    {
                        ModelState.AddModelError("MaDKDV", "Vui lòng chọn mã đăng ký dịch vụ.");
                        await LoadDropdownDataAsync();
                        return View(hoaDon);
                    }

                    // Check if DangKyDichVu exists
                    var dangKyDichVu = await _dangKyDichVuRepository.GetByIdAsync(hoaDon.MaDKDV);
                    if (dangKyDichVu == null)
                    {
                        ModelState.AddModelError("MaDKDV", "Mã đăng ký dịch vụ không tồn tại.");
                        await LoadDropdownDataAsync();
                        return View(hoaDon);
                    }

                    // Check if DangKyDichVu is already used by another invoice
                    var existingDKDVInvoice = await _hoaDonRepository.GetByMaDKDVAsync(hoaDon.MaDKDV);
                    if (existingDKDVInvoice != null && existingDKDVInvoice.MaHoaDon != hoaDon.MaHoaDon)
                    {
                        ModelState.AddModelError("MaDKDV", "Mã đăng ký dịch vụ này đã được sử dụng cho một hóa đơn khác.");
                        await LoadDropdownDataAsync();
                        return View(hoaDon);
                    }

                    // Update invoice with service information
                    existingHoaDon.MaDKDV = hoaDon.MaDKDV;
                    existingHoaDon.MaDN = null;
                    existingHoaDon.TongTien = dangKyDichVu.TongTien;
                }
                else
                {
                    ModelState.AddModelError("LoaiChiPhi", "Loại chi phí không hợp lệ.");
                    await LoadDropdownDataAsync();
                    return View(hoaDon);
                }

                // Update common fields
                existingHoaDon.NgayLap = hoaDon.NgayLap;
                existingHoaDon.MaSV = hoaDon.MaSV;
                existingHoaDon.MaNV = hoaDon.MaNV;
                existingHoaDon.LoaiChiPhi = hoaDon.LoaiChiPhi;
                // Keep TrangThai as is unless explicitly changed by payment action

                await _hoaDonRepository.UpdateAsync(existingHoaDon);
                TempData["Success"] = "Cập nhật hóa đơn thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật hóa đơn: " + ex.Message);
                await LoadDropdownDataAsync();
                return View(hoaDon);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var hoaDon = await _hoaDonRepository.GetByIdAsync(id);
            if (hoaDon == null) return NotFound();
            return View(hoaDon);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _hoaDonRepository.DeleteAsync(id);
                TempData["Success"] = "Xóa hóa đơn thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi xóa hóa đơn: " + ex.Message);
                return View();
            }
        }

        // Hiển thị trang thanh toán (GET)
        public async Task<IActionResult> Pay(string id)
        {
            var hoaDon = await _hoaDonRepository.GetByIdAsync(id);
            if (hoaDon == null) return NotFound();

            // Check if already paid
            if (hoaDon.TrangThai == "Đã thanh toán")
            {
                TempData["Info"] = "Hóa đơn này đã được thanh toán.";
                return RedirectToAction(nameof(Display), new { id = hoaDon.MaHoaDon });
            }

            return View("~/Views/Payment/ThanhToan.cshtml", hoaDon);
        }

        // Xử lý thanh toán (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayConfirmed(string id)
        {
            var hoaDon = await _hoaDonRepository.GetByIdAsync(id);
            if (hoaDon == null) return NotFound();

            // Only update if not already paid
            if (hoaDon.TrangThai != "Đã thanh toán")
            {
                hoaDon.TrangThai = "Đã thanh toán";
                try
                {
                    await _hoaDonRepository.UpdateAsync(hoaDon);
                    TempData["Success"] = "Thanh toán hóa đơn thành công!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Lỗi khi cập nhật trạng thái thanh toán: {ex.Message}";
                }
            }
            else
            {
                TempData["Info"] = "Hóa đơn này đã được thanh toán trước đó.";
            }

            return RedirectToAction("OrderDetail", "Payment", new { id = hoaDon.MaHoaDon });
        }

        public async Task<IActionResult> ExportPdf(string id)
        {
            var hoaDon = await _hoaDonRepository.GetByIdAsync(id);
            if (hoaDon == null) return NotFound();

            // Get related data
            var sinhVien = await _sinhVienRepository.GetByIdAsync(hoaDon.MaSV);
            var nhanVien = await _nhanVienRepository.GetByIdAsync(hoaDon.MaNV);

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // Register a font that supports Vietnamese characters
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");
                if (!System.IO.File.Exists(fontPath))
                {
                    // Fallback to a generic font if ARIALUNI.TTF is not found, or provide instructions to user
                    // For now, I'll use a commonly available font and note this as a potential point of failure.
                    // In a real application, you might embed the font or instruct deployment.
                    // For simplicity, let's assume Arial is available and supports basic characters if Unicode isn't.
                    fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                    if (!System.IO.File.Exists(fontPath)){
                         TempData["Error"] = "Không tìm thấy font Arial hoặc Arial Unicode MS. Vui lòng cài đặt font này trên hệ thống.";
                         return RedirectToAction("Display", new { id = hoaDon.MaHoaDon });
                    }
                }

                BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                // Add title
                Font titleFont = new Font(bf, 18, Font.BOLD);
                Paragraph title = new Paragraph("HÓA ĐƠN", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;
                document.Add(title);

                // Add invoice details
                Font normalFont = new Font(bf, 12, Font.NORMAL);
                Font boldFont = new Font(bf, 12, Font.BOLD);

                PdfPTable table = new PdfPTable(2);
                table.WidthPercentage = 100;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 20f;

                // Add invoice information
                AddTableRow(table, "Mã hóa đơn:", hoaDon.MaHoaDon.ToString(), boldFont, normalFont);
                AddTableRow(table, "Ngày tạo:", hoaDon.NgayLap.ToString("dd/MM/yyyy"), boldFont, normalFont);
                AddTableRow(table, "Trạng thái:", hoaDon.TrangThai, boldFont, normalFont);
                AddTableRow(table, "Loại chi phí:", hoaDon.LoaiChiPhi, boldFont, normalFont);

                // Add student information
                if (sinhVien != null)
                {
                    AddTableRow(table, "Sinh viên:", sinhVien.HoTen, boldFont, normalFont);
                    AddTableRow(table, "Mã sinh viên:", sinhVien.MaSV, boldFont, normalFont);
                    AddTableRow(table, "Lớp:", sinhVien.Lop, boldFont, normalFont);
                }

                // Add staff information
                if (nhanVien != null)
                {
                    AddTableRow(table, "Nhân viên tạo:", nhanVien.HoTen, boldFont, normalFont);
                    AddTableRow(table, "Mã nhân viên:", nhanVien.MaNV, boldFont, normalFont);
                }

                // Add payment details
                AddTableRow(table, "Tổng tiền:", hoaDon.TongTien.ToString("N0") + " VNĐ", boldFont, normalFont);

                document.Add(table);

                // Add footer
                Paragraph footer = new Paragraph("Hóa đơn này được tạo tự động bởi hệ thống.", normalFont);
                footer.Alignment = Element.ALIGN_CENTER;
                footer.SpacingBefore = 30f;
                document.Add(footer);

                document.Close();
                writer.Close();

                byte[] bytes = ms.ToArray();
                return File(bytes, "application/pdf", $"HoaDon_{hoaDon.MaHoaDon}.pdf");
            }
        }

        private void AddTableRow(PdfPTable table, string label, string value, Font labelFont, Font valueFont)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label, labelFont));
            labelCell.Border = Rectangle.NO_BORDER;
            labelCell.Padding = 5f;

            PdfPCell valueCell = new PdfPCell(new Phrase(value, valueFont));
            valueCell.Border = Rectangle.NO_BORDER;
            valueCell.Padding = 5f;

            table.AddCell(labelCell);
            table.AddCell(valueCell);
        }

        // ------------------ Private Helper ------------------

        private async Task LoadDropdownDataAsync()
        {
            var sinhViens = await _sinhVienRepository.GetAllAsync();
            var nhanViens = await _nhanVienRepository.GetAllAsync();
            var dangKyDichVus = await _dangKyDichVuRepository.GetAllAsync();
            var dienNuocs = await _dienNuocRepository.GetAllAsync();

            ViewBag.SinhVienList = sinhViens;
            ViewBag.NhanVienList = nhanViens;
            ViewBag.DangKyDichVuList = dangKyDichVus;
            ViewBag.DienNuocList = dienNuocs;
        }
    }
}
