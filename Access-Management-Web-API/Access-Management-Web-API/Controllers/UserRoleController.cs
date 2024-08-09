using Access_Management_Web_API.Repos.Models;
using Access_Management_Web_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Access_Management_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleServices _userRole;

        public UserRoleController(IUserRoleServices userRole)
        {
            _userRole = userRole;
        }

        [HttpPost("assignrolepermission")]
        public async Task<IActionResult> assignrolepermission(List<TblRolepermission> rolepermissions)
        {
            var data = await _userRole.AssignRolePermission(rolepermissions);
            return Ok(data);
        }
    }
}
