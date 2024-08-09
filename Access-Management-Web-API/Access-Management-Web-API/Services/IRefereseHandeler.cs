namespace Access_Management_Web_API.Services
{
    public interface IRefereseHandeler
    {
       Task<string> GenerateToken(string username);
    }
}
