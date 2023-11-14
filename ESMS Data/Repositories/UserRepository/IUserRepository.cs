using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories.UserRepository
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        public Task<List<object>> GetUserList(string userName);
        public Task<object> GetUserDetails(string userNameOrEmail);
        public Task<User> GetUser(string userNameOrEmail);
        public Task<string> GetUserMail(string userName);
        public Task<List<User>> GetUserList(List<string> username);

    }
}