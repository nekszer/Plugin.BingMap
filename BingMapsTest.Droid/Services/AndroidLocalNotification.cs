using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using BingMapsTest.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingMapsTest.Droid.Services
{
    public class LocalNotificationImplementation : ILocalNotification
    {
        private string Message = string.Empty;
        private string Title = string.Empty;
        private int Action = 0;
        private TimeSpan? When;
        const string channelId = "default";
        const string channelName = "Default";
        const string channelDescription = "The default channel for notifications.";

        bool channelInitialized = false;
        int messageId = -1;
        NotificationManager NotificationManager;

        public event EventHandler<Dictionary<string, string>> NotificationReceived;

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

        public ILocalNotification SetWhen(TimeSpan delay)
        {
            When = delay;
            return this;
        }

        public LocalNotificationImplementation()
        {
            NotificationManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);
        }

        public async Task<int> Show()
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            messageId++;

            Intent intent = new Intent(Application.Context, typeof(SplashActivity));
            intent.PutExtra(LocalNotificationConstants.TitleKey, Title);
            intent.PutExtra(LocalNotificationConstants.MessageKey, Message);
            intent.PutExtra(LocalNotificationConstants.ActionKey, Action);
            intent.PutExtra(LocalNotificationConstants.IdKey, messageId);

            PendingIntent activity = PendingIntent.GetActivity(Application.Context, messageId, intent, PendingIntentFlags.CancelCurrent);

            var bitmap = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Mipmap.icon);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(Application.Context, channelId)
                .SetContentTitle(Title)
                .SetContentText(Message)
                .SetLargeIcon(bitmap)
                .SetBadgeIconType((int)NotificationBadgeIconType.Small)
                .SetSmallIcon(Resource.Mipmap.icon)
                .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate)
                .SetContentIntent(activity);

            if (When.HasValue)
            {
                var notification = builder.Build();
                Intent notificationIntent = new Intent(Application.Context, typeof(NotificationAlarmReceiver));
                notificationIntent.PutExtra(NotificationAlarmReceiver.NOTIFICATION_ID, messageId);
                notificationIntent.PutExtra(NotificationAlarmReceiver.NOTIFICATION, notification);
                PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, messageId, notificationIntent, PendingIntentFlags.CancelCurrent);
                var delay = (long)When.Value.TotalMilliseconds;
                long futureInMillis = SystemClock.ElapsedRealtime() + delay;
                AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
                alarmManager.Set(AlarmType.ElapsedRealtimeWakeup, futureInMillis, pendingIntent);
            }
            else
            {
                var notification = builder.Build();
                NotificationManager.Notify(messageId, notification);
            }
            await Task.Delay(1);
            return messageId;
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = channelDescription
                };
                NotificationManager.CreateNotificationChannel(channel);
            }
            channelInitialized = true;
        }

        public ILocalNotification SetAction(NotificationAction action)
        {
            Action = (int)action;
            return this;
        }

        public void ReceiveNotification(Dictionary<string, string> data) => NotificationReceived?.Invoke(this, data);

        public void Clear(int id = 0)
        {
            if (id <= 0)
                NotificationManager.CancelAll();
            else
                NotificationManager.Cancel(id);
        }
    }

    [BroadcastReceiver]
    [IntentFilter(new string[] { "android.intent.action.BOOT_COMPLETED" }, Priority = (int)IntentFilterPriority.LowPriority)]
    public class NotificationAlarmReceiver : BroadcastReceiver
    {

        public static string NOTIFICATION_ID = "notification_id";
        public static string NOTIFICATION = "notification";

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                NotificationManager notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                Notification notification = (Notification)intent.GetParcelableExtra(NOTIFICATION);
                int notificationId = intent.GetIntExtra(NOTIFICATION_ID, 0);
                notificationManager.Notify(notificationId, notification);
            }
            catch { }
        }
    }
}