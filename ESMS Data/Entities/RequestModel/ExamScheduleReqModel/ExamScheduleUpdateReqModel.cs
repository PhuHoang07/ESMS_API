using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Entities.RequestModel.ExamScheduleReqModel
{
    public class ExamScheduleUpdateReqModel
    {
        [Required]
        public int Idt { get; set; }

        [Required]
        public string SubjectID { get; set; }

        [Required]
        public string RoomNumber { get; set; }

        public string? UpdSubjectID { get; set; }

        public string? UpdRoomNumber { get; set; }

        public string? UpdForm { get; set; }

        public string? UpdType { get; set; }

    }
}
