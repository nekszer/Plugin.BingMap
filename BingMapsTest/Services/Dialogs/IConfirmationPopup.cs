using System.Threading.Tasks;

namespace BingMapsTest.Services
{
    public interface IConfirmationPopup
    {

        Task<bool> Show(string title, string message, string ok = null, string cancel = null);

    }
}