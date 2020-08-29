using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingMapsTest.Services
{
    public interface ILocalNotification
    {

        ILocalNotification SetWhen(TimeSpan when);

        ILocalNotification SetTile(string title);

        ILocalNotification SetMessage(string message);

        ILocalNotification SetAction(NotificationAction action);

        void ReceiveNotification(Dictionary<string, string> data);

        event EventHandler<Dictionary<string, string>> NotificationReceived;
        Task<int> Show();

        void Clear(int id = 0);

    }
}