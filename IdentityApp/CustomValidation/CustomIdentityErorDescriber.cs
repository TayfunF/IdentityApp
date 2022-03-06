using Microsoft.AspNetCore.Identity;

namespace IdentityApp.CustomValidation
{
    public class CustomIdentityErorDescriber : IdentityErrorDescriber
    {
        //INGILIZCE OLARAK YAZAN MESAJLARI, TURKCE YAPMAK ISTEDIKLERIMI override edicem burda

        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError() { Code = "InvalidUserName", Description = $"{userName} geçersizdir !" };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError() { Code = "DuplicateUserName", Description = $"Kullanıcı adı({userName}) zaten kayıtlı !" };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError() { Code = "DuplicateEmail", Description = $"{email} zaten kayıtlı !" };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError() { Code = "PasswordTooShort", Description = $" Şifreniz en az {length} karakter olmalıdır !" };
        }        
    }
}
