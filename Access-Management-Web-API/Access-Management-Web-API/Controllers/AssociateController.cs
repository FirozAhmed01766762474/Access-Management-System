using Access_Management_Web_API.Repos;
using Access_Management_Web_API.Repos.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Access_Management_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssociateController : ControllerBase
    {
        private readonly LarnDataContext _context;

        public AssociateController(LarnDataContext context)
        {
            _context = context;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            string sqlquery = "exec sp_getCustomer";
            var data =  await _context.TblCustomers.FromSqlRaw(sqlquery).ToListAsync();
            if(data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetAllCustome")]
        public async Task<IActionResult> GetAllCustome()
        {
            string sqlquery = "exec sp_getcustomer_custom";
            var data = await _context.customerdetails.FromSqlRaw(sqlquery).ToListAsync();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetAllCustomeq")]
        public async Task<IActionResult> GetAllCustomeq()
        {
            string sqlquery = "Select *,'Active' as Statusname from tbl_customer";
            var data = await _context.customerdetails.FromSqlRaw(sqlquery).ToListAsync();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
        [HttpGet("GetbyCode")]
        public async Task<IActionResult> GetbyCode(string code)
        {
            string sqlquery = "Select *,'Active' as Statusname from tbl_customer where code=@code";
            SqlParameter paramerer = new SqlParameter("@code",code);
            var data = await _context.customerdetails.FromSqlRaw(sqlquery,paramerer).FirstOrDefaultAsync();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(TblCustomer customer)
        {
            string sqlquery = "exec sp_createcustomer @code,@name,@email,@phone,@creditlimit,@active,@taxcode,@type";
            //SqlParameter paramerer = new SqlParameter("@code", code);
            SqlParameter[] paramerer =
            {
                new SqlParameter("@code",customer.Code),
                new SqlParameter("@name",customer.Name),
                new SqlParameter("@email",customer.Email),
                new SqlParameter("@phone",customer.Phone),
                new SqlParameter("@creditlimit",customer.Creditlimit),
                new SqlParameter("@active",customer.IsActive),
                new SqlParameter("@taxcode",customer.Taxcode),
                new SqlParameter("@type","add")
            };
            var data = await _context.Database.ExecuteSqlRawAsync(sqlquery, paramerer);
       
            return Ok(data);
        }
        [HttpPost("Update")]
        public async Task<IActionResult> Update(string code, TblCustomer customer)
        {
            string sqlquery = "exec sp_createcustomer @code,@name,@email,@phone,@creditlimit,@active,@taxcode,@type";
            //SqlParameter paramerer = new SqlParameter("@code", code);
            SqlParameter[] paramerer =
            {
                new SqlParameter("@code",code),
                new SqlParameter("@name",customer.Name),
                new SqlParameter("@email",customer.Email),
                new SqlParameter("@phone",customer.Phone),
                new SqlParameter("@creditlimit",customer.Creditlimit),
                new SqlParameter("@active",customer.IsActive),
                new SqlParameter("@taxcode",customer.Taxcode),
                new SqlParameter("@type","update")
            };
            var data = await _context.Database.ExecuteSqlRawAsync(sqlquery, paramerer);

            return Ok(data);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> delete(string code)
        {
            string sqlquery = "exec sp_deletecustomer @code";
            SqlParameter[] parameter =
            {
                new SqlParameter("@code",code)
            };
            var data = await _context.Database.ExecuteSqlRawAsync(sqlquery, parameter);
            return Ok(data);

        }
    }
}
