using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.BingMap.Shared;

namespace Plugin.BingMap
{
    /// <summary>
    /// Interface for BingMap
    /// </summary>
    public class BingMapImplementation : IBingMap
    {
        /// <summary>
        /// Calcula la ruta de n puntos
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
                var routeresponse = JsonConvert.DeserializeObject<RouteResponse>(json);
                return routeresponse;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return null;
        }

        /// <summary>
        /// Devuelve las posibles direcciones de un punto en especifico
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> FindAddressFromLocation(string apikey, Location location)
        {
            var endpoint = $"https://dev.virtualearth.net/REST/v1/Locations/{location}?includeEntityTypes=Address&key={apikey}";
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(endpoint);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;
                var json = await response.Content.ReadAsStringAsync();
                var findaddress = JsonConvert.DeserializeObject<FindAddressFromLocationResonse>(json);
                if (findaddress.StatusCode != 200) return null;
                if (findaddress.ResourceSets == null) return null;
                var result = findaddress.ResourceSets.FirstOrDefault();
                if (result == null) return null;
                if (result.Resources == null || result.Resources.Count < 1) return null;
                var list = new List<string>();
                foreach (var resource in result.Resources)
                    list.Add(resource.Name);
                return list;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return null;
        }

        /// <summary>
        /// Devuelva las predicciones de ubicacion gps con base a un query
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="query"></param>
        /// <param name="maxresults"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AddressLocation>> FindLocationByQuery(string apikey, string query, int maxresults = 5, string twolettercountry = null, Location userlocation = null)
        {
            var endpoint = $"http://dev.virtualearth.net/REST/v1/Locations?query={query}&maxResults={maxresults}&key={apikey}";
            if (string.IsNullOrEmpty(twolettercountry))
                endpoint = $"{endpoint}&ur={twolettercountry}";
            if (userlocation != null)
                endpoint = $"{endpoint}&ul={userlocation}";
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(endpoint);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;
                var json = await response.Content.ReadAsStringAsync();
                var findaddress = JsonConvert.DeserializeObject<FindLocationByQueryResponse>(json);
                if (findaddress.StatusCode != 200) return null;
                if (findaddress.ResourceSets == null) return null;
                var resourceset = findaddress.ResourceSets.FirstOrDefault();
                if (resourceset == null) return null;
                if (resourceset.Resources == null) return null;
                var locations = new List<AddressLocation>();
                foreach (var resource in resourceset.Resources)
                {
                    var coordinates = resource.Point?.Coordinates;
                    if (coordinates == null) continue;
                    var lat = coordinates[0];
                    var lng = coordinates[0];
                    locations.Add(new AddressLocation(resource.Name, new Location(lat, lng)));
                }
                return locations;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return null;
        }
    }
}