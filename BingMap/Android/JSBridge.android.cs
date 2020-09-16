using Android.Webkit;
using Java.Interop;
using System;
using System.Linq;

namespace Plugin.BingMap
{
    public class JSBridge : Java.Lang.Object
    {
        readonly WeakReference<BingMapAndroid> _bingmap;

        public JSBridge(BingMapAndroid hybridRenderer)
        {
            _bingmap = new WeakReference<BingMapAndroid>(hybridRenderer);
        }

        [JavascriptInterface]
        [Export("polylineClick")]
        public void PolylineClick(string hashcode)
        {
            if (_bingmap != null && _bingmap.TryGetTarget(out BingMapAndroid bingmap))
            {
                bingmap.Post(() =>
                {
                    if (int.TryParse(hashcode, out int hash))
                    {
                        var polylineitem = bingmap.Element.Polylines.FirstOrDefault(e => e.GetHashCode() == hash);
                        if (polylineitem != null)
                        {
                            System.Diagnostics.Debug.WriteLine("PolylineClick", "BingMap");
                            polylineitem.OnClick();
                        }
                    }
                });
            }
        }

        [JavascriptInterface]
        [Export("onViewChange")]
        public void OnViewChange(string e, string lat, string lng)
        {
            if (_bingmap != null && _bingmap.TryGetTarget(out BingMapAndroid bingmap))
            {
                bingmap.Post(() =>
                {
                    bingmap.Element.OnViewChange(e, double.Parse(lat), double.Parse(lng));
                });
            }
        }

        [JavascriptInterface]
        [Export("onMapClick")]
        public void OnMapClick(string lat, string lng)
        {
            if (_bingmap != null && _bingmap.TryGetTarget(out BingMapAndroid bingmap))
            {
                bingmap.Post(() =>
                {
                    bingmap.Element.OnMapClicked(double.Parse(lat), double.Parse(lng));
                });
            }
        }

        [JavascriptInterface]
        [Export("pinClick")]
        public void PinClick(string str)
        {
            if (_bingmap != null && _bingmap.TryGetTarget(out BingMapAndroid bingmap))
            {
                bingmap.Post(() =>
                {
                    var pininlist = bingmap.Element.Pins.FirstOrDefault(e => e.GetHashCode() == int.Parse(str));
                    if (pininlist != null)
                    {
                        System.Diagnostics.Debug.WriteLine("PinClick", "BingMap");
                        pininlist.OnClick();
                    }
                });
            }
        }

        [JavascriptInterface]
        [Export("onLoadComplete")]
        public void OnLoadComplete(string str)
        {
            System.Diagnostics.Debug.WriteLine("OnLoadComplete", "BingMap");
            if (_bingmap != null && _bingmap.TryGetTarget(out BingMapAndroid bingmap))
            {
                bingmap.Post(() =>
                {
                    bingmap.Element.OnLoadComplete(str);
                });
            }
        }
    }
}
