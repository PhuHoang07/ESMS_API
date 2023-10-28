using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Entities.RequestModel
{
    public class ExamScheduleAddReqModel
    {
        public int Idt { get; set; }

        public string SubjectID { get; set; }

        public string? RoomNumber { get; set; }

        public string Form {  get; set; }

        public string Type { get; set;}

    }
}
