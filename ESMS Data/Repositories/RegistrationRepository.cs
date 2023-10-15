﻿using ESMS_Data.Models;
using ESMS_Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories
{
    public class RegistrationRepository : RepositoryBase<Registration>
    {
        private ESMSContext _context;
        public RegistrationRepository(ESMSContext context) : base(context)
        {
            _context = context;
        }
    }
}
