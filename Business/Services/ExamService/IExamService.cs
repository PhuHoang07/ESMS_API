using ESMS_Data.Entities.RequestModel;
using ESMS_Data.Entities.RequestModel.ExamScheduleReqModel;
using ESMS_Data.Entities.RequestModel.ExamTimeReqModel;
using ESMS_Data.Entities.RequestModel.ParticipationReqModel;
using ESMS_Data.Entities.RequestModel.RegistrationReqModel;
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
        public Task<ResultModel> GetSemesters();
        public Task<ResultModel> GetSubjects();
        public Task<ResultModel> GetAvailableRooms(int idt, string subjectId);
        public Task<ResultModel> AddTime(ExamTimeAddReqModel req);
        public Task<ResultModel> UpdateTime(ExamTimeUpdateReqModel req);
        public Task<ResultModel> DeleteTime(int idt);
        public Task<ResultModel> AddExamSchedule(ExamScheduleAddReqModel req);
        public Task<ResultModel> UpdateExamSchedule(ExamScheduleUpdateReqModel req);
        public Task<ResultModel> DeleteExamSchedule(ExamScheduleDeleteReqModel req);
        public Task<ResultModel> AddProctorToExamTime(RegistrationAddRemoveReqModel req);
        public Task<ResultModel> RemoveProctorFromExamTime(RegistrationAddRemoveReqModel req);
        public Task<ResultModel> GetStudents(int idt, string subject, string room);
        public Task<ResultModel> AddStudents(ParticipationAddRemoveReqModel req);
        public Task<ResultModel> RemoveStudents(ParticipationAddRemoveReqModel req);
        public Task<ResultModel> UpdateProctorsToExamSchedule(int idt);
        public Task<ResultModel> ViewProctorList(int idt);

    }
}
