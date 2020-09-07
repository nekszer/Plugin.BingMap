using Newtonsoft.Json;
using System.Collections.Generic;

namespace Plugin.BingMap.Shared
{
    public enum DistanceUnit
    {
        Mile, Kilometer
    }

    public class WayPoint
    {
        public WayPointType Type { get; set; }
        public Location Location { get; set; }
    }

    public enum WayPointType
    {
        Start, Intermediate, End
    }

    public class ActualEnd
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; }
    }

    public class ActualStart
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; }
    }

    public class Shield
    {
        [JsonProperty("labels")]
        public List<string> Labels { get; set; }

        [JsonProperty("roadShieldType")]
        public int RoadShieldType { get; set; }
    }

    public class RoadShieldRequestParameters
    {
        [JsonProperty("bucket")]
        public int Bucket { get; set; }

        [JsonProperty("shields")]
        public List<Shield> Shields { get; set; }
    }

    public class Detail
    {
        [JsonProperty("compassDegrees")]
        public int CompassDegrees { get; set; }

        [JsonProperty("endPathIndices")]
        public List<int> EndPathIndices { get; set; }

        [JsonProperty("maneuverType")]
        public string ManeuverType { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("roadType")]
        public string RoadType { get; set; }

        [JsonProperty("startPathIndices")]
        public List<int> StartPathIndices { get; set; }

        [JsonProperty("names")]
        public List<string> Names { get; set; }

        [JsonProperty("locationCodes")]
        public List<string> LocationCodes { get; set; }

        [JsonProperty("roadShieldRequestParameters")]
        public RoadShieldRequestParameters RoadShieldRequestParameters { get; set; }
    }

    public class Instruction
    {
        [JsonProperty("formattedText")]
        public object FormattedText { get; set; }

        [JsonProperty("maneuverType")]
        public string ManeuverType { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class ManeuverPoint
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; }
    }

    public class Warning
    {
        [JsonProperty("severity")]
        public string Severity { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("warningType")]
        public string WarningType { get; set; }
    }

    public class Hint
    {
        [JsonProperty("hintType")]
        public string HintType { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class ItineraryItem
    {
        [JsonProperty("compassDirection")]
        public string CompassDirection { get; set; }

        [JsonProperty("details")]
        public List<Detail> Details { get; set; }

        [JsonProperty("exit")]
        public string Exit { get; set; }

        [JsonProperty("iconType")]
        public string IconType { get; set; }

        [JsonProperty("instruction")]
        public Instruction Instruction { get; set; }

        [JsonProperty("isRealTimeTransit")]
        public bool IsRealTimeTransit { get; set; }

        [JsonProperty("maneuverPoint")]
        public ManeuverPoint ManeuverPoint { get; set; }

        [JsonProperty("realTimeTransitDelay")]
        public int RealTimeTransitDelay { get; set; }

        [JsonProperty("sideOfStreet")]
        public string SideOfStreet { get; set; }

        [JsonProperty("tollZone")]
        public string TollZone { get; set; }

        [JsonProperty("towardsRoadName")]
        public string TowardsRoadName { get; set; }

        [JsonProperty("transitTerminus")]
        public string TransitTerminus { get; set; }

        [JsonProperty("travelDistance")]
        public double TravelDistance { get; set; }

        [JsonProperty("travelDuration")]
        public int TravelDuration { get; set; }

        [JsonProperty("travelMode")]
        public string TravelMode { get; set; }

        [JsonProperty("warnings")]
        public List<Warning> Warnings { get; set; }

        [JsonProperty("hints")]
        public List<Hint> Hints { get; set; }
    }

    public class EndWaypoint
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("isVia")]
        public bool IsVia { get; set; }

        [JsonProperty("locationIdentifier")]
        public string LocationIdentifier { get; set; }

        [JsonProperty("routePathIndex")]
        public int RoutePathIndex { get; set; }
    }

    public class StartWaypoint
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("isVia")]
        public bool IsVia { get; set; }

        [JsonProperty("locationIdentifier")]
        public string LocationIdentifier { get; set; }

        [JsonProperty("routePathIndex")]
        public int RoutePathIndex { get; set; }
    }

    public class RouteSubLeg
    {
        [JsonProperty("endWaypoint")]
        public EndWaypoint EndWaypoint { get; set; }

        [JsonProperty("startWaypoint")]
        public StartWaypoint StartWaypoint { get; set; }

        [JsonProperty("travelDistance")]
        public double TravelDistance { get; set; }

        [JsonProperty("travelDuration")]
        public int TravelDuration { get; set; }
    }

    public class RouteLeg
    {
        [JsonProperty("actualEnd")]
        public ActualEnd ActualEnd { get; set; }

        [JsonProperty("actualStart")]
        public ActualStart ActualStart { get; set; }

        [JsonProperty("alternateVias")]
        public List<object> AlternateVias { get; set; }

        [JsonProperty("cost")]
        public int Cost { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("itineraryItems")]
        public List<ItineraryItem> ItineraryItems { get; set; }

        [JsonProperty("routeRegion")]
        public string RouteRegion { get; set; }

        [JsonProperty("routeSubLegs")]
        public List<RouteSubLeg> RouteSubLegs { get; set; }

        [JsonProperty("travelDistance")]
        public double TravelDistance { get; set; }

        [JsonProperty("travelDuration")]
        public int TravelDuration { get; set; }
    }

    public class Line
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<List<double>> Coordinates { get; set; }
    }

    public class RoutePath
    {
        [JsonProperty("generalizations")]
        public List<object> Generalizations { get; set; }

        [JsonProperty("line")]
        public Line Line { get; set; }
    }

    public class Resource
    {
        [JsonProperty("__type")]
        public string Type { get; set; }

        [JsonProperty("bbox")]
        public List<double> Bbox { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("distanceUnit")]
        public string DistanceUnit { get; set; }

        [JsonProperty("durationUnit")]
        public string DurationUnit { get; set; }

        [JsonProperty("routeLegs")]
        public List<RouteLeg> RouteLegs { get; set; }

        [JsonProperty("routePath")]
        public RoutePath RoutePath { get; set; }

        [JsonProperty("trafficCongestion")]
        public string TrafficCongestion { get; set; }

        [JsonProperty("trafficDataUsed")]
        public string TrafficDataUsed { get; set; }

        [JsonProperty("travelDistance")]
        public double TravelDistance { get; set; }

        [JsonProperty("travelDuration")]
        public int TravelDuration { get; set; }

        [JsonProperty("travelDurationTraffic")]
        public int TravelDurationTraffic { get; set; }

        [JsonProperty("travelMode")]
        public string TravelMode { get; set; }
    }

    public class ResourceSet
    {
        [JsonProperty("estimatedTotal")]
        public int EstimatedTotal { get; set; }

        [JsonProperty("resources")]
        public List<Resource> Resources { get; set; }
    }

    public class RouteResponse
    {
        [JsonProperty("authenticationResultCode")]
        public string AuthenticationResultCode { get; set; }

        [JsonProperty("brandLogoUri")]
        public string BrandLogoUri { get; set; }

        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("resourceSets")]
        public List<ResourceSet> ResourceSets { get; set; }

        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("statusDescription")]
        public string StatusDescription { get; set; }

        [JsonProperty("traceId")]
        public string TraceId { get; set; }
    }

}