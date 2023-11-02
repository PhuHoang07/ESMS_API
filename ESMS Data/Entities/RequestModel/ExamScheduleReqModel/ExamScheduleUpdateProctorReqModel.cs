using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Entities.RequestModel.ExamScheduleReqModel
{
    public class ExamScheduleUpdateProctorReqModel
    {
        [Required]
        public int Idt { get; set; }

        [Required]
        public string SubjectID { get; set; }

        [Required]
        public string RoomNumber { get; set; }
        public string UpdProctor {  get; set; }
    }
}
