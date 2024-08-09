using Access_Management_Web_API.Repos;
using Access_Management_Web_API.Repos.Models;
using Access_Management_Web_API.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Access_Management_Web_API.Container
{
    public class RefereseHandeler : IRefereseHandeler
    {
        private readonly LarnDataContext _context;

        public RefereseHandeler( LarnDataContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateToken(string username)
        {
            var randomnumber = new byte[32];
            using (var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string refreshtoken = Convert.ToBase64String(randomnumber);
                var Existtoken = _context.TblRefereshtokenns.FirstOrDefaultAsync(item => item.UserId == username).Result;
                if (Existtoken != null)
                {
                    Existtoken.Refereshtoken = refreshtoken;
                }
                else
                {
                    await _context.TblRefereshtokenns.AddAsync(new TblRefereshtokenn
                    {
                        UserId = username,
                        Tokenid = new Random().Next().ToString(),
                        Refereshtoken = refreshtoken
                    });
                }
                await _context.SaveChangesAsync();

                return refreshtoken;

            }
        }
    }
}
