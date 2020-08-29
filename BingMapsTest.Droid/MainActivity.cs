using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Rg.Plugins.Popup;
using FFImageLoading.Forms.Platform;
using Acr.UserDialogs;
using Android.Content;
using BingMapsTest.Services;
using LightForms;
using System.Collections.Generic;
using BingMapsTest.Droid.Services;
using Sharpnado.Presentation.Forms.Droid;
using Xamarin.Forms;

namespace BingMapsTest.Droid
{
    [Activity(Label = "BingMapsTest", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IPlatformInitializer
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            UserDialogs.Init(this);
            Popup.Init(this, savedInstanceState);
            CachedImageRenderer.Init(true);
            SharpnadoInitializer.Initialize();
            FormsMaterial.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(this));
            CreateNotificationFromIntent(base.Intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            if (Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }

        public void RegisterTypes(ICrossContainer container)
        {
            container.Register<ILocalNotification, LocalNotificationImplementation>(FetchTarget.Singleton);
        }

        #region Notifications
        protected override void OnNewIntent(Intent intent)
        {
            CreateNotificationFromIntent(intent);
        }

        void CreateNotificationFromIntent(Intent intent)
        {
            if (intent?.Extras == null) return;
            string title = intent.Extras.GetString(LocalNotificationConstants.TitleKey);
            string message = intent.Extras.GetString(LocalNotificationConstants.MessageKey);
            int action = intent.Extras.GetInt(LocalNotificationConstants.ActionKey);
            int id = intent.Extras.GetInt(LocalNotificationConstants.IdKey);
            CrossContainer.Instance.Create<ILocalNotification>().ReceiveNotification(new Dictionary<string, string>
            {
                { LocalNotificationConstants.TitleKey, title },
                { LocalNotificationConstants.MessageKey, message },
                { LocalNotificationConstants.ActionKey, action.ToString() },
                { LocalNotificationConstants.IdKey, id.ToString() },
            });
        }
        #endregion
    }
}