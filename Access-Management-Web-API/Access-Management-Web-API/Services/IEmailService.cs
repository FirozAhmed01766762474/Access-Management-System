using Access_Management_Web_API.Model;

namespace Access_Management_Web_API.Services
{
    public interface IEmailService
    {
        Task SendEmail(Mailrequest mailrequest);
    }
}
