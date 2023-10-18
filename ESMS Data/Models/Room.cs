using System;
using System.Collections.Generic;

#nullable disable

namespace ESMS_Data.Models
{
    public partial class Room
    {
        public Room()
        {
            ExamSchedules = new HashSet<ExamSchedule>();
        }

        public string Number { get; set; }
        public int? Capacity { get; set; }

        public virtual ICollection<ExamSchedule> ExamSchedules { get; set; }
    }
}
