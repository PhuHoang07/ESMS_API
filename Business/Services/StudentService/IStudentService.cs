using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.StudentService
{
    public interface IStudentService
    {
        public Task<ResultModel> GetAssignedExamSchedules(string email);
        public Task<ResultModel> GetPreviewExamSchedule();

    }
}
