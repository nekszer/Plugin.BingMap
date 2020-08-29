using Plugin.Media;
using System.IO;
using System.Threading.Tasks;

namespace BingMapsTest.Services
{
    public class CameraStreamSource : IStreamSource
    {
        public async Task<Stream> Get()
        {
            if (!CrossMedia.Current.IsCameraAvailable) return null;
            var media = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
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