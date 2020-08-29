using System.IO;
using System.Threading.Tasks;

namespace BingMapsTest.Services
{
    public interface IStreamSource
    {
        Task<Stream> Get();
    }
}