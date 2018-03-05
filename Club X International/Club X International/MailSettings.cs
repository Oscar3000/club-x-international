using System.Net;
using System.Net.Mail;
using System.Web.Configuration;

namespace Club_X_International
{
    public class MailSettings
    {
        public static string smtpHost = WebConfigurationManager.AppSettings["smtpHost"].ToString();
        public static int smtpPort = int.Parse(WebConfigurationManager.AppSettings["smtpPort"].ToString());
        public static string smtpCredUN = WebConfigurationManager.AppSettings["smtpCredUN"].ToString();
        public static string smtpCredUP = WebConfigurationManager.AppSettings["smtpCredUP"].ToString();

        public static void Mail(MailMessage message)
        {
            var smtp = new SmtpClient(smtpHost,smtpPort);
            smtp.Credentials = new NetworkCredential(smtpCredUN, smtpCredUP);
            smtp.EnableSsl = true;
            smtp.Send(message);

        }
    }
}