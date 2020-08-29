using Plugin.BingMap;
using System.Collections.Generic;
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
            map.Polylines.Add(new Polyline
            {
                Locations = new List<Location>
                {
                    new Location(e.Latitude, e.Longitude),
                    new Location(e.Latitude + 0.5, e.Longitude + 0.5)
                }
            });
        }

        private void Map_ViewChanged(object sender, ViewChanged e)
        {
            System.Diagnostics.Debug.WriteLine(e.EventType, "Map");
            System.Diagnostics.Debug.WriteLine(e.Location.Latitude + "," + e.Location.Longitude, "Map");
        }
    }
}
