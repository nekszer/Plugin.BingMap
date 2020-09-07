using Android.Content;
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
        const string PinClick = "function invokePinClick(str){jsBridge.pinClick(str);}";
        const string OnLoadComplete = "function invokeOnLoadComplete(str){jsBridge.onLoadComplete(str);}";
        const string OnMapClick = "function onMapClick(lat, lng) { jsBridge.onMapClick(lat, lng); }";
        const string OnViewChange = "function onViewChange(event, lat, lng) { jsBridge.onViewChange(event, lat, lng); }";

        Context _context;

        public BingMapAndroid(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var webView = new Android.Webkit.WebView(_context)
                {
                    LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
                };
                webView.Settings.SetGeolocationEnabled(true);
                webView.Settings.AllowContentAccess = true;
                webView.Settings.JavaScriptEnabled = true;
                SetNativeControl(webView);
            }

            if (e.NewElement != null)
            {
                var html = GetHtml(e.NewElement.ApiKey, e.NewElement.Theme, e.NewElement.Style);
                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                Control.LoadDataWithBaseURL(null, html, "text/html", "UTF-8", null);
                // InjectJS(OnLoadComplete);
                // InjectJS(PinClick);
                // InjectJS(OnMapClick);
                // InjectJS(OnViewChange);
            }

            if (Element != null)
                Element.ReceiveAction += Element_Send;
            
        }

        private void Element_Send(object sender, object e)
        {
            if (Control != null)
            {
                if (sender is Map view)
                {
                    switch (view.Action)
                    {
                        case Plugin.BingMap.Action.SetCenter:
                            if (e is Center center)
                            {
                                Control.EvaluateJavascript($"setCenter({center.Latitude},{center.Longitude},{center.Zoom})", null);
                            }
                            break;

                        case Plugin.BingMap.Action.AddPin:
                            if (e is Pin pin)
                            {
                                if (pin.Image != null)
                                {
                                    Control.EvaluateJavascript($"addPinImage({pin.GetHashCode()}, {pin.Latitude}, {pin.Longitude}, '{pin.Title}', '{pin.Data}', '{pin.Image.Source}', {pin.Image.X}, {pin.Image.Y})", null);
                                }
                                else
                                {
                                    Control.EvaluateJavascript($"addPin({pin.GetHashCode()}, {pin.Latitude}, {pin.Longitude}, '{pin.Title}', '{pin.Data}')", null);
                                }
                            }
                            break;

                        case Plugin.BingMap.Action.RemoveAllPins:
                            if (e is null)
                            {
                                Control.EvaluateJavascript("removeAllPins()", null);
                            }
                            break;

                        case Plugin.BingMap.Action.Polyline:
                            if(e is Polyline polyline)
                            {
                                Control.EvaluateJavascript($"addPolyline({JsonConvert.SerializeObject(polyline)}, {polyline.GetHashCode()})", null);
                            }
                            break;

                        case Plugin.BingMap.Action.RemoveAllPolylines:
                            if(e is null)
                            {
                                Control.EvaluateJavascript($"removeAllPolylines()", null);
                            }
                            break;

                        case Plugin.BingMap.Action.ZoomForLocations:
                            if(e is IEnumerable<Location> locations)
                            {
                                var json = JsonConvert.SerializeObject(locations);
                                Control.EvaluateJavascript($"zoomForLocations({json})", null);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        void InjectJS(string script)
        {
            if (Control != null)
            {
                Control.Post(() =>
                {
                    Control.LoadUrl(string.Format("javascript: {0}", script));
                });
            }
        }

        private string GetHtml(string apikey, MapTheme theme, MapStyle style)
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

                            function loadMapScenario() {
                                // iniciamos el mapa
                                map = new Microsoft.Maps.Map(document.getElementById('bingv8map'), {
                                    showBreadcrumb: false,
                                    showLocateMeButton: false,
                                    showMapTypeSelector: false,
                                    showZoomButtons: false,
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

                                pushpin.metadata = {
                                    HashCode: hashcode,
                                    Latitude: lat,
                                    Longitude: lon,
                                    Title: title,
                                    Data: data,
                                    Image: {
                                        Source: source,
                                        X: x,
                                        Y: y
                                    }
                                };

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

                                pushpin.metadata = {
                                    HashCode: hashcode,
                                    Latitude: lat,
                                    Longitude: lon,
                                    Title: title,
                                    Data: data,
                                    Image: {
                                        Source: '',
                                        X: 0,
                                        Y: 0
                                    }
                                };

                                map.entities.push(pushpin);

                                pushpin.setOptions({ enableHoverStyle: true, enableClickedStyle: true });

                                Microsoft.Maps.Events.addHandler(pushpin, 'click', function (pin) {
                                    invokePinClick(JSON.stringify(pin.target.metadata));
                                });
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
                        </script>
                    </head>
                    <body onload='loadMapScenario();'>
                        <div id='printoutPanel'></div>
                        <div id='bingv8map' style='width: 100vw; height: 100vh;'></div>
                    </body>
                    </html>";
        }
    }

}