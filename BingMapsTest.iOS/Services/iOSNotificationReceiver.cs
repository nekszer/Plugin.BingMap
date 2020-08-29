using System;
using UserNotifications;

namespace BingMapsTest.iOS.Services
{
    public class iOSNotificationReceiver : UNUserNotificationCenterDelegate
    {
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            /*
            int.TryParse(notification.Request.Content.Subtitle, out int action);
            CrossContainer.Instance.Create<ILocalNotification>().ReceiveNotification(notification.Request.Content.Title, notification.Request.Content.Body, (NotificationAction) action);
            completionHandler(UNNotificationPresentationOptions.Alert);
            */
        }
    }
}