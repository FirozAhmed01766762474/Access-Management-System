using Access_Management_Web_API.Model;
using Access_Management_Web_API.Repos.Models;
using AutoMapper;

namespace Access_Management_Web_API.Helper
{
    public class AutoMapperHandelar:Profile
    {
        public AutoMapperHandelar()
        {
            CreateMap<TblCustomer, CustomerModel>().ForMember(item=>item.StatusName, opt=>opt.MapFrom(item=>
            (item.IsActive != null && item.IsActive.Value)? "Active":"In Active")).ReverseMap();
            CreateMap<TblUser, UserModel>().ForMember(item => item.Statusname, opt => opt.MapFrom(
              item => (item.Isactive != null && item.Isactive.Value) ? "Active" : "In active")).ReverseMap();
        }
    }
}
