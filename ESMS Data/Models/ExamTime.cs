using System;
using System.Collections.Generic;

#nullable disable

namespace ESMS_Data.Models
{
    public partial class ExamTime
    {
        public ExamTime()
        {
            ExamSchedules = new HashSet<ExamSchedule>();
            Registrations = new HashSet<Registration>();
        }

        public int Idt { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public DateTime? PublishDate { get; set; }
        public int? SlotId { get; set; }
        public string Semester { get; set; }
        public virtual Slot Slot { get; set; }
        public virtual ICollection<ExamSchedule> ExamSchedules { get; set; }
        public virtual ICollection<Registration> Registrations { get; set; }
    }
}
