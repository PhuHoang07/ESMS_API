using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace ESMS_Data.Repositories.ExamScheduleRepository
{
    public class ExamScheduleRepository : RepositoryBase<ExamSchedule>, IExamScheduleRepository
    {
        public ExamScheduleRepository(ESMSContext context) : base(context)
        {            
        }
    }
}
