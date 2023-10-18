using System;
using System.Collections.Generic;

#nullable disable

namespace ESMS_Data.Models
{
    public partial class Participation
    {
        public string UserName { get; set; }
        public string SubjectId { get; set; }
        public string RoomNumber { get; set; }
        public int Idt { get; set; }

        public virtual ExamSchedule ExamSchedule { get; set; }
        public virtual User UserNameNavigation { get; set; }
    }
}
