using System.Threading.Tasks;

namespace BingMapsTest.Services
{
    public interface IActionSheetService
    {
        Task<string> Show(string title, string cancel, params string[] options);
    }
}