using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using BingMapsTest.Services;
using Microsoft.AppCenter.Crashes;
using UIKit;
using UserNotifications;

namespace BingMapsTest.iOS.Services
{
    public class LocalNotificationImplementation : ILocalNotification
    {
        private string Message = string.Empty;
        private string Title = string.Empty;
        private TimeSpan? When;
        int messageId = -1;
        bool hasNotificationsPermission;
        public event EventHandler<Dictionary<string, string>> NotificationReceived;
        private int Action = 0;

        public ILocalNotification SetMessage(string message)
        {
            Message = message ?? "";
            return this;
        }

        public ILocalNotification SetTile(string title)
        {
            Title = title ?? "";
            return this;
        }

        public ILocalNotification SetWhen(TimeSpan when)
        {
            When = when;
            return this;
        }

        public async Task<int> Show()
        {
            if (!hasNotificationsPermission) return -1;

            messageId++;

            var content = new UNMutableNotificationContent
            {
                Title = Title,
                Subtitle = Action.ToString(),
                Body = Message
            };

            var dictionary = new Dictionary<string, string>
            {
                { LocalNotificationConstants.ActionKey, Action.ToString() },
                { LocalNotificationConstants.TitleKey, Title },
                { LocalNotificationConstants.MessageKey, Message },
                { LocalNotificationConstants.IdKey, messageId.ToString() }
            };

            content.UserInfo = NSDictionary.FromObjectsAndKeys(dictionary.Values.ToArray(), dictionary.Keys.ToArray());

            var interval = 0.25;
            if (When.HasValue)
                interval = When.Value.TotalSeconds;

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(interval, false);
            var request = UNNotificationRequest.FromIdentifier(messageId.ToString(), content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to schedule notification: {err}");
                }
            });

            await Task.Delay(1);
            return messageId;
        }

        public void OnLaunching(NSDictionary options)
        {
            if (options == null)
                return;

            if (!options.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey))
            {
                Crashes.TrackError(new Exception("Optios not contain LaunchOptionsLocalNotificationKey"));
                return;
            }

            UILocalNotification localNotification = options[UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
            if (localNotification == null)
            {
                Crashes.TrackError(new Exception("Local Notification is null"));
                return;
            }

            var dic = new Dictionary<string, string>();
            try
            {
                var keys = options.Keys;
                var values = options.Values;
                for (int i = 0; i < keys.Length; i++)
                {
                    dic.Add(keys[i].ToString(), values[i].ToString());
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return;
            }

            ReceiveNotification(dic);
        }

        public Task<bool> Init()
        {
            TaskCompletionSource<bool> completionsource = new TaskCompletionSource<bool>();
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) =>
            {
                hasNotificationsPermission = approved;
                completionsource.SetResult(approved);
            });
            return completionsource.Task;
        }

        public void ReceiveNotification(Dictionary<string, string> data) => NotificationReceived?.Invoke(this, data);

        public ILocalNotification SetAction(NotificationAction action)
        {
            Action = (int)action;
            return this;
        }

        public void Clear(int id = 0)
        {
            if (id <= 0)
                UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
            else
                UNUserNotificationCenter.Current.RemoveDeliveredNotifications(new string[] { id.ToString() });
        }
    }
}