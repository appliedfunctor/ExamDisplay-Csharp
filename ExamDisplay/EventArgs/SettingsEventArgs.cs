using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDisplay
{
    public class SettingsEventArgs
    {
        public string ExamDisplay { get; set; }
        public string StartDisplay { get; set; }
        public string FinishDisplay { get; set; }

        public TimeSpan Duration { get; set; }
        public TimeSpan StartTime { get; set; }

        public int DurationHours { get; set; }
        public int DurationMinutes { get; set; }
        public int StartHours { get; set; }
        public int StartMinutes { get; set; }

        public string Centre { get; set; }
        public string Unit { get; set; }
        public string Board { get; set; }
        public string Subject { get; set; }
    }
}
