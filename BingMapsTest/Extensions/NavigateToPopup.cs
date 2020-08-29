using LightForms.Services;
using Rg.Plugins.Popup.Pages;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tantra.Extensions
{
    [ContentProperty(nameof(Route))]
    public class NavigateToPopup : IMarkupExtension
    {
        public string Route { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var command = (ICommand)new LightForms.Commands.AsyncCommand(async () =>
           {
               var page = (PopupPage)LightForms.CrossContainer.Instance.Create<INavigationService>().Resolve(Route);
               await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);
           });
            return command;
        }

    }
}