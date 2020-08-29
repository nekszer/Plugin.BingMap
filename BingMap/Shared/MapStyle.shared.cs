using Newtonsoft.Json;

namespace Plugin.BingMap
{
    public partial class MapStyle
    {
        [JsonProperty("elements")]
        public Elements Elements { get; set; }

        [JsonProperty("settings")]
        public Settings Settings { get; set; }
    }

    public partial class Elements
    {
        [JsonProperty("area")]
        public Zone Area { get; set; }

        [JsonProperty("water")]
        public Zone Water { get; set; }

        [JsonProperty("tollRoad")]
        public Line TollRoad { get; set; }

        [JsonProperty("arterialRoad")]
        public Line ArterialRoad { get; set; }

        [JsonProperty("road")]
        public Line Road { get; set; }

        [JsonProperty("street")]
        public Line Street { get; set; }

        [JsonProperty("transit")]
        public Zone Transit { get; set; }
    }

    public partial class Zone
    {
        [JsonProperty("fillColor")]
        public string FillColor { get; set; }
    }

    public partial class Line
    {
        [JsonProperty("fillColor")]
        public string FillColor { get; set; }

        [JsonProperty("strokeColor")]
        public string StrokeColor { get; set; }
    }

    public partial class Settings
    {
        [JsonProperty("landColor")]
        public string LandColor { get; set; }
    }
}
