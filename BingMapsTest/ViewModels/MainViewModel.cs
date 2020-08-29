using LightForms.Attributes;
using BingMapsTest.Services;

namespace BingMapsTest.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        [Injectable]
        public IToastPopup Toast { get; set; }

        #region Notified Property LightFormsVersion
        /// <summary>
        /// LightFormsVersion
        /// </summary>
        private float lightFormsVersion;
        public float LightFormsVersion
        {
            get { return lightFormsVersion; }
            set { lightFormsVersion = value; OnPropertyChanged(); }
        }
        #endregion

        public override void Appearing(string route, object data)
        {
            base.Appearing(route, data);
            LightFormsVersion = 2.4f;
            var resourcemanager = Routing.GetLocalizationManager(route);
            var appname = resourcemanager.Localize<string>("AppName");
            Toast.Show(appname);
        }
    }
}