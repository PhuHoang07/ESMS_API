using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.LecturerService
{
    public interface ILecturerService
    {
        public Task<ResultModel> GetRegisteredExamTimes(string email);
        public Task<ResultModel> GetAvailableExamTimes(string email);

    }
}
