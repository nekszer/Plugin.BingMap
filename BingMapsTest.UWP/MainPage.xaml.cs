using FFImageLoading.Forms.Platform;
using LightForms;
using BingMapsTest.UWP.Services;
using BingMapsTest.Services;

namespace BingMapsTest.UWP
{
    public sealed partial class MainPage : IPlatformInitializer
    {
        public MainPage()
        {
            this.InitializeComponent();
            CachedImageRenderer.Init();
            LoadApplication(new BingMapsTest.App(this));
        }

        public void RegisterTypes(ICrossContainer container)
        {
            container.Register<ILocalNotification, LocalNotificationImplementation>(FetchTarget.Singleton);
        }
    }
}