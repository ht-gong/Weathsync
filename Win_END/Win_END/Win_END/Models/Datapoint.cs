using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Win_END.Models
{
    class Datapoint
    {
        public double Concentration { get; set; }
        public DateTime Time { get; set; }
        public TimeSpan Continued_time { get; set; }
        public bool IsMonitorError { get; set; }
        public string Monitor_Error { get; set; }
        public bool IsHighPollution { get; set; }
        public string Pollution_Level { get; set; }
        public string Field { get; set; }

        public bool isProblematic()
        {
            if (IsMonitorError || IsHighPollution)
                return true;
            else
                return false;
        }
        public bool Equals(Datapoint compare)
        {
            if (this.isProblematic() && compare.isProblematic())
                if (Monitor_Error == compare.Monitor_Error && Pollution_Level == compare.Pollution_Level &&compare.Field == Field)
                    return true;
            return false;
        }
        
        public void Mergewith(Datapoint merging,TimeSpan timeSpan)
        {
            Continued_time += merging.Continued_time;
            Continued_time += timeSpan;

        }

        public void Reset()
        {
            Continued_time = new TimeSpan(0);
            IsHighPollution = false;
            IsMonitorError = false;
            Monitor_Error = "";
            Pollution_Level = "";
            Field = "";
        }

        public void Copy(Datapoint copied)
        {
            Concentration = copied.Concentration;
            Time = copied.Time;
            Continued_time = copied.Continued_time;
            IsHighPollution = copied.IsHighPollution;
            IsMonitorError = copied.IsMonitorError;
            Monitor_Error = copied.Monitor_Error;
            Pollution_Level = copied.Pollution_Level;
            Field = copied.Field;
        }

    
    }
}
