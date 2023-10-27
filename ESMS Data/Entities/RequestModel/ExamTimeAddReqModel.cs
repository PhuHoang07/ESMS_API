using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Entities.RequestModel
{
    public class ExamTimeAddReqModel
    {
        public DateTime Date {  get; set; }
        public TimeSpan Start {  get; set; }
        public TimeSpan End { get; set; }
        public DateTime PublishDate { get; set; }
    }
}
