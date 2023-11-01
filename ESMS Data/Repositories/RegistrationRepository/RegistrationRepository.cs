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
        private DbSet<Registration> _registrations;
        public RegistrationRepository(ESMSContext context) : base(context)
        {
            _context = context;
            _registrations = _context.Set<Registration>();
        }

        public async Task<List<string>> GetProctorList(int idt, List<string> assignedProctorList)
        {
            var allProctor = await _registrations.Where(r => r.Idt == idt)
                                       .Select(r => r.UserName)
                                       .ToListAsync();
            return allProctor.Except(assignedProctorList).ToList();
        }
    }
}
