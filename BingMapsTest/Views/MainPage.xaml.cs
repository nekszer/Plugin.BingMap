using Plugin.BingMap;
using System.ComponentModel;
using Xamarin.Forms;

namespace BingMapsTest.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Map_MapClicked(object sender, Location e)
        {
            var map = (sender as Map);
            var pin = new Pin(e)
            {
                Title = "Hello " + e.Latitude + "," + e.Longitude,
                Data = "Hello " + e.Latitude + "," + e.Longitude
            };
            pin.Click += Pin_Click;
            map.Pins.Add(pin);
        }

        private void Pin_Click(object sender, string e)
        {
            System.Diagnostics.Debug.WriteLine(e);
        }

        private void Map_ViewChanged(object sender, ViewChanged e)
        {
            System.Diagnostics.Debug.WriteLine(e.EventType, "Map");
            System.Diagnostics.Debug.WriteLine(e.Location.Latitude + "," + e.Location.Longitude, "Map");
        }
    }
}