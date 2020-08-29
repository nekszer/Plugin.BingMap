using FFImageLoading.Forms.Platform;
using Foundation;
using LightForms;
using BingMapsTest.iOS.Services;
using BingMapsTest.Services;
using Rg.Plugins.Popup;
using Sharpnado.Presentation.Forms.iOS;
using UIKit;
using Xamarin.Forms;

namespace BingMapsTest.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IPlatformInitializer
    {

        public LocalNotificationImplementation LocalNotification { get; private set; }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Popup.Init();
            CachedImageRenderer.Init();
            SharpnadoInitializer.Initialize();
            FormsMaterial.Init();
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App(this));
            LocalNotification?.OnLaunching(options);
            return base.FinishedLaunching(app, options);
        }

        public async void RegisterTypes(ICrossContainer container)
        {
            container.Register<ILocalNotification, LocalNotificationImplementation>(FetchTarget.Singleton);
            LocalNotification = container.Create<ILocalNotification>() as LocalNotificationImplementation;
            await LocalNotification.Init();
        }
    }
}