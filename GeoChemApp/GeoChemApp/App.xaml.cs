using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace GeoChemApp
{

    public partial class App : Application
    {
        private HomePage _HomePage;
        public App()
        {
            InitializeComponent();
            _HomePage = new HomePage();
            MainPage = new NavigationPage( _HomePage){ BarBackgroundColor = Color.FromHex("#ED9743"), BarTextColor = Color.White };
        }

        protected override void OnStart()
        {
            LoadPersisted();
            
        }

        protected override void OnSleep()
        {
            Current.Properties["SwitchOnOff"] = _HomePage.CellOnOff();
        }

        protected override void OnResume()
        {
            LoadPersisted();
        }

        private void LoadPersisted()
        {
            if (Application.Current.Properties.ContainsKey("SwitchOnOff"))
            {
                _HomePage.ToggleCell ((bool)Current.Properties["SwitchOnOff"]);
            }
        }
    }
}
