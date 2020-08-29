using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BingMapsTest.Resources.Images
{
    [ContentProperty(nameof(Source))]
    public class EmbeddedResourceExtension : BaseResource, IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null) return null;
            Stream stream = GetStream(typeof(EmbeddedResourceExtension), Source);
            if (stream == null) return null;
            var imageSource = ImageSource.FromStream(() => stream);
            return imageSource;
        }
    }
}