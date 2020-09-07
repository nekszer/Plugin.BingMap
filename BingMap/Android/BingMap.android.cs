using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Plugin.BingMap.Shared;

namespace Plugin.BingMap
{
    /// <summary>
    /// Interface for BingMap
    /// </summary>
    public class BingMapImplementation : IBingMap
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="waypoints"></param>
        /// <param name="distanceUnit"></param>
        /// <returns></returns>
        public async Task<RouteResponse> CalculateRoute(string apikey, IEnumerable<WayPoint> waypoints, DistanceUnit distanceUnit = DistanceUnit.Kilometer)
        {
            if (string.IsNullOrEmpty(apikey)) throw new ArgumentNullException("Apikey cannot  be null");
            if (waypoints == null) throw new ArgumentNullException("Waypoints cannot be null");
            if (waypoints.Count() < 2) throw new ArgumentNullException("The point list needs at least two items");
            var start = waypoints.FirstOrDefault(w => w.Type == WayPointType.Start);
            var end = waypoints.FirstOrDefault(w => w.Type == WayPointType.End);
            var intermediates = waypoints.Where(w => w.Type == WayPointType.Intermediate);
            if (start == null || end == null)
                throw new ArgumentException("The point list needs an element of WayPointType Start and end");
            var list = new List<WayPoint>
            {
                start
            };
            list.AddRange(intermediates);
            list.Add(end);
            var wp = new List<string>();
            int number = 0;
            foreach (var item in list)
            {
                var name = "";
                if (item.Type == WayPointType.Intermediate)
                    name = $"vwp";
                else
                    name = $"wp";
                name = $"{name}.{ number}={ item.Location.Latitude},{ item.Location.Longitude}";
                wp.Add(name);
                number++;
            }
            var wplist = string.Join("&", wp);
            var distance = distanceUnit == DistanceUnit.Kilometer ? "km" : "mi";
            var url = $"https://dev.virtualearth.net/REST/V1/Routes?{wplist}&key={apikey}&du={distance}&routePathOutput=Points";
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(url);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;
                var json = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<RouteResponse>(json);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return null;
        }
    }
}