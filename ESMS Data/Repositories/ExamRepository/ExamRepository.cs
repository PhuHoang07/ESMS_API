using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories.ExamRepository
{
    public class ExamRepository : RepositoryBase<ExamTime>, IExamRepository
    {
        private ESMSContext _context;
        private DbSet<ExamTime> _examTimes;
        private DbSet<ExamSchedule> _examSchedules;
        private DbSet<Subject> _subjects;
        private DbSet<Slot> _slots;
        private DbSet<Room> _rooms;
        private DbSet<User> _users;
        private DbSet<Participation> _participations;
        public ExamRepository(ESMSContext context) : base(context)
        {
            _context = context;
            _examTimes = _context.Set<ExamTime>();
            _examSchedules = _context.Set<ExamSchedule>();
            _subjects = _context.Set<Subject>();
            _slots = _context.Set<Slot>();
            _rooms = _context.Set<Room>();
            _users = _context.Set<User>();
            _participations = _context.Set<Participation>();
        }

        public new IQueryable<ExamTime> GetAll()
        {
            var qr = _examTimes.Include(et => et.ExamSchedules)
                               .ThenInclude(et => et.Registration)
                               .Select(et => new ExamTime
                               {
                                   Idt = et.Idt,
                                   Date = et.Date,
                                   Start = et.Start,
                                   End = et.End,
                                   PublishDate = et.PublishDate,
                                   SlotId = et.SlotId,
                                   Semester = et.Semester,
                                   Registrations = et.Registrations,
                                   ExamSchedules = et.ExamSchedules.AsQueryable()
                                                                   .Include(es => es.Participations)
                                                                   .Include(es => es.RoomNumberNavigation)
                                                                   .ToList()
                               });

            return qr;
        }

        public IQueryable<ExamTime> FilterSemester(IQueryable<ExamTime> qr, string semester)
        {
            qr = qr.Where(et => et.Semester.Equals(semester));

            return qr;
        }

        public IQueryable<ExamTime> FilterSubject(IQueryable<ExamTime> qr, List<string> subjects)
        {
            qr = qr.Where(et => et.ExamSchedules.Any(es => subjects.Contains(es.SubjectId)))
                   .Select(et => new ExamTime
                   {
                       Idt = et.Idt,
                       Date = et.Date,
                       Start = et.Start,
                       End = et.End,
                       PublishDate = et.PublishDate,
                       SlotId = et.SlotId,
                       Semester = et.Semester,
                       Registrations = et.Registrations,
                       ExamSchedules = et.ExamSchedules
                                         .Where(es => subjects.Contains(es.SubjectId))
                                         .ToList()
                   });

            return qr;
        }

        public IQueryable<ExamTime> FilterDate(IQueryable<ExamTime> qr, DateTime from, DateTime to)
        {
            qr = qr.Where(et => et.Date >= from && et.Date <= to);

            return qr;
        }

        public IQueryable<ExamTime> FilterTime(IQueryable<ExamTime> qr, TimeSpan start, TimeSpan end)
        {
            qr = qr.Where(et => et.Start >= start && et.Start <= end);

            return qr;
        }

        public async Task<Dictionary<string, List<object>>> GroupBySemester(IQueryable<ExamTime> qr)
        {
            var list = await qr.ToListAsync();

            var group = list
                        //.OrderByDescending(e => e.Semester.Substring(e.Semester.Length - 2)
                        .GroupBy(e => e.Semester)
                        .ToList()
                        .OrderBy(gr => gr.Key.Substring(gr.Key.Length - 2))
                        .ThenBy(gr => gr.Key.Contains("FALL") ? 0 : gr.Key.Contains("SUMMER") ? 1 : 2)
                        .ToDictionary(group => group.Key,
                                    group => group
                                        .OrderBy(i => i.Date)
                                        .ThenBy(i => i.Start)
                                        .ThenBy(i => i.End)
                                        .Select(i => new
                                        {
                                            i.Idt,
                                            Date = i.Date.ToString("dd/MM/yyyy"),
                                            Start = i.Start.ToString(@"hh\:mm"),
                                            End = i.End.ToString(@"hh\:mm"),
                                            PublishDate = i.PublishDate?.ToString("dd/MM/yyyy"),
                                            Slot = i.SlotId,
                                            TotalSupervisor = i.Registrations.Count(),
                                            RequireSupervisor = GetRequireSupervisorAmount(i.Idt),
                                            ExamSchedules = i.ExamSchedules
                                                                    .OrderBy(et => et.RoomNumber)
                                                                    .Select(es => new
                                                                    {
                                                                        Subject = es.SubjectId,
                                                                        Room = es.RoomNumber,
                                                                        es.Form,
                                                                        es.Type,
                                                                        TotalStudent = es.Participations.Count(),
                                                                        Capacity = es.RoomNumberNavigation.Capacity,
                                                                        Proctor = es.Proctor
                                                                    })
                                        })
                                        .ToList<object>()
                        );

            return group;
        }

        public int GetRequireSupervisorAmount(int idt)
        {
            var count = _examTimes.Include(et => et.ExamSchedules)
                             .Where(et => et.Idt == idt)
                             .SelectMany(et => et.ExamSchedules)
                             .Count();

            // 4 room = 1 addition proctor
            return (int)Math.Round(5 / 4d * count, MidpointRounding.AwayFromZero);
        }

        public async Task<List<string>> GetSemester()
        {
            var qr = (await _examTimes.Select(et => et.Semester)
                               .Distinct()
                               .ToListAsync())
                               .OrderBy(s => s.Substring(s.Length - 2))
                               .ThenBy(s => s.Contains("FALL") ? 0 : s.Contains("SUMMER") ? 1 : 2);

            return qr.ToList();
        }

        public async Task<List<string>> GetSubject()
        {
            var qr = _subjects.Select(s => s.Id)
                              .OrderBy(s => s);

            return await qr.ToListAsync();
        }

        public async Task<int?> GetSlot(TimeSpan start)
        {
            var qr = _slots.Where(s => s.Start <= start && start <= s.End)
                           .Select(s => s.Id);

            return await qr.FirstOrDefaultAsync();
        }

        public async Task<ExamTime> GetExamTime(int idt)
        {
            return await _examTimes.FindAsync(idt);
        }

        public async Task<List<string>> GetAvailableRoom(int idt, string subjectId)
        {
            var qr = _rooms;

            //Find exception room with same idt and same subjectId
            var filteredExamTimesByIdt = _examTimes
                                    .Include(et => et.ExamSchedules)
                                    .Where(et => et.Idt == idt);

            var roomExcept = filteredExamTimesByIdt
                        .SelectMany(et => et.ExamSchedules
                                            .Where(es => es.SubjectId.Equals(subjectId))
                                            .Select(es => es.RoomNumber));

            var exceptRooms = qr.Where(room => roomExcept.Contains(room.Number));

            //Find exception room in one day
            var date = _examTimes.Where(et => et.Idt == idt)
                                 .Select(et => et.Date).FirstOrDefault();

            var start = _examTimes.Where(et => et.Idt == idt)
                                  .Select(et => et.Start).FirstOrDefault();

            var end = _examTimes.Where(et => et.Idt == idt)
                                  .Select(et => et.End).FirstOrDefault();

            var filteredExamTimesByDate = _examTimes
                                    .Include(et => et.ExamSchedules)
                                    .Where(et => (et.Date == date
                                              && et.Idt != idt
                                              && et.Start <= start
                                              && et.End >= start)
                                              ||
                                              (et.Date == date
                                              && et.Idt != idt
                                              && et.Start <= end
                                              && et.End >= end));

            var roomExceptByDay = filteredExamTimesByDate
                .SelectMany(et => et.ExamSchedules.Select(es => es.RoomNumber));

            var exceptRoomsByDay = qr.Where(room => roomExceptByDay.Contains(room.Number));

            //Get available room
            var availableRoom = _rooms.Except(exceptRoomsByDay.Concat(exceptRooms))
                                      .Select(r => r.Number);

            return await availableRoom.ToListAsync();
        }

        public async Task<ExamSchedule> GetExamSchedule(int idt, string subjectID, string roomNumber)
        {
            return await _examSchedules.Where(es => es.Idt == idt
                                                 && es.SubjectId.Equals(subjectID)
                                                 && es.RoomNumber.Equals(roomNumber)).FirstOrDefaultAsync();
        }

        public async Task<List<ExamSchedule>> GetExamScheduleWithDistinctRoom(int idt)
        {
            // Fetch the records meeting the criteria from the database
            var examSchedules = await _examSchedules
                .Where(es => es.Idt == idt && es.Proctor == null)
                .ToListAsync();

            // Group by RoomNumber using LINQ to Objects (client-side)
            var groupedSchedules = examSchedules
                .GroupBy(es => es.RoomNumber)
                .Select(es => es.First())
                .ToList();


            return groupedSchedules;
        }

        public async Task<List<string>> GetAssignedProctorList(int idt)
        {
            return await _examSchedules.Where(es => es.Idt == idt
                                                 && es.Proctor != null)
                                       .Select(es => es.Proctor).ToListAsync();
        }

        public async Task<List<ExamSchedule>> GetExamScheduleHasNoProctor(int idt)
        {
            return await _examSchedules.Where(es => es.Idt == idt
                                                 && es.Proctor == null).ToListAsync();
        }

        public async Task<List<ExamSchedule>> GetExamScheduleHasProctor(int idt)
        {
            return await _examSchedules.Where(es => es.Idt == idt
                                                 && es.Proctor != null).ToListAsync();

        }

        public async Task<List<ExamSchedule>> GetExistedExamSchedules(string username)
        {
            return await _participations.Where(p => p.UserName == username)
                                        .Include(p => p.ExamSchedule)
                                        .Select(p => p.ExamSchedule)
                                        .ToListAsync();
        }

        public async Task<DateTime> GetDate(ExamSchedule examSchedule)
        {
            return await _examTimes.Where(et => et.Idt == examSchedule.Idt)
                                   .Select(et => et.Date).FirstOrDefaultAsync();
        }

        public async Task<TimeSpan> GetStart(ExamSchedule examSchedule)
        {
            return await _examTimes.Where(et => et.Idt == examSchedule.Idt)
                                   .Select(et => et.Start).FirstOrDefaultAsync();
        }

        public async Task<TimeSpan> GetEnd(ExamSchedule examSchedule)
        {
            return await _examTimes.Where(et => et.Idt == examSchedule.Idt)
                                   .Select(et => et.End).FirstOrDefaultAsync();
        }

    }
}