using DoAnCoSo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Danh sách người dùng
    public IActionResult Users()
    {
        var users = _userManager.Users.ToList();
        return View(users);
    }

    // GET: Edit Role
    public async Task<IActionResult> EditRole(string userId)
    {
        if (userId == null) return NotFound();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);
        ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
        ViewBag.UserRoles = userRoles;

        return View(user);
    }

    // POST: Edit Role
    [HttpPost]
    public async Task<IActionResult> EditRole(string userId, string role)
    {
        if (userId == null || role == null) return BadRequest();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        // Xóa hết roles cũ
        await _userManager.RemoveFromRolesAsync(user, roles);

        // Thêm role mới
        await _userManager.AddToRoleAsync(user, role);

        return RedirectToAction("Users");
    }
}
