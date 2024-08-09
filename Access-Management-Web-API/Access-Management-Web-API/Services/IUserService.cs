using Access_Management_Web_API.Helper;
using Access_Management_Web_API.Model;

namespace Access_Management_Web_API.Services
{
    public interface IUserService
    {
        Task<ApiResponse> UserRegisteration(UserRegister userRegister);

        Task<ApiResponse> ConfirmRegister(int userid, string username,string otptext);

        Task<ApiResponse> ResetPassword(string username, string oldpassword, string newpassword);
        Task<ApiResponse> ForgetPassword(string username);
        Task<ApiResponse> UpdatePassword(string username, string Password, string Otptext);
        Task<ApiResponse> UpdateStatus(string username, bool status);
        Task<ApiResponse> UpdateUserRole(string username, string userRole);
    }
}
