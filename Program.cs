using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using DoAnCoSo.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Cấu hình DbContext
builder.Services.AddDbContext<QLKTXDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Identity + UI
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<QLKTXDbContext>();

// 🚀 ĐIỀU CHỈNH CẤU HÌNH COOKIE XÁC THỰC
builder.Services.ConfigureApplicationCookie(options =>
{
    // Đã thay đổi đường dẫn đăng nhập mặc định từ /Identity/Account/Login
    // sang trang gộp mới của bạn là /Identity/Account/Auth
    options.LoginPath = "/Identity/Account/Auth";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";

    // THÊM 2 DÒNG NÀY ĐỂ BẮT BUỘC ĐĂNG NHẬP
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.Redirect("/Identity/Account/Auth");
        return Task.CompletedTask;
    };
});

builder.Services.AddRazorPages();

// 🔹 THÊM CẤU HÌNH NÀY ĐỂ BẮT BUỘC ĐĂNG NHẬP CHO CONTROLLERS
builder.Services.AddControllersWithViews()
    .AddRazorPagesOptions(options =>
    {
        // Yêu cầu đăng nhập cho tất cả Razor Pages
        options.Conventions.AuthorizeFolder("/");

        // CHO PHÉP KHÔNG CẦN ĐĂNG NHẬP TRANG ĐĂNG NHẬP
        options.Conventions.AllowAnonymousToPage("/Identity/Account/Auth");
        options.Conventions.AllowAnonymousToPage("/Identity/Account/Login");
        options.Conventions.AllowAnonymousToPage("/Identity/Account/Register");
        options.Conventions.AllowAnonymousToPage("/Identity/Account/ForgotPassword");
        options.Conventions.AllowAnonymousToPage("/Identity/Account/ResetPassword");
        options.Conventions.AllowAnonymousToPage("/Identity/Account/AccessDenied");
    });

// 🔹 CẤU HÌNH GIỚI HẠN REQUEST SIZE CHO ẢNH LỚN
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 50 * 1024 * 1024; // 50MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBoundaryLengthLimit = int.MaxValue;
    options.MultipartHeadersCountLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// 🔹 CẤU HÌNH KESTREL CHO FILE LỚN
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50MB
});

// 🔹 THÊM CORS CHO SIGNALR
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true)
              .AllowCredentials();
    });
});

// 🔹 SIGNALR - CẤU HÌNH CHO MESSAGE LỚN
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 50 * 1024 * 1024; // 50MB
    options.StreamBufferCapacity = 1024;
});

// 🔹 ĐĂNG KÝ JSON OPTIONS CHO REQUEST LỚN
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// 🔹 Đăng ký tất cả Repository
builder.Services.AddScoped<ISinhVienRepository, SinhVienRepository>();
builder.Services.AddScoped<IChucVuRepository, ChucVuRepository>();
builder.Services.AddScoped<INhanVienRepository, NhanVienRepository>();
builder.Services.AddScoped<ILichTrucNhanVienRepository, LichTrucNhanVienRepository>();
builder.Services.AddScoped<IQuanTriVienRepository, QuanTriVienRepository>();
builder.Services.AddScoped<IThongBaoRepository, ThongBaoRepository>();
builder.Services.AddScoped<ISuKienRepository, SuKienRepository>();
builder.Services.AddScoped<IPhongRepository, PhongRepository>();
builder.Services.AddScoped<IToaRepository, ToaRepository>();
builder.Services.AddScoped<IThietBiRepository, ThietBiRepository>();
builder.Services.AddScoped<IChiTietThietBiRepository, ChiTietThietBiRepository>();
builder.Services.AddScoped<ISuCoBaoTriRepository, SuCoBaoTriRepository>();
builder.Services.AddScoped<IHopDongRepository, HopDongRepository>();
builder.Services.AddScoped<IHoaDonRepository, HoaDonRepository>();
builder.Services.AddScoped<IDichVuRepository, DichVuRepository>();
builder.Services.AddScoped<IDangKyDichVuRepository, DangKyDichVuRepository>();
builder.Services.AddScoped<IPhanHoiRepository, PhanHoiRepository>();
builder.Services.AddScoped<IViPhamRepository, ViPhamRepository>();
builder.Services.AddScoped<INoiQuyRepository, NoiQuyRepository>();
builder.Services.AddScoped<IDangKyORepository, DangKyORepository>();
builder.Services.AddScoped<IDienNuocRepository, DienNuocRepository>();
builder.Services.AddScoped<IPhieuDuyetRepository, PhieuDuyetRepository>();
builder.Services.AddScoped<IThongKeRepository, ThongKeRepository>();

var app = builder.Build();

// 🔹 Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// 🔹 CẤU HÌNH STATIC FILES CHO ẢNH LỚN
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    DefaultContentType = "application/octet-stream",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=3600");
    }
});

app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// 🔹 SIGNALR HUB - CẤU HÌNH ĐƯỜNG DẪN
app.MapHub<ChatHub>("/chatHub", options =>
{
    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets |
                         Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
});

// 🔹 Routes - THÊM CẤU HÌNH CHO CONTROLLERS
app.MapControllerRoute(
    name: "area",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .RequireAuthorization(); // THÊM DÒNG NÀY

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 🔹 THÊM HEALTH CHECK - CHO PHÉP TRUY CẬP KHÔNG CẦN ĐĂNG NHẬP
app.MapGet("/health", () => "API is running!");

// 🔹 CHO PHÉP TRUY CẬP TRANG LỖI KHÔNG CẦN ĐĂNG NHẬP
app.MapControllerRoute(
    name: "error",
    pattern: "/Home/Error");

// 🔹 🔹 TẠO ROLE VÀ TÀI KHOẢN ADMIN MẶC ĐỊNH
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = new[] { "Admin", "NhanVien", "SinhVien" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var adminEmail = "admin@ktx.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Admin KTX",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, "Admin123!");
        // TK: admin@ktx.com
        // MK: Admin123!
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

app.Run();




