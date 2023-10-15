using ESMS_Data.Models;
using ESMS_Data.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories
{
    public class UserRepository : RepositoryBase<User>
    {
        private ESMSContext _context;
        private DbSet<User> _users;
        private DbSet<Role> _roles;
        public UserRepository(ESMSContext context) : base(context)
        {
            _context = context;
            _users = _context.Set<User>();
            _roles = _context.Set<Role>();
        }

        public async Task<List<object>> GetBasic()
        {
            var qr = from user in _users
                     join role in _roles
                     on user.RoleId equals role.Id
                     select new { user.UserName, user.Name, user.Email, RoleName = role.Name};

            return await qr.ToListAsync<object>();
        }
    }
}
