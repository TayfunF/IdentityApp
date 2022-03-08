using IdentityApp.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels
{
    //REGISTER ICIN KULLANDIM
    public class UserVM
    {
        [Display(Name = "Kullanıcı Adı :")]
        [Required(ErrorMessage = "Kullanıcı adı girmek zorunludur !")]
        public string UserName { get; set; }


        [Display(Name = "Telefon Numarası :")]
        public string PhoneNumber { get; set; }


        [Display(Name = "E-Posta Adresi :")]
        [Required(ErrorMessage = "E-posta adresi girmek zorunludur !")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-posta adresi doğru formatta yazılmadı !")]
        public string Email { get; set; }


        [Display(Name = "Şifre :")]
        [Required(ErrorMessage = "Şifre girmek zorunludur !")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Şifre en az 4 karakter olmalıdır !")]
        public string Password { get; set; }

        [Display(Name = "Şehir :")]
        public string City { get; set; }

        [Display(Name = "Resim :")]
        public string Picture { get; set; }

        [Display(Name = "Doğum Tarihi :")]
        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }

        [Display(Name = "Cinsiyet :")]
        public Gender Gender { get; set; }
    }
}
