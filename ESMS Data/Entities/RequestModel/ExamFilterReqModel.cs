using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Entities.RequestModel
{
    public class ExamFilterReqModel
    {
        public string? Semester {  get; set; }
        public List<string>? Subjects { get; set; }
        public DateTime From { get; set; } = DateTime.MinValue;
        public DateTime To { get; set; } = DateTime.MaxValue;
        public TimeSpan Start { get; set; } = new TimeSpan(00, 00, 00);
        public TimeSpan End { get; set; } = new TimeSpan(23, 59, 00);
    }
}