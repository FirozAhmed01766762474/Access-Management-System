using Access_Management_Web_API.Helper;
using Access_Management_Web_API.Model;
using Access_Management_Web_API.Repos.Models;

namespace Access_Management_Web_API.Services
{
    public interface ICustomerService
    {
         public Task<List<CustomerModel>> GetAll();
         public Task<CustomerModel> GetbyCode(string code);

         Task<ApiResponse> Create(CustomerModel data);
        Task<ApiResponse> Remove(string code);

        Task<ApiResponse> Update(CustomerModel customer,string code);
    }
}
