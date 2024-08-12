using Access_Management_Web_API.Model;
using Access_Management_Web_API.Repos;
using Access_Management_Web_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Access_Management_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly LarnDataContext _context;
        private readonly JwtSettings _jwtSettings;
        private readonly IRefereseHandeler _referese;
        public AuthorizeController(LarnDataContext context, IOptions<JwtSettings> options,IRefereseHandeler referese)
        {
            _context = context;
            _referese = referese;
            _jwtSettings = options.Value;
        }


   

        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken([FromBody] UserCard userCard)
        {
            var user = await _context.TblUsers.FirstOrDefaultAsync(item => item.Username == userCard.username && item.Password == userCard.password);
            if (user != null)
            {
                var tokenhandelar = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.SequrityKey);
                var tokenDes = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role)

                    }),
                    Expires = DateTime.UtcNow.AddSeconds(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)


                };

                var token = tokenhandelar.CreateToken(tokenDes);
                var finaltoken = tokenhandelar.WriteToken(token);

                return Ok(new TokenResponse() { Token=finaltoken,ReferaseToken= await _referese.GenerateToken(userCard.username), UserRole = user.Role});

            }
            else
            {
                return Unauthorized();
            }


        }

        [HttpPost("GenerateRefreshToken")]
        public async Task<IActionResult> GenerateToken([FromBody] TokenResponse token)
        {
            var _refreshtoken = await _context.TblRefereshtokenns.FirstOrDefaultAsync(item => item.Refereshtoken == token.ReferaseToken);
            if (_refreshtoken != null)
            {
                //generate token
                var tokenhandler = new JwtSecurityTokenHandler();
                var tokenkey = Encoding.UTF8.GetBytes(_jwtSettings.SequrityKey);
                SecurityToken securityToken;
                var principal = tokenhandler.ValidateToken(token.Token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenkey),
                    ValidateIssuer = false,
                    ValidateAudience = false,

                }, out securityToken);

                var _token = securityToken as JwtSecurityToken;
                if (_token != null && _token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                {
                    string username = principal.Identity?.Name;
                    var _existdata = await _context.TblRefereshtokenns.FirstOrDefaultAsync(item => item.UserId == username
                    && item.Refereshtoken== token.ReferaseToken);
                    if (_existdata != null)
                    {
                        var _newtoken = new JwtSecurityToken(
                            claims: principal.Claims.ToArray(),
                            expires: DateTime.Now.AddSeconds(30),
                            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SequrityKey)),
                            SecurityAlgorithms.HmacSha256)
                            );

                        var _finaltoken = tokenhandler.WriteToken(_newtoken);
                        return Ok(new TokenResponse() { Token = _finaltoken, ReferaseToken = await _referese.GenerateToken(username), UserRole = token.UserRole });
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }

                //var tokendesc = new SecurityTokenDescriptor
                //{
                //    Subject = new ClaimsIdentity(new Claim[]
                //    {
                //        new Claim(ClaimTypes.Name,user.Code),
                //        new Claim(ClaimTypes.Role,user.Role)
                //    }),
                //    Expires = DateTime.UtcNow.AddSeconds(30),
                //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
                //};
                //var token = tokenhandler.CreateToken(tokendesc);
                //var finaltoken = tokenhandler.WriteToken(token);
                //return Ok(new TokenResponse() { Token = finaltoken, RefreshToken = await this.refresh.GenerateToken(userCred.username) });

            }
            else
            {
                return Unauthorized();
            }

        }
    }
}
