using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories.ExamTimeRepository
{
    public class ExamTimeRepository : RepositoryBase<ExamTime>, IExamTimeRepository
    {
        private ESMSContext _context;
        private DbSet<ExamTime> _examTimes;
        private DbSet<ExamSchedule> _examSchedules;
        public ExamTimeRepository(ESMSContext context) : base(context)
        {
            _context = context;
            _examTimes = _context.Set<ExamTime>();
            _examSchedules = _context.Set<ExamSchedule>();
        }

        public IQueryable<ExamTime> FilterSemester(IQueryable<ExamTime> qr, string semester)
        {
            qr = qr.Where(et => et.Semester.Equals(semester));

            return qr;
        }

        public IQueryable<ExamTime> FilterSubject(IQueryable<ExamTime> qr, List<string> subject)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ExamTime> FilterDate(IQueryable<ExamTime> qr, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ExamTime> FilterTime(IQueryable<ExamTime> qr, TimeSpan start, TimeSpan end)
        {
            throw new NotImplementedException();
        }

        public async Task<List<object>> GroupBySemester(IQueryable<ExamTime> qr)
        {
            var list = await qr.ToListAsync();

            var group = list
                        .GroupBy(e => e.Semester)
                        .Select(group => new
                        {
                            Semester = group.Key,
                            Exams = group.Select(i => new
                            {
                                i.Idt,
                                Date = i.Date.ToString("dd/MM/yyyy"),
                                Start = i.Start.ToString(@"hh\:mm"),
                                End = i.End.ToString(@"hh\:mm"),
                                PublishDate = i.PublishDate?.ToString("dd/MM/yyyy"),
                                Slot = i.SlotId,
                                ExamSchedules = _examSchedules.Where(es => es.Idt == i.Idt)
                                                              .Select(es => new
                                                              {
                                                                  Subject = es.SubjectId,
                                                                  Room = es.RoomNumber,
                                                                  es.Form,
                                                                  es.Type
                                                              })
                            })
                        });

            return group.ToList<object>();
        }
    }
}
