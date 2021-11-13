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
                    if (item.Time.AddHours(StreamCheckDelay) < DateTime.Now && item.User.Presence.Activity.ActivityType != ActivityType.Streaming)
                    {
                        Program.Client.Logger.LogInformation(CustomLogEvents.LiveStream, $"User {item.User.Username} removed from Live Stream List - {LiveStream.StreamCheckDelay} hours passed.");
                        list.Remove(item);
                    }
                }
            }
            catch (Exception)
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.LiveStream, "Live Stream list is empty. No-one to remove or check.");
            }
        }

        public static async Task ActivatedRolesCheck(List<ActivateRolesTimer> list)
        {
            try
            {
                foreach (var item in list)
                {
                    if (item.Time.AddMinutes(5) < DateTime.Now)
                    {
                        await item.Role.ModifyAsync(mentionable: false);
                        list.Remove(item);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("[System] ActivateRolesTimer list is empty!");
            }
        }
    }
}