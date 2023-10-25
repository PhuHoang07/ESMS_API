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
            int month = DateTime.Now.Month;
            String year = DateTime.Now.ToString("yy");
            String semester;

            if (month < 5)
            {
                semester = "SPRING";
            }else if (month < 9)
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
