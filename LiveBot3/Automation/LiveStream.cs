namespace LiveBot.Automation
{
    internal static class LiveStream
    {
        public static List<LiveStreamer> LiveStreamerList { get; set; } = new List<LiveStreamer>();

        public static readonly int StreamCheckDelay = 5;

        public static async Task Stream_Notification(object Client, PresenceUpdateEventArgs e)
        {
            if (e.User == null || e.User.IsBot) return;
            DiscordGuild guild = e.User.Presence.Guild;
            List<DB.StreamNotifications> streamNotifications = DB.DBLists.StreamNotifications.Where(w => w.Server_ID == guild.Id).ToList();
            if (streamNotifications.Count < 1) return;
            foreach (var StreamNotification in streamNotifications)
            {
                DiscordChannel channel = guild.GetChannel(StreamNotification.Channel_ID);
                if (!Program.ServerIdList.Contains(guild.Id)) return;
                LiveStreamer streamer = new()
                {
                    User = e.User,
                    Time = DateTime.UtcNow,
                    Guild = guild
                };
                int ItemIndex;
                try
                {
                    ItemIndex = LiveStreamerList.FindIndex(a => a.User.Id == e.User.Id
                    && a.Guild.Id == e.User.Presence.Guild.Id);
                }
                catch (Exception)
                {
                    ItemIndex = -1;
                }
                if (ItemIndex >= 0
                    && e.User.Presence.Activities.FirstOrDefault(w => w.Name.ToLower() == "twitch" || w.Name.ToLower() == "youtube") == null)
                {
                    //removes user from list
                    if (LiveStreamerList[ItemIndex].Time.AddHours(StreamCheckDelay) < DateTime.UtcNow
                        && e.User.Presence.Activities.FirstOrDefault(w => w.Name.ToLower() == "twitch" || w.Name.ToLower() == "youtube") == LiveStreamerList[ItemIndex].User.Presence.Activities.FirstOrDefault(w => w.Name.ToLower() == "twitch" || w.Name.ToLower() == "youtube"))
                    {
                        LiveStreamerList.RemoveAt(ItemIndex);
                    }
                }
                else if (ItemIndex == -1
                && e.User.Presence.Activities.FirstOrDefault(w => w.Name.ToLower() == "twitch" || w.Name.ToLower() == "youtube") != null
                && e.User.Presence.Activities.FirstOrDefault(w => w.Name.ToLower() == "twitch" || w.Name.ToLower() == "youtube").ActivityType.Equals(ActivityType.Streaming))
                {
                    Services.StreamNotificationService.QueueStream(StreamNotification, e, guild, channel, streamer);
                }
            }
            await Task.Delay(1);
        }
    }

    internal class LiveStreamer
    {
        public DiscordUser User { get; set; }
        public DateTime Time { get; set; }
        public DiscordGuild Guild { get; set; }
    }
}