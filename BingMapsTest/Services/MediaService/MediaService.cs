using LightForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BingMapsTest.Services
{
    public class MediaService : IMediaService
    {
        public async Task<Stream> GetMedia(string title, string cancel)
        {
            var container = CrossContainer.Instance;
            var source = new Dictionary<string, string>();
            foreach (MediaSource item in Enum.GetValues(typeof(MediaSource)))
            {
                try { source.Add(GetTranslate(item), item.ToString()); } catch { }
            }
            var response = await container.Create<IActionSheetService>().Show(title, cancel, source.Select(s => s.Key).ToArray());
            if (!source.ContainsKey(response)) return null;
            if (!Enum.TryParse(source[response], out MediaSource selectedsource)) return null;
            var factory = CrossContainer.Instance.Create<IEnumFactory<MediaSource, IStreamSource>>();
            var streamsource = factory.Resolve(selectedsource);
            return await streamsource.Get();
        }

        public string GetTranslate(MediaSource source)
        {
            switch (source)
            {
                case MediaSource.Camera:
                    return TranslateCamera();

                case MediaSource.Galery:
                    return TranslateGallery();
            }
            return string.Empty;
        }

        private string TranslateGallery()
        {
            var culture = LightFormsApplication.Instance.Culture ?? new CultureInfo("en");
            if (culture.TwoLetterISOLanguageName == "en")
            {
                return "Elegir foto";
            }
            else if (culture.TwoLetterISOLanguageName == "es")
            {
                return "Pick photo";
            }
            return "Pick photo";
        }

        private string TranslateCamera()
        {
            var culture = LightFormsApplication.Instance.Culture ?? new CultureInfo("en");
            if (culture.TwoLetterISOLanguageName == "en")
            {
                return "Take photo";
            }
            else if (culture.TwoLetterISOLanguageName == "es")
            {
                return "Tomar foto";
            }
            return "Take photo";
        }
    }
}