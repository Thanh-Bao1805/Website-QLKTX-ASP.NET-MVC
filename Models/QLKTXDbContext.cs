using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoAnCoSo.Models
{
	public class QLKTXDbContext : IdentityDbContext<ApplicationUser>
	{
		public QLKTXDbContext(DbContextOptions<QLKTXDbContext> options) : base(options)
		{
		}

		// DbSet cho các model
		public DbSet<NhanVien> NhanViens { get; set; }
		public DbSet<ChucVu> ChucVus { get; set; }
		public DbSet<LichTrucNhanVien> LichTrucNhanViens { get; set; }
		public DbSet<HopDong> HopDongs { get; set; }
		public DbSet<HoaDon> HoaDons { get; set; }
		public DbSet<HoaDonChiTiet> HoaDonChiTiets { get; set; }
		public DbSet<Phong> Phongs { get; set; }
		public DbSet<Toa> Toas { get; set; }
		public DbSet<SinhVien> SinhViens { get; set; }
		public DbSet<PhieuDuyet> PhieuDuyets { get; set; }
		public DbSet<PhanHoi> PhanHois { get; set; }
		public DbSet<DangKyDichVu> DangKyDichVus { get; set; }
		public DbSet<DichVu> DichVus { get; set; }
		public DbSet<NoiQuy> NoiQuys { get; set; }
		public DbSet<ViPham> ViPhams { get; set; }
		public DbSet<ThietBi> ThietBis { get; set; }
		public DbSet<ChiTietThietBi> ChiTietThietBis { get; set; }
		public DbSet<SuCoBaoTri> SuCoBaoTris { get; set; }
		public DbSet<SuKien> SuKiens { get; set; }
		public DbSet<ThongBao> ThongBaos { get; set; }
		public DbSet<QuanTriVien> QuanTriViens { get; set; }
		public DbSet<DangKyO> DangKyOs { get; set; }
		public DbSet<DienNuoc> DienNuocs { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder); // Quan trọng với IdentityDbContext

			// Cấu hình Fluent API cho các bảng
			ConfigureNhanVien(modelBuilder);
			ConfigureChucVu(modelBuilder);
			ConfigureHopDong(modelBuilder);
			ConfigurePhong(modelBuilder);
			ConfigureSinhVien(modelBuilder);
			ConfigurePhieuDuyet(modelBuilder);
			ConfigureDangKyO(modelBuilder);
			ConfigureDangKyDichVu(modelBuilder);
			ConfigureViPham(modelBuilder);
			ConfigureChiTietThietBi(modelBuilder);
			ConfigureSuCoBaoTri(modelBuilder);

			// Seed dữ liệu
			SeedData(modelBuilder);
		}

		private void ConfigureNhanVien(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<NhanVien>()
				.HasOne(nv => nv.chucVu)
				.WithMany(cv => cv.NhanViens)
				.HasForeignKey(nv => nv.MaChucVu)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<NhanVien>()
				.HasMany(nv => nv.LichTrucs)
				.WithOne(lt => lt.NhanVien)
				.HasForeignKey(lt => lt.MaNV)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<NhanVien>()
				.HasMany(nv => nv.HopDongs)
				.WithOne(hd => hd.NhanVien)
				.HasForeignKey(hd => hd.MaNV)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<NhanVien>()
				.HasMany(nv => nv.HoaDons)
				.WithOne(hd => hd.NhanVien)
				.HasForeignKey(hd => hd.MaNV)
				.OnDelete(DeleteBehavior.Restrict);
		}

		private void ConfigureChucVu(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ChucVu>()
				.HasMany(cv => cv.NhanViens)
				.WithOne(nv => nv.chucVu)
				.HasForeignKey(nv => nv.MaChucVu)
				.OnDelete(DeleteBehavior.Restrict);
		}

		private void ConfigureHopDong(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<HopDong>()
				.HasOne(hd => hd.SinhVien)
				.WithMany(sv => sv.HopDongs)
				.HasForeignKey(hd => hd.MaSV)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<HopDong>()
				.HasOne(hd => hd.Phong)
				.WithMany(p => p.HopDongs)
				.HasForeignKey(hd => hd.MaPhong)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<HopDong>()
	  .HasOne(h => h.NhanVien)
	  .WithMany()
	  .HasForeignKey(h => h.MaNV)
	  .OnDelete(DeleteBehavior.Restrict);


		}

		private void ConfigurePhong(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Phong>()
				.HasOne(p => p.Toa)
				.WithMany(t => t.Phongs)
				.HasForeignKey(p => p.MaToaID)
				.OnDelete(DeleteBehavior.Cascade);
		}

		private void ConfigureSinhVien(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<SinhVien>()
				.HasMany(sv => sv.HopDongs)
				.WithOne(hd => hd.SinhVien)
				.HasForeignKey(hd => hd.MaSV)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<SinhVien>()
				.HasMany(sv => sv.DangKyDichVus)
				.WithOne(dkdv => dkdv.SinhVien)
				.HasForeignKey(dkdv => dkdv.MaSV)
				.OnDelete(DeleteBehavior.Cascade);
		}

		private void ConfigurePhieuDuyet(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<PhieuDuyet>()
				.HasKey(pd => pd.MaPhieu);

			modelBuilder.Entity<PhieuDuyet>()
				.HasOne(pd => pd.QuanTriVien)
				.WithMany(qtv => qtv.PhieuDuyets)
				.HasForeignKey(pd => pd.MaQTV)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<PhieuDuyet>()
				.HasOne(pd => pd.DangKyO)
				.WithOne()
				.HasForeignKey<PhieuDuyet>(pd => pd.MaDK)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void ConfigureDangKyO(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<DangKyO>()
				.HasKey(dko => dko.MaDK);

			modelBuilder.Entity<DangKyO>()
				.HasOne(dko => dko.Phong)
				.WithMany(p => p.DangKyOs)
				.HasForeignKey(dko => dko.MaPhong)
				.OnDelete(DeleteBehavior.Cascade);

		
		}


		private void ConfigureDangKyDichVu(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<DangKyDichVu>()
				.HasOne(dkdv => dkdv.SinhVien)
				.WithMany(sv => sv.DangKyDichVus)
				.HasForeignKey(dkdv => dkdv.MaSV)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<DangKyDichVu>()
				.HasOne(dkdv => dkdv.DichVu)
				.WithMany(dv => dv.DangKyDichVus)
				.HasForeignKey(dkdv => dkdv.MaDV)
				.OnDelete(DeleteBehavior.Cascade);


			modelBuilder.Entity<DangKyDichVu>()
		 .HasOne(dkdv => dkdv.HoaDon)
		 .WithOne(hd => hd.DangKyDichVu)
		 .HasForeignKey<HoaDon>(hd => hd.MaDKDV)
		 .OnDelete(DeleteBehavior.Cascade);
		}

		private void ConfigureViPham(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ViPham>()
				.HasOne(vp => vp.NoiQuy)
				.WithMany(nq => nq.ViPhams)
				.HasForeignKey(vp => vp.MaNoiQuy)
				.OnDelete(DeleteBehavior.Cascade);
		}

		private void ConfigureChiTietThietBi(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ChiTietThietBi>()
				.HasKey(cttb => new { cttb.MaPhong, cttb.MaThietBi });

			modelBuilder.Entity<ChiTietThietBi>()
				.HasOne(cttb => cttb.Phong)
				.WithMany(p => p.ChiTietThietBis)
				.HasForeignKey(cttb => cttb.MaPhong)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<ChiTietThietBi>()
				.HasOne(cttb => cttb.ThietBi)
				.WithMany(tb => tb.ChiTietThietBis)
				.HasForeignKey(cttb => cttb.MaThietBi)
				.OnDelete(DeleteBehavior.Cascade);
		}

		private void ConfigureSuCoBaoTri(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<SuCoBaoTri>()
				.HasOne(scbt => scbt.Phong)
				.WithMany(p => p.SuCoBaoTris)
				.HasForeignKey(scbt => scbt.MaPhong)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void ConfigureDienNuoc(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<DienNuoc>()
				.HasOne(dn => dn.Phong)
				.WithMany(p => p.DienNuocs)
				.HasForeignKey(dn => dn.MaPhong)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<DienNuoc>()
				.HasOne(dn => dn.HoaDon)
				.WithOne(hd => hd.DienNuoc)
				.HasForeignKey<HoaDon>(hd => hd.MaDN)
				.OnDelete(DeleteBehavior.Cascade);
		}
		private void SeedData(ModelBuilder modelBuilder)
		{
			// Seed dữ liệu cho bảng Toa
			modelBuilder.Entity<Toa>().HasData(
				new Toa { MaToa = "A", TenToa = "Tòa A", SoTang = 5, DiaChi = "Khu A", LoaiSuDung = "Ký túc xá" },
				new Toa { MaToa = "B", TenToa = "Tòa B", SoTang = 4, DiaChi = "Khu B", LoaiSuDung = "Ký túc xá" },
				new Toa { MaToa = "C", TenToa = "Tòa C", SoTang = 6, DiaChi = "Khu C", LoaiSuDung = "Ký túc xá" },
				new Toa { MaToa = "D", TenToa = "Tòa D", SoTang = 3, DiaChi = "Khu D", LoaiSuDung = "Ký túc xá" },
				new Toa { MaToa = "E", TenToa = "Tòa E", SoTang = 7, DiaChi = "Khu E", LoaiSuDung = "Ký túc xá" }
			);

			// Seed dữ liệu cho bảng ChucVu
			modelBuilder.Entity<ChucVu>().HasData(
				new ChucVu { MaChucVu = "1", TenChucVu = "Nhân viên Kế toán" },
				new ChucVu { MaChucVu = "2", TenChucVu = "Bảo vệ" },
				new ChucVu { MaChucVu = "3", TenChucVu = "Thu ngân" }
			);
			modelBuilder.Entity<NoiQuy>().HasData(
			   new NoiQuy { MaNoiQuy = "NQ01", NoiDung = "Không hút thuốc trong ký túc xá", MucPhat = "200.000 VNĐ" },
			   new NoiQuy { MaNoiQuy = "NQ02", NoiDung = "Không gây ồn ào sau 22h", MucPhat = "150.000 VNĐ" },
			   new NoiQuy { MaNoiQuy = "NQ03", NoiDung = "Không nuôi động vật trong phòng", MucPhat = "300.000 VNĐ" },
			   new NoiQuy { MaNoiQuy = "NQ04", NoiDung = "Không tự ý thay đổi thiết bị trong phòng", MucPhat = "250.000 VNĐ" },
			   new NoiQuy { MaNoiQuy = "NQ05", NoiDung = "Không mang chất cháy nổ vào ký túc xá", MucPhat = "500.000 VNĐ" },
			   new NoiQuy { MaNoiQuy = "NQ06", NoiDung = "Không xả rác bừa bãi", MucPhat = "100.000 VNĐ" },
			   new NoiQuy { MaNoiQuy = "NQ07", NoiDung = "Không tổ chức tiệc trong phòng", MucPhat = "200.000 VNĐ" }
		   );
		}
	}
}