using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DoAnCoSo.Models;

namespace DoAnCoSo.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly QLKTXDbContext _context;
		private const int PageSize = 12;

		public HomeController(ILogger<HomeController> logger, QLKTXDbContext context)
		{
			_logger = logger;
			_context = context;
		}

		[AllowAnonymous]
		public IActionResult Index(string searchTerm, string toaFilter, string genderFilter, string sortOrder, decimal? minPrice, decimal? maxPrice, int page = 1)
		{
			// Lấy danh sách tất cả tòa để hiển thị trong dropdown
			var allToas = _context.Toas.OrderBy(t => t.TenToa).ToList();
			ViewBag.AllToas = allToas;

			// Lấy danh sách phòng cơ bản
			var query = _context.Phongs
							  .Include(p => p.Toa)
							  .Where(p => p.TrangThai == "Trống" || p.TrangThai == "Còn chỗ");

			// Lọc theo từ khóa tìm kiếm (tìm theo mã phòng, tên tòa, loại phòng, giới tính phòng)
			if (!string.IsNullOrEmpty(searchTerm))
			{
				searchTerm = searchTerm.ToLower();
				query = query.Where(p => p.MaPhong.ToLower().Contains(searchTerm) ||
									   (p.Toa != null && p.Toa.TenToa.ToLower().Contains(searchTerm)) ||
									   p.LoaiPhong.ToLower().Contains(searchTerm) ||
									   p.GioiTinhPhong.ToLower().Contains(searchTerm));
				ViewBag.SearchTerm = searchTerm;
			}

			// Lọc theo tòa
			if (!string.IsNullOrEmpty(toaFilter))
			{
				query = query.Where(p => p.Toa != null && p.Toa.MaToa == toaFilter);
				ViewBag.ToaFilter = toaFilter;
			}

			// Lọc theo giới tính phòng (sửa từ LoaiPhong sang GioiTinhPhong)
			if (!string.IsNullOrEmpty(genderFilter))
			{
				query = query.Where(p => p.GioiTinhPhong == genderFilter);
				ViewBag.GenderFilter = genderFilter;
			}

			// Lọc theo khoảng giá
			if (minPrice.HasValue && minPrice.Value > 0)
			{
				query = query.Where(p => p.GiaPhong >= minPrice.Value);
				ViewBag.MinPrice = minPrice.Value;
			}

			if (maxPrice.HasValue && maxPrice.Value > 0)
			{
				query = query.Where(p => p.GiaPhong <= maxPrice.Value);
				ViewBag.MaxPrice = maxPrice.Value;
			}

			// Sắp xếp theo giá
			switch (sortOrder)
			{
				case "price_asc":
					query = query.OrderBy(p => p.GiaPhong);
					break;
				case "price_desc":
					query = query.OrderByDescending(p => p.GiaPhong);
					break;
				default:
					query = query.OrderBy(p => p.MaPhong);
					break;
			}
			ViewBag.SortOrder = sortOrder;

			// Tính toán phân trang
			var totalItems = query.Count();
			var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
			
			var phongs = query
				.Skip((page - 1) * PageSize)
				.Take(PageSize)
				.ToList();

			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = totalPages;
			ViewBag.HasNextPage = page < totalPages;
			ViewBag.TotalItems = totalItems;

			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			{
				return PartialView("_RoomList", phongs);
			}

			return View(phongs);
		}
		
		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		[AllowAnonymous]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
		
		[AllowAnonymous]
		public IActionResult DangKy(string maPhong)
		{
			return RedirectToAction("AddWithRoom", "DangKyO", new { maPhong });
		}
	}
}
