using Plugin.BingMap.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.BingMap
{
    public interface IBingMap
    {
        Task<RouteResponse> CalculateRoute(string apikey, IEnumerable<WayPoint> waypoints, DistanceUnit distanceUnit = DistanceUnit.Kilometer);
    }
}