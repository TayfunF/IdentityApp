using System.Net.Mail;

namespace IdentityApp.Helper
{
    //Statik yaptim. Direk ulasabilmek icin
    public static class PasswordResetHelper
    {
        public static void PasswordResetSendEmail(string link, string Email)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("IdentityApp@outlook.com"); //Bu mail ile sifre yenileme linki gonderiyorum.
            mail.To.Add($"{Email}");
            mail.Subject = $"TAYFUN FIRTINA IdentityApp :: Şifre Sıfırlama";
            mail.Body = "<h2>Şifrenizi yenilemek için lütfen aşağıdaki bağlantı adresine tıklayın</h2><hr/><br/>";
            mail.Body += $"<strong><a href='{link}'> Buradan şifrenizi sıfırlayabilirsiniz</a></strong>";
            mail.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient();

            smtpClient.Host = "smtp.outlook.com"; //Host tipi gmail olacaksa => "smtp.gmail.com" yazicam.
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new System.Net.NetworkCredential("IdentityApp@outlook.com", "NetworkCredential587");
            smtpClient.Send(mail);
        }
    }
}
