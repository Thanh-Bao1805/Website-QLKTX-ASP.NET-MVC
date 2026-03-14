using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoAnCoSo.Migrations
{
    /// <inheritdoc />
    public partial class database_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DichVus",
                columns: table => new
                {
                    MaDV = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenDichVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVus", x => x.MaDV);
                });

            migrationBuilder.CreateTable(
                name: "NoiQuys",
                columns: table => new
                {
                    MaNoiQuy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MucPhat = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoiQuys", x => x.MaNoiQuy);
                });

            migrationBuilder.CreateTable(
                name: "QuanTriViens",
                columns: table => new
                {
                    MaQTV = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuanTriViens", x => x.MaQTV);
                });

            migrationBuilder.CreateTable(
                name: "SinhViens",
                columns: table => new
                {
                    MaSV = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lop = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Khoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChiThuongTru = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayNhapKTX = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinhViens", x => x.MaSV);
                });

            migrationBuilder.CreateTable(
                name: "ThietBis",
                columns: table => new
                {
                    MaThietBi = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenThietBi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Loai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThietBis", x => x.MaThietBi);
                });

            migrationBuilder.CreateTable(
                name: "Toas",
                columns: table => new
                {
                    MaToa = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenToa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoTang = table.Column<int>(type: "int", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiSuDung = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Toas", x => x.MaToa);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChucVus",
                columns: table => new
                {
                    MaChucVu = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenChucVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChucVus", x => x.MaChucVu);
                    table.ForeignKey(
                        name: "FK_ChucVus_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SuKiens",
                columns: table => new
                {
                    MaSuKien = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenSuKien = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NgayToChuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiaDiem = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaQTV = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuKiens", x => x.MaSuKien);
                    table.ForeignKey(
                        name: "FK_SuKiens_QuanTriViens_MaQTV",
                        column: x => x.MaQTV,
                        principalTable: "QuanTriViens",
                        principalColumn: "MaQTV");
                });

            migrationBuilder.CreateTable(
                name: "ThongBaos",
                columns: table => new
                {
                    MaTB = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DoiTuong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaQTV = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaos", x => x.MaTB);
                    table.ForeignKey(
                        name: "FK_ThongBaos_QuanTriViens_MaQTV",
                        column: x => x.MaQTV,
                        principalTable: "QuanTriViens",
                        principalColumn: "MaQTV");
                });

            migrationBuilder.CreateTable(
                name: "DangKyDichVus",
                columns: table => new
                {
                    MaDKDV = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaSV = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MaDV = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyDichVus", x => x.MaDKDV);
                    table.ForeignKey(
                        name: "FK_DangKyDichVus_DichVus_MaDV",
                        column: x => x.MaDV,
                        principalTable: "DichVus",
                        principalColumn: "MaDV",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DangKyDichVus_SinhViens_MaSV",
                        column: x => x.MaSV,
                        principalTable: "SinhViens",
                        principalColumn: "MaSV",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhanHois",
                columns: table => new
                {
                    MaPhanHoi = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiGui = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NguoiNhan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DaDoc = table.Column<bool>(type: "bit", nullable: false),
                    LoaiTinNhan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuongDanAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SinhVienMaSV = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanHois", x => x.MaPhanHoi);
                    table.ForeignKey(
                        name: "FK_PhanHois_SinhViens_SinhVienMaSV",
                        column: x => x.SinhVienMaSV,
                        principalTable: "SinhViens",
                        principalColumn: "MaSV");
                });

            migrationBuilder.CreateTable(
                name: "ViPhams",
                columns: table => new
                {
                    MaViPham = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayViPham = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaSV = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaNoiQuy = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViPhams", x => x.MaViPham);
                    table.ForeignKey(
                        name: "FK_ViPhams_NoiQuys_MaNoiQuy",
                        column: x => x.MaNoiQuy,
                        principalTable: "NoiQuys",
                        principalColumn: "MaNoiQuy",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViPhams_SinhViens_MaSV",
                        column: x => x.MaSV,
                        principalTable: "SinhViens",
                        principalColumn: "MaSV",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Phongs",
                columns: table => new
                {
                    MaPhong = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SoGiuong = table.Column<int>(type: "int", nullable: false),
                    SoSinhVienToiDa = table.Column<int>(type: "int", nullable: false),
                    SoSinhVienHienTai = table.Column<int>(type: "int", nullable: false),
                    LoaiPhong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaPhong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TienCoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaToaID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phongs", x => x.MaPhong);
                    table.ForeignKey(
                        name: "FK_Phongs_Toas_MaToaID",
                        column: x => x.MaToaID,
                        principalTable: "Toas",
                        principalColumn: "MaToa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhanViens",
                columns: table => new
                {
                    MaNV = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaChucVu = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanViens", x => x.MaNV);
                    table.ForeignKey(
                        name: "FK_NhanViens_ChucVus_MaChucVu",
                        column: x => x.MaChucVu,
                        principalTable: "ChucVus",
                        principalColumn: "MaChucVu",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietThietBis",
                columns: table => new
                {
                    MaPhong = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    MaThietBi = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietThietBis", x => new { x.MaPhong, x.MaThietBi });
                    table.ForeignKey(
                        name: "FK_ChiTietThietBis_Phongs_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "Phongs",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietThietBis_ThietBis_MaThietBi",
                        column: x => x.MaThietBi,
                        principalTable: "ThietBis",
                        principalColumn: "MaThietBi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DangKyOs",
                columns: table => new
                {
                    MaDK = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MSSV = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lop = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Khoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChiThuongTru = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayBatDauO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThucO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaPhong = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyOs", x => x.MaDK);
                    table.ForeignKey(
                        name: "FK_DangKyOs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DangKyOs_Phongs_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "Phongs",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DienNuocs",
                columns: table => new
                {
                    MaDN = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NgayGhi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoDien = table.Column<int>(type: "int", nullable: false),
                    SoNuoc = table.Column<int>(type: "int", nullable: false),
                    DonGiaDien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DonGiaNuoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaPhong = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DienNuocs", x => x.MaDN);
                    table.ForeignKey(
                        name: "FK_DienNuocs_Phongs_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "Phongs",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HinhAnhPhongs",
                columns: table => new
                {
                    MaHinhAnh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiHinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaPhong = table.Column<string>(type: "nvarchar(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HinhAnhPhongs", x => x.MaHinhAnh);
                    table.ForeignKey(
                        name: "FK_HinhAnhPhongs_Phongs_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "Phongs",
                        principalColumn: "MaPhong");
                });

            migrationBuilder.CreateTable(
                name: "SuCoBaoTris",
                columns: table => new
                {
                    MaSuCo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NgayPhatHien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TinhTrang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaPhong = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    MaThietBi = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuCoBaoTris", x => x.MaSuCo);
                    table.ForeignKey(
                        name: "FK_SuCoBaoTris_Phongs_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "Phongs",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SuCoBaoTris_ThietBis_MaThietBi",
                        column: x => x.MaThietBi,
                        principalTable: "ThietBis",
                        principalColumn: "MaThietBi");
                });

            migrationBuilder.CreateTable(
                name: "HopDongs",
                columns: table => new
                {
                    MaHopDong = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaSV = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaPhong = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    MaNV = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NhanVienMaNV = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HopDongs", x => x.MaHopDong);
                    table.ForeignKey(
                        name: "FK_HopDongs_NhanViens_MaNV",
                        column: x => x.MaNV,
                        principalTable: "NhanViens",
                        principalColumn: "MaNV",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HopDongs_NhanViens_NhanVienMaNV",
                        column: x => x.NhanVienMaNV,
                        principalTable: "NhanViens",
                        principalColumn: "MaNV");
                    table.ForeignKey(
                        name: "FK_HopDongs_Phongs_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "Phongs",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HopDongs_SinhViens_MaSV",
                        column: x => x.MaSV,
                        principalTable: "SinhViens",
                        principalColumn: "MaSV",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichTrucNhanViens",
                columns: table => new
                {
                    MaLT = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NgayTruc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CaTruc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaNV = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichTrucNhanViens", x => x.MaLT);
                    table.ForeignKey(
                        name: "FK_LichTrucNhanViens_NhanViens_MaNV",
                        column: x => x.MaNV,
                        principalTable: "NhanViens",
                        principalColumn: "MaNV",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuDuyets",
                columns: table => new
                {
                    MaPhieu = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayDuyet = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaQTV = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaDK = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuDuyets", x => x.MaPhieu);
                    table.ForeignKey(
                        name: "FK_PhieuDuyets_DangKyOs_MaDK",
                        column: x => x.MaDK,
                        principalTable: "DangKyOs",
                        principalColumn: "MaDK",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhieuDuyets_QuanTriViens_MaQTV",
                        column: x => x.MaQTV,
                        principalTable: "QuanTriViens",
                        principalColumn: "MaQTV",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HoaDons",
                columns: table => new
                {
                    MaHoaDon = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayLap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoaiChiPhi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaSV = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MaNV = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MaDKDV = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MaDN = table.Column<string>(type: "nvarchar(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.MaHoaDon);
                    table.ForeignKey(
                        name: "FK_HoaDons_DangKyDichVus_MaDKDV",
                        column: x => x.MaDKDV,
                        principalTable: "DangKyDichVus",
                        principalColumn: "MaDKDV",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoaDons_DienNuocs_MaDN",
                        column: x => x.MaDN,
                        principalTable: "DienNuocs",
                        principalColumn: "MaDN");
                    table.ForeignKey(
                        name: "FK_HoaDons_NhanViens_MaNV",
                        column: x => x.MaNV,
                        principalTable: "NhanViens",
                        principalColumn: "MaNV",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoaDons_SinhViens_MaSV",
                        column: x => x.MaSV,
                        principalTable: "SinhViens",
                        principalColumn: "MaSV");
                });

            migrationBuilder.CreateTable(
                name: "HoaDonChiTiets",
                columns: table => new
                {
                    MaChiTietHoaDon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHoaDon = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenThe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GiamGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDonChiTiets", x => x.MaChiTietHoaDon);
                    table.ForeignKey(
                        name: "FK_HoaDonChiTiets_HoaDons_MaHoaDon",
                        column: x => x.MaHoaDon,
                        principalTable: "HoaDons",
                        principalColumn: "MaHoaDon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ChucVus",
                columns: new[] { "MaChucVu", "ApplicationUserId", "TenChucVu" },
                values: new object[,]
                {
                    { "1", null, "Nhân viên Kế toán" },
                    { "2", null, "Bảo vệ" },
                    { "3", null, "Thu ngân" }
                });

            migrationBuilder.InsertData(
                table: "NoiQuys",
                columns: new[] { "MaNoiQuy", "MucPhat", "NoiDung" },
                values: new object[,]
                {
                    { "NQ01", "200.000 VNĐ", "Không hút thuốc trong ký túc xá" },
                    { "NQ02", "150.000 VNĐ", "Không gây ồn ào sau 22h" },
                    { "NQ03", "300.000 VNĐ", "Không nuôi động vật trong phòng" },
                    { "NQ04", "250.000 VNĐ", "Không tự ý thay đổi thiết bị trong phòng" },
                    { "NQ05", "500.000 VNĐ", "Không mang chất cháy nổ vào ký túc xá" },
                    { "NQ06", "100.000 VNĐ", "Không xả rác bừa bãi" },
                    { "NQ07", "200.000 VNĐ", "Không tổ chức tiệc trong phòng" }
                });

            migrationBuilder.InsertData(
                table: "Toas",
                columns: new[] { "MaToa", "DiaChi", "LoaiSuDung", "SoTang", "TenToa" },
                values: new object[,]
                {
                    { "A", "Khu A", "Ký túc xá", 5, "Tòa A" },
                    { "B", "Khu B", "Ký túc xá", 4, "Tòa B" },
                    { "C", "Khu C", "Ký túc xá", 6, "Tòa C" },
                    { "D", "Khu D", "Ký túc xá", 3, "Tòa D" },
                    { "E", "Khu E", "Ký túc xá", 7, "Tòa E" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietThietBis_MaThietBi",
                table: "ChiTietThietBis",
                column: "MaThietBi");

            migrationBuilder.CreateIndex(
                name: "IX_ChucVus_ApplicationUserId",
                table: "ChucVus",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyDichVus_MaDV",
                table: "DangKyDichVus",
                column: "MaDV");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyDichVus_MaSV",
                table: "DangKyDichVus",
                column: "MaSV");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyOs_MaPhong",
                table: "DangKyOs",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyOs_UserId",
                table: "DangKyOs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DienNuocs_MaPhong",
                table: "DienNuocs",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhPhongs_MaPhong",
                table: "HinhAnhPhongs",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonChiTiets_MaHoaDon",
                table: "HoaDonChiTiets",
                column: "MaHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaDKDV",
                table: "HoaDons",
                column: "MaDKDV",
                unique: true,
                filter: "[MaDKDV] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaDN",
                table: "HoaDons",
                column: "MaDN",
                unique: true,
                filter: "[MaDN] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaNV",
                table: "HoaDons",
                column: "MaNV");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaSV",
                table: "HoaDons",
                column: "MaSV");

            migrationBuilder.CreateIndex(
                name: "IX_HopDongs_MaNV",
                table: "HopDongs",
                column: "MaNV");

            migrationBuilder.CreateIndex(
                name: "IX_HopDongs_MaPhong",
                table: "HopDongs",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_HopDongs_MaSV",
                table: "HopDongs",
                column: "MaSV");

            migrationBuilder.CreateIndex(
                name: "IX_HopDongs_NhanVienMaNV",
                table: "HopDongs",
                column: "NhanVienMaNV");

            migrationBuilder.CreateIndex(
                name: "IX_LichTrucNhanViens_MaNV",
                table: "LichTrucNhanViens",
                column: "MaNV");

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_MaChucVu",
                table: "NhanViens",
                column: "MaChucVu");

            migrationBuilder.CreateIndex(
                name: "IX_PhanHois_SinhVienMaSV",
                table: "PhanHois",
                column: "SinhVienMaSV");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuDuyets_MaDK",
                table: "PhieuDuyets",
                column: "MaDK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuDuyets_MaQTV",
                table: "PhieuDuyets",
                column: "MaQTV");

            migrationBuilder.CreateIndex(
                name: "IX_Phongs_MaToaID",
                table: "Phongs",
                column: "MaToaID");

            migrationBuilder.CreateIndex(
                name: "IX_SuCoBaoTris_MaPhong",
                table: "SuCoBaoTris",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_SuCoBaoTris_MaThietBi",
                table: "SuCoBaoTris",
                column: "MaThietBi");

            migrationBuilder.CreateIndex(
                name: "IX_SuKiens_MaQTV",
                table: "SuKiens",
                column: "MaQTV");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_MaQTV",
                table: "ThongBaos",
                column: "MaQTV");

            migrationBuilder.CreateIndex(
                name: "IX_ViPhams_MaNoiQuy",
                table: "ViPhams",
                column: "MaNoiQuy");

            migrationBuilder.CreateIndex(
                name: "IX_ViPhams_MaSV",
                table: "ViPhams",
                column: "MaSV");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ChiTietThietBis");

            migrationBuilder.DropTable(
                name: "HinhAnhPhongs");

            migrationBuilder.DropTable(
                name: "HoaDonChiTiets");

            migrationBuilder.DropTable(
                name: "HopDongs");

            migrationBuilder.DropTable(
                name: "LichTrucNhanViens");

            migrationBuilder.DropTable(
                name: "PhanHois");

            migrationBuilder.DropTable(
                name: "PhieuDuyets");

            migrationBuilder.DropTable(
                name: "SuCoBaoTris");

            migrationBuilder.DropTable(
                name: "SuKiens");

            migrationBuilder.DropTable(
                name: "ThongBaos");

            migrationBuilder.DropTable(
                name: "ViPhams");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropTable(
                name: "DangKyOs");

            migrationBuilder.DropTable(
                name: "ThietBis");

            migrationBuilder.DropTable(
                name: "QuanTriViens");

            migrationBuilder.DropTable(
                name: "NoiQuys");

            migrationBuilder.DropTable(
                name: "DangKyDichVus");

            migrationBuilder.DropTable(
                name: "DienNuocs");

            migrationBuilder.DropTable(
                name: "NhanViens");

            migrationBuilder.DropTable(
                name: "DichVus");

            migrationBuilder.DropTable(
                name: "SinhViens");

            migrationBuilder.DropTable(
                name: "Phongs");

            migrationBuilder.DropTable(
                name: "ChucVus");

            migrationBuilder.DropTable(
                name: "Toas");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
