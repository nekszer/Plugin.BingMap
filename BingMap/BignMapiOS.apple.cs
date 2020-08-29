using Foundation;
using System;
using System.IO;
using System.Linq;
using WebKit;
using Xamarin.Forms.Platform.iOS;

namespace Plugin.BingMap
{
    public class BingMapiOS : ViewRenderer<Map, WKWebView>, IWKScriptMessageHandler
    {
        const string PinClick = "function invokePinClick(data){window.webkit.messageHandlers.PinClick.postMessage(data);}";
        const string OnLoadComplete = "function invokeOnLoadComplete(data){window.webkit.messageHandlers.OnLoadComplete.postMessage(data);}";

        const string OnLoadCompleteMethod = "OnLoadComplete";
        const string PinClickMethod = "PinClick";

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
                var html = GetHtml(e.NewElement.ApiKey);
                Control.LoadHtmlString(new NSString(html), new NSUrl("/"));
                // string fileName = Path.Combine(NSBundle.MainBundle.BundlePath, string.Format("BingMap/index.html"));
                // Control.LoadRequest(new NSUrlRequest(new NSUrl(fileName, false)));
            }

            if (Element != null)
                Element.ReceiveAction += Element_Send;
        }

        private string GetHtml(string apiKey)
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
                        font-family: 'Segoe UI', Helvetica, Arial, Sans-Serif
                    }
                </style>
                <meta name='viewport' content='width=device-width, initial-scale=1'>
                <meta http-equiv='Content-Type' content='text/html; charset=utf-8' />
                <script type='text/javascript' src='https://www.bing.com/api/maps/mapcontrol?key=" + apiKey + @"'></script>
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
                            liteMode: true
                        });
                        // invocamos al metodo, on load complete
                        invokeOnLoadComplete('bingmapv8_loadcomplete');
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

                        locations.push(new Microsoft.Maps.Location(lat, lon));

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

                        pushpin.setOptions({
                            enableHoverStyle: true,
                            enableClickedStyle: true
                        });

                        Microsoft.Maps.Events.addHandler(pushpin, 'click', function(pin) {
                            invokePinClick(JSON.stringify(pin.target.metadata));
                        });
                    }

                    function addPin(hashcode, lat, lon, title, data) {
                        // si el mapa no se ha cargado, regresamos
                        if (map == undefined) return;

                        locations.push(new Microsoft.Maps.Location(lat, lon));

                        var pin = {
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

                        pushpin.setOptions({
                            enableHoverStyle: true,
                            enableClickedStyle: true
                        });

                        Microsoft.Maps.Events.addHandler(pushpin, 'click', function(pin) {
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
                        locations = [];
                    };

                    function zoomForAllPins() {
                        if (map != undefined) {
                            if (locations.length > 0) {
                                var bestview = Microsoft.Maps.LocationRect.fromLocations(locations);
                                map.setView({
                                    bounds: bestview
                                });
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
                                var r = await Control.EvaluateJavaScriptAsync($"setCenter({center.Latitude},{center.Longitude},{center.Zoom})");
                            }
                            break;

                        case Action.AddPin:
                            if (e is Pin pin)
                            {
                                if (pin.Image != null)
                                {
                                    var r = await Control.EvaluateJavaScriptAsync($"addPinImage({pin.GetHashCode()}, {pin.Latitude}, {pin.Longitude}, '{pin.Title}', '{pin.Data}', '{pin.Image.Source}', {pin.Image.X}, {pin.Image.Y})");
                                }
                                else
                                {
                                    var r = await Control.EvaluateJavaScriptAsync($"addPin({pin.GetHashCode()}, {pin.Latitude}, {pin.Longitude}, '{pin.Title}', '{pin.Data}')");
                                }
                            }
                            break;

                        case Action.RemoveAllPins:
                            if (e is null)
                            {
                                var r = await Control.EvaluateJavaScriptAsync("removeAllPins()");
                            }
                            break;

                        case Action.ZoomForAllPins:
                            if (e is null)
                            {
                                var r = await Control.EvaluateJavaScriptAsync("zoomForAllPins()");
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            var name = message.Name;
            var result = message.Body.ToString();

            switch (name)
            {
                case OnLoadCompleteMethod:
                    System.Diagnostics.Debug.WriteLine("OnLoadComplete", "BingMap");
                    Element.OnLoadComplete(result);
                    break;

                case PinClickMethod:
                    Pin pin = null;
                    try
                    {
                        pin = Element.DeserializeObject<Pin>(result);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.StackTrace, "BingMap");
                    }

                    if (pin != null)
                    {
                        var pinfound = Element.Pins.FirstOrDefault(p => p.GetHashCode() == pin.HashCode);
                        if (pinfound != null)
                        {
                            System.Diagnostics.Debug.WriteLine("PinClick", "BingMap");
                            pinfound.OnClick();
                        }
                    }
                    break;

                default:
                    break;
            }
        }
    }
}