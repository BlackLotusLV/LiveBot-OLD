using LiveBot.Automation;

namespace LiveBot
{
    internal static class TimerMethod
    {
        public static void StreamListCheck(List<LiveStreamer> list, int StreamCheckDelay)
        {
            try
            {
                foreach (var item in list)
                {
                    if (item.Time.AddHours(StreamCheckDelay) < DateTime.UtcNow && item.User.Presence.Activity.ActivityType != ActivityType.Streaming)
                    {
                        Program.Client.Logger.LogInformation(CustomLogEvents.LiveStream, "User {UserName} removed from Live Stream List - {CheckDelay} hours passed.", item.User.Username, LiveStream.StreamCheckDelay);
                        list.Remove(item);
                    }
                }
            }
            catch (Exception)
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.LiveStream, "Live Stream list is empty. No-one to remove or check.");
            }
        }
    }
}