using ESMS_Data.Entities;
using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories.ParticipationRepository
{
    public class ParticipationRepository : RepositoryBase<Participation>, IParticipationRepository
    {
        private ESMSContext _context;
        private DbSet<Participation> _participations;
        private DbSet<User> _users;
        private DbSet<Room> _rooms;
        private DbSet<ExamSchedule> _examSchedules;
        public ParticipationRepository(ESMSContext context) : base(context)
        {
            _context = context;
            _participations = _context.Set<Participation>();
            _users = _context.Set<User>();
            _rooms = _context.Set<Room>();
            _examSchedules = _context.Set<ExamSchedule>();
        }

        public async Task<object> GetStudentListInExam(int idt, string subject, string room)
        {
            var studentUserNames = await _participations.Where(p => p.Idt == idt
                                                     && p.SubjectId.Equals(subject)
                                                     && p.RoomNumber.Equals(room))
                                            .Select(p => p.UserName)
                                            .ToListAsync();

            var students = _users.Where(u => studentUserNames.Contains(u.UserName))
                                 .Select(u => new
                                 {
                                     u.UserName,
                                     u.Name,
                                     u.RollNumber
                                 });

            return await students.ToListAsync();
        }

        public async Task<List<Participation>> GetParticipationList(int idt, string subject, string room)
        {
            return await _participations.Where(p => p.Idt == idt &&
                                              p.SubjectId.Equals(subject) &&
                                              p.RoomNumber.Equals(room))
                                        .ToListAsync();
        }

        public async Task<List<User>> GetListToExportExcel(int idt, string subject, string room)
        {
            return await _participations.Include(p => p.UserNameNavigation)
                                        .Where(p => p.Idt == idt &&
                                              p.SubjectId.Equals(subject) &&
                                              p.RoomNumber.Equals(room))
                                        .Select(p => p.UserNameNavigation)
                                        .ToListAsync();
        }

        public async Task<List<Participation>> GetParticipationListWithIdt(int idt)
        {
            return await _participations.Where(p => p.Idt == idt)
                                        .ToListAsync();
        }

        public async Task<List<string>> GetStudentListInParticipation(int idt, string subject, string room)
        {
            return await _participations.Where(p => p.Idt == idt &&
                                              p.SubjectId.Equals(subject) &&
                                              p.RoomNumber.Equals(room))
                                        .Select(p => p.UserName)
                                        .ToListAsync();
        }

        public async Task<int?> GetRoomCapacity(string room)
        {
            return await _rooms.Where(r => r.Number.Equals(room))
                               .Select(r => r.Capacity)
                               .FirstAsync();
        }

        public async Task<int> GetTotalStudentInRoom(int idt, string room)
        {
            var qr = _participations.Where(p => p.Idt == idt && p.RoomNumber.Equals(room));

            return await qr.CountAsync();
        }

        public async Task<List<Participation>> GetParticipationsOnList(int idt, string subject, string room, List<string> students)
        {
            return await _participations.Where(p => p.Idt == idt
                                                 && p.SubjectId.Equals(subject)
                                                 && p.RoomNumber.Equals(room)
                                                 && students.Contains(p.UserName))
                                        .ToListAsync();
        }

        public async Task<object> GetOwnExamSchedule(string username, string semester)
        {
            var schedules = await _participations
                .Where(p => p.UserName.Equals(username) && p.ExamSchedule.IdtNavigation.Semester.Equals(semester))
                .Include(p => p.ExamSchedule)
                .Include(p => p.ExamSchedule.IdtNavigation)
                .Include(p => p.ExamSchedule.Subject)
                .Select(p => new
                {
                    SubjectId = p.SubjectId,
                    SubjectName = p.ExamSchedule.Subject.Name,
                    Date = p.ExamSchedule.IdtNavigation.Date,
                    Room = p.RoomNumber,
                    Time = $"{p.ExamSchedule.IdtNavigation.Start.ToString(@"hh\:mm")} - {p.ExamSchedule.IdtNavigation.End.ToString(@"hh\:mm")}",
                    Form = p.ExamSchedule.Form,
                    Type = p.ExamSchedule.Type,
                    PublishDate = p.ExamSchedule.IdtNavigation.PublishDate
                })
                .ToListAsync();

            var formattedSchedules = schedules.Select(p => new
            {
                p.SubjectId,
                p.SubjectName,
                Date = p.Date.ToString("dd/MM/yyyy"),
                p.Room,
                p.Time,
                p.Form,
                p.Type,
                PublishDate = p.PublishDate != null ? p.PublishDate.Value.ToString("dd/MM/yyyy") : "N/A"
            })
            .OrderBy(p => p.Date);

            return formattedSchedules.ToList();
        }

        public async Task<object> GetPreviewExamScheduleList(string semester)
        {
            var selectedSchedules = _examSchedules.Where(es => es.IdtNavigation.Semester.Equals(semester))
                                                  .Include(p => p.IdtNavigation)
                                                  .AsEnumerable()
                                                  .GroupBy(es => new
                                                  {
                                                      es.Idt,
                                                      es.SubjectId
                                                  })
                                                  .Select(es => es.OrderBy(es => es.RoomNumber).First())
                                                  .Select(es => new
                                                  {
                                                      SubjectId = es.SubjectId,
                                                      Date = es.IdtNavigation.Date,
                                                      Time = $"{es.IdtNavigation.Start.ToString(@"hh\:mm")} - {es.IdtNavigation.End.ToString(@"hh\:mm")}",
                                                      Form = es.Form,
                                                      Type = es.Type,
                                                      PublishDate = es.IdtNavigation.PublishDate
                                                  })
                                                  .OrderBy(es => es.SubjectId)
                                                  .ToList();



            var formattedSchedules = selectedSchedules.Select(p => new
            {
                p.SubjectId,
                Date = p.Date.ToString("dd/MM/yyyy"),
                p.Time,
                p.Form,
                p.Type,
                PublishDate = p.PublishDate != null ? p.PublishDate.Value.ToString("dd/MM/yyyy") : "N/A"
            });

            return await Task.FromResult(formattedSchedules.ToList());
        }

        private async Task<DataTable> GetExamScheduleInfo(int idt, string subjectId, string room)
        {
            DataTable dt = new DataTable();

            dt.TableName = $"{room}";
            dt.Columns.Add("Student code", typeof(string));
            dt.Columns.Add("Full Name", typeof(string));
            dt.Columns.Add("Check Attendance", typeof(string));
            dt.Columns.Add("Finished Time", typeof(string));
            dt.Columns.Add("Signature", typeof(string));

            var studentList = await GetListToExportExcel(idt, subjectId, room);
            if (studentList.Count > 0)
            {
                studentList.ForEach(sl =>
                {
                    dt.Rows.Add(sl.UserName, sl.Name, "", "", "");
                });
            }
            return dt;
        }

        public async Task<List<DataTable>> GetExamScheduleInfoToExport(int idt)
        {
            var examSchedules = await _examSchedules.Where(es => es.Idt == idt)
                                                    .OrderBy(es => es.RoomNumber)
                                                    .ToListAsync();

            var dataTableList = new List<DataTable>();

            foreach (var examSchedule in examSchedules)
            {
                dataTableList.Add(await GetExamScheduleInfo(examSchedule.Idt, examSchedule.SubjectId, examSchedule.RoomNumber));
            }

            return dataTableList;
        }

    }
}
