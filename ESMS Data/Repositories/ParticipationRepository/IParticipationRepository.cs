using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories.ParticipationRepository
{
    public interface IParticipationRepository : IRepositoryBase<Participation>
    {
        public Task<object> GetStudentListInExam(int idt, string subject, string room);
        public Task<List<Participation>> GetParticipationList(int idt, string subject, string room);
        public Task<List<string>> GetStudentListInParticipation(int idt, string subject, string room);
        public Task<int?> GetRoomCapacity(string room);
        public Task<int> GetTotalStudentInRoom(int idt, string room);
        public Task<List<Participation>> GetParticipationsOnList(int idt, string subject, string room, List<string> students);
        public Task<object> GetOwnExamSchedule(string username, string semester);
        public Task<object> GetPreviewExamScheduleList(string semester);

    }
}
