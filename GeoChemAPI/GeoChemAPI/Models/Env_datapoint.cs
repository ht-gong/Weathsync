using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoChemAPI.Models
{
    public class Env_datapoint
    {
        public int Id { get; set; }
        public double Concentration { get; set; }
        public string Time { get; set; }
        public string Continued_time { get; set; }
        public bool IsMonitorError { get; set; }
        public string Monitor_Error { get; set; }
        public bool IsHighPollution { get; set; }
        public string Pollution_Level { get; set; }
        public string Field { get; set; }
        
        public DateTime GetDateTime()
        {
            DateTime a = new DateTime();
            DateTime.TryParse(Time, out a);
            return a;
        }

    }
}