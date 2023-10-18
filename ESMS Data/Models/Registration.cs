using System;
using System.Collections.Generic;

#nullable disable

namespace ESMS_Data.Models
{
    public partial class Registration
    {
        public Registration()
        {
            ExamSchedules = new HashSet<ExamSchedule>();
        }

        public string UserName { get; set; }
        public int Idt { get; set; }

        public virtual ExamTime IdtNavigation { get; set; }
        public virtual User UserNameNavigation { get; set; }
        public virtual ICollection<ExamSchedule> ExamSchedules { get; set; }
    }
}
