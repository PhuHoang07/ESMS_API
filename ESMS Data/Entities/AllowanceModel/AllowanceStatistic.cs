using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMS_Data.Entities.AllowanceModel
{
    public class AllowanceStatistic
    {
        public string Semester { get; set; }
        public List<AllowanceModel> allowances { get; set; }
    }
}
