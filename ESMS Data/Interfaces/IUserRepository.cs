using ESMS_Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Interfaces
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        public Task<List<object>> GetUserList(String userName);
        public Task<object> GetUserDetails(String userName);
        public Task<User> GetUser(String userName);
    }
}
