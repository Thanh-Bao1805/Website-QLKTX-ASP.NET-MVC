using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCoSo.Controllers
{
	public class DichVuController : Controller
	{
		private readonly IDichVuRepository _dichVuRepository;

		public DichVuController(IDichVuRepository dichVuRepository)
		{
			_dichVuRepository = dichVuRepository;
		}

		// Hiển thị danh sách dịch vụ
		public async Task<IActionResult> Index()
		{
			var dichVus = await _dichVuRepository.GetAllAsync();
			return View(dichVus);
		}

		// Hiển thị form thêm dịch vụ (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
		{
            var nextCode = await GenerateNextMaDVAsync();
            return View(new DichVu
            {
                MaDV = nextCode
            });
		}

		// Xử lý thêm dịch vụ (POST) - Đã thêm xử lý upload hình ảnh
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Add(DichVu dichVu, IFormFile? HinhAnhFile)
		{
            ModelState.Remove(nameof(DichVu.MaDV));

            if (!ModelState.IsValid)
            {
                dichVu.MaDV = await GenerateNextMaDVAsync();
                return View(dichVu);
            }

			if (HinhAnhFile != null && HinhAnhFile.Length > 0)
			{
				var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "dichvu");
				if (!Directory.Exists(uploadsFolder))
					Directory.CreateDirectory(uploadsFolder);

				var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(HinhAnhFile.FileName);
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await HinhAnhFile.CopyToAsync(stream);
				}

				dichVu.HinhAnh = "/images/dichvu/" + uniqueFileName;
			}

			try
			{
                dichVu.MaDV = await GenerateNextMaDVAsync();
				await _dichVuRepository.AddAsync(dichVu);
				TempData["Success"] = "Thêm dịch vụ thành công.";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", "Lỗi khi thêm dịch vụ: " + ex.Message);
                dichVu.MaDV = await GenerateNextMaDVAsync();
				return View(dichVu);
			}
		}

		// Hiển thị chi tiết dịch vụ
		public async Task<IActionResult> Display(string id)
		{
			var dichVu = await _dichVuRepository.GetByIdAsync(id);
			if (dichVu == null) return NotFound();
			return View(dichVu);
		}

		// Hiển thị form cập nhật dịch vụ (GET)
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Update(string id)
		{
			var dichVu = await _dichVuRepository.GetByIdAsync(id);
			if (dichVu == null) return NotFound();
			return View(dichVu);
		}

		// Xử lý cập nhật dịch vụ (POST)
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Update(DichVu dichVu, IFormFile? HinhAnhFile)
		{
			if (!ModelState.IsValid)
				return View(dichVu);

			var existingDichVu = await _dichVuRepository.GetByIdAsync(dichVu.MaDV);
			if (existingDichVu == null)
				return NotFound();

			if (HinhAnhFile != null && HinhAnhFile.Length > 0)
			{
				var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "dichvu");
				if (!Directory.Exists(uploadsFolder))
					Directory.CreateDirectory(uploadsFolder);

				// Delete old image if exists
				if (!string.IsNullOrEmpty(existingDichVu.HinhAnh))
				{
					var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingDichVu.HinhAnh.TrimStart('/'));
					if (System.IO.File.Exists(oldImagePath))
					{
						System.IO.File.Delete(oldImagePath);
					}
				}

				var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(HinhAnhFile.FileName);
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await HinhAnhFile.CopyToAsync(stream);
				}

				dichVu.HinhAnh = "/images/dichvu/" + uniqueFileName;
			}
			else
			{
				// Keep existing image if no new image is uploaded
				dichVu.HinhAnh = existingDichVu.HinhAnh;
			}

			try
			{
				await _dichVuRepository.UpdateAsync(dichVu);
				TempData["Success"] = "Cập nhật dịch vụ thành công.";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", "Lỗi khi cập nhật dịch vụ: " + ex.Message);
				return View(dichVu);
			}
		}

		// Hiển thị form xác nhận xóa dịch vụ (GET)
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(string id)
		{
			var dichVu = await _dichVuRepository.GetByIdAsync(id);
			if (dichVu == null) return NotFound();
			return View(dichVu);
		}

		// Xử lý xóa dịch vụ (POST)
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				await _dichVuRepository.DeleteAsync(id);
				TempData["Success"] = "Xóa dịch vụ thành công.";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", "Lỗi khi xóa dịch vụ: " + ex.Message);
				return View();
			}
		}

        private async Task<string> GenerateNextMaDVAsync()
        {
            const string prefix = "DV";
            const int padding = 3;

            var codes = (await _dichVuRepository.GetAllAsync())
                .Select(d => d.MaDV)
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .ToList();

            var codeSet = codes.ToHashSet();

            var maxNumber = codes
                .Select(code =>
                {
                    if (!code.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        return 0;
                    }

                    var numericPart = code.Substring(prefix.Length);
                    return int.TryParse(numericPart, out var number) ? number : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            var candidate = maxNumber + 1;
            string newCode;

            do
            {
                newCode = $"{prefix}{candidate.ToString().PadLeft(padding, '0')}";
                candidate++;
            } while (codeSet.Contains(newCode));

            return newCode;
        }
	}
}