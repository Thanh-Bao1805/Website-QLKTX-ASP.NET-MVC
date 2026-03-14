using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Models
{
    public class EditRoleViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Chọn Role")]
        public string SelectedRole { get; set; }
        public List<string> Roles { get; set; }
    }
}
