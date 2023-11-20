using ESMS_Data.Entities;
using ESMS_Data.Entities.RequestModel.UserReqModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.UserService
{
    public interface IUserService
    {
        public Task<ResultModel> GetUserList();
        public Task<ResultModel> GetUserList(string userName);
        public Task<ResultModel> GetUserDetails(string userNameOrEmail);
        public Task<ResultModel> UpdateSettings(string currentEmail, UserSettingsReqModel req);
        public Task<ResultModel> UpdateUser(UserModel currentUser, UserProfileReqModel userProfileReqModel);
    }
}
