using LightForms;
using LightForms.Services;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BingMapsTest.Resources.Images
{
    [ContentProperty(nameof(Source))]
    public class LocalizeResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null) return null;
            var routing = CrossContainer.Instance.Create<IRoutingService>();
            var navigation = CrossContainer.Instance.Create<INavigationService>();
            var manager = routing.GetLocalizationManager(navigation.Route);
            if (manager == null) return null;
            var str = manager.Localize<string>(Source);
            if (string.IsNullOrEmpty(str)) return null;
            byte[] bytes = null;
            try
            {
                bytes = Convert.FromBase64String(str);
            }
            catch { }
            if (bytes == null) return null;
            var imageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
            return imageSource;
        }
    }
}
