using ESMS_Data.Entities;
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
        public Task<Registration> GetRegistrationOfProctor(int idt, string proctor);
        public Task<List<string>> GetAvailableProctors(int idt, List<string> assignedProctorList);
        public Task<List<User>> GetAvailable(int idt, List<string> assignedProctorList);
        public Task<List<object>> GetProctors(int idt);
        public Task<object> FormatRegisteredExamTimes(string username, string semester);
        public Task<List<ExamTime>> GetRegisteredExamTimes(string username, string semester);
        public Task<List<ExamTimeInfoModel>> GetAvailableExamTimes(List<ExamTime> registeredExamTimes, string semester);
        public int GetRegisteredAmount(int idt);
    }
}
