//using Access_Management_Web_API.Model;
//using Access_Management_Web_API.Services;
//using MailKit.Net.Smtp;
//using MailKit.Security;
//using Microsoft.Extensions.Options;
//using MimeKit;


//namespace Access_Management_Web_API.Container
//{
//    public class EmailService:IEmailService
//    {

//        private readonly EmailSettings emailSettings;
//        public EmailService(IOptions<EmailSettings> options)
//        {
//            this.emailSettings = options.Value;
//        }
//        public async Task SendEmail(Mailrequest mailrequest)
//        {
//            var email = new MimeMessage();
//            email.Sender = MailboxAddress.Parse(emailSettings.Email);
//            email.To.Add(MailboxAddress.Parse(mailrequest.Email));
//            email.Subject = mailrequest.Subject;
//            var builder = new BodyBuilder();
//            builder.HtmlBody = mailrequest.Emailbody;
//            email.Body = builder.ToMessageBody();

//            using var smptp = new SmtpClient();
//            smptp.Connect(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);

//            smptp.Authenticate(emailSettings.Email, emailSettings.Password);
//            await smptp.SendAsync(email);
//            smptp.Disconnect(true);
//        }

//    }
//}
using Access_Management_Web_API.Model;
using Access_Management_Web_API.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Access_Management_Web_API.Container
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            this.emailSettings = options.Value;
        }

        public async Task SendEmail(Mailrequest mailrequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(mailrequest.Email));
            email.Subject = mailrequest.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = mailrequest.Emailbody;
            email.Body = builder.ToMessageBody();

            using var smtpClient = new SmtpClient();

            // Asynchronous connect
            await smtpClient.ConnectAsync(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);

            // Asynchronous authenticate
            await smtpClient.AuthenticateAsync(emailSettings.Email, emailSettings.Password);

            // Asynchronous send
            await smtpClient.SendAsync(email);

            // Asynchronous disconnect
            await smtpClient.DisconnectAsync(true);
        }
    }
}
