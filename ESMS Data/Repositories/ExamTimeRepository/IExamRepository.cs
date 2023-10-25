using ESMS_Data.Models;
using ESMS_Data.Repositories.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Repositories.ExamTimeRepository
{
    public interface IExamRepository : IRepositoryBase<ExamTime>
    {
        public IQueryable<ExamTime> FilterSemester(IQueryable<ExamTime> qr, string semester);
        public IQueryable<ExamTime> FilterSubject(IQueryable<ExamTime> qr, List<string> subject);
        public IQueryable<ExamTime> FilterDate(IQueryable<ExamTime> qr, DateTime from, DateTime to);
        public IQueryable<ExamTime> FilterTime(IQueryable<ExamTime> qr, TimeSpan start, TimeSpan end);
        public Task<Dictionary<string, List<object>>> GroupBySemester(IQueryable<ExamTime> qr);
    }
}