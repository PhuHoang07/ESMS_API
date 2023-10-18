using ESMS_Data.Interfaces;
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
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private ESMSContext _context;
        private DbSet<User> _users;
        private DbSet<Role> _roles;
        private DbSet<Department> _departments;
        public UserRepository(ESMSContext context) : base(context)
        {
            _context = context;
            _users = _context.Set<User>();
            _roles = _context.Set<Role>();
            _departments = _context.Set<Department>();
        }

        public async Task<List<object>> GetUserList(String userName)
        {
            // get basic info of users and available roles
            var qr = from user in _users
                     join role in _roles
                     on user.RoleId equals role.Id
                     where user.UserName.Contains(userName)
                     select new
                     {
                         user.UserName,
                         user.Name,
                         user.Email,
                         Role = role.Name,
                         AvailableRoles =  (from r in _roles
                                           where r.Id != user.RoleId
                                           select new {r.Id, r.Name}).ToList(),
                         user.IsActive
                     };

            return await qr.ToListAsync<object>();
        }

        public async Task<object> GetUserDetails(String userName)
        {
            var qr = from user in _users                     

                     join role in _roles
                     on user.RoleId equals role.Id

                     join dpm in _departments
                     on user.DepartmentId equals dpm.Id

                     where user.UserName == userName

                     select new
                     {
                         user.UserName,
                         user.Image,
                         user.Name,
                         user.DateOfBirth,
                         user.Gender,
                         user.Idcard,
                         user.Address,
                         user.PhoneNumber,
                         user.Email,
                         RollNumber = role.Name == "Student" ? user.RollNumber : null,
                         Major = role.Name == "Student" ? dpm.Name : null,
                         Department = role.Name != "Student" ? dpm.Name : null
                     };

            return await qr.FirstOrDefaultAsync();
        }

        public async Task<User> GetUser(String userName)
        {          
            return await _users.FindAsync(userName);
        }
    }
}
