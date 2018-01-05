namespace TZGCMS.Infrastructure.Email
{
    public interface IEmailService
    {
        void SendMail(string sender, string senderEmail, string mailTo, string mailcc, string subject, string body,
           string smtpServer, string fromEmail, string displayName, string userName, string password, int port, bool enableSsl);
    }
}
