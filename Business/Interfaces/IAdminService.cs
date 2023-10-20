using ESMS_Data.Entities.RequestModel;
using ESMS_Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IAdminService
    {
        public Task<ResultModel> GetUserList();
        public Task<ResultModel> FindByUserName(String userName);
        public Task<ResultModel> GetUserDetails(String userName);
        public Task<ResultModel> SetRole(UserReqModel req);
    }
}
