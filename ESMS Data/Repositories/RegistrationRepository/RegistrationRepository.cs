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

        public RegistrationRepository(ESMSContext context) : base(context)
        {
            _context = context;
            _registrations = _context.Set<Registration>();
            _examTimes = _context.Set<ExamTime>();
        }

        public async Task<List<string>> GetAvailableProctors(int idt, List<string> assignedProctorList)
        {
            var allProctor = await _registrations.Where(r => r.Idt == idt)
                                       .Select(r => r.UserName)
                                       .ToListAsync();
            return allProctor.Except(assignedProctorList).ToList();
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
                                       .Where(r => r.UserName.Equals(username)
                                                && r.IdtNavigation.Semester.Equals(semester))
                                       .Select(r => r.IdtNavigation).ToListAsync();

            var formattedExamTimes = registeredExamTimes.Select(aet => new
            {
                Date = aet.Date.ToString("dd/MM/yyyy"),
                Start = aet.Start.ToString(@"hh\:mm"),
                End = aet.End.ToString(@"hh\:mm"),
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

        public async Task<object> GetAvailableExamTimes(List<ExamTime> registeredExamTimes, string semester)
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

            var formattedExamTimes = availableExamTimes.Select(aet => new
            {
                Date = aet.Date.ToString("dd/MM/yyyy"),
                Start = aet.Start.ToString(@"hh\:mm"),
                End = aet.End.ToString(@"hh\:mm"),
            })
            .OrderBy(aet => aet.Date)
            .ToList();

            return formattedExamTimes;
        }

        public int GetRegisteredAmount(int idt)
        {
            return _registrations.Where(r => r.Idt == idt).Count();
        }
    }
}
