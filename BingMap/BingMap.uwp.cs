using Plugin.BingMap.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.BingMap
{
    /// <summary>
    /// Interface for BingMap
    /// </summary>
    public class BingMapImplementation : IBingMap
    {
        public Task<RouteResponse> CalculateRoute(string apikey, IEnumerable<WayPoint> waypoints, DistanceUnit distanceUnit = DistanceUnit.Kilometer)
        {
            throw new NotImplementedException();
        }
    }
}
