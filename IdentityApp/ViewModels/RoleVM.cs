using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels
{
    public class RoleVM
    {
        [Display(Name="Rol İsmi :")]
        [Required(ErrorMessage ="Role ismi boş bırakılamaz")]
        public string Name { get; set; }
        public string Id { get; set; } //Rol guncellerken kullanicam
    }
}
