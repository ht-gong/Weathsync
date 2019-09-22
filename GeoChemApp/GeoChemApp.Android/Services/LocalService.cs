using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;

namespace GeoChemApp.Droid.Services
{
    [Service]
    class LocalService: Service
    {
        CancellationTokenSource _cts;
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            _cts = new CancellationTokenSource();
            Task.Run(() =>
            {
                try
                {
                    var servicer = new GetService();
                    servicer.RunService(_cts.Token).Wait();
                }
                catch (System.OperationCanceledException) {
                }
            },_cts.Token);

            return StartCommandResult.Sticky;
        }
        public override void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Token.ThrowIfCancellationRequested();

                _cts.Cancel();
            }
            base.OnDestroy();
        }

    }
}