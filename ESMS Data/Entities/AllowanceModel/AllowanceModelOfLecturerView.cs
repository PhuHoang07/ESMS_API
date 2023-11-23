using ESMS_Data.Entities.ExamTimeModel;
using ESMS_Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Entities.AllowanceModel
{
    public class AllowanceModelOfLecturerView
    {
        public string Semester { get; set; }
        public List<ExamTimeBasicInfo> ExamTime { get; set; }
        public AllowanceModel AllowanceModel { get; set; }
    }
}
