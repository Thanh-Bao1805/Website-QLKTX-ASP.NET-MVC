// File: Areas/Identity/Pages/Account/Auth.cshtml.cs

#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using DoAnCoSo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

// Đảm bảo namespace này khớp với vị trí file của bạn
namespace DoAnCoSo.Areas.Identity.Pages.Account
{
    public class AuthModel : PageModel
    {
        // Khai báo Services: Gộp từ LoginModel và RegisterModel
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<AuthModel> _logger;
        private readonly IEmailSender _emailSender; // Thêm nếu bạn dùng

        public AuthModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AuthModel> logger,
            IEmailSender emailSender)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        // ------------------ PROPERTIES ------------------

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        [TempData] // THÊM TEMP DATA ĐỂ HIỂN THỊ THÔNG BÁO ĐĂNG KÝ THÀNH CÔNG
        public string StatusMessage { get; set; }

        // ------------------ INPUT MODEL GỘP ------------------

        public class InputModel
        {
            // Dùng chung cho cả Login và Register

            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            // GỠ BỎ [Required] Ở ĐÂY ĐỂ TRÁNH XUNG ĐỘT KHI ĐĂNG NHẬP (chỉ cần kiểm tra thủ công trong OnPostRegisterAsync)
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            // Chỉ dùng cho Register - GỠ BỎ [Required] Ở ĐÂY
            public string Fullname { get; set; }

            // Chỉ dùng cho Register - GỠ BỎ [Required] Ở ĐÂY
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            // Chỉ dùng cho Login (không cần Required)
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }

            // Dành cho Role (Register)
            public string? Role { get; set; }
            [ValidateNever]
            public IEnumerable<SelectListItem> RoleList { get; set; }
        }

        // ------------------ ON GET ASYNC (Init) ------------------

        public async Task OnGetAsync(string returnUrl = null)
        {
            // Logic khởi tạo RoleList từ RegisterModel
            if (!_roleManager.RoleExistsAsync(ChucVu.Role_SinhVien).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(ChucVu.Role_SinhVien)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(ChucVu.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(ChucVu.Role_NhanVien)).GetAwaiter().GetResult();
            }

            Input = new()
            {
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem()
                {
                    Text = i,
                    Value = i
                })
            };

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }

        // ------------------ ON POST HANDLERS ------------------

        // HÀM XỬ LÝ ĐĂNG NHẬP
        public async Task<IActionResult> OnPostLoginAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Bỏ qua Validation của các trường Register (Fullname, ConfirmPassword)
            ModelState.Remove("Input.Fullname");
            ModelState.Remove("Input.ConfirmPassword");

            // Kiểm tra thủ công các trường bắt buộc cho Login
            if (string.IsNullOrEmpty(Input.Email))
            {
                ModelState.AddModelError("Input.Email", "Vui lòng nhập Email.");
            }
            if (string.IsNullOrEmpty(Input.Password))
            {
                ModelState.AddModelError("Input.Password", "Vui lòng nhập Mật khẩu.");
            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            return Page();
        }

        // HÀM XỬ LÝ ĐĂNG KÝ
        public async Task<IActionResult> OnPostRegisterAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Bỏ qua Validation của các trường Login (RememberMe)
            ModelState.Remove("Input.RememberMe");

            // Kiểm tra thủ công các trường Register bị gỡ [Required]
            if (string.IsNullOrEmpty(Input.Fullname))
            {
                ModelState.AddModelError("Input.Fullname", "Vui lòng nhập Họ tên.");
            }
            if (string.IsNullOrEmpty(Input.Email))
            {
                ModelState.AddModelError("Input.Email", "Vui lòng nhập Email.");
            }
            if (string.IsNullOrEmpty(Input.Password))
            {
                ModelState.AddModelError("Input.Password", "Vui lòng nhập Mật khẩu.");
            }
            if (Input.Password != Input.ConfirmPassword)
            {
                ModelState.AddModelError("Input.ConfirmPassword", "Mật khẩu xác nhận không khớp.");
            }

            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.FullName = Input.Fullname;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Thêm Role
                    if (!String.IsNullOrEmpty(Input.Role))
                    {
                        await _userManager.AddToRoleAsync(user, Input.Role);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, ChucVu.Role_SinhVien);
                    }

                    // Logic gửi email xác nhận và chuyển hướng
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        // Nếu cần xác nhận email, chuyển hướng đến trang thông báo
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        // ĐÃ SỬA: BỎ TỰ ĐỘNG ĐĂNG NHẬP VÀ CHUYỂN HƯỚNG VỀ TRANG AUTH
                        // Bỏ dòng: await _signInManager.SignInAsync(user, isPersistent: false);

                        // Thiết lập thông báo thành công và chuyển hướng về trang Đăng nhập/Đăng ký
                        StatusMessage = "Đăng ký thành công! Vui lòng đăng nhập.";
                        return RedirectToPage("./Auth");
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        // ------------------ PRIVATE METHODS ------------------

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}