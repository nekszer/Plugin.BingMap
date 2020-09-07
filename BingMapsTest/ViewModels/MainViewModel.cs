using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Plugin.BingMap;
using Plugin.BingMap.Shared;

namespace BingMapsTest.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        public override void Appearing(string route, object data)
        {
            base.Appearing(route, data);
            var map = FindViewByName<Map>("Map");
            map.LoadComplete += Map_LoadComplete;
        }

        private async void Map_LoadComplete(object sender, System.EventArgs e)
        {
            var response = await CrossBingMap.Current.CalculateRoute("AiaCvUwc1w8NPFNP3njRj6x28yQUDDj-I_l9K87f3rEj9d30JT_vyvGOF_wOP1zx", new List<WayPoint>
            {
                new WayPoint
                {
                    Location = new Location(19.479926, -98.834021),
                    Type = WayPointType.Start
                },
                new WayPoint
                {
                    Location = new Location(19.511986, -98.882633),
                    Type = WayPointType.End
                }
            }, DistanceUnit.Kilometer);
            if (response == null) return;
            if (response.StatusCode != 200) return;
            var findroute = response.ResourceSets?.FirstOrDefault();
            if (findroute == null) return;
            var resource = findroute.Resources.FirstOrDefault();
            if (resource == null) return;
            var path = resource.RoutePath;
            var map = (sender as Map);
            var locations = path.Line.Coordinates.Select(s => new Location(s[0], s[1]));
            var instructions = resource.RouteLegs.FirstOrDefault().ItineraryItems.Select(i => i.Instruction);
            var polyline = new Polyline
            {
                Locations = locations,
                Data = JsonConvert.SerializeObject(instructions)
            };
            polyline.Click += Polyline_Click;
            map.Polylines.Add(polyline);
            map.SetBounds(locations);
        }

        private void Polyline_Click(object sender, EventArgs e)
        {
            Console.WriteLine((sender as Polyline).Data);
        }
    }
}