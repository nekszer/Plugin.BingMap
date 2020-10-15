using Newtonsoft.Json;
using Plugin.BingMap;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Map), typeof(BingMapUWP))]
namespace Plugin.BingMap
{
    public class BingMapUWP : ViewRenderer<Map, MapControl>
    {
        private MapControl BingMap { get; set; }

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                BingMap = new MapControl
                {
                    ZoomInteractionMode = MapInteractionMode.GestureAndControl,
                    TiltInteractionMode = MapInteractionMode.GestureAndControl,
                    MapServiceToken = Element.ApiKey,
                    VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch,
                    HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch
                };
                BingMap.MapElementClick += BingMap_MapElementClick;
                BingMap.Loaded += BingMap_Loaded;
                SetNativeControl(BingMap);
            }

            if (Element != null)
                SetFromElement(Element);
        }

        private void BingMap_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Map Load Completed");
            Element?.OnLoadComplete("Load complete");
        }

        private void BingMap_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            var latitude = args.Location.Position.Latitude;
            var longitude = args.Location.Position.Longitude;
            Element?.OnMapClicked(latitude, longitude);
        }

        private void SetFromElement(Map element)
        {
            element.ReceiveAction += Element_Send;
            switch (element.Theme)
            {
                case MapTheme.Default:
                    BingMap.StyleSheet = MapStyleSheet.RoadLight();
                    break;
                case MapTheme.Dark:
                    BingMap.StyleSheet = MapStyleSheet.RoadDark();
                    break;
                case MapTheme.Light:
                    BingMap.StyleSheet = MapStyleSheet.RoadLight();
                    break;
                case MapTheme.CustomTile:
                    BingMap.StyleSheet = MapStyleSheet.ParseFromJson(@"{
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
                    }");
                    break;
                case MapTheme.CustomStyle:
                    BingMap.StyleSheet = MapStyleSheet.ParseFromJson(JsonConvert.SerializeObject(Element.Style));
                    break;
            }
        }

        private async void Element_Send(object sender, object e)
        {
            if (!(sender is Map view))
                return;

            if (BingMap == null) return;

            switch (view.Action)
            {
                case Action.SetCenter:
                    if (!(e is Center center)) return;
                    BasicGeoposition cityPosition = new BasicGeoposition() { Latitude = center.Latitude, Longitude = center.Longitude };
                    Geopoint cityCenter = new Geopoint(cityPosition);
                    BingMap.Center = cityCenter;
                    BingMap.ZoomLevel = center.Zoom;
                    BingMap.LandmarksVisible = true;
                    break;

                case Action.AddPin:
                    if (!(e is Pin pin)) return;
                    IRandomAccessStreamReference reference = null;
                    if (pin.Image != null)
                    {
                        var bytearray = await ToByteArray(pin.Image);
                        var randomaccesstream = await ConvertToRandomAccessStream(bytearray);
                        reference = RandomAccessStreamReference.CreateFromStream(randomaccesstream);
                    }
                    var pinlayer = new List<MapElement>();
                    BasicGeoposition snPosition = new BasicGeoposition { Latitude = pin.Latitude, Longitude = pin.Longitude };
                    Geopoint snPoint = new Geopoint(snPosition);
                    var mapicon = new MapIcon
                    {
                        Location = snPoint,
                        Title = pin.Title,
                        ZIndex = 0
                    };

                    if (reference != null)
                        mapicon.Image = reference;

                    pinlayer.Add(mapicon);
                    var mapelementlayer = new MapElementsLayer
                    {
                        ZIndex = 1,
                        MapElements = pinlayer
                    };
                    BingMap.Layers.Add(mapelementlayer);
                    break;

                case Action.RemoveAllPins:
                    if (!(e is null)) return;

                    break;

                case Action.Polyline:
                    if (!(e is Polyline polyline)) return;

                    break;

                case Action.RemoveAllPolylines:
                    if (!(e is null)) return;

                    break;

                case Action.ZoomForLocations:
                    if (!(e is IEnumerable<Location> locations)) return;

                    break;

                case Action.Reload:
                    if (!(e is null)) return;
                    break;

                default:
                    break;
            }
        }

        private async Task<InMemoryRandomAccessStream> ConvertToRandomAccessStream(byte[] arr)
        {
            try
            {
                var randomAccessStream = new InMemoryRandomAccessStream();
                await randomAccessStream.WriteAsync(arr.AsBuffer());
                randomAccessStream.Seek(0);
                return randomAccessStream;
            }
            catch (Exception ex)
            {
                Map.OnErrorHandler(this, ex);
            }
            return null;
        }

        private async Task<byte[]> ToByteArray(Image image)
        {
            try
            {
                if (image.ByteArray != null) return image.ByteArray;
                var client = new HttpClient();
                return await client.GetByteArrayAsync(image.Source);
            }
            catch (Exception ex)
            {
                Map.OnErrorHandler(this, ex);
            }
            return null;
        }
    }
}
