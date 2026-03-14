using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
    [Authorize]
    public class PhanHoiController : Controller
    {
        private readonly IPhanHoiRepository _phanHoiRepository;

        public PhanHoiController(IPhanHoiRepository phanHoiRepository)
        {
            _phanHoiRepository = phanHoiRepository;
        }

        // Redirect theo role
        public IActionResult Index()
        {
            if (User.IsInRole("SinhVien"))
                return RedirectToAction("ChatWithAdmin");
            else if (User.IsInRole("Admin"))
                return RedirectToAction("AdminIndex");
            return RedirectToAction("ChatWithAdmin");
        }

        // ==================== SINH VIÊN ====================
        [Authorize(Roles = "SinhVien")]
        public async Task<IActionResult> ChatWithAdmin()
        {
            var userName = User.Identity.Name;
            var messages = await _phanHoiRepository.GetMessagesBetweenAsync(userName, "Admin");
            messages = messages.OrderBy(m => m.NgayGui).ToList();
            return View(messages);
        }

        // ==================== ADMIN ====================
        [Authorize(Roles = "Admin")]
       
        public async Task<IActionResult> AdminIndex()
        {
            var users = await _phanHoiRepository.GetUsersChattedWithAdminAsync();
            ViewBag.Users = users; // ✅ PHẢI CÓ DÒNG NÀY
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChatWithStudent(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("AdminIndex");

            var messages = await _phanHoiRepository.GetMessagesBetweenAsync("Admin", id);
            messages = messages.OrderBy(m => m.NgayGui).ToList();

            ViewBag.StudentId = id;
            return View(messages);
        }

        // ==================== API ====================
        [HttpGet]
        public async Task<JsonResult> GetChatHistory(string user1, string user2)
        {
            var messages = await _phanHoiRepository.GetMessagesBetweenAsync(user1, user2);
            messages = messages.OrderBy(m => m.NgayGui).ToList();

            var result = messages.Select(m => new {
                nguoiGui = m.NguoiGui,
                noiDung = m.NoiDung,
                ngayGui = m.NgayGui.ToString("HH:mm dd/MM/yyyy"),
                // ✅ THÊM 2 DÒNG NÀY
                loaiTinNhan = m.LoaiTinNhan,
                duongDanAnh = m.DuongDanAnh
            });

            return Json(result);
        }

        // ==================== GỬI TIN NHẮN (DỰ PHÒNG) ====================
        [HttpPost]
        [Authorize(Roles = "Admin,SinhVien")]
        public async Task<IActionResult> SendMessage(string content, string receiver)
        {
            if (string.IsNullOrEmpty(content))
            {
                TempData["Error"] = "Nội dung không được để trống";
                return RedirectToAction("Index");
            }

            var message = new PhanHoi
            {
                NoiDung = content,
                NguoiGui = User.Identity.Name,
                NguoiNhan = receiver
            };

            await _phanHoiRepository.AddAsync(message);

            if (User.IsInRole("Admin"))
                return RedirectToAction("ChatWithStudent", new { id = receiver });
            else
                return RedirectToAction("ChatWithAdmin");
        }
    }
}