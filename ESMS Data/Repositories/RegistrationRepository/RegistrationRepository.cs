using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories.RegistrationRepository
{
    public class RegistrationRepository : RepositoryBase<Registration>, IRegistrationRepository
    {
        private ESMSContext _context;
        private DbSet<User> _users;
        private DbSet<ExamTime> _examTimes;
        public RegistrationRepository(ESMSContext context) : base(context)
        {
            _context = context;
            _users = _context.Set<User>();
            _examTimes = _context.Set<ExamTime>();
        }

    }
}
