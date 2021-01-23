﻿using Plugin.BingMap;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Examples.ViewModels
{
    public class MainViewModel : BindableObject
    {

        #region Notified Property Map
        /// <summary>
        /// Map
        /// </summary>
        private Map map;
        public Map Map
        {
            get { return map; }
            set { map = value; OnPropertyChanged(); SetMap(); }
        }

        private void SetMap()
        {
            if (Map == null) return;
            Map.LoadComplete += Map_LoadComplete;
        }

        private void Map_LoadComplete(object sender, EventArgs e)
        {
            Polygon polygon = new Polygon
            {
                Locations = MaxBounds
            };
            Map.Polygons.Add(polygon);
        }
        #endregion

        #region Notified Property MaxBounds
        /// <summary>
        /// MaxBounds
        /// </summary>
        private IEnumerable<Location> maxbounds;
        public IEnumerable<Location> MaxBounds
        {
            get { return maxbounds; }
            set { maxbounds = value; OnPropertyChanged(); }
        }
        #endregion

        public MainViewModel()
        {
            MaxBounds = new List<Location>
            {
                new Location(-4.242780, -69.945970),
                new Location(1.222430, -66.841673),
                new Location(6.226323, -67.411158),
                new Location(12.724630, -71.236506),
                new Location(9.526617, -78.409202),
                new Location(1.425987, -79.198806),
                new Location(-4.242780, -69.945970)
            };
        }
    }
}