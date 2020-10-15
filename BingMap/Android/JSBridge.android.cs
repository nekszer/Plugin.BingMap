using Android.Webkit;
using Java.Interop;
using System;
using System.Linq;

namespace Plugin.BingMap
{
    public class JSBridge : Java.Lang.Object
    {
        readonly BingMapAndroid Bingmap;

        public JSBridge(BingMapAndroid hybridRenderer)
        {
            Bingmap = hybridRenderer;
        }

        [JavascriptInterface]
        [Export("polylineClick")]
        public void PolylineClick(string hashcode)
        {
            try
            {
                Bingmap.Post(() =>
                {
                    try
                    {
                        if (int.TryParse(hashcode, out int hash))
                        {
                            var polylineitem = Bingmap.Element.Polylines.FirstOrDefault(e => e.GetHashCode() == hash);
                            if (polylineitem != null)
                            {
                                System.Diagnostics.Debug.WriteLine("PolylineClick", "BingMap");
                                polylineitem.OnClick();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Map.OnErrorHandler(this, ex);
                    }
                });
            }
            catch(Exception ex)
            {
                Map.OnErrorHandler(this, ex);
            }
        }

        [JavascriptInterface]
        [Export("onViewChange")]
        public void OnViewChange(string e, string lat, string lng)
        {
            try
            {
                Bingmap.Post(() =>
                {
                    try
                    {
                        Bingmap.Element.OnViewChange(e, double.Parse(lat), double.Parse(lng));
                    }
                    catch (Exception ex)
                    {
                        Map.OnErrorHandler(this, ex);
                    }
                });
            }
            catch (Exception ex)
            {
                Map.OnErrorHandler(this, ex);
            }
        }

        [JavascriptInterface]
        [Export("onMapClick")]
        public void OnMapClick(string lat, string lng)
        {
            try
            {
                Bingmap.Post(() =>
                {
                    try
                    {
                        Bingmap.Element.OnMapClicked(double.Parse(lat), double.Parse(lng));
                    }
                    catch (Exception ex)
                    {
                        Map.OnErrorHandler(this, ex);
                    }
                });
            }
            catch(Exception ex)
            {
                Map.OnErrorHandler(this, ex);
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        [JavascriptInterface]
        [Export("pinClick")]
        public void PinClick(string str)
        {
            try
            {
                Bingmap.Post(() =>
                {
                    try
                    {
                        var pininlist = Bingmap.Element.Pins.FirstOrDefault(e => e.GetHashCode() == int.Parse(str));
                        if (pininlist != null)
                        {
                            System.Diagnostics.Debug.WriteLine("PinClick", "BingMap");
                            pininlist.OnClick();
                        }
                    }
                    catch (Exception ex)
                    {
                        Map.OnErrorHandler(this, ex);
                    }
                });
            }
            catch(Exception ex)
            {
                Map.OnErrorHandler(this, ex);
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        [JavascriptInterface]
        [Export("onLoadComplete")]
        public void OnLoadComplete(string str)
        {
            System.Diagnostics.Debug.WriteLine("OnLoadComplete", "BingMap");
            try
            {
                Bingmap.Post(() =>
                {
                    try
                    {
                        Bingmap.Element.OnLoadComplete(str);
                    }
                    catch (Exception ex)
                    {
                        Map.OnErrorHandler(this, ex);
                    }
                });
            }
            catch (Exception ex)
            {
                Map.OnErrorHandler(this, ex);
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}
