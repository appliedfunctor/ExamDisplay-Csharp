using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDisplay.Library
{
    class Pathing
    {
        public static string ScheduledName()
        {
            return DateTime.Now.ToString("yyyy-MM-dd-tt");
        }

        public static string CurrentSession()
        {
            return DateTime.Now.ToString("tt");
        }
    }
}
