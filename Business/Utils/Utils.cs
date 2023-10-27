using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Utils
{
    public class Utils
    {
        public String GetCurrentSemester()
        {
            return GetSemester(DateTime.Now);
        }

        public String GetSemester(DateTime date)
        {
            int month = date.Month;
            String year = date.ToString("yy");

            String semester;
            if (month < 5)
            {
                semester = "SPRING";
            }
            else if (month < 9)
            {
                semester = "SUMMER";
            }
            else
            {
                semester = "FALL";
            }

            semester += year;

            return semester;
        }
    }

}
