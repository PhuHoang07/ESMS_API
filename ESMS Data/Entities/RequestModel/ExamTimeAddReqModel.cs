using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Entities.RequestModel
{
    public class ExamTimeAddReqModel
    {
        [Required]
        public DateTime Date {  get; set; }

        [Required]
        public TimeSpan Start {  get; set; }

        [Required]
        public TimeSpan End { get; set; }

        public DateTime PublishDate { get; set; }
    }
}
