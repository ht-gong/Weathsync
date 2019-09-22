using GeoChemApp.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoChemApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
        public HomePage ()
		{
           
			InitializeComponent ();
		}


        public void switchcell_Toggled(object sender, ToggledEventArgs e)
        {
            try
            {


                Application.Current.Properties["SwitchOnOff"] = switchcell.IsToggled;
                if (e.Value == true)
                {
                    var message = new StartSrvcMessage();
                    MessagingCenter.Send(message, "StartSrvcMessage");
                }
                else if (e.Value == false)
                {
                    var message = new StopSrvcMessage();
                    MessagingCenter.Send(message, "StopSrvcMessage");
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void ToggleCell(bool e)
        {
            switchcell.IsToggled = e;
        }

        public bool CellOnOff()
        {
            return switchcell.IsToggled;
        }

        private void MenuItem1_Activated(object sender, EventArgs e)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var file = Path.Combine(documents, "GeoChemDatapoints.xml");
            if (!File.Exists(file))
            {
                File.Create(file).Dispose();
            }
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(file);
                if (xDoc.ChildNodes.Count != 0)
                    Navigation.PushAsync(new ListPage());
            }
            catch (Exception ex)
            {
                if(ex.Message == "Root element is missing.") 
                 DisplayAlert("错误", "未运行过监测", "OK");
            }
        }
    }
}