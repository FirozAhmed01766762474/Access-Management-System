using Access_Management_Web_API.Helper;
using Access_Management_Web_API.Model;
using Access_Management_Web_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Access_Management_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("UserRegistation")]

        public async Task<IActionResult> UserRegistation(UserRegister register)
        {
            var data = await _service.UserRegisteration(register);

            return Ok(data);    
        }

        [HttpPost("ConfirmRegistatio")]

        public async Task<IActionResult> ConfirmRegistatio(int userid, string username,string otptext)
        {
            var data = await _service.ConfirmRegister(userid, username,otptext);

            return Ok(data);
        }

        [HttpPost("ResetPassword")]

        public async Task<IActionResult> ResetPassword(string username, string oldpassword, string newpassword)
        {
            var data = await _service.ResetPassword( username,  oldpassword,  newpassword);

            return Ok(data);
        }

        [HttpPost("ForgetPassword")]

        public async Task<IActionResult> ForgetPassword(string username)
        {
            var data = await _service.ForgetPassword(username);

            return Ok(data);
        }

        [HttpPost("UpdatePassword")]

        public async Task<IActionResult> UpdatePassword(string username, string Password, string Otptext)
        {
            var data = await _service.UpdatePassword(username,Password,Otptext);

            return Ok(data);
        }

        [HttpPost("UpdateStatus")]

        public async Task<IActionResult> UpdateStatus(string username, bool status)
        {
            var data = await _service.UpdateStatus(username, status);

            return Ok(data);
        }

        [HttpPost("UpdateUserRole")]

        public async Task<IActionResult> UpdateUserRole(string username, string userRole)
        {
            var data = await _service.UpdateUserRole(username, userRole);

            return Ok(data);
        }
    }
}
