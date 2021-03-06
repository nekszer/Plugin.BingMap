﻿
namespace Plugin.BingMap
{
    public class Location
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public Location(double lat, double lng)
        {
            Latitude = lat;
            Longitude = lng;
        }

        public override string ToString()
        {
            return $"{Latitude},{Longitude}";
        }
    }
}
