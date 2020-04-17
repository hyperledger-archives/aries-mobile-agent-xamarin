using System;
using System.Diagnostics;
using Autofac;
using FFImageLoading.Forms.Platform;
using Foundation;
using Microsoft.Extensions.DependencyInjection;
using Osma.Mobile.App.Converters;
using Osma.Mobile.App.Services;
using UIKit;

namespace Osma.Mobile.App.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private App _application;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Rg.Plugins.Popup.Popup.Init();
            Xamarin.Forms.Forms.Init();

            // Initializing FFImageLoading
            CachedImageRenderer.Init();

#if GORILLA

            LoadApplication(UXDivers.Gorilla.iOS.Player.CreateApplication(
                new UXDivers.Gorilla.Config("Good Gorilla")
                    .RegisterAssemblyFromType<InverseBooleanConverter>()
                    .RegisterAssemblyFromType<FFImageLoading.Transformations.CircleTransformation>()
                    .RegisterAssemblyFromType<FFImageLoading.Forms.CachedImage>()
                ));
#else
            // Initializing QR Code Scanning support
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            var host = App
               .BuildHost(typeof(PlatformModule).Assembly)
               .Build();

            _application = host.Services.GetRequiredService<App>();
            LoadApplication(_application);
#endif
            return base.FinishedLaunching(app, options);
        }
    }
}
