using Acr.UserDialogs;

namespace BingMapsTest.Services
{
    public class ProgressPopup : IProgressPopup
    {
        public void Show()
        {
            UserDialogs.Instance.ShowLoading();
        }

        public void Hide()
        {
            UserDialogs.Instance.HideLoading();
        }
    }
}