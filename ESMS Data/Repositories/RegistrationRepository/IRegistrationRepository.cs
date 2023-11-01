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
        public Task<List<string>> GetAvailableProctors(int idt, List<string> assignedProctorList);
        public Task<List<object>> GetProctors(int idt);

    }
}
