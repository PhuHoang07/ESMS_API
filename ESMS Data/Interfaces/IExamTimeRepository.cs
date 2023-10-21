using ESMS_Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Interfaces
{
    public interface IExamTimeRepository : IRepositoryBase<ExamTime>
    {
        public Task<object> GetAll();
    }
}
