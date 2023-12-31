﻿using ESMS_Data.Entities.RequestModel;
using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories.UserRepository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
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

        public async Task<List<object>> GetUserList(string userName)
        {
            var qr = _users.Where(u => u.UserName.Contains(userName))
                           .Include(u => u.Role)
                           .Select(u => new
                           {
                               u.UserName,
                               u.Name,
                               u.Email,
                               u.PhoneNumber,
                               Role = u.Role.Name,
                               AvailableRoles = _roles.Where(r => r.Id != u.Role.Id)
                                                      .Select(r => new { r.Id, r.Name })
                                                      .ToList<object>()
                           });

            return await qr.ToListAsync<object>();
        }

        public async Task<object> GetUserDetails(string userNameOrEmail)
        {
            var qr = _users.Where(u => u.UserName.Equals(userNameOrEmail) || u.Email.Equals(userNameOrEmail))
                           .Include(u => u.Role)
                           .Include(u => u.Department)
                           .Select(u => new
                           {
                               u.UserName,
                               u.Image,
                               u.Name,
                               DateOfBirth = u.DateOfBirth != null ? u.DateOfBirth.Value.ToString("dd/MM/yyyy") : "N/A",
                               u.Gender,
                               u.Idcard,
                               u.Address,
                               u.PhoneNumber,
                               u.Email,
                               Department = u.Department.Name,
                               Role = u.Role.Name,
                               u.CurrentSemester,
                               u.RollNumber
                           });

            return await qr.FirstOrDefaultAsync();
        }

        public async Task<User> GetUser(string userNameOrEmail)
        {
            return await _users
                               .Include(u => u.Role)
                               .FirstOrDefaultAsync(u => u.UserName.Equals(userNameOrEmail) || u.Email.Equals(userNameOrEmail));
        }

        public async Task<string> GetUserMail(string userName)
        {
            return await _users.Where(u => u.UserName.Equals(userName)).Select(u => u.Email.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetUserList(List<string> username)
        {
            return await _users.Where(u => username.Contains(u.UserName)).ToListAsync();
        }

        public async Task<List<string>> GetUserMailByRoleId(int roleId)
        {
            return await _users.Where(u => u.RoleId == roleId)
                               .Select(u => u.Email)
                               .ToListAsync();
        }
    }
}
