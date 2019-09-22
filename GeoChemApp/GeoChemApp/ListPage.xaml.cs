using GeoChemApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoChemApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ListPage : ContentPage
	{
        public ObservableCollection<Display_Datapoint> displays;
		public ListPage ()
		{
			InitializeComponent ();
            displays = new ObservableCollection<Display_Datapoint>();
            ConvertDatapoints();
		}

        private void ConvertDatapoints()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var file = Path.Combine(documents, "GeoChemDatapoints.xml");
            var list = new List<Env_Datapoint>();
            displays = new ObservableCollection<Display_Datapoint>();
            

            list = GetService.ReadfromXML(file);
            //int i = 0;
            foreach(Env_Datapoint item in list)
            {
                displays.Add(new Display_Datapoint()
                {
                    ImageSource = FindPictures(item.Field),
                    TimePeriod = AddTimePeriod(item.Time, item.Continued_time),
                    MonitorErrors = FindMonitorerr(item.Monitor_Error),
                    Pollution_Lvl = FindPollutionLvl(item.Pollution_Level)
                });
            }

            LVFoundData.ItemsSource = displays;

        }

        private string FindPollutionLvl(string pollution_Level)
        {
            string str = "";
            if (pollution_Level.Contains("Intermidiate Pollution")) 
                str += "中度污染";
            if (pollution_Level.Contains("Heavy Pollution"))
                str += "重度污染";
            if (pollution_Level.Contains("Severe Pollution"))
                str += "严重污染";
            return str;
        }

        private string FindMonitorerr(string monitor_Error)
        {
            string str = "";
            if (monitor_Error.Contains("valuetoosmall"))
                str += "仪器数据过小";
            if (monitor_Error.Contains("valuetoobig"))
                str += "仪器数据过大";
            return str;
                
        }

        private string AddTimePeriod(string time, string continued_time)
        {
            DateTime output = new DateTime();
            DateTime.TryParse(time, out output);
            TimeSpan output2 = new TimeSpan();
            TimeSpan.TryParse(continued_time, out output2);
            if (output2 != new TimeSpan(0))
                return string.Format("{0} -- {1}", output.ToString("yyyy/MM/dd HH:mm:ss"), output.Add(output2).ToString("yyyy/MM/dd HH:mm:ss"));
            else return output.ToString("yyyy/MM/dd HH:mm:ss");
        }

        private string FindPictures(string field)
        {
            switch (field)
            {
                case "PM2.5":
                    return "PM2point5.png";
                case "PM10":
                    return "PM10.png";
                case "SO2":
                    return "so2.png";
                case "NO2":
                    return "no2.png";
                case "O3":
                    return "O3.png";
                case "CO":
                    return "co.png";
                case "TSP":
                    return "tsp.png";
                case "NOx":
                    return "nox.png";
                case "BaP":
                    return "BaP.png";
            default: return "";
        }
        }
    }
}