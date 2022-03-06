using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels
{
    public class PasswordChangeVM
    {
        [Display(Name = "Eski Şifre :")]
        [Required(ErrorMessage = "Eski Şifre alanını doldurun")]
        [DataType(DataType.Password)]
        [MinLength(4,ErrorMessage ="Eski Şifre en az 4 karakterli olmak zorundadır")]
        public string PasswordOld { get; set; }


        [Display(Name = "Yeni Şifre :")]
        [Required(ErrorMessage = "Yeni Şifre alanını doldurun")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Yeni Şifre en az 4 karakterli olmak zorundadır")]
        public string PasswordNew { get; set; }


        [Display(Name = "Yeni Şifre Onay :")]
        [Required(ErrorMessage = "Yeni Şifre Onay alanını doldurun")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Yeni Şifre Onayı en az 4 karakterli olmak zorundadır")]
        [Compare("PasswordNew",ErrorMessage ="Yeni Şifre ve Yeni Şifre Onay Eşleşmiyor !")]
        public string PasswordConfirm { get; set; }
    }
}
