using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using GeoChemApp.Messages;
using Xamarin.Forms;
using Android.Content;

namespace GeoChemApp.Droid
{
    [Activity(Label = "Excel空气指数监测", Icon = "@drawable/environmentalplanning", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            this.Window.AddFlags(WindowManagerFlags.Fullscreen); // hide the status bar
            WireUpServices();

        }

        void WireUpServices()
        {
            MessagingCenter.Subscribe<StartSrvcMessage>(this, "StartSrvcMessage", message =>
              {
                  try
                  {
                      var intent = new Intent(this, typeof(Services.LocalService));
                      this.StartService(intent);
                  }
                  catch (Exception ex)
                  {
                      throw;
                  }
              });

            MessagingCenter.Subscribe<StopSrvcMessage>(this, "StopSrvcMessage", message =>
             {
                 var intent = new Intent(this, typeof(Services.LocalService));
                 StopService(intent);
             });
        }
    }
}