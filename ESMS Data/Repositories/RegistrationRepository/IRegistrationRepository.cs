using ESMS_Data.Entities.AllowanceModel;
using ESMS_Data.Entities.ExamTimeModel;
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
        public Task<List<Registration>> GetRegistration(int idt);
        public Task<List<string>> GetProctorUsername(int idt);
        public Task<Registration> GetSpecificRegistrationOfProctor(int idt, string proctor);
        public Task<List<AllowanceModelOfLecturerView>> GetAllRegisteredAndAllowance(string username, List<string> semesterList);
        public Task<List<AllowanceStatistic>> GetAllowanceStatistic(List<string> proctor, List<string> semester);
        public Task<List<string>> GetSupervisorList();
        public Task<List<string>> GetAvailableProctors(int idt, List<string> assignedProctorList);
        public Task<List<User>> GetAvailable(int idt, List<string> assignedProctorList);
        public Task<List<object>> GetProctors(int idt);
        public Task<object> FormatRegisteredExamTimes(string username, string semester);
        public Task<List<ExamTime>> GetRegisteredExamTimes(string username, string semester);
        public Task<List<ExamTimeInfoModel>> GetAvailableExamTimes(List<ExamTime> registeredExamTimes, string semester);
        public int GetRegisteredAmount(int idt);
    }
}
