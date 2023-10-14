using System;
using System.Collections.Generic;

#nullable disable

namespace ESMS_Data.Models
{
    public partial class Slot
    {
        public Slot()
        {
            ExamTimes = new HashSet<ExamTime>();
        }

        public int Id { get; set; }
        public TimeSpan? Start { get; set; }
        public TimeSpan? End { get; set; }

        public virtual ICollection<ExamTime> ExamTimes { get; set; }
    }
}
