using Access_Management_Web_API.Helper;
using Access_Management_Web_API.Model;
using Access_Management_Web_API.Repos;
using Access_Management_Web_API.Repos.Models;
using Access_Management_Web_API.Services;
using AutoMapper.Internal;
using Microsoft.EntityFrameworkCore;

namespace Access_Management_Web_API.Container
{
    public class UserService : IUserService
    {
        private readonly LarnDataContext _context;

        public UserService(LarnDataContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse> ConfirmRegister(int userid, string username, string otptext)
        {
            ApiResponse response = new ApiResponse();
            bool otpresponse = await ValidateOTP(username, otptext);
            if (!otpresponse)
            {
                response.Result = "fail";
                response.Message = "Invalid OTP or Expired";
            }
            else
            {
                var _tempdata = await _context.TblTempusers.FirstOrDefaultAsync(item => item.Id == userid);
                var _user = new TblUser()
                {
                    Username = username,
                    Name = _tempdata.Name,
                    Password = _tempdata.Password,
                    Email = _tempdata.Email,
                    Phone = _tempdata.Phone,
                    Failattempt = 0,
                    Isactive = true,
                    Islocked = false,
                    Role = "user"
                };
                await _context.TblUsers.AddAsync(_user);
                await _context.SaveChangesAsync();
                await UpdatePWDManager(username, _tempdata.Password);
                response.Result = "pass";
                response.Message = "Registered successfully.";
            }

            return response;
        }
        public async Task<ApiResponse> UserRegisteration(UserRegister userRegister)
        {
            ApiResponse response = new ApiResponse();
            int userid = 0;
            bool isvalid = true;

            try
            {
                // duplicate user
                var _user = await _context.TblUsers.Where(item => item.Username == userRegister.UserName).ToListAsync();
                if (_user.Count > 0)
                {
                    isvalid = false;
                    response.Result = "fail";
                    response.Message = "Duplicate username";
                }

                // duplicate Email
                var _useremail = await _context.TblUsers.Where(item => item.Email == userRegister.Email).ToListAsync();
                if (_useremail.Count > 0)
                {
                    isvalid = false;
                    response.Result = "fail";
                    response.Message = "Duplicate Email";
                }


                if (userRegister != null && isvalid)
                {
                    var _tempuser = new TblTempuser()
                    {
                        Code = userRegister.UserName,
                        Name = userRegister.Name,
                        Email = userRegister.Email,
                        Password = userRegister.Password,
                        Phone = userRegister.Phone,
                    };
                    await _context.TblTempusers.AddAsync(_tempuser);
                    await _context.SaveChangesAsync();
                    userid = _tempuser.Id;
                    string OTPText = Generaterandomnumber();
                    await UpdateOtp(userRegister.UserName, OTPText, "register");
                    await SendOtpMail(userRegister.Email, OTPText, userRegister.Name);
                    response.Result = "pass";
                    response.Message = userid.ToString();
                }

            }
            catch (Exception ex)
            {
                response.Result = "fail";
            }

            return response;

        }

        public async Task<ApiResponse> ResetPassword(string username, string oldpassword, string newpassword)
        {
            ApiResponse response = new ApiResponse();
            var _user = await _context.TblUsers.FirstOrDefaultAsync(item => item.Username == username &&
            item.Password == oldpassword && item.Isactive == true);
            if (_user != null)
            {
                var _pwdhistory = await Validatepwdhistory(username, newpassword);
                if (_pwdhistory)
                {
                    response.Result = "fail";
                    response.Message = "Don't use the same password that used in last 3 transaction";
                }
                else
                {
                    _user.Password = newpassword;
                    await _context.SaveChangesAsync();
                    await UpdatePWDManager(username, newpassword);
                    response.Result = "pass";
                    response.Message = "Password changed.";
                }
            }
            else
            {
                response.Result = "fail";
                response.Message = "Failed to validate old password.";
            }
            return response;
        }

        public async Task<ApiResponse> ForgetPassword(string username)
        {
            ApiResponse response = new ApiResponse();
            var _user = await _context.TblUsers.FirstOrDefaultAsync(item => item.Username == username && item.Isactive == true);
            if (_user != null)
            {
                string otptext = Generaterandomnumber();
                await UpdateOtp(username, otptext, "forgetpassword");
                await SendOtpMail(_user.Email, otptext, _user.Name);
                response.Result = "pass";
                response.Message = "OTP sent";

            }
            else
            {
                response.Result = "fail";
                response.Message = "Invalid User";
            }
            return response;
        }

        public async Task<ApiResponse> UpdatePassword(string username, string Password, string Otptext)
        {
            ApiResponse response = new ApiResponse();

            bool otpvalidation = await ValidateOTP(username, Otptext);
            if (otpvalidation)
            {
                bool pwdhistory = await Validatepwdhistory(username, Password);
                if (pwdhistory)
                {
                    response.Result = "fail";
                    response.Message = "Don't use the same password that used in last 3 transaction";
                }
                else
                {
                    var _user = await _context.TblUsers.FirstOrDefaultAsync(item => item.Username == username && item.Isactive == true);
                    if (_user != null)
                    {
                        _user.Password = Password;
                        await _context.SaveChangesAsync();
                        await UpdatePWDManager(username, Password);
                        response.Result = "pass";
                        response.Message = "Password changed";
                    }
                }
            }
            else
            {
                response.Result = "fail";
                response.Message = "Invalid OTP";
            }
            return response;
        }

        public async Task<ApiResponse> UpdateStatus(string username, bool status)
        {
            ApiResponse response = new ApiResponse();
            var user = await _context.TblUsers.FirstOrDefaultAsync(item => item.Username == username);

            if(user != null)
            {
                user.Isactive = status;
                await _context.SaveChangesAsync();
                response.Result = "Pass";
                response.Message = "User Status Changed";
            }

            else
            {
                response.Result = "falil";
                response.Message = "Invalid User";
            }



            return response;
        }

        public async Task<ApiResponse> UpdateUserRole(string username, string userrole)
        {
            ApiResponse response = new ApiResponse();
            var user = await _context.TblUsers.FirstOrDefaultAsync(item => item.Username == username);

            if (user != null)
            {
                user.Role = userrole;
                await _context.SaveChangesAsync();
                response.Result = "Pass";
                response.Message = "User Role Changed";
            }

            else
            {
                response.Result = "falil";
                response.Message = "Invalid User";
            }



            return response;
        }

        private async Task<bool> ValidateOTP(string username, string OTPText)
        {
            bool response = false;
            var _data = await _context.TblOtpManagers.FirstOrDefaultAsync(item => item.Username == username
            && item.Otptext == OTPText && item.Expiration > DateTime.Now);
            if (_data != null)
            {
                response = true;
            }
            return response;
        }

        private async Task UpdatePWDManager(string username, string password)
        {
            var _opt = new TblPwdManger()
            {
                Username = username,
                Password = password,
                ModifyDate = DateTime.Now
            };
            await _context.TblPwdMangers.AddAsync(_opt);
            await _context.SaveChangesAsync();
        }

        private string Generaterandomnumber()
        {
            Random random = new Random();
            string randomno = random.Next(0, 1000000).ToString("D6");
            return randomno;
        }

        private async Task UpdateOtp(string username, string otptext, string otptype)
        {
            var _opt = new TblOtpManager()
            {
                Username = username,
                Otptext = otptext,
                Expiration = DateTime.Now.AddMinutes(30),
                Createddate = DateTime.Now,
                Otptype = otptype
            };

            await _context.TblOtpManagers.AddAsync(_opt);
            await _context.SaveChangesAsync();
        }

        private async Task SendOtpMail(string useremail, string OtpText, string Name)
        {
            //var mailrequest = new Mailrequest();
            //mailrequest.Email = useremail;
            //mailrequest.Subject = "Thanks for registering : OTP";
            //mailrequest.Emailbody = GenerateEmailBody(Name, OtpText);
            //await this.emailService.SendEmail(mailrequest);

        }

        private async Task<bool> Validatepwdhistory(string Username, string password)
        {
            bool response = false;
            var _pwd = await _context.TblPwdMangers.Where(item => item.Username == Username).
                OrderByDescending(p => p.ModifyDate).Take(3).ToListAsync();
            if (_pwd.Count > 0)
            {
                var validate = _pwd.Where(o => o.Password == password);
                if (validate.Any())
                {
                    response = true;
                }
            }

            return response;

        }
    }
}
