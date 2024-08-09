using Access_Management_Web_API.Helper;
using Access_Management_Web_API.Repos;
using Access_Management_Web_API.Repos.Models;
using Access_Management_Web_API.Services;

namespace Access_Management_Web_API.Container
{
    public class UserRoleService:IUserRoleServices
    {
        private readonly LarnDataContext _context;

        public UserRoleService(LarnDataContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> AssignRolePermission(List<TblRolepermission> _data)
        {
            ApiResponse response = new ApiResponse();
            int processcount = 0;
            try
            {
                using (var dbtransaction = await _context.Database.BeginTransactionAsync())
                {
                    if (_data.Count > 0)
                    {
                        _data.ForEach(item =>
                        {
                            var userdata = _context.TblRolepermissions.FirstOrDefault(item1 => item1.Userrole == item.Userrole &&
                            item1.Menucode == item.Menucode);
                            if (userdata != null)
                            {
                                userdata.Haveview = item.Haveview;
                                userdata.Haveadd = item.Haveadd;
                                userdata.Havedelete = item.Havedelete;
                                userdata.Haveedit = item.Haveedit;
                                processcount++;
                            }
                            else
                            {
                                _context.TblRolepermissions.Add(item);
                                processcount++;

                            }

                        });

                        if (_data.Count == processcount)
                        {
                            await _context.SaveChangesAsync();
                            await dbtransaction.CommitAsync();
                            response.Result = "pass";
                            response.Message = "Saved successfully.";
                        }
                        else
                        {
                            await dbtransaction.RollbackAsync();
                        }

                    }
                    else
                    {
                        response.Result = "fail";
                        response.Message = "Failed";
                    }
                }

            }
            catch (Exception ex)
            {
                response = new ApiResponse();
            }

            return response;
        }
    }
}
