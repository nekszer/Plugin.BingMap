﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Plugin.BingMap
{

    public class Layer
    {
        public string Data { get; set; }
        public IEnumerable<Location> Locations { get; set; }
        public event EventHandler Click;
        internal void OnClick()
        {
            Click?.Invoke(this, EventArgs.Empty);
        }
    }

    public class Polyline : Layer
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

    public class Polygon : Layer
    {
        private PolygonStyle style;
        public PolygonStyle Style
        {
            get
            {
                if (style == null) return new PolygonStyle
                {
                    FillColor = "blue",
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
    }

    public class PolygonStyle : PolylineStyle
    {
        [JsonProperty("fillColor")]
        public string FillColor { get; set; }
    }
}