using Foundation;
using Newtonsoft.Json;
using Plugin.BingMap;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Map), typeof(BingMapiOS))]
namespace Plugin.BingMap
{
    public class BingMapiOS : ViewRenderer<Map, WKWebView>, IWKScriptMessageHandler
    {
        const string PinClick = "function invokePinClick(hashcode){window.webkit.messageHandlers.PinClick.postMessage(hashcode);}";
        const string PinClickMethod = "PinClick";

        const string OnLoadComplete = "function invokeOnLoadComplete(data){window.webkit.messageHandlers.OnLoadComplete.postMessage(data);}";
        const string OnLoadCompleteMethod = "OnLoadComplete";

        const string OnMapClick = "function onMapClick(lat, lng) { window.webkit.messageHandlers.OnMapClick.postMessage(JSON.stringify({ lat: lat, lng: lng })); }";
        const string OnMapClickMethod = "OnMapClick";

        const string OnViewChange = "function onViewChange(event, lat, lng) { window.webkit.messageHandlers.OnViewChange.postMessage(JSON.stringify({ event: event, lat: lat, lng: lng })); }";
        const string OnViewChangeMethod = "OnViewChange";

        const string OnPolylineClick = "function invokePolylineClick(hashcode) { window.webkit.messageHandlers.OnPolylineClick.postMessage(hashcode); }";
        const string OnPolylineClickMethod = "OnPolylineClick";

        WKUserContentController userController;

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                userController = new WKUserContentController();

                var script = new WKUserScript(new NSString(OnLoadComplete), WKUserScriptInjectionTime.AtDocumentEnd, false);
                userController.AddUserScript(script);
                userController.AddScriptMessageHandler(this, OnLoadCompleteMethod);

                script = new WKUserScript(new NSString(PinClick), WKUserScriptInjectionTime.AtDocumentEnd, false);
                userController.AddUserScript(script);
                userController.AddScriptMessageHandler(this, PinClickMethod);

                script = new WKUserScript(new NSString(OnMapClick), WKUserScriptInjectionTime.AtDocumentEnd, false);
                userController.AddUserScript(script);
                userController.AddScriptMessageHandler(this, OnMapClickMethod);

                script = new WKUserScript(new NSString(OnViewChange), WKUserScriptInjectionTime.AtDocumentEnd, false);
                userController.AddUserScript(script);
                userController.AddScriptMessageHandler(this, OnViewChangeMethod);

                script = new WKUserScript(new NSString(OnPolylineClick), WKUserScriptInjectionTime.AtDocumentEnd, false);
                userController.AddUserScript(script);
                userController.AddScriptMessageHandler(this, OnPolylineClickMethod);

                var config = new WKWebViewConfiguration { UserContentController = userController };
                var webView = new WKWebView(Frame, config);
                SetNativeControl(webView);
            }
            if (e.OldElement != null)
            {
                userController.RemoveAllUserScripts();
                userController.RemoveScriptMessageHandler(OnLoadCompleteMethod);
                userController.RemoveScriptMessageHandler(PinClickMethod);
                var hybridWebView = e.OldElement as Map;
            }
            if (e.NewElement != null)
            {
                string contentDirectoryPath = Path.Combine(NSBundle.MainBundle.BundlePath, "Content/");
                var html = GetHtml(e.NewElement.ApiKey, e.NewElement.Theme, e.NewElement.Style);
                Control.LoadHtmlString(html, new NSUrl(contentDirectoryPath, true));
            }

            if (Element != null)
                Element.ReceiveAction += Element_Send;
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
                                    invokePinClick(JSON.stringify(pin.target.metadata.HashCode));
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
                                    invokePinClick(pin.target.metadata);
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

        private async void Element_Send(object sender, object e)
        {
            if (Control != null)
            {
                if (sender is Map view)
                {
                    switch (view.Action)
                    {
                        case Action.SetCenter:
                            if (e is Center center)
                            {
                                await Control.EvaluateJavaScriptAsync($"setCenter({center.Latitude},{center.Longitude},{center.Zoom})");
                            }
                            break;

                        case Action.AddPin:
                            if (e is Pin pin)
                            {
                                if (pin.Image != null)
                                {
                                    await Control.EvaluateJavaScriptAsync($"addPinImage({pin.GetHashCode()}, {pin.Latitude}, {pin.Longitude}, '{pin.Title}', '{pin.Data}', '{pin.Image.Source}', {pin.Image.X}, {pin.Image.Y})");
                                }
                                else
                                {
                                    await Control.EvaluateJavaScriptAsync($"addPin({pin.GetHashCode()}, {pin.Latitude}, {pin.Longitude}, '{pin.Title}', '{pin.Data}')");
                                }
                            }
                            break;

                        case Action.RemoveAllPins:
                            if (e is null)
                            {
                                await Control.EvaluateJavaScriptAsync("removeAllPins()");
                            }
                            break;

                        case Action.Polyline:
                            if (e is Polyline polyline)
                            {
                                await Control.EvaluateJavaScriptAsync($"addPolyline({JsonConvert.SerializeObject(polyline)}, {polyline.GetHashCode()})");
                            }
                            break;

                        case Action.RemoveAllPolylines:
                            if (e is null)
                            {
                                await Control.EvaluateJavaScriptAsync($"removeAllPolylines()");
                            }
                            break;

                        case Action.ZoomForLocations:
                            if (e is IEnumerable<Location> locations)
                            {
                                var json = JsonConvert.SerializeObject(locations);
                                await Control.EvaluateJavaScriptAsync($"zoomForLocations({json})");
                            }
                            break;

                        case Action.Reload:
                            if (e is null)
                            {
                                Control.Reload();
                            }
                            break;
                    }
                }
            }
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            var name = message.Name;
            var result = message.Body.ToString();

            System.Diagnostics.Debug.WriteLine(name);
            System.Diagnostics.Debug.WriteLine(result);

            switch (name)
            {
                case OnLoadCompleteMethod:
                    Element.OnLoadComplete(result);
                    break;

                case PinClickMethod:
                    if (!string.IsNullOrEmpty(result))
                    {
                        var pinfound = Element.Pins.FirstOrDefault(p => p.GetHashCode() == int.Parse(result));
                        pinfound?.OnClick();
                    }
                    break;

                case OnMapClickMethod:
                    {
                        var viewchangeargs = JsonConvert.DeserializeObject<ViewChangeArgs>(result);
                        Element.OnMapClicked(viewchangeargs.Latitude, viewchangeargs.Longitude);
                    }
                    break;

                case OnViewChangeMethod:
                    {
                        var viewchangeargs = JsonConvert.DeserializeObject<ViewChangeArgs>(result);
                        Element.OnViewChange(viewchangeargs.Event, viewchangeargs.Latitude, viewchangeargs.Longitude);
                    }
                    break;

                case OnPolylineClickMethod:
                    if (!string.IsNullOrEmpty(result))
                    {
                        var polylinefound = Element.Polylines.FirstOrDefault(p => p.GetHashCode() == int.Parse(result));
                        polylinefound?.OnClick();
                    }
                    break;

                default:
                    break;
            }
        }

        private class ViewChangeArgs
        {
            [JsonProperty("event")]
            public string Event { get; set; }

            [JsonProperty("lat")]
            public double Latitude { get; set; }

            [JsonProperty("lng")]
            public double Longitude { get; set; }

        }
    }
}