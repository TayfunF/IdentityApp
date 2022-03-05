using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels
{
    public class LoginVM
    {
        [Display(Name ="E-posta :")]
        [Required(ErrorMessage ="E-posta alanı boş bırakılamaz !")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }



        [Display(Name ="Şifre :")]
        [Required(ErrorMessage ="Şifre alanı boş bırakılamaz !")]
        [DataType (DataType.Password)]
        [MinLength(4,ErrorMessage ="Şifre en az 4 karakter olmalıdır !")]
        public string Password { get; set; }


        public bool RememberMe { get; set; }
    }
}
