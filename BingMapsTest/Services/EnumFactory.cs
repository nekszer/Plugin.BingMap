using System;
using System.Collections.Generic;

namespace BingMapsTest.Services
{
    public class EnumFactory<TEnum, KInterface> : IEnumFactory<TEnum, KInterface>
    {
        private readonly Dictionary<TEnum, KInterface> _factories;

        public EnumFactory()
        {
            _factories = new Dictionary<TEnum, KInterface>();

            foreach (TEnum action in Enum.GetValues(typeof(TEnum)))
            {
                try
                {
                    var subname = typeof(KInterface).Name;
                    if (subname.StartsWith("I")) subname = subname.Remove(0, 1);
                    var factory = (KInterface)Activator.CreateInstance(Type.GetType(ServiceHandler.ServicesNameSpace + "." + Enum.GetName(typeof(TEnum), action) + subname));
                    _factories.Add(action, factory);
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
            }
        }

        public KInterface Resolve(TEnum action)
        {
            if (_factories.ContainsKey(action))
            {
                return _factories[action];
            }
            else
            {
                return default(KInterface);
            }
        }
    }
}
