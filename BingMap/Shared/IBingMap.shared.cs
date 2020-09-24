using Plugin.BingMap.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.BingMap
{
    public interface IBingMap
    {
        Task<RouteResponse> CalculateRoute(string apikey, IEnumerable<WayPoint> waypoints, DistanceUnit distanceUnit = DistanceUnit.Kilometer);

        Task<IEnumerable<string>> FindAddressFromLocation(string apikey, Location location);

        Task<IEnumerable<AddressLocation>> FindLocationByQuery(string apikey, string query, int maxresults = 5, string twolettercountry = null, Location userlocation = null);
    }

    public class AddressLocation
    {
        public string Name { get; set; }
        public Location Location { get; set; }
        public AddressLocation(string name, Location location)
        {
            Name = name;
            Location = location;
        }
    }
}