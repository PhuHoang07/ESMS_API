using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Interfaces
{
    public interface IUserRepository
    {
        public Task<List<object>> GetAll();
    }
}
