using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Plugin.BingMap
{
    public partial class FindAddressFromLocationResonse
    {
        [JsonProperty("authenticationResultCode")]
        public string AuthenticationResultCode { get; set; }

        [JsonProperty("brandLogoUri")]
        public Uri BrandLogoUri { get; set; }

        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("resourceSets")]
        public List<ResourceSet> ResourceSets { get; set; }

        [JsonProperty("statusCode")]
        public long StatusCode { get; set; }

        [JsonProperty("statusDescription")]
        public string StatusDescription { get; set; }

        [JsonProperty("traceId")]
        public string TraceId { get; set; }
    }

    public partial class ResourceSet
    {
        [JsonProperty("estimatedTotal")]
        public long EstimatedTotal { get; set; }

        [JsonProperty("resources")]
        public List<Resource> Resources { get; set; }
    }

    public partial class Resource
    {
        [JsonProperty("__type")]
        public string Type { get; set; }

        [JsonProperty("bbox")]
        public List<double> Bbox { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("point")]
        public AddressPoint Point { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("confidence")]
        public string Confidence { get; set; }

        [JsonProperty("entityType")]
        public string EntityType { get; set; }

        [JsonProperty("geocodePoints")]
        public List<GeocodePoint> GeocodePoints { get; set; }

        [JsonProperty("matchCodes")]
        public List<string> MatchCodes { get; set; }
    }

    public partial class Address
    {
        [JsonProperty("addressLine")]
        public string AddressLine { get; set; }

        [JsonProperty("adminDistrict")]
        public string AdminDistrict { get; set; }

        [JsonProperty("adminDistrict2")]
        public string AdminDistrict2 { get; set; }

        [JsonProperty("countryRegion")]
        public string CountryRegion { get; set; }

        [JsonProperty("formattedAddress")]
        public string FormattedAddress { get; set; }

        [JsonProperty("intersection")]
        public Intersection Intersection { get; set; }

        [JsonProperty("locality")]
        public string Locality { get; set; }

        [JsonProperty("postalCode")]
        public long PostalCode { get; set; }
    }

    public partial class Intersection
    {
        [JsonProperty("baseStreet")]
        public string BaseStreet { get; set; }

        [JsonProperty("secondaryStreet1")]
        public string SecondaryStreet1 { get; set; }

        [JsonProperty("intersectionType")]
        public string IntersectionType { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }

    public partial class GeocodePoint
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; }

        [JsonProperty("calculationMethod")]
        public string CalculationMethod { get; set; }

        [JsonProperty("usageTypes")]
        public List<string> UsageTypes { get; set; }
    }

    public partial class AddressPoint
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; }
    }
}
