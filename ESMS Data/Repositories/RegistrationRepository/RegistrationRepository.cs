using ESMS_Data.Entities.AllowanceModel;
using ESMS_Data.Entities.ExamTimeModel;
using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ESMS_Data.Repositories.RegistrationRepository
{
    public class RegistrationRepository : RepositoryBase<Registration>, IRegistrationRepository
    {
        private ESMSContext _context;
        private DbSet<Registration> _registrations;
        private DbSet<ExamTime> _examTimes;
        private DbSet<User> _users;

        public RegistrationRepository(ESMSContext context) : base(context)
        {
            _context = context;
            _registrations = _context.Set<Registration>();
            _examTimes = _context.Set<ExamTime>();
            _users = _context.Set<User>();
        }

        public async Task<List<Registration>> GetRegistration(int idt)
        {
            return await _registrations.Where(r => r.Idt == idt).ToListAsync();
        }

        public async Task<List<string>> GetProctorUsername(int idt)
        {
            return await _registrations.Where(r => r.Idt == idt)
                                       .Select(r => r.UserName)
                                       .ToListAsync();
        }

        public async Task<Registration> GetSpecificRegistrationOfProctor(int idt, string proctor)
        {
            return await _registrations.Where(r => r.Idt == idt &&
                                                   r.UserName.Equals(proctor)).FirstOrDefaultAsync();
        }

        private async Task<List<ExamTimeBasicInfo>> GetRegisteredListInSemester(string username, string semester)
        {
            var examTimes = await _registrations.Where(r => r.UserName.Equals(username) &&
                                                   r.IdtNavigation.Semester.Equals(semester))
                                       .Include(r => r.IdtNavigation)
                                       .Select(r => r.IdtNavigation).ToListAsync();

            var basicInfo = examTimes.Select(et => new ExamTimeBasicInfo
            {
                Date = et.Date.ToString("dd/MM/yyyy"),
                Start = et.Start.ToString(@"hh\:mm"),
                End = et.End.ToString(@"hh\:mm")
            }).ToList();

            return basicInfo;
        }

        private async Task<AllowanceModel> GetAllowanceOfProctorBySemester(string username, string semester)
        {
            var registrations = await _registrations
                .Where(r => r.UserName.Equals(username) && r.IdtNavigation.Semester.Equals(semester))
                .Include(r => r.IdtNavigation)
                .Include(r => r.UserNameNavigation)
                .ToListAsync();

            var allowance = new AllowanceModel();

            if (registrations.Any())
            {
                allowance.Username = username;
                allowance.Fullname = registrations.First().UserNameNavigation.Name;
                allowance.TotalTime = Math.Round(registrations.Sum(r => (r.IdtNavigation.End - r.IdtNavigation.Start).TotalHours), 2);
                allowance.Allowance = string.Format("{0:N0}VND", registrations.Sum(r => (r.IdtNavigation.End - r.IdtNavigation.Start).TotalHours) * 100000).Replace(",", ".");
            }

            return allowance;
        }

        public async Task<List<AllowanceModelOfLecturerView>> GetAllRegisteredAndAllowance(string username, List<string> semesterList)
        {
            var allowances = new List<AllowanceModelOfLecturerView>();
            foreach (var semester in semesterList)
            {
                allowances.Add(new AllowanceModelOfLecturerView
                {
                    Semester = semester,
                    ExamTime = await GetRegisteredListInSemester(username, semester),
                    AllowanceModel = await GetAllowanceOfProctorBySemester(username, semester)
                });
            }
            return allowances;
        }

        private async Task<List<AllowanceModel>> GetAllExamTimeOfProctorsBySemester(List<string> usernames, string semester)
        {
            var registrations = await _registrations
                .Where(r => usernames.Contains(r.UserName) && r.IdtNavigation.Semester.Equals(semester))
                .Include(r => r.IdtNavigation)
                .Include(r => r.UserNameNavigation)
                .ToListAsync();

            var groupedRegistrations = registrations
                .AsEnumerable()
                .GroupBy(r => r.UserName)
                .Select(group => new AllowanceModel
                {
                    Username = group.Key,
                    Fullname = group.First().UserNameNavigation.Name,
                    TotalTime = Math.Round(group.Sum(r => (r.IdtNavigation.End - r.IdtNavigation.Start).TotalHours), 2),
                    Allowance = string.Format("{0:N0}VND", group.Sum(r => (r.IdtNavigation.End - r.IdtNavigation.Start).TotalHours) * 100000).Replace(",", ".")
                })
                .ToList();

            return groupedRegistrations;
        }

        public async Task<List<AllowanceStatistic>> GetAllowanceStatistic(List<string> username, List<string> semesterList)
        {
            var allowanceStatistic = new List<AllowanceStatistic>();
            foreach (var semester in semesterList)
            {
                allowanceStatistic.Add(new AllowanceStatistic
                {
                    Semester = semester,
                    allowances = await GetAllExamTimeOfProctorsBySemester(username, semester)
                });
            }

            return allowanceStatistic;
        }

        public async Task<List<string>> GetSupervisorList()
        {
            var teachers = await _users.Where(u => u.RoleId == 4)
                                       .Select(u => u.UserName).ToListAsync();
            var registrations = await _registrations.Select(u => u.UserName).ToListAsync();
            return teachers.Union(registrations).ToList();
        }

        public async Task<List<string>> GetAvailableProctors(int idt, List<string> assignedProctorList)
        {
            var allProctor = await _registrations.Where(r => r.Idt == idt)
                                       .Select(r => r.UserName)
                                       .ToListAsync();
            return allProctor.Except(assignedProctorList).ToList();

        }

        public async Task<List<User>> GetAvailable(int idt, List<string> assignedProctorList)
        {
            var allProctor = await _registrations.Where(r => r.Idt == idt)
                                                 .Select(r => r.UserName)
                                                 .ToListAsync();

            var available = allProctor.Except(assignedProctorList, StringComparer.OrdinalIgnoreCase)
                                      .Select(username => username.Trim())
                                      .ToList();

            var users = await _users.ToListAsync();

            return users.Where(u => available.Contains(u.UserName.Trim(), StringComparer.OrdinalIgnoreCase))
                        .ToList();
        }

        public async Task<List<object>> GetProctors(int idt)
        {
            return await _registrations.Where(r => r.Idt == idt)
                                       .Include(r => r.UserNameNavigation)
                                       .Select(r => new
                                       {
                                           Username = r.UserName,
                                           Name = r.UserNameNavigation.Name,
                                           Email = r.UserNameNavigation.Email
                                       })
                                       .ToListAsync<object>();
        }

        public async Task<object> FormatRegisteredExamTimes(string username, string semester)
        {
            var registeredExamTimes = await _registrations.Include(r => r.IdtNavigation)
                                                           .Where(r => r.UserName.Equals(username) &&
                                                                       r.IdtNavigation.Semester.Equals(semester) &&
                                                                       r.IdtNavigation.IsPublic == true)
                                                           .Select(r => r.IdtNavigation).ToListAsync();

            var formattedExamTimes = registeredExamTimes.Select(aet => new
            {
                Idt = aet.Idt,
                Date = aet.Date.ToString("dd/MM/yyyy"),
                Start = aet.Start.ToString(@"hh\:mm"),
                End = aet.End.ToString(@"hh\:mm")
            })
            .OrderBy(aet => aet.Date)
            .ToList();

            return formattedExamTimes;
        }

        public async Task<List<ExamTime>> GetRegisteredExamTimes(string username, string semester)
        {
            return await _registrations.Include(r => r.IdtNavigation)
                                       .Where(r => r.UserName.Equals(username)
                                                && r.IdtNavigation.Semester.Equals(semester))
                                       .Select(r => r.IdtNavigation).ToListAsync();
        }

        public async Task<List<ExamTimeInfoModel>> GetAvailableExamTimes(List<ExamTime> registeredExamTimes, string semester)
        {
            var currentExamTimes = await _examTimes.Where(et => et.Semester.Equals(semester) &&
                                                                et.IsPublic == true).ToListAsync();

            var availableExamTimes = currentExamTimes.Except(registeredExamTimes).ToList();

            foreach (var examTimes in registeredExamTimes)
            {
                for (int i = availableExamTimes.Count - 1; i >= 0; i--)
                {
                    var check = availableExamTimes[i];
                    if ((examTimes.Date == check.Date &&
                        examTimes.Start >= check.Start &&
                        examTimes.Start <= check.End)
                        ||
                        (examTimes.Date == check.Date &&
                        examTimes.End >= check.Start &&
                        examTimes.End <= check.End)
                        )
                    {
                        availableExamTimes.RemoveAt(i);
                    }
                }
            }

            var formattedExamTimes = availableExamTimes.Where(ae => ae.IsPublic == true)
                                                       .Select(aet => new ExamTimeInfoModel
                                                       {
                                                           Idt = aet.Idt,
                                                           Date = aet.Date.ToString("dd/MM/yyyy"),
                                                           Start = aet.Start.ToString(@"hh\:mm"),
                                                           End = aet.End.ToString(@"hh\:mm"),
                                                           Required = GetRequireSupervisorAmount(aet.Idt),
                                                           Registered = GetRegisteredAmount(aet.Idt),
                                                           Amount = $"{GetRegisteredAmount(aet.Idt)}/{GetRequireSupervisorAmount(aet.Idt)}"
                                                       })
                                                      .OrderBy(aet => aet.Date)
                                                      .ToList();

            return formattedExamTimes;
        }

        public int GetRegisteredAmount(int idt)
        {
            return _registrations.Where(r => r.Idt == idt).Count();
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
    }
}
