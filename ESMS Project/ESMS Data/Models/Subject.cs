using System;
using System.Collections.Generic;

#nullable disable

namespace ESMS_Data.Models
{
    public partial class Subject
    {
        public Subject()
        {
            ExamSchedules = new HashSet<ExamSchedule>();
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ExamSchedule> ExamSchedules { get; set; }
    }
}
