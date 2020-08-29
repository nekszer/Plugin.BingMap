using BingMapsTest.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingMapsTest.UWP.Services
{
    public class LocalNotificationImplementation : ILocalNotification
    {
        public event EventHandler<Dictionary<string, string>> NotificationReceived;

        public void Clear(int id = 0)
        {
            throw new NotImplementedException();
        }

        public void ReceiveNotification(Dictionary<string, string> data)
        {
            throw new NotImplementedException();
        }

        public ILocalNotification SetAction(NotificationAction action)
        {
            throw new NotImplementedException();
        }

        public ILocalNotification SetMessage(string message)
        {
            throw new NotImplementedException();
        }

        public ILocalNotification SetTile(string title)
        {
            throw new NotImplementedException();
        }

        public ILocalNotification SetWhen(TimeSpan when)
        {
            throw new NotImplementedException();
        }

        public Task<int> Show()
        {
            throw new NotImplementedException();
        }
    }
}
