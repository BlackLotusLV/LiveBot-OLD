using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveBot.Automation
{
    internal static class LiveStream
    {
        public static List<LiveStreamer> LiveStreamerList { get; set; } = new List<LiveStreamer>();

        public readonly static int StreamCheckDelay = 5;

        public static async Task Stream_Notification(object Client, PresenceUpdateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.User != null)
                {
                    DiscordGuild guild = e.User.Presence.Guild;
                    DB.StreamNotifications StreamNotification = DB.DBLists.StreamNotifications.FirstOrDefault(w => w.Server_ID == guild.Id);
                    DiscordChannel channel = guild.GetChannel(Convert.ToUInt64(StreamNotification.Channel_ID));
                    if (Program.ServerIdList.Contains(guild.Id))
                    {
                        LiveStreamer streamer = new LiveStreamer
                        {
                            User = e.User,
                            Time = DateTime.Now,
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
                            if (LiveStreamerList[ItemIndex].Time.AddHours(StreamCheckDelay) < DateTime.Now
                                && e.User.Presence.Activities.FirstOrDefault(w => w.Name.ToLower() == "twitch" || w.Name.ToLower() == "youtube") == LiveStreamerList[ItemIndex].User.Presence.Activities.FirstOrDefault(w => w.Name.ToLower() == "twitch" || w.Name.ToLower() == "youtube"))
                            {
                                LiveStreamerList.RemoveAt(ItemIndex);
                            }
                        }
                        else if (ItemIndex == -1
                        && e.User.Presence.Activities.FirstOrDefault(w => w.Name.ToLower() == "twitch" || w.Name.ToLower() == "youtube") != null
                        && e.User.Presence.Activities.FirstOrDefault(w => w.Name.ToLower() == "twitch" || w.Name.ToLower() == "youtube").ActivityType.Equals(ActivityType.Streaming))
                        {
                            DiscordMember StreamMember = await guild.GetMemberAsync(e.User.Id);
                            bool role = false, game = false;
                            string gameTitle = e.User.Presence.Activities.FirstOrDefault(w => w.Name.ToLower() == "twitch" || w.Name.ToLower() == "youtube").RichPresence.State;
                            string streamTitle = e.User.Presence.Activities.FirstOrDefault(w => w.Name.ToLower() == "twitch" || w.Name.ToLower() == "youtube").RichPresence.Details;
                            string streamURL = e.User.Presence.Activities.FirstOrDefault(w => w.Name.ToLower() == "twitch" || w.Name.ToLower() == "youtube").StreamUrl;
                            if (StreamNotification.Roles_ID != null)
                            {
                                foreach (DiscordRole urole in StreamMember.Roles)
                                {
                                    foreach (decimal roleid in StreamNotification.Roles_ID)
                                    {
                                        if (urole.Id == roleid)
                                        {
                                            role = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (StreamNotification.Roles_ID == null)
                            {
                                role = true;
                            }
                            if (StreamNotification.Games != null)
                            {
                                foreach (string ugame in StreamNotification.Games)
                                {
                                    try
                                    {
                                        if (gameTitle == ugame)
                                        {
                                            game = true;
                                            break;
                                        }
                                    }
                                    catch { game = false; }
                                }
                            }
                            else if (StreamNotification.Games == null)
                            {
                                game = true;
                            }
                            if (game && role)
                            {
                                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                                {
                                    Color = new DiscordColor(0x6441A5),
                                    Author = new DiscordEmbedBuilder.EmbedAuthor
                                    {
                                        IconUrl = e.User.AvatarUrl,
                                        Name = "STREAM",
                                        Url = streamURL
                                    },
                                    Description = $"**Streamer:**\n {e.User.Mention}\n\n" +
                            $"**Game:**\n{gameTitle}\n\n" +
                            $"**Stream title:**\n{streamTitle}\n\n" +
                            $"**Stream Link:**\n{streamURL}",
                                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                                    {
                                        Url = e.User.AvatarUrl
                                    },
                                    Title = $"Check out {e.User.Username} is now Streaming!"
                                };
                                await channel.SendMessageAsync(embed: embed);
                                //adds user to list
                                LiveStreamerList.Add(streamer);
                            }
                        }
                    }
                }
            });
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