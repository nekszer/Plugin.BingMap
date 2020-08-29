using LightForms.Services;

namespace BingMapsTest.Services
{
    public class ServiceHandler : IServiceHandler
    {
        public const string ServicesNameSpace = "BingMapsTest.Services";
        string IServiceHandler.ServicesNameSpace
        {
            get => ServicesNameSpace;
            set { }
        }
    }
}