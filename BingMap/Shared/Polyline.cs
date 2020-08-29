using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Plugin.BingMap
{
    public class Polyline
    {
        private PolylineStyle style;
        public PolylineStyle Style
        {
            get
            {
                if (style == null) return new PolylineStyle
                {
                    StrokeColor = "blue",
                    StrokeThickness = 2,
                    StrokeDashArray = new int[0]
                };
                return style;
            }

            set
            {
                style = value;
            }
        }
        public IEnumerable<Location> Locations { get; set; }
    }

    public class PolylineStyle
    {
        [JsonProperty("strokeColor")]
        public string StrokeColor { get; set; }

        [JsonProperty("strokeThickness")]
        public int StrokeThickness { get; set; }

        [JsonProperty("strokeDashArray")]
        public int[] StrokeDashArray { get; set; }
    }
}