using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels
{
    public class PasswordResetVM
    {
        [Display(Name ="E-posta Adresi :")]
        [Required(ErrorMessage ="E-posta alanı zorunludur !")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
