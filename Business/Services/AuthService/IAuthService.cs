using ESMS_Data.Entities.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.AuthService
{
    public interface IAuthService
    {
        public Task<ResultModel> Authenticate(UserModel user);
        public ResultModel GenerateToken(UserModel user);
    }
}
