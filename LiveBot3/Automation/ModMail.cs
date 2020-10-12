using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LiveBot.Automation
{
    static class ModMail
    {
        public readonly static int TimeoutMinutes = 120;

        public static async Task ModMailDM(DiscordClient Client, MessageCreateEventArgs e)
        {
            var MMEntry = DB.DBLists.ModMail.FirstOrDefault(w => w.User_ID == e.Author.Id && w.IsActive);
            if (e.Guild == null && MMEntry != null && !(e.Message.Content.StartsWith("/mm") || e.Message.Content.StartsWith("/modmail")))
            {
                DiscordGuild Guild = await Program.Client.GetGuildAsync((ulong)MMEntry.Server_ID);
                DiscordChannel ModMailChannel = Guild.GetChannel((ulong)DB.DBLists.ServerSettings.FirstOrDefault(w => w.ID_Server == MMEntry.Server_ID).ModMailID);
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
                DB.DBLists.UpdateModMail(MMEntry);

                Client.Logger.LogInformation(CustomLogEvents.ModMail, $"New Mod Mail message sent to {ModMailChannel.Name}({ModMailChannel.Id}) in {ModMailChannel.Guild.Name} from {e.Author.Username}({e.Author.Id})");
            }
        }

        public async static Task ModMailCloser()
        {
            var TimedOutEntry = DB.DBLists.ModMail.FirstOrDefault(w => w.IsActive && (DateTime.Now - w.LastMSGTime) > TimeSpan.FromMinutes(TimeoutMinutes));
            if (TimedOutEntry != null)
            {
                DiscordUser User = await Program.Client.GetUserAsync((ulong)TimedOutEntry.User_ID);
                await CloseModMail(
                    TimedOutEntry,
                    User,
                    "Mod Mail entry auto closed.",
                    "**Mod Mail auto closed!\n----------------------------------------------------**");
            }
        }

        public async static Task CloseModMail(DB.ModMail ModMail, DiscordUser Closer, string ClosingText, string ClosingTextToUser)
        {
            ModMail.IsActive = false;
            string DMNotif = string.Empty;
            DiscordGuild Guild = await Program.Client.GetGuildAsync((ulong)ModMail.Server_ID);
            DiscordChannel ModMailChannel = Guild.GetChannel((ulong)DB.DBLists.ServerSettings.FirstOrDefault(w => w.ID_Server == Guild.Id).ModMailID);
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Title = $"[CLOSED] #{ModMail.ID} {ClosingText}",
                Color = new DiscordColor(ModMail.ColorHex),
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = $"{Closer.Username} ({Closer.Id})",
                    IconUrl = Closer.AvatarUrl
                },
            };
            try
            {
                DiscordMember member = await Guild.GetMemberAsync((ulong)ModMail.User_ID);
                await member.SendMessageAsync(ClosingTextToUser);
            }
            catch
            {
                DMNotif = "User could not be contacted anymore, either blocked the bot, left the server or turned off DMs";
            }
            DB.DBLists.UpdateModMail(ModMail);
            await ModMailChannel.SendMessageAsync(DMNotif, embed: embed);
        }
    }
}