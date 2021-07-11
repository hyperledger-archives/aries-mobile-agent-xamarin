using System;
using System.Collections.Generic;
using System.Linq;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Platform;
using Java.Lang;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hyperledger.Aries.Max.Android;
using Xamarin.Forms;
using Resource = Hyperledger.Aries.Max.Android.Resource;

namespace Hyperledger.Aries.Max.Droid
{
    [Activity(Label = "AriesMax", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Forms.Init(this, bundle);

            Acr.UserDialogs.UserDialogs.Init(this);
            // Initializing FFImageLoading
            CachedImageRenderer.Init(false);
            //Rg.Plugins.Popup.Popup.Init(this, bundle);
            // Initializing Xamarin Essentials
            Xamarin.Essentials.Platform.Init(this, bundle);

            // Initializing QR Code Scanning support
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);

            // Initializing User Dialogs
            // Android requires that we set content root.
            var host = Hyperledger.Aries.Max.App.BuildHost(typeof(PlatformModule).Assembly)
                .UseContentRoot(System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal)).Build();

            //Loading dependent libindy
            JavaSystem.LoadLibrary("c++_shared");
            JavaSystem.LoadLibrary("indy");

            LoadApplication(host.Services.GetRequiredService<Hyperledger.Aries.Max.App>());

            CheckAndRequestRequiredPermissions();
        }

        readonly string[] _permissionsRequired =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera
        };

        private int _requestCode = -1;
        private List<string> _permissionsToBeGranted = new List<string>();

        private void CheckAndRequestRequiredPermissions()
        {
            for (int i = 0; i < _permissionsRequired.Length; i++)
                if (CheckSelfPermission(_permissionsRequired[i]) != (int)Permission.Granted)
                    _permissionsToBeGranted.Add(_permissionsRequired[i]);

            if (_permissionsToBeGranted.Any())
            {
                _requestCode = 10;
                RequestPermissions(_permissionsRequired.ToArray(), _requestCode);
            }
            else
                System.Diagnostics.Debug.WriteLine("Device already has all the required permissions");
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            if (grantResults.Length == _permissionsToBeGranted.Count)
                System.Diagnostics.Debug.WriteLine("All permissions required that werent granted, have now been granted");
            else
                System.Diagnostics.Debug.WriteLine("Some permissions requested were denied by the user");
           
           Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
           base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

