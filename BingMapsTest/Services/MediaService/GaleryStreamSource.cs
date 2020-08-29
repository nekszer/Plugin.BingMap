using Plugin.Media;
using System.IO;
using System.Threading.Tasks;

namespace BingMapsTest.Services
{
    public class GaleryStreamSource : IStreamSource
    {
        public async Task<Stream> Get()
        {
            if (!CrossMedia.Current.IsPickPhotoSupported) return null;
            var media = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                SaveMetaData = true,
                MaxWidthHeight = 1280,
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                CustomPhotoSize = 75
            });
            return media?.GetStream();
        }
    }
}