using ESMS_Data.Models;
using ESMS_Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories
{
    public class RoleRepository : RepositoryBase<Role>
    {
        private ESMSContext _context;
        public RoleRepository(ESMSContext context) : base(context)
        {
            _context = context;
        }
    }
}
