using Plugin.BingMap.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Plugin.BingMap.Behaviors
{
    public class MapSetRouteBehavior : Behavior<Map>
    {

        #region BindableProperty Waypoints
        /// <summary>
        /// Description of property
        /// </summary>
        public static readonly BindableProperty WaypointsProperty = BindableProperty.Create(nameof(Waypoints), typeof(IEnumerable<WayPoint>), typeof(MapSetRouteBehavior), null, BindingMode.OneWay);

        /// <summary>
        /// Description of property
        /// </summary>
        public IEnumerable<WayPoint> Waypoints
        {
            get
            {
                return (IEnumerable<WayPoint>) GetValue(WaypointsProperty);
            }

            set
            {
                SetValue(WaypointsProperty, value);
            }
        }
        #endregion

        #region BindableProperty ClickCommand
        /// <summary>
        /// Click
        /// </summary>
        public static readonly BindableProperty ClickCommandProperty = BindableProperty.Create(nameof(ClickCommand), typeof(ICommand), typeof(ContentView), default(ICommand), BindingMode.OneWay);

        /// <summary>
        /// Click
        /// </summary>
        public ICommand ClickCommand
        {
            get
            {
                return (ICommand)GetValue(ClickCommandProperty);
            }

            set
            {
                SetValue(ClickCommandProperty, value);
            }
        }
        #endregion

        public string ApiKey { get; set; }

        private Map CurrentMap { get; set; }

        public MapSetRouteBehavior()
        {
            BindableProperties = new List<BindablePropertyChanged>
            {
                new BindablePropertyChanged(WaypointsProperty, SetRoute)
            };
        }

        private async void SetRoute()
        {
            if (CurrentMap == null) return;
            if (Waypoints == null || Waypoints.Count() == 0)
            {
                CurrentMap.Polylines.Clear();
                return;
            }
            Response = await CrossBingMap.Current.CalculateRoute(ApiKey, Waypoints, DistanceUnit.Kilometer);
            if (Response == null)
            {
                CurrentMap.Polylines.Clear();
                return;
            }
            if (Response.StatusCode != 200)
            {
                CurrentMap.Polylines.Clear();
                return;
            }
            var findroute = Response.ResourceSets?.FirstOrDefault();
            if (findroute == null)
            {
                CurrentMap.Polylines.Clear();
                return;
            }
            var resource = findroute.Resources.FirstOrDefault();
            if (resource == null)
            {
                CurrentMap.Polylines.Clear();
                return;
            }
            var path = resource.RoutePath;
            var locations = path.Line.Coordinates.Select(s => new Location(s[0], s[1]));
            var instructions = resource.RouteLegs.FirstOrDefault().ItineraryItems.Select(i => i.Instruction);
            var polyline = new Polyline
            {
                Locations = locations
            };
            polyline.Click += Polyline_Click;
            CurrentMap.Polylines.Clear();
            CurrentMap.Polylines.Add(polyline);
            CurrentMap.SetBounds(locations);
        }

        private void Polyline_Click(object sender, EventArgs e)
        {
            ClickCommand?.Execute(Response);
        }

        public Map AssociatedObject { get; private set; }

        protected override void OnAttachedTo(Map bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject = bindable;
            bindable.BindingContextChanged += Bindable_BindingContextChanged;
            bindable.LoadComplete += Bindable_LoadComplete;
        }

        private void Bindable_BindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
        }

        private void Bindable_LoadComplete(object sender, EventArgs e)
        {
            CurrentMap = sender as Map;
        }

        protected override void OnDetachingFrom(Map bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.LoadComplete -= Bindable_LoadComplete;
        }

        #region Bindable Property Changed
        public List<BindablePropertyChanged> BindableProperties { get; set; }
        public RouteResponse Response { get; private set; }

        protected override void OnPropertyChanged(string propertyname = null)
        {
            base.OnPropertyChanged(propertyname);
            if (BindableProperties == null) return;
            BindableProperties.FirstOrDefault(b => b.PropertyChanged(propertyname));
        }

        public class BindablePropertyChanged
        {
            protected string PropetyName
            {
                get
                {
                    return SourceProperty?.PropertyName;
                }
            }
            protected BindableProperty SourceProperty;
            protected System.Action Action { get; set; }

            public BindablePropertyChanged(BindableProperty sourceproperty, System.Action action)
            {
                SourceProperty = sourceproperty;
                Action = action;
            }

            public bool PropertyChanged(string propertyname)
            {
                if (SourceProperty.PropertyName != propertyname) return false;
                Action?.Invoke();
                return true;
            }
        }
        #endregion
    }
}
