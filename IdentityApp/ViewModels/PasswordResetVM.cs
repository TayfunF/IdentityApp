using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels
{
    public class PasswordResetVM
    {
        [Display(Name ="E-posta Adresi :")]
        [Required(ErrorMessage ="E-posta alanı zorunludur !")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }



        [Display(Name = "Yeni Şifre :")]
        [Required(ErrorMessage = "Yeni şifre alanı boş bırakılamaz !")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Yeni şifre en az 4 karakter olmalıdır !")]
        public string PasswordNew { get; set; }
    }
}
