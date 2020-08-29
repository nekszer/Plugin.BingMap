using Android.App;
using Android.Content;
using Android.OS;

namespace BingMapsTest.Droid
{
    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Intent intent = new Intent(this, typeof(MainActivity));
            if (Intent?.Extras != null)
                intent.PutExtras(Intent.Extras);
            StartActivity(intent);
        }
    }
}