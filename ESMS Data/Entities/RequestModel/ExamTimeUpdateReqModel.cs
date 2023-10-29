using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Entities.RequestModel
{
    public class ExamTimeUpdateReqModel
    {
        [Required]
        public int Idt {  get; set; }
        public DateTime Date {  get; set; }
        public TimeSpan Start {  get; set; }
        public TimeSpan End { get; set; }
        public DateTime PublishDate { get; set; }
    }
}
