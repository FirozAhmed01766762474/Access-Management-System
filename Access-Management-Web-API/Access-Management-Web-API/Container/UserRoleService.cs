using Access_Management_Web_API.Helper;
using Access_Management_Web_API.Model;
using Access_Management_Web_API.Repos;
using Access_Management_Web_API.Repos.Models;
using Access_Management_Web_API.Services;
using Microsoft.EntityFrameworkCore;

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


        public async Task<List<TblMenu>> GetAllMenues()
        {
            return await _context.TblMenus.ToListAsync();
        }

        public async Task<List<TblRole>> GetAllRoles()
        {
            return await _context.TblRoles.ToListAsync();
        }

        public async Task<List<Appmenu>> GetAllMenuesbyRole(string userrole)
        {
            List<Appmenu> appmenus = new List<Appmenu>();

            var accessdata =  (from menu in  _context.TblRolepermissions.Where(o => o.Userrole == userrole && o.Haveview)
                              join m in _context.TblMenus on menu.Menucode equals m.Code into _jointable
                              from p in _jointable.DefaultIfEmpty()
                              select new { code = menu.Menucode, name = p.Name }).ToList();
            if (accessdata.Any())
            {
                accessdata.ForEach(item =>
                {
                    appmenus.Add(new Appmenu()
                    {
                        Code = item.code,
                        Name = item.name
                    });
                });
            }

            return appmenus;
        }

        public async Task<Menupermission> GetMenupermissionbyrole(string userrole, string menucode)
        {
            Menupermission menupermission = new Menupermission();
            var _data = await _context.TblRolepermissions.FirstOrDefaultAsync(o => o.Userrole == userrole && o.Haveview
            && o.Menucode == menucode);
            if (_data != null)
            {
                menupermission.code = _data.Menucode;
                menupermission.Haveview = _data.Haveview;
                menupermission.Haveadd = _data.Haveadd;
                menupermission.Haveedit = _data.Haveedit;
                menupermission.Havedelete = _data.Havedelete;
            }
            return menupermission;
        }
    }
}
