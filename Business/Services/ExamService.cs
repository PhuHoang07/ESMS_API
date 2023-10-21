using Business.Interfaces;
using ESMS_Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamTimeRepository _examTimeRepository;
        public Task<ResultModel> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
