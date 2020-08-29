using System.IO;
using System.Threading.Tasks;

namespace BingMapsTest.Services
{
    public interface IMediaService
    {
        Task<Stream> GetMedia(string title, string cancel);
    }
}
