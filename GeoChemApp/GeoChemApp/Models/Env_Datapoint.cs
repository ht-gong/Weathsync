using System;
using System.Collections.Generic;
using System.Text;

namespace GeoChemApp.Models
{
    [Serializable()]
    public class Env_Datapoint
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

        public bool Equals(Env_Datapoint obj)
        {

            if (Id != obj.Id || Concentration != obj.Concentration || Time != obj.Time || Continued_time != obj.Continued_time || Monitor_Error != obj.Monitor_Error || Pollution_Level != obj.Pollution_Level || Field != obj.Field)
                return false;
            return true;
        }
    }
}
