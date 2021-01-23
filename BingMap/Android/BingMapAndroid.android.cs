using Android.Content;
using Android.Net.Http;
using Android.Runtime;
using Android.Webkit;
using Newtonsoft.Json;
using Plugin.BingMap;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(Map), typeof(BingMapAndroid))]
namespace Plugin.BingMap
{
    public class BingMapAndroid : ViewRenderer<Map, Android.Webkit.WebView>
    {
        Context _context;

        public BingMapAndroid(Context context) : base(context)
        {
            _context = context;
        }

        public Android.Webkit.WebView Instance { get; set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                Instance = new Android.Webkit.WebView(_context)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
                };
                Instance.Settings.SetGeolocationEnabled(true);
                Instance.Settings.AllowContentAccess = true;
                Instance.Settings.JavaScriptEnabled = true;
                Instance.Settings.CacheMode = CacheModes.CacheElseNetwork;
                Instance.Settings.SetAppCacheEnabled(true);
                Instance.Settings.SetRenderPriority(WebSettings.RenderPriority.High);
                SetNativeControl(Instance);
            }

            if (e.NewElement != null)
            {
                var html = GetHtml(e.NewElement.ApiKey, e.NewElement.Theme, e.NewElement.Style, e.NewElement.MaxBounds, e.NewElement.MapType);
                Instance.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                Instance.LoadDataWithBaseURL(null, html, "text/html", "UTF-8", null);
            }

            if (Element != null)
                Element.ReceiveAction += Element_Send;
        }

        private void Element_Send(object sender, object e)
        {
            if (Instance == null)
                return;

            if (!(sender is Map view))
                return;

            switch (view.Action)
            {
                case Plugin.BingMap.Action.SetCenter:
                    if (e is Center center)
                    {
                        Instance.EvaluateJavascript($"setCenter({center.Latitude},{center.Longitude},{center.Zoom})", null);
                    }
                    break;

                case Plugin.BingMap.Action.AddPin:
                    if (e is Pin pin)
                    {
                        if (pin.Image != null)
                        {
                            Instance.EvaluateJavascript($"addPinImage({pin.GetHashCode()}, {pin.Latitude}, {pin.Longitude}, '{pin.Title}', '{pin.Data}', '{pin.Image.Source}', {pin.Image.X}, {pin.Image.Y})", null);
                        }
                        else
                        {
                            Instance.EvaluateJavascript($"addPin({pin.GetHashCode()}, {pin.Latitude}, {pin.Longitude}, '{pin.Title}', '{pin.Data}')", null);
                        }
                    }
                    break;

                case Plugin.BingMap.Action.RemoveAllPins:
                    if (e is null)
                    {
                        Instance.EvaluateJavascript("removeAllPins()", null);
                    }
                    break;

                case Plugin.BingMap.Action.Polyline:
                    if (e is Polyline polyline)
                    {
                        Instance.EvaluateJavascript($"addPolyline({JsonConvert.SerializeObject(polyline)}, {polyline.GetHashCode()})", null);
                    }
                    break;

                case Action.Polygon:
                    if (e is Polygon polygon)
                    {
                        Instance.EvaluateJavascript($"addPolygon({JsonConvert.SerializeObject(polygon)}, {polygon.GetHashCode()})", null);
                    }
                    break;

                case Action.RemoveAllPolygons:
                    if (e is null)
                    {
                        Instance.EvaluateJavascript($"removeAllPolygons()", null);
                    }
                    break;

                case Plugin.BingMap.Action.RemoveAllPolylines:
                    if (e is null)
                    {
                        Instance.EvaluateJavascript($"removeAllPolylines()", null);
                    }
                    break;

                case Plugin.BingMap.Action.RemovePin:
                    if(e is int hashcode)
                    {
                        Instance.EvaluateJavascript($"removePin("+ hashcode + ")", null);
                    }
                    break;

                case Plugin.BingMap.Action.ZoomForLocations:
                    if (e is IEnumerable<Location> locations)
                    {
                        var json = JsonConvert.SerializeObject(locations);
                        Instance.EvaluateJavascript($"zoomForLocations({json})", null);
                    }
                    break;

                case Action.Reload:
                    if (e is null)
                    {
                        Instance.Reload();
                    }
                    break;

                default:
                    break;
            }
        }

        private string GetHtml(string apikey, MapTheme theme, MapStyle style, IEnumerable<Location> maxbounds, MapType type = MapType.Road)
        {
            return @"<!DOCTYPE html>
                    <html>
                    <head>
                        <title>BingMap</title>
                        <style type='text/css'>
                            body {
                                margin: 0;
                                padding: 0;
                                overflow: hidden;
                                font-family: 'Segoe UI',Helvetica,Arial,Sans-Serif
                            }
                        </style>
                        <meta name='viewport' content='width=device-width, initial-scale=1'>
                        <meta http-equiv='Content-Type' content='text/html; charset=utf-8' />
                        <script type='text/javascript' src='https://www.bing.com/api/maps/mapcontrol?key=" + apikey + @"'></script>
                        <script type='text/javascript'>
                            var map;
                            var locations = [];
                            
                            function invokePinClick(str){ jsBridge.pinClick(str); }

                            function invokeOnLoadComplete(str){ jsBridge.onLoadComplete(str); }

                            function onMapClick(lat, lng) { jsBridge.onMapClick(lat, lng); }

                            function onViewChange(event, lat, lng) { jsBridge.onViewChange(event, lat, lng); }

                            function invokePolylineClick(hashcode) { jsBridge.polylineClick(hashcode); }

                            function invokePolygonClick(hashcode) { jsBridge.polygonClick(hashcode); }

                            function loadMapScenario() {
                                // iniciamos el mapa
                                map = new Microsoft.Maps.Map(document.getElementById('bingv8map'), { " + 
                                    Map.ResolveMapType(type) + @"
                                    showBreadcrumb: false,
                                    showLocateMeButton: false,
                                    showMapTypeSelector: false,
                                    showZoomButtons: false," + 
                                    Map.ResolveMaxBounds(maxbounds) + @"
                                    liteMode: true" +
                                    Map.ResolveTheme(theme, style)
                                + @"});
                                // invocamos al metodo, on load complete
                                invokeOnLoadComplete('bingmapv8_loadcomplete');
                                Microsoft.Maps.Events.addHandler(map, 'click', function (e) 
                                { 
                                    var location = e.location;
                                    var latitude = location.latitude;
                                    var longitude = location.longitude;
                                    onMapClick(latitude, longitude);
                                });

                                Microsoft.Maps.Events.addHandler(map, 'viewchangestart', function (e) { viewchange('viewchangestart', e); });
                                Microsoft.Maps.Events.addHandler(map, 'viewchange', function (e) { viewchange('viewchange', e); });
                                Microsoft.Maps.Events.addHandler(map, 'viewchangeend', function (e) { viewchange('viewchangeend', e); });
                                Microsoft.Maps.Events.addHandler(map, 'viewrendered', function (e) { viewchange('viewrendered', e); });
                                Microsoft.Maps.Events.addHandler(map, 'maptypechanged', function (e) { viewchange('maptypechanged', e); });

                                function viewchange(event, args){
                                    var center = map.getCenter();
                                    onViewChange(event, center.latitude, center.longitude);
                                }
                            }

                            function setCenter(lat, lon, zoom) {
                                if (map != undefined) {
                                    map.setView({
                                        center: new Microsoft.Maps.Location(parseFloat(lat), parseFloat(lon)),
                                        zoom: zoom
                                    });
                                }
                            }

                            function addPinImage(hashcode, lat, lon, title, data, source, x, y) {
                                // si el mapa no se ha cargado, regresamos
                                if (map == undefined) return;
                                var pin = {
                                    latitude: parseFloat(lat),
                                    longitude: parseFloat(lon),
                                    altitude: 0,
                                    altitudeReference: -1
                                };

                                var pushpin = new Microsoft.Maps.Pushpin(pin, {
                                    icon: source,
                                    anchor: new Microsoft.Maps.Point(x, y),
                                    title: title
                                });

                                pushpin.metadata = hashcode;

                                map.entities.push(pushpin);

                                pushpin.setOptions({ enableHoverStyle: true, enableClickedStyle: true });

                                Microsoft.Maps.Events.addHandler(pushpin, 'click', function (pin) {
                                    invokePinClick(JSON.stringify(pin.target.metadata));
                                });
                            }

                            function addPin(hashcode, lat, lon, title, data) {
                                // si el mapa no se ha cargado, regresamos
                                if (map == undefined) return;
                                var pin =
                                {
                                    latitude: parseFloat(lat),
                                    longitude: parseFloat(lon),
                                    altitude: 0,
                                    altitudeReference: -1
                                };

                                var pushpin = new Microsoft.Maps.Pushpin(pin, {
                                    title: title
                                });

                                pushpin.metadata = hashcode;

                                map.entities.push(pushpin);

                                pushpin.setOptions({ enableHoverStyle: true, enableClickedStyle: true });

                                Microsoft.Maps.Events.addHandler(pushpin, 'click', function (pin) {
                                    invokePinClick(JSON.stringify(pin.target.metadata));
                                });
                            }

                            function removePin(hash){
                                for (var i = map.entities.getLength() - 1; i >= 0; i--) {
                                    var pushpin = map.entities.get(i);
                                    if (pushpin instanceof Microsoft.Maps.Pushpin) {
                                        if(pushpin.metadata == hash){
                                            map.entities.removeAt(i);
                                            break;
                                        }
                                    }
                                }
                            }

                            function removeAllPins() {
                                for (var i = map.entities.getLength() - 1; i >= 0; i--) {
                                    var pushpin = map.entities.get(i);
                                    if (pushpin instanceof Microsoft.Maps.Pushpin) {
                                        map.entities.removeAt(i);
                                    }
                                }
                            };

                            function addPolyline(polyline, hashcode){
                                var polylinelocations = polyline.Locations;
                                var maplocations = [];
                                for (let index = 0; index < polylinelocations.length; index++) {
                                    const location = polylinelocations[index];
                                    maplocations.push(new Microsoft.Maps.Location(location.Latitude, location.Longitude));
                                }
                                var polyline = new Microsoft.Maps.Polyline(maplocations, polyline.Style);
                                polyline.metadata = hashcode;
                                Microsoft.Maps.Events.addHandler(polyline, 'click', function (args) {
                                    invokePolylineClick(args.target.metadata);
                                });
                                map.entities.push(polyline);
                            }

                            function zoomForLocations(latlngarray) {
                                if (map != undefined) {
                                    for (let index = 0; index < latlngarray.length; index++) {
                                        const location = latlngarray[index];
                                        locations.push(new Microsoft.Maps.Location(location.Latitude, location.Longitude));
                                    }
                                    if (locations.length > 0) {
                                        var bestview = Microsoft.Maps.LocationRect.fromLocations(locations);
                                        map.setView({ bounds: bestview });
                                    }
                                }
                            }

                            function removeAllPolylines(){
                                for (var i = map.entities.getLength() - 1; i >= 0; i--) {
                                    var polyline = map.entities.get(i);
                                    if (polyline instanceof Microsoft.Maps.Polyline) {
                                        map.entities.removeAt(i);
                                    }
                                }
                            }

                            function addPolygon(polygon, hashcode){
                                var polylinelocations = polygon.Locations;
                                var maplocations = [];
                                for (let index = 0; index < polylinelocations.length; index++) {
                                    const location = polylinelocations[index];
                                    maplocations.push(new Microsoft.Maps.Location(location.Latitude, location.Longitude));
                                }
                                var polygon = new Microsoft.Maps.Polygon(maplocations, polygon.Style);
                                polygon.metadata = hashcode;
                                Microsoft.Maps.Events.addHandler(polygon, 'click', function (args) {
                                    invokePolygonClick(args.target.metadata);
                                });
                                map.entities.push(polygon);
                            }

                            function removeAllPolygons(){
                                for (var i = map.entities.getLength() - 1; i >= 0; i--) {
                                    var polygon = map.entities.get(i);
                                    if (polygon instanceof Microsoft.Maps.Polygon) {
                                        map.entities.removeAt(i);
                                    }
                                }
                            }
                        </script>
                    </head>
                    <body onload='loadMapScenario();'>
                        <div id='printoutPanel'></div>
                        <div id='bingv8map' style='width: 100vw; height: 100vh;'></div>
                    </body>
                    </html>";
        }
    }

    public class WebClient : WebViewClient
    {
        System.Action ActionError;

        public WebClient(System.Action error)
        {
            ActionError = error;
        }

        public override void OnReceivedError(Android.Webkit.WebView view, [GeneratedEnum] ClientError errorCode, string description, string failingUrl)
        {
            base.OnReceivedError(view, errorCode, description, failingUrl);
            System.Diagnostics.Debug.WriteLine(description);
            ActionError?.Invoke();
        }

        public override void OnReceivedError(Android.Webkit.WebView view, IWebResourceRequest request, WebResourceError error)
        {
            base.OnReceivedError(view, request, error);
            System.Diagnostics.Debug.WriteLine(error);
            ActionError?.Invoke();
        }

        public override void OnReceivedHttpError(Android.Webkit.WebView view, IWebResourceRequest request, WebResourceResponse errorResponse)
        {
            base.OnReceivedHttpError(view, request, errorResponse);
            System.Diagnostics.Debug.WriteLine(errorResponse);
            ActionError?.Invoke();
        }

        public override void OnReceivedSslError(Android.Webkit.WebView view, SslErrorHandler handler, SslError error)
        {
            base.OnReceivedSslError(view, handler, error);
            System.Diagnostics.Debug.WriteLine(error);
            ActionError?.Invoke();
        }
    }

}