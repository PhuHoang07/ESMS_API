using ESMS_Data.Entities.RequestModel;
using ESMS_Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.AdminService
{
    public interface IAdminService
    {
        public Task<ResultModel> GetUserList();
        public Task<ResultModel> GetUserList(string userName);
        public Task<ResultModel> GetUserDetails(string userName);
        public Task<ResultModel> SetRole(UserReqModel req);
    }
}
