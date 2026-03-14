using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
	public class DangKyDichVuController : Controller
	{
		private readonly IDangKyDichVuRepository _dangKyDichVuRepository;
		private readonly ISinhVienRepository _sinhVienRepository;
		private readonly IDichVuRepository _dichVuRepository;

		public DangKyDichVuController(
			IDangKyDichVuRepository dangKyDichVuRepository,
			ISinhVienRepository sinhVienRepository,
			IDichVuRepository dichVuRepository)
		{
			_dangKyDichVuRepository = dangKyDichVuRepository;
			_sinhVienRepository = sinhVienRepository;
			_dichVuRepository = dichVuRepository;
		}

		// Hiển thị danh sách đăng ký dịch vụ
		public async Task<IActionResult> Index()
		{
			var dangKyDichVus = await _dangKyDichVuRepository.GetAllAsync();
			return View(dangKyDichVus);
		}

		// Hiển thị form thêm mới (GET)
		[Authorize(Roles = "Admin, SinhVien")]
		public async Task<IActionResult> Add(string maDV = null)
		{
			await LoadDropdownDataAsync();
			ViewBag.MaDKDV = await _dangKyDichVuRepository.GenerateNextMaDKDVAsync();
			
			if (!string.IsNullOrEmpty(maDV))
			{
				var dichVu = await _dichVuRepository.GetByIdAsync(maDV);
				if (dichVu != null)
				{
					ViewBag.SelectedDichVu = dichVu;
					return View(new DangKyDichVu { MaDV = maDV });
				}
			}
			
			return View();
		}

		// Xử lý thêm mới (POST)
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin , SinhVien")]
		public async Task<IActionResult> Add(DangKyDichVu dangKyDichVu)
		{
			if (!ModelState.IsValid)
			{
				await LoadDropdownDataAsync();
				return View(dangKyDichVu);
			}

			dangKyDichVu.MaDKDV = await _dangKyDichVuRepository.GenerateNextMaDKDVAsync();
			dangKyDichVu.NgayDangKy = DateTime.Now;

			// Calculate total amount
			var dichVu = await _dichVuRepository.GetByIdAsync(dangKyDichVu.MaDV);
			if (dichVu != null)
			{
				dangKyDichVu.TongTien = dangKyDichVu.SoLuong * dichVu.DonGia;
			}

			try
			{
				await _dangKyDichVuRepository.AddAsync(dangKyDichVu);
				TempData["Success"] = "Thêm đăng ký dịch vụ thành công.";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", "Lỗi khi thêm đăng ký dịch vụ: " + ex.Message);
				await LoadDropdownDataAsync();
				return View(dangKyDichVu);
			}
		}

		// Hiển thị chi tiết đăng ký
		public async Task<IActionResult> Display(string id)
		{
			var dangKy = await _dangKyDichVuRepository.GetByIdAsync(id);
			if (dangKy == null) return NotFound();
			return View(dangKy);
		}

		// Hiển thị form cập nhật (GET)
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Update(string id)
		{
			var dangKy = await _dangKyDichVuRepository.GetByIdAsync(id);
			if (dangKy == null) return NotFound();

			await LoadDropdownDataAsync();
			return View(dangKy);
		}

		// Xử lý cập nhật (POST)
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Update(DangKyDichVu dangKyDichVu)
		{
			if (!ModelState.IsValid)
			{
				await LoadDropdownDataAsync();
				return View(dangKyDichVu);
			}

			// Calculate total amount
			var dichVu = await _dichVuRepository.GetByIdAsync(dangKyDichVu.MaDV);
			if (dichVu != null)
			{
				dangKyDichVu.TongTien = dangKyDichVu.SoLuong * dichVu.DonGia;
			}

			try
			{
				await _dangKyDichVuRepository.UpdateAsync(dangKyDichVu);
				TempData["Success"] = "Cập nhật đăng ký dịch vụ thành công.";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", "Lỗi khi cập nhật đăng ký dịch vụ: " + ex.Message);
				await LoadDropdownDataAsync();
				return View(dangKyDichVu);
			}
		}

		// Hiển thị xác nhận xóa (GET)
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(string id)
		{
			var dangKy = await _dangKyDichVuRepository.GetByIdAsync(id);
			if (dangKy == null) return NotFound();
			return View(dangKy);
		}

		// Xử lý xóa (POST)
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				await _dangKyDichVuRepository.DeleteAsync(id);
				TempData["Success"] = "Xóa đăng ký dịch vụ thành công.";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Lỗi khi xóa đăng ký dịch vụ: " + ex.Message;
				return RedirectToAction(nameof(Index));
			}
		}

		// ------------------ Private Helpers ------------------

		private async Task LoadDropdownDataAsync()
		{
			var sinhViens = await _sinhVienRepository.GetAllAsync();
			var dichVus = await _dichVuRepository.GetAllAsync();

			ViewBag.SinhViens = new SelectList(sinhViens, "MaSV", "HoTen");
			ViewBag.DichVus = new SelectList(dichVus, "MaDV", "TenDichVu");
		}
	}
}
