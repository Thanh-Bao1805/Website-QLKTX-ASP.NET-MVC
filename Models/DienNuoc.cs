using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnCoSo.Models
{
    public class DienNuoc
    {
        [Key]
        [StringLength(10)]
        [Required(ErrorMessage = "Mã điện nước là bắt buộc")]
        public string MaDN { get; set; }

        [Required(ErrorMessage = "Ngày ghi là bắt buộc")]
        [Display(Name = "Ngày ghi")]
        public DateTime NgayGhi { get; set; }

        [Required(ErrorMessage = "Số điện là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số điện phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Số điện (kWh)")]
        public int SoDien { get; set; }

        [Required(ErrorMessage = "Số nước là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số nước phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Số nước (m³)")]
        public int SoNuoc { get; set; }

        [Required(ErrorMessage = "Đơn giá điện là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá điện phải lớn hơn 0")]
        [Display(Name = "Đơn giá điện")]
        public decimal DonGiaDien { get; set; }

        [Required(ErrorMessage = "Đơn giá nước là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá nước phải lớn hơn 0")]
        [Display(Name = "Đơn giá nước")]
        public decimal DonGiaNuoc { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal TongTien { get; set; }

        [Required(ErrorMessage = "Phòng là bắt buộc")]
        [ForeignKey("Phong")]
        [StringLength(10)]
        [Display(Name = "Mã phòng")]
        public string? MaPhong { get; set; }

        public Phong? Phong { get; set; }
        public HoaDon? HoaDon { get; set; }
    }
}