using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tantra.Extensions
{
    [ContentProperty("Command")]
    public class BackFromPopup : IMarkupExtension
    {
        public object Parameter { get; set; }
        public ICommand Command { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var command = (ICommand)new LightForms.Commands.AsyncCommand(async () =>
            {
                var instance = Rg.Plugins.Popup.Services.PopupNavigation.Instance;
                if (instance.PopupStack.Count == 0) return;
                if (Command != null)
                    Command.Execute(Parameter);
                await instance.PopAllAsync();
            });
            return command;
        }

    }
}
