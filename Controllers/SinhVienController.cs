using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
	[Authorize]
	public class SinhVienController : Controller
	{
		private readonly ISinhVienRepository _sinhVienRepository;
		private readonly IChucVuRepository _chucVuRepository;

		public SinhVienController(
			ISinhVienRepository sinhVienRepository,
			IChucVuRepository chucVuRepository)
		{
			_sinhVienRepository = sinhVienRepository;
			_chucVuRepository = chucVuRepository;
		}

		// Hiển thị danh sách sinh viên
		public async Task<IActionResult> Index()
		{
			var danhSachSinhVien = await _sinhVienRepository.GetAllAsync();
			return View(danhSachSinhVien);
		}

		// Trang thêm sinh viên (GET)
		 [Authorize(Roles = "Admin,NhanVien")]
		public async Task<IActionResult> Add()
		{
			var chucVus = await _chucVuRepository.GetAllAsync();
			ViewBag.ChucVus = new SelectList(chucVus, "MaChucVu", "TenChucVu");
			return View();
		}

		// Xử lý thêm sinh viên (POST)
		[HttpPost]
		[ValidateAntiForgeryToken]
		 [Authorize(Roles = "Admin,NhanVien")]
		public async Task<IActionResult> Add(SinhVien sinhVien)
		{
			if (!ModelState.IsValid)
			{
				var chucVus = await _chucVuRepository.GetAllAsync();
				ViewBag.ChucVus = new SelectList(chucVus, "MaChucVu", "TenChucVu");
				return View(sinhVien);
			}

			try
			{
				await _sinhVienRepository.AddAsync(sinhVien);
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Có lỗi xảy ra khi thêm sinh viên: {ex.Message}");
				return View(sinhVien);
			}
		}

		// Xem chi tiết sinh viên
		public async Task<IActionResult> Display(string id)
		{
			var sinhVien = await _sinhVienRepository.GetByIdAsync(id);
			if (sinhVien == null)
			{
				return NotFound();
			}
			return View(sinhVien);
		}

		// Trang chỉnh sửa sinh viên (GET)
		 [Authorize(Roles = "Admin,NhanVien")]
		public async Task<IActionResult> Update(string id)
		{
			var sinhVien = await _sinhVienRepository.GetByIdAsync(id);
			if (sinhVien == null)
			{
				return NotFound();
			}

			var chucVus = await _chucVuRepository.GetAllAsync();
			ViewBag.ChucVus = new SelectList(chucVus, "MaChucVu", "TenChucVu");
			return View(sinhVien);
		}

		// Xử lý chỉnh sửa sinh viên (POST)
		[HttpPost]
		[ValidateAntiForgeryToken]
	 [Authorize(Roles = "Admin,NhanVien")]
		public async Task<IActionResult> Update(string id, SinhVien sinhVien)
		{
			if (id != sinhVien.MaSV)
			{
				return NotFound();
			}

			if (!ModelState.IsValid)
			{
				var chucVus = await _chucVuRepository.GetAllAsync();
				ViewBag.ChucVus = new SelectList(chucVus, "MaChucVu", "TenChucVu");
				return View(sinhVien);
			}

			try
			{
				await _sinhVienRepository.UpdateAsync(sinhVien);
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Có lỗi xảy ra khi cập nhật sinh viên: {ex.Message}");
				return View(sinhVien);
			}
		}

		// Trang xác nhận xóa sinh viên (GET)
		 [Authorize(Roles = "Admin,NhanVien")]
		public async Task<IActionResult> Delete(string id)
		{
			var sinhVien = await _sinhVienRepository.GetByIdAsync(id);
			if (sinhVien == null)
			{
				return NotFound();
			}
			return View(sinhVien);
		}

		// Xử lý xóa sinh viên (POST)
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		 [Authorize(Roles = "Admin,NhanVien")]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				var sinhVien = await _sinhVienRepository.GetByIdAsync(id);
				if (sinhVien == null)
				{
					return NotFound();
				}

				await _sinhVienRepository.DeleteAsync(id); // Xóa sinh viên từ repository
				return RedirectToAction(nameof(Index)); // Quay lại trang danh sách sinh viên
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Có lỗi xảy ra khi xóa sinh viên: {ex.Message}");
				var sinhVien = await _sinhVienRepository.GetByIdAsync(id);
				return View("Delete", sinhVien); // Quay lại trang Delete nếu có lỗi
			}
		
	}
	}
}
