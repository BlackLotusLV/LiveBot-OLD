using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveBot.Automation
{
    internal class ModMail
    {
        public static int TimeoutMinutes = 120;

        public static async Task ModMailDM(MessageCreateEventArgs e)
        {
            var MMEntry = DB.DBLists.ModMail.Where(w => w.User_ID == e.Author.Id && w.IsActive).FirstOrDefault();
            if (e.Guild == null && MMEntry != null && !(e.Message.Content.StartsWith("/mm") || e.Message.Content.StartsWith("/modmail")))
            {
                DiscordGuild Guild = await Program.Client.GetGuildAsync((ulong)MMEntry.Server_ID);
                DiscordChannel ModMailChannel = Guild.GetChannel((ulong)DB.DBLists.ServerSettings.Where(w => w.ID_Server == MMEntry.Server_ID).FirstOrDefault().ModMailID);
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        IconUrl = e.Author.AvatarUrl,
                        Name = $"{e.Author.Username} ({e.Author.Id})"
                    },
                    Color = new DiscordColor(MMEntry.ColorHex),
                    Title = $"[INBOX] #{MMEntry.ID} Mod Mail user message.",
                    Description = e.Message.Content
                };

                if (e.Message.Attachments != null)
                {
                    foreach (var attachment in e.Message.Attachments)
                    {
                        embed.AddField("Atachment", attachment.Url, false);
                    }
                }

                await ModMailChannel.SendMessageAsync(embed: embed);

                MMEntry.HasChatted = true;
                MMEntry.LastMSGTime = DateTime.Now;
                DB.DBLists.UpdateModMail(new List<DB.ModMail> { MMEntry });
            }
        }

        public async static void ModMailCloser()
        {
            var TimedOutEntry = DB.DBLists.ModMail.Where(w => w.IsActive && (DateTime.Now - w.LastMSGTime) > TimeSpan.FromMinutes(TimeoutMinutes)).FirstOrDefault();
            if (TimedOutEntry != null)
            {
                TimedOutEntry.IsActive = false;
                string DMNotif = string.Empty;
                DiscordGuild Guild = await Program.Client.GetGuildAsync((ulong)TimedOutEntry.Server_ID);
                DiscordChannel ModMailChannel = Guild.GetChannel((ulong)DB.DBLists.ServerSettings.Where(w => w.ID_Server == Guild.Id).FirstOrDefault().ModMailID);
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Title = $"[CLOSED] #{TimedOutEntry.ID} Mod Mail auto closed.",
                    Color = new DiscordColor(TimedOutEntry.ColorHex)
                };
                try
                {
                    DiscordMember member = await Guild.GetMemberAsync((ulong)TimedOutEntry.User_ID);
                    await member.SendMessageAsync("Mod Mail timed out and now is automatically closed.");
                }
                catch
                {
                    DMNotif = "User could not be contacted anymore, either blocked the bot, left the server or turned off DMs";
                }
                DB.DBLists.UpdateModMail(new List<DB.ModMail> { TimedOutEntry });
                await ModMailChannel.SendMessageAsync(DMNotif, embed: embed);
            }
        }
    }
}