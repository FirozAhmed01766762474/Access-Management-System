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

        [HttpPost("ConfirmRegistation")]

        public async Task<IActionResult> ConfirmRegistation(Confirmpassword confirmpassword)
        {
            var data = await _service.ConfirmRegister(confirmpassword.userid, confirmpassword.username, confirmpassword.otptext);

            return Ok(data);
        }

        [HttpPost("ResetPassword")]

        public async Task<IActionResult> ResetPassword(Resetpassword resetpassword)
        {
            var data = await _service.ResetPassword(resetpassword.username, resetpassword.oldpassword, resetpassword.newpassword);

            return Ok(data);
        }

        [HttpGet("ForgetPassword")]

        public async Task<IActionResult> ForgetPassword(string username)
        {
            var data = await _service.ForgetPassword(username);

            return Ok(data);
        }

        [HttpPost("UpdatePassword")]

        public async Task<IActionResult> UpdatePassword(Updatepassword updatepassword)
        {
            var data = await _service.UpdatePassword(updatepassword.username, updatepassword.password, updatepassword.otptext);

            return Ok(data);
        }

        [HttpPost("UpdateStatus")]

        public async Task<IActionResult> UpdateStatus(Updatestatus updatestatus)
        {
            var data = await _service.UpdateStatus(updatestatus.username, updatestatus.status);

            return Ok(data);
        }

        [HttpPost("UpdateUserRole")]

        public async Task<IActionResult> UpdateUserRole(UpdateRole updateRole)
        {
            var data = await _service.UpdateUserRole(updateRole.username, updateRole.role);

            return Ok(data);
        }

        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {

            var data = await _service.GetAll();

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);

        }



        [HttpGet("GetAbyCode")]

        public async Task<IActionResult> GetAllbycode( string code)
        {

            var data = await _service.GetbyCode(code);

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);

        }






    }
}
