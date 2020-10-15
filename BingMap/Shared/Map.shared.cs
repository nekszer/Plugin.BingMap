using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace Plugin.BingMap
{
    /// <summary>
    /// 
    /// </summary>
    public enum MapType
    {
        Road, StreetSide, Aerial, BirdsEye, CanvasDark, CanvasLight, Grayscale
    }

    /// <summary>
    /// vista del mapa de bing
    /// </summary>
    public class Map : View
    {

        /// <summary>
        /// Lista de pins
        /// </summary>
        public ObservableCollection<Pin> Pins { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<Polyline> Polylines { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MapTheme Theme { get; set; }

        #region BindableProperty MapType
        /// <summary>
        /// Description of property
        /// </summary>
        public static readonly BindableProperty MapTypeProperty = BindableProperty.Create(nameof(MapType), typeof(MapType), typeof(Map), MapType.Road, BindingMode.OneWay);

        /// <summary>
        /// Description of property
        /// </summary>
        public MapType MapType
        {
            get
            {
                return (MapType)GetValue(MapTypeProperty);
            }

            set
            {
                SetValue(MapTypeProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// Indica si el mapa se cargo o no
        /// </summary>
        public bool HasBeenLoaded { get; internal set; }

        /// <summary>
        /// Permite detectar los errores en el plugin [Solo para log]
        /// </summary>
        public static event EventHandler<Exception> ErrorHandler;

        /// <summary>
        /// Realiza una instsancia del mapa
        /// </summary>
        public Map()
        {
            if (Pins == null) Pins = new ObservableCollection<Pin>();
            Pins.CollectionChanged += Pins_CollectionChanged;
            if (Polylines == null) Polylines = new ObservableCollection<Polyline>();
            Polylines.CollectionChanged += Polylines_CollectionChanged;
        }

        private void Polylines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RemoveAllPolylines();
            foreach (var item in Polylines)
                if (item is Polyline polyline)
                    AddPolyline(polyline);
        }

        public void Reload()
        {
            Action = Action.Reload;
            Pins.Clear();
            Polylines.Clear();
            HasBeenLoaded = false;
            ReceiveAction?.Invoke(this, null);
        }

        private void RemoveAllPolylines()
        {
            Action = Action.RemoveAllPolylines;
            ReceiveAction?.Invoke(this, null);
        }

        /// <summary>
        /// Este metodo se ejecuta cuando la vista del mapa cambia
        /// </summary>
        public event EventHandler<ViewChanged> ViewChanged;

        public void SetBounds(IEnumerable<Location> locations)
        {
            Action = Action.ZoomForLocations;
            ReceiveAction?.Invoke(this, locations);
        }

        /// <summary>
        /// Este evento se ejecuta cuando el usuario hace un clic sobre el mapa, devolviendo la posicion gps del clic
        /// </summary>
        public event EventHandler<Location> MapClicked;

        /// <summary>
        /// Asigna la ApiKey del mapa
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Evento a ejecutar cuando se hace un LoadComplete del mapa
        /// </summary>
        public event EventHandler<EventArgs> LoadComplete;

        /// <summary>
        /// Permite asignar el centro del mapa
        /// </summary>
        /// <param name="center"></param>
        public void SetCenter(Center center)
        {
            Action = Action.SetCenter;
            ReceiveAction?.Invoke(this, center);
        }

        /// <summary>
        /// Agrega un pin
        /// </summary>
        /// <param name="pin"></param>
        private void AddPin(Pin pin)
        {
            Action = Action.AddPin;
            ReceiveAction?.Invoke(this, pin);
        }
        
        /// <summary>
        /// Agrega una linea
        /// </summary>
        /// <param name="polyline"></param>
        private void AddPolyline(Polyline polyline)
        {
            Action = Action.Polyline;
            ReceiveAction?.Invoke(this, polyline);
        }

        /// <summary>
        /// Refrescamos los pins en el mapa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pins_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                RemoveAllPins();
                return;
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move) return;
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace) return;

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                foreach (var item in e.OldItems)
                    if (item is Pin pin)
                        RemovePin(pin.GetHashCode());

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                foreach (var item in e.NewItems)
                    if (item is Pin pin)
                        AddPin(pin);
        }

        /// <summary>
        /// Permite ejecutar un evento de error
        /// </summary>
        /// <param name="ex"></param>
        internal static void OnErrorHandler(object sender, Exception ex)
        {
            ErrorHandler?.Invoke(sender, ex);
        }

        private void RemovePin(int hashcode)
        {
            Action = Action.RemovePin;
            ReceiveAction?.Invoke(this, hashcode);
        }

        /// <summary>
        /// Metodo para lanzar el evento OnMapClicked
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        internal void OnMapClicked(double lat, double lng)
        {
            MapClicked?.Invoke(this, new Location(lat, lng));
        }

        /// <summary>
        /// Remueve todos los items del mapa
        /// </summary>
        private void RemoveAllPins()
        {
            Action = Action.RemoveAllPins;
            ReceiveAction?.Invoke(this, null);
        }

        /// <summary>
        /// Esta variable almacena la action actual o la ultima accion lanzada por la vista
        /// </summary>
        internal Action Action { get; set; }

        /// <summary>
        /// Se ejecuta al cargar el mapa
        /// </summary>
        internal void OnLoadComplete(string load)
        {
            if (!string.IsNullOrEmpty(load))
            {
                HasBeenLoaded = load == "bingmapv8_loadcomplete";
                if (HasBeenLoaded)
                {
                    LoadComplete?.Invoke(this, EventArgs.Empty);
                    System.Diagnostics.Debug.WriteLine("Se ha cargado el mapa", "BingMapV8");
                }
            }
        }

        /// <summary>
        /// Define el estilo del mapa
        /// </summary>
        public new MapStyle Style { get; set; }


        #region BindableProperty MaxBounds
        /// <summary>
        /// Description of property
        /// </summary>
        public static readonly BindableProperty MaxBoundsProperty = BindableProperty.Create(nameof(MaxBounds), typeof(IEnumerable<Location>), typeof(Map), default(IEnumerable<Location>), BindingMode.OneWay);

        /// <summary>
        /// Description of property
        /// </summary>
        public IEnumerable<Location> MaxBounds
        {
            get
            {
                return (IEnumerable<Location>)GetValue(MaxBoundsProperty);
            }

            set
            {
                SetValue(MaxBoundsProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// Permite enviar objetos
        /// </summary>
        internal event EventHandler<object> ReceiveAction;

        /// <summary>
        /// Este metodo permite convertir un json a un objecto C#
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        internal T DeserializeObject<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

        /// <summary>
        /// Permite ejecutar el evento Viewchanged
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        internal void OnViewChange(string ev, double lat, double lng)
        {
            ViewChanged?.Invoke(this, new ViewChanged(ResolveEvent(ev), new Location(lat, lng)));
        }

        internal static string ResolveMaxBounds(IEnumerable<Location> maxbounds)
        {
            if (maxbounds == null) return string.Empty;
            if (maxbounds.Count() < 4) return string.Empty;
            var strlist = new List<string>();
            foreach (var bound in maxbounds)
                strlist.Add($"new Microsoft.Maps.Location({bound.Latitude}, {bound.Longitude})");
            var array = string.Join(",", strlist);
            return $"maxBounds: Microsoft.Maps.LocationRect.fromLocations({array}),";
        }

        /// <summary>
        /// Resuelve el tipo de mapa a mostrar
        /// </summary>
        /// <param name="maptype"></param>
        /// <returns></returns>
        internal static string ResolveMapType(MapType maptype)
        {
            switch (maptype)
            {
                case MapType.Road:
                    return "mapTypeId: Microsoft.Maps.MapTypeId.road,";
                case MapType.StreetSide:
                    return "mapTypeId: Microsoft.Maps.MapTypeId.streetside,";
                case MapType.Aerial:
                    return "mapTypeId: Microsoft.Maps.MapTypeId.aerial,";
                case MapType.BirdsEye:
                    return "mapTypeId: Microsoft.Maps.MapTypeId.birdseye,";
                case MapType.CanvasDark:
                    return "mapTypeId: Microsoft.Maps.MapTypeId.canvasdark,";
                case MapType.CanvasLight:
                    return "mapTypeId: Microsoft.Maps.MapTypeId.canvaslight,";
                case MapType.Grayscale:
                    return "mapTypeId: Microsoft.Maps.MapTypeId.grayscale,";
                default:
                    return "mapTypeId: Microsoft.Maps.MapTypeId.road,";
            }
        }

        /// <summary>
        /// Resuelve el nombre del evento lanzado por la vista
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        private ViewChanged.Type ResolveEvent(string ev)
        {
            switch (ev)
            {
                case "viewchangestart": return BingMap.ViewChanged.Type.ViewChangeStart;
                case "viewchange": return BingMap.ViewChanged.Type.ViewChange;
                case "viewchangeend": return BingMap.ViewChanged.Type.ViewChangeEnd;
                case "viewrendered": return BingMap.ViewChanged.Type.ViewRendered;
                case "maptypechanged": return BingMap.ViewChanged.Type.MapTypeChanged;
            }
            return BingMap.ViewChanged.Type.MapTypeChanged;
        }

        /// <summary>
        /// Resuelve el tema para el mapa
        /// </summary>
        /// <param name="theme"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        internal static string ResolveTheme(MapTheme theme, MapStyle style)
        {
            switch (theme)
            {
                default:
                case MapTheme.Default:
                    return string.Empty;

                case MapTheme.Dark:
                    return @", mapTypeId: Microsoft.Maps.MapTypeId.canvasDark, supportedMapTypes:[Microsoft.Maps.MapTypeId.road, Microsoft.Maps.MapTypeId.aerial, Microsoft.Maps.MapTypeId.canvasDark]";

                case MapTheme.Light:
                    return @", mapTypeId: Microsoft.Maps.MapTypeId.canvasLight, supportedMapTypes: [Microsoft.Maps.MapTypeId.road, Microsoft.Maps.MapTypeId.aerial, Microsoft.Maps.MapTypeId.canvasLight]";

                case MapTheme.CustomTile:
                    return @", customMapStyle: {
                        elements: {
                            area: { fillColor: '#b6e591' },
                            water: { fillColor: '#75cff0' },
                            tollRoad: { fillColor: '#a964f4', strokeColor: '#a964f4' },
                            arterialRoad: { fillColor: '#ffffff', strokeColor: '#d7dae7' },
                            road: { fillColor: '#ffa35a', strokeColor: '#ff9c4f' },
                            street: { fillColor: '#ffffff', strokeColor: '#ffffff' },
                            transit: { fillColor: '#000000' }
                        },
                        settings: {
                            landColor: '#efe9e1'
                        }
                    }";

                case MapTheme.CustomStyle:
                    if (style == null) throw new NullReferenceException("Para usar un tema personalizaado debes asignar un stilo [Style]");
                    return @", customMapStyle: " + JsonConvert.SerializeObject(style);
            }
        }
    }
}
