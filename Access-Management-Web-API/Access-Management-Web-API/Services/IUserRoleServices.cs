using Access_Management_Web_API.Helper;
using Access_Management_Web_API.Repos.Models;

namespace Access_Management_Web_API.Services
{
    public interface IUserRoleServices
    {
        Task<ApiResponse> AssignRolePermission(List<TblRolepermission> _data);
    }
}
