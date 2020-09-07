using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoChemAPI.Models
{
    public class TimeCompare: IComparer<string>
    {
        public int Compare(string a, string b)
        {
            DateTime resulta = new DateTime();
            DateTime resultb = new DateTime();
            DateTime.TryParse(a, out resulta);
            DateTime.TryParse(b, out resultb);
            return DateTime.Compare(resulta, resultb);
        }
    }
}