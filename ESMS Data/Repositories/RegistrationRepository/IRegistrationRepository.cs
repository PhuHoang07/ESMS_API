using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories.RegistrationRepository
{
    public interface IRegistrationRepository : IRepositoryBase<Registration>
    {
        public Task<List<string>> GetProctorList(int idt, List<string> assignedProctorList);

    }
}
