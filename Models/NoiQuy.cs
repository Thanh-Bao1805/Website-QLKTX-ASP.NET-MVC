using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
	public class NoiQuy
	{
		[Key]
		public string MaNoiQuy { get; set; }
		public string NoiDung { get; set; }
		public string MucPhat { get; set; }

		public ICollection<ViPham>? ViPhams { get; set; }
	}
}