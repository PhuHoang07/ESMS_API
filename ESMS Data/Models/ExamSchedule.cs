using System;
using System.Collections.Generic;

#nullable disable

namespace ESMS_Data.Models
{
    public partial class ExamSchedule
    {
        public ExamSchedule()
        {
            Participations = new HashSet<Participation>();
        }

        public int Idt { get; set; }
        public string SubjectId { get; set; }
        public string RoomNumber { get; set; }
        public string Form { get; set; }
        public string Type { get; set; }
        public string Proctor { get; set; }

        public virtual ExamTime IdtNavigation { get; set; }
        public virtual Registration Registration { get; set; }
        public virtual Room RoomNumberNavigation { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual ICollection<Participation> Participations { get; set; }
    }
}
