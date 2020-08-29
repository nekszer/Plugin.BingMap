using LightForms.Services;
using BingMapsTest.Resources.Strings;
using BingMapsTest.ViewModels;
using BingMapsTest.Views;

namespace BingMapsTest
{
    public class AppRoutes
    {

        /// <summary>
        /// Pagina principal de la app
        /// </summary>
        public static string Main { get; } = "/main";

        /// <summary>
        /// RoutingService
        /// </summary>
        /// <param name="routingservice"></param>
        public AppRoutes(IRoutingService routingservice)
        {
            routingservice.Route<MainPage, MainViewModel>(Main, new RouteInfo
            {

            }, new JsonLocalizationManager("Main.json"));
        }
    }
}