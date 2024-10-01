using Access_Management_Web_API.Helper;
using Access_Management_Web_API.Model;
using Access_Management_Web_API.Repos;
using Access_Management_Web_API.Repos.Models;
using Access_Management_Web_API.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Access_Management_Web_API.Container
{
    public class CustomerService : ICustomerService
    {
        private readonly LarnDataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(LarnDataContext context, IMapper mapper, ILogger<CustomerService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse> Create(CustomerModel data)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                _logger.LogInformation("Create Begins");
                TblCustomer customer = _mapper.Map<CustomerModel, TblCustomer>(data);
                await _context.TblCustomers.AddAsync(customer);
                await _context.SaveChangesAsync();
                response.ResponseCode = 201;
                response.Result = "pass";

            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
                _logger.LogError(ex.Message,ex);
            }

            return response;
        }


        public async Task<List<CustomerModel>> GetAll()
        {
            List<CustomerModel> response = new List<CustomerModel>();
            var data = await this._context.TblCustomers.ToListAsync();
            if (data != null)
            {
                response = _mapper.Map<List<TblCustomer>, List<CustomerModel>>(data);
            }
            return response;
        }

        public async Task<CustomerModel> GetbyCode(string code)
        {
            CustomerModel response = new CustomerModel();
            var data = await this._context.TblCustomers.FindAsync(code);
            if (data != null)
            {
                response = _mapper.Map<TblCustomer, CustomerModel>(data);
            }
            return response;
        }

       public async Task<ApiResponse> Remove(string code)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var customer = await _context.TblCustomers.FindAsync(code);

                if (customer != null)
                {
                   _context.TblCustomers.Remove(customer);
                   await _context.SaveChangesAsync();
                    response.ResponseCode = 200;
                    response.Result = "pass";
                }
                else
                {
                    response.ResponseCode = 404;
                    response.Result = "Data NotFound";
                }

            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse> Update(CustomerModel customer, string code)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var data = await _context.TblCustomers.FindAsync(code);
                if (data != null)
                {
                    data.Name = customer.Name;
                    data.Email = customer.Email;
                    data.Phone = customer.Phone;
                    data.IsActive = customer.IsActive;
                    data.Creditlimit = customer.Creditlimit;
                    await _context.SaveChangesAsync();
                    response.ResponseCode = 200;
                    response.Result = "pass";
                }
                else
                {
                    response.ResponseCode = 404;
                    response.ErrorMessage = "Data not found";
                }

            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
    }
}
