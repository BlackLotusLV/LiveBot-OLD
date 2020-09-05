using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiveBot.Automation
{
    internal class AutoMod
    {
        public static DiscordChannel TC1Photomode;
        public static DiscordChannel TC2Photomode;
#pragma warning disable IDE0044 // Add readonly modifier
        private static List<DiscordMessage> MessageList = new List<DiscordMessage>();
#pragma warning restore IDE0044 // Add readonly modifier

        public static async Task Auto_Moderator_Banned_Words(MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot && e.Guild != null)
            {
                DiscordMember member = await e.Guild.GetMemberAsync(e.Author.Id);
                if (!CustomMethod.CheckIfMemberAdmin(member))
                {
                    var wordlist = (from bw in DB.DBLists.AMBannedWords
                                    where bw.Server_ID == e.Guild.Id
                                    select bw).ToList();
                    foreach (var word in wordlist)
                    {
                        if (Regex.IsMatch(e.Message.Content.ToLower(), @$"\b{word.Word}\b"))
                        {
                            await e.Message.DeleteAsync();
                            if (DB.DBLists.ServerRanks.Where(w => w.Server_ID == e.Guild.Id && w.User_ID == e.Author.Id).FirstOrDefault().Warning_Level < 5)
                            {
                                await CustomMethod.WarnUserAsync(e.Author, Program.Client.CurrentUser, e.Guild, e.Channel, $"{word.Offense} - Trigger word: `{word.Word}`", true);
                            }
                        }
                    }
                }
            }
        }

        public static async Task Photomode_Cleanup(MessageCreateEventArgs e)
        {
            if ((e.Channel == TC1Photomode || e.Channel == TC2Photomode) && !e.Author.IsBot)
            {
                if (e.Message.Attachments.Count == 0)
                {
#pragma warning disable IDE0059 // Value assigned to symbol is never used
                    if (Uri.TryCreate(e.Message.Content, UriKind.Absolute, out Uri uri))
#pragma warning restore IDE0059 // Value assigned to symbol is never used
                    {
                    }
                    else
                    {
                        await e.Message.DeleteAsync();
                        DiscordMessage m = await e.Channel.SendMessageAsync("This channel is for sharing images only, please use the content comment channel for discussions. If this is a mistake please contact a moderator.");
                        await Task.Delay(9000);
                        await m.DeleteAsync();
                    }
                }
            }
        }

        public static async Task Delete_Log(MessageDeleteEventArgs e)
        {
            if (e.Guild != null)
            {
                DiscordMessage msg = e.Message;
                DiscordUser author = msg.Author;
                var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                     where ss.ID_Server == e.Guild.Id
                                     select ss).FirstOrDefault();
                string Description = string.Empty;

                if (GuildSettings.Delete_Log != 0)
                {
                    DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(GuildSettings.ID_Server));
                    DiscordChannel DeleteLog = Guild.GetChannel(Convert.ToUInt64(GuildSettings.Delete_Log));
                    if (author != null)
                    {
                        if (!author.IsBot)
                        {
                            string converteddeletedmsg = msg.Content;
                            if (converteddeletedmsg == "")
                            {
                                converteddeletedmsg = "*message didn't contain any text, probably file*";
                            }

                            Description = $"{author.Mention}'s message was deleted in {e.Channel.Mention}\n" +
                                $"**Contents:** {converteddeletedmsg}";
                            if (Description.Length <= 2000)
                            {
                                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                                {
                                    Color = new DiscordColor(0xFF6600),
                                    Author = new DiscordEmbedBuilder.EmbedAuthor
                                    {
                                        IconUrl = author.AvatarUrl,
                                        Name = $"{author.Username}'s message deleted"
                                    },
                                    Description = Description,
                                    Footer = new DiscordEmbedBuilder.EmbedFooter
                                    {
                                        Text = $"Time posted: {msg.CreationTimestamp}"
                                    }
                                };
                                await DeleteLog.SendMessageAsync(embed: embed);
                            }
                            else
                            {
                                File.WriteAllText($"{Program.tmpLoc}{e.Message.Id}-DeleteLog.txt", Description);
                                using var upFile = new FileStream($"{Program.tmpLoc}{e.Message.Id}-BulkDeleteLog.txt", FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
                                await DeleteLog.SendFileAsync(upFile, $"Deleted message and info too long, uploading fail instead.");
                            }
                        }
                    }
                }
                var DeletedMSG = MessageList.Where(w => w.Timestamp.Equals(e.Message.Timestamp) && w.Content.Equals(e.Message.Content)).FirstOrDefault();
                if (DeletedMSG != null)
                {
                    MessageList.Remove(DeletedMSG);
                }
            }
        }

        public static async Task Bulk_Delete_Log(MessageBulkDeleteEventArgs e)
        {
            var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                 where ss.ID_Server == e.Guild.Id
                                 select ss).ToList();
            if (GuildSettings[0].Delete_Log != 0)
            {
                DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(GuildSettings[0].ID_Server));
                DiscordChannel DeleteLog = Guild.GetChannel(Convert.ToUInt64(GuildSettings[0].Delete_Log));
                StringBuilder sb = new StringBuilder();
                foreach (var message in e.Messages)
                {
                    if (message.Author != null)
                    {
                        if (!message.Author.IsBot)
                        {
                            sb.AppendLine($"{message.Author.Username}{message.Author.Mention} {message.Timestamp} " +
                                $"\n{message.Channel.Mention} - {message.Content}");
                        }
                    }
                    else
                    {
                        sb.AppendLine($"Author Unknown {message.Timestamp}" +
                                $"\n- Bot was offline when this message was created.");
                    }
                }
                if (sb.ToString().Length < 2000)
                {
                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(0xFF6600),
                        Title = "Bulk delete log",
                        Description = sb.ToString()
                    };
                    await DeleteLog.SendMessageAsync(embed: embed);
                }
                else
                {
                    File.WriteAllText($"{Program.tmpLoc}{e.Messages.Count}-BulkDeleteLog.txt", sb.ToString());
                    using var upFile = new FileStream($"{Program.tmpLoc}{e.Messages.Count}-BulkDeleteLog.txt", FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
                    await DeleteLog.SendFileAsync(upFile, $"Bulk delete log(Over the message cap)");
                }
            }
        }

        public static async Task User_Join_Log(GuildMemberAddEventArgs e)
        {
            var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                 where ss.ID_Server == e.Guild.Id
                                 select ss).ToList();
            DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(GuildSettings[0].ID_Server));
            if (GuildSettings[0].User_Traffic != 0)
            {
                DiscordChannel UserTraffic = Guild.GetChannel(Convert.ToUInt64(GuildSettings[0].User_Traffic));
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Title = $"📥{e.Member.Username}({e.Member.Id}) has joined the server",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = e.Member.AvatarUrl,
                        Text = $"User joined ({e.Guild.MemberCount})"
                    },
                    Color = new DiscordColor(0x00ff00),
                };
                await UserTraffic.SendMessageAsync(embed: embed);
            }
        }

        public static async Task User_Leave_Log(GuildMemberRemoveEventArgs e)
        {
            var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                 where ss.ID_Server == e.Guild.Id
                                 select ss).ToList();
            DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(GuildSettings[0].ID_Server));
            if (GuildSettings[0].User_Traffic != 0)
            {
                DiscordChannel UserTraffic = Guild.GetChannel(Convert.ToUInt64(GuildSettings[0].User_Traffic));
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Title = $"📤{e.Member.Username}({e.Member.Id}) has left the server",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = e.Member.AvatarUrl,
                        Text = $"User left ({e.Guild.MemberCount})"
                    },
                    Color = new DiscordColor(0xff0000),
                };
                await UserTraffic.SendMessageAsync(embed: embed);
            }
        }

        public static async Task User_Kicked_Log(GuildMemberRemoveEventArgs e)
        {
            DateTimeOffset time = DateTimeOffset.Now.UtcDateTime;
            DateTimeOffset beforetime = time.AddSeconds(-5);
            DateTimeOffset aftertime = time.AddSeconds(10);
            decimal uid = e.Member.Id;
            var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                 where ss.ID_Server == e.Guild.Id
                                 select ss).ToList();
            DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(GuildSettings[0].ID_Server));
            var logs = await Guild.GetAuditLogsAsync(1, action_type: AuditLogActionType.Kick);
            if (GuildSettings[0].WKB_Log != 0)
            {
                DiscordChannel wkbLog = Guild.GetChannel(Convert.ToUInt64(GuildSettings[0].WKB_Log));
                if (logs[0].CreationTimestamp >= beforetime && logs[0].CreationTimestamp <= aftertime)
                {
                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                    {
                        Title = $"👢 {e.Member.Username} ({e.Member.Id}) has been kicked",
                        Description = $"*by {logs[0].UserResponsible.Mention}*\n**Reason:** {logs[0].Reason}",
                        Footer = new DiscordEmbedBuilder.EmbedFooter
                        {
                            IconUrl = e.Member.AvatarUrl,
                            Text = $"User kicked"
                        },
                        Color = new DiscordColor(0xff0000),
                    };
                    await wkbLog.SendMessageAsync(embed: embed);

                    var UserSettings = DB.DBLists.ServerRanks.FirstOrDefault(f => e.Member.Id == f.User_ID);
                    if (UserSettings is null)
                    {
                        DiscordUser user = await Program.Client.GetUserAsync(e.Member.Id);
                        CustomMethod.AddUserToServerRanks(user, e.Guild);
                        UserSettings = DB.DBLists.ServerRanks.FirstOrDefault(f => e.Member.Id == f.User_ID && e.Guild.Id == f.Server_ID);
                    }
                    UserSettings.Kick_Count++;
                    UserSettings.Followers /= 2;
                    DB.DBLists.UpdateServerRanks(UserSettings);
                }
            }
        }

        public static async Task User_Banned_Log(GuildBanAddEventArgs e)
        {
            var wkb_Settings = (from ss in DB.DBLists.ServerSettings
                                where ss.ID_Server == e.Guild.Id
                                select ss).ToList();
            DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(wkb_Settings[0].ID_Server));
            if (wkb_Settings[0].WKB_Log != 0)
            {
                await Task.Delay(2000);
                var logs = await Guild.GetAuditLogsAsync(1, action_type: AuditLogActionType.Ban);
                DiscordChannel wkbLog = Guild.GetChannel(Convert.ToUInt64(wkb_Settings[0].WKB_Log));
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Title = $"❌ {e.Member.Username} ({e.Member.Id}) has been banned",
                    Description = $"*by {logs[0].UserResponsible.Mention}*\n**Reason:** {logs[0].Reason}",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = e.Member.AvatarUrl,
                        Text = $"User banned"
                    },
                    Color = new DiscordColor(0xff0000),
                };
                await wkbLog.SendMessageAsync(embed: embed);
            }
            var UserSettings = DB.DBLists.ServerRanks.FirstOrDefault(f => e.Member.Id == f.User_ID && e.Guild.Id == f.Server_ID);
            if (UserSettings == null)
            {
                DiscordUser user = await Program.Client.GetUserAsync(e.Member.Id);
                CustomMethod.AddUserToServerRanks(user, e.Guild);
                UserSettings = DB.DBLists.ServerRanks.FirstOrDefault(f => e.Member.Id == f.User_ID && e.Guild.Id == f.Server_ID);
            }
            UserSettings.Ban_Count += 1;
            UserSettings.Followers = 0;
            DB.DBLists.UpdateServerRanks(UserSettings);
            await Task.Delay(0);
        }

        public static async Task User_Unbanned(GuildBanRemoveEventArgs e)
        {
            var wkb_Settings = (from ss in DB.DBLists.ServerSettings
                                where ss.ID_Server == e.Guild.Id
                                select ss).ToList();
            DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(wkb_Settings[0].ID_Server));
            if (wkb_Settings[0].WKB_Log != 0)
            {
                await Task.Delay(1000);
                var logs = await Guild.GetAuditLogsAsync(1, action_type: AuditLogActionType.Unban);
                DiscordChannel wkbLog = Guild.GetChannel(Convert.ToUInt64(wkb_Settings[0].WKB_Log));
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Title = $"✓ {e.Member.Username} ({e.Member.Id}) has been unbanned",
                    Description = $"*by {logs[0].UserResponsible.Mention}*",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = e.Member.AvatarUrl,
                        Text = $"User unbanned"
                    },
                    Color = new DiscordColor(0x606060),
                };
                await wkbLog.SendMessageAsync(embed: embed);
            }
        }

        public static async Task Spam_Protection(MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot && e.Guild != null)
            {
                var Server_Settings = (from ss in DB.DBLists.ServerSettings
                                       where ss.ID_Server == e.Guild.Id
                                       select ss).FirstOrDefault();
                DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(Server_Settings.ID_Server));

                if (Server_Settings.WKB_Log != 0 && !Server_Settings.Spam_Exception_Channels.Any(id => id == e.Channel.Id))
                {
                    DiscordMember member = await e.Guild.GetMemberAsync(e.Author.Id);
                    if (!CustomMethod.CheckIfMemberAdmin(member))
                    {
                        MessageList.Add(e.Message);
                        var duplicatemessages = MessageList.Where(w => w.Author == e.Author && w.Content == e.Message.Content && e.Guild == w.Channel.Guild).ToList();
                        int i = duplicatemessages.Count();
                        if (duplicatemessages.Count() >= 5)
                        {
                            TimeSpan time = (duplicatemessages[i - 1].CreationTimestamp - duplicatemessages[i - 5].CreationTimestamp) / 5;
                            if (time < TimeSpan.FromSeconds(6))
                            {
                                List<DiscordChannel> ChannelList = duplicatemessages.GetRange(i - 5, 5).Select(s => s.Channel).Distinct().ToList();
                                foreach (DiscordChannel channel in ChannelList)
                                {
                                    await channel.DeleteMessagesAsync(duplicatemessages.GetRange(i - 5, 5));
                                }
                                if (DB.DBLists.ServerRanks.Where(w=>w.Server_ID==e.Guild.Id && w.User_ID==e.Author.Id).FirstOrDefault().Warning_Level < 5)
                                {
                                    await CustomMethod.WarnUserAsync(e.Author, Program.Client.CurrentUser, e.Guild, e.Channel, $"Spam protection triggered.", true);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void ClearMSGCache()
        {
            if (MessageList.Count > 100)
            {
                MessageList.RemoveRange(0, MessageList.Count - 100);
            }
        }
    }
}