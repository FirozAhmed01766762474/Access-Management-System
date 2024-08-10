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
        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var data = await _userRole.GetAllRoles();
            if(data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
        [HttpGet("GetAllManues")]
        public async Task<IActionResult> GetAllMenues()
        {
            var data = await _userRole.GetAllMenues();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetAllMenuesbyRole")]
        public async Task<IActionResult> GetAllMenuesbyRole(string userrole)
        {
            var data = await _userRole.GetAllMenuesbyRole(userrole);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetMenupermissionbyrole")]
        public async Task<IActionResult> GetMenupermissionbyrole(string userrole, string menucode)
        {
            var data = await _userRole.GetMenupermissionbyrole(userrole, menucode);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
    }
}
