using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Plugin.BingMap
{

    /// <summary>
    /// vista del mapa de bing
    /// </summary>
    public class Map : View
    {
        /// <summary>
        /// Lista de pins
        /// </summary>
        public ObservableCollection<Pin> Pins { get; private set; }

        public ObservableCollection<Polyline> Polylines { get; private set; }

        public MapTheme Theme { get; set; }

        /// <summary>
        /// Indica si el mapa se cargo o no
        /// </summary>
        public bool HasBeenLoaded { get; private set; }

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

        private void RemoveAllPolylines()
        {
            Action = Action.RemoveAllPolylines;
            ReceiveAction?.Invoke(this, null);
        }

        /// <summary>
        /// Este metodo se ejecuta cuando la vista del mapa cambia
        /// </summary>
        public event EventHandler<ViewChanged> ViewChanged;

        /// <summary>
        /// Este evento se ejecuta cuando el usuario hace un clic sobre el mapa, devolviendo la posicion gps del clic
        /// </summary>
        public event EventHandler<Location> MapClicked;

        /// <summary>
        /// Este metodo permite hacer un escalado en el mapa que muestra todos los pins
        /// </summary>
        public void ZoomForAllPins()
        {
            Action = Action.ZoomForAllPins;
            ReceiveAction?.Invoke(this, null);
        }

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
            RemoveAllPins();
            foreach (var item in Pins)
            {
                if (item is Pin pin)
                {
                    AddPin(pin);
                }
            }
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
        internal Action Action { get; private set; }

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

                case MapTheme.CustomStyleARGB:
                    if (style == null) throw new NullReferenceException("Para usar un tema personalizaado debes asignar un stilo [Style]");
                    return @", customMapStyle: " + JsonConvert.SerializeObject(style);
            }
        }
    }
}
