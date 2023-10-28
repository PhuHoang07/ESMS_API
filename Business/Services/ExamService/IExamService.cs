using ESMS_Data.Entities.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.ExamService
{
    public interface IExamService
    {
        public Task<ResultModel> GetCurrent();
        public Task<ResultModel> Get(ExamFilterReqModel req);
        public Task<ResultModel> GetSemester();
        public Task<ResultModel> GetSubject();
        public Task<ResultModel> AddTime(ExamTimeAddReqModel req);
        public Task<ResultModel> UpdateTime(ExamTimeUpdReqModel req);
        public Task<ResultModel> DeleteTime(int idt);
        public Task<ResultModel> AddExamSchedule(ExamScheduleAddReqModel req);
    }
}
