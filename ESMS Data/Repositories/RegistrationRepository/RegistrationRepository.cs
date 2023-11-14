using ESMS_Data.Entities;
using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Registration> GetRegistrationOfProctor(int idt, string proctor)
        {
            return await _registrations.Where(r => r.Idt == idt &&
                                                   r.UserName.Equals(proctor)).FirstOrDefaultAsync();
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
            var currentExamTimes = await _examTimes.Where(et => et.Semester.Equals(semester)).ToListAsync();

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
