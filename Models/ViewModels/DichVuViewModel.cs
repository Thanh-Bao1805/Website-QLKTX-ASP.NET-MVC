using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models.ViewModels
{
    public class DichVuViewModel
    {
        [Key]
        public string MaDV { get; set; }
        public string TenDichVu { get; set; }
        public decimal DonGia { get; set; }
        public string MoTa { get; set; }
        public string? HinhAnh { get; set; }
        public IFormFile? ImageFile { get; set; }

        // Convert từ ViewModel sang Model
        public DichVu ToDichVu()
        {
            return new DichVu
            {
                MaDV = this.MaDV,
                TenDichVu = this.TenDichVu,
                DonGia = this.DonGia,
                MoTa = this.MoTa,
                HinhAnh = this.HinhAnh
            };
        }

        // Convert từ Model sang ViewModel
        public static DichVuViewModel FromDichVu(DichVu dichVu)
        {
            return new DichVuViewModel
            {
                MaDV = dichVu.MaDV,
                TenDichVu = dichVu.TenDichVu,
                DonGia = dichVu.DonGia,
                MoTa = dichVu.MoTa,
                HinhAnh = dichVu.HinhAnh
            };
        }
    }
} 