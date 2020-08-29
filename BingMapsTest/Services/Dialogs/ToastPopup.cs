using Acr.UserDialogs;

namespace BingMapsTest.Services
{
    public class ToastPopup : IToastPopup
    {
        public void Show(string text)
        {
            UserDialogs.Instance.Toast(text);
        }
    }
}