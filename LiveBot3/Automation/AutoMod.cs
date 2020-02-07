using DSharpPlus;
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

        public static async Task Auto_Moderator_Banned_Words(MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot)
            {
                bool permissionCheck = false;
                DiscordMember member = await e.Guild.GetMemberAsync(e.Author.Id);
                foreach (DiscordRole role in member.Roles)
                {
                    if (role.CheckPermission(Permissions.ManageMessages) == PermissionLevel.Allowed
                        || role.CheckPermission(Permissions.KickMembers) == PermissionLevel.Allowed
                        || role.CheckPermission(Permissions.BanMembers) == PermissionLevel.Allowed
                        || role.CheckPermission(Permissions.Administrator) == PermissionLevel.Allowed)
                    {
                        permissionCheck = true;
                    }
                }
                if (!permissionCheck)
                {
                    var wordlist = (from bw in DB.DBLists.AMBannedWords
                                    where bw.Server_ID == e.Guild.Id.ToString()
                                    select bw).ToList();
                    foreach (var word in wordlist)
                    {
                        if (Regex.IsMatch(e.Message.Content.ToLower(), @$"\b{word.Word}\b"))
                        {
                            await e.Message.DeleteAsync();
                            await CustomMethod.WarnUserAsync(member, Program.Client.CurrentUser, e.Guild, e.Channel, $"{word.Offense} - Trigger word: `{word.Word}`", true);
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
            DiscordMessage msg = e.Message;
            DiscordUser author = msg.Author;
            var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                 where ss.ID_Server == e.Guild.Id.ToString()
                                 select ss).ToList();
            if (GuildSettings[0].Delete_Log != "0")
            {
                DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(GuildSettings[0].ID_Server));
                DiscordChannel DeleteLog = Guild.GetChannel(Convert.ToUInt64(GuildSettings[0].Delete_Log));
                if (author != null)
                {
                    if (!author.IsBot)
                    {
                        string converteddeletedmsg = msg.Content;
                        if (converteddeletedmsg.StartsWith("/"))
                        {
                            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor(0xFF6600),
                                Author = new DiscordEmbedBuilder.EmbedAuthor
                                {
                                    IconUrl = author.AvatarUrl,
                                    Name = author.Username
                                },
                                Description = $"Command initialization was deleted in {e.Channel.Mention}\n" +
                                $"**Author:** {author.Username}\t ID:{author.Id}\n" +
                                $"**Content:** {converteddeletedmsg}\n" +
                                $"**Time Posted:** {msg.CreationTimestamp}"
                            };
                            await DeleteLog.SendMessageAsync(embed: embed);
                        }
                        else
                        {
                            if (converteddeletedmsg == "")
                            {
                                converteddeletedmsg = "*message didn't contain any text, probably file*";
                            }

                            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor(0xFF6600),
                                Author = new DiscordEmbedBuilder.EmbedAuthor
                                {
                                    IconUrl = author.AvatarUrl,
                                    Name = author.Username
                                },
                                Description = $"{author.Mention}'s message was deleted in {e.Channel.Mention}\n" +
                                $"**Contents:** {converteddeletedmsg}\n" +
                                $"Time posted: {msg.CreationTimestamp}"
                            };
                            await DeleteLog.SendMessageAsync(embed: embed);
                        }
                    }
                }
            }
        }

        public static async Task Bulk_Delete_Log(MessageBulkDeleteEventArgs e)
        {

            var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                 where ss.ID_Server == e.Guild.Id.ToString()
                                 select ss).ToList();
            if (GuildSettings[0].Delete_Log != "0")
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
                                $"\n- {message.Content}");
                        }
                    }
                    else
                    {
                        sb.AppendLine($"Author Unknown {message.Timestamp}" +
                                $"\n- Bot was offline when this message was created.");
                    }
                }
                if (sb.ToString().Length<2000)
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
                                 where ss.ID_Server == e.Guild.Id.ToString()
                                 select ss).ToList();
            DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(GuildSettings[0].ID_Server));
            if (GuildSettings[0].User_Traffic != "0")
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
                                 where ss.ID_Server == e.Guild.Id.ToString()
                                 select ss).ToList();
            DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(GuildSettings[0].ID_Server));
            if (GuildSettings[0].User_Traffic != "0")
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
            string uid = e.Member.Id.ToString();
            bool UserCheck = false;
            var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                 where ss.ID_Server == e.Guild.Id.ToString()
                                 select ss).ToList();
            DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(GuildSettings[0].ID_Server));
            var logs = await Guild.GetAuditLogsAsync(1, action_type: AuditLogActionType.Kick);
            if (GuildSettings[0].WKB_Log != "0")
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
                }
            }
            // Checks if user was kicked.
            foreach (var item in logs)
            {
                if (item.CreationTimestamp >= beforetime && item.CreationTimestamp <= aftertime)
                {
                    var UserSettings = DB.DBLists.ServerRanks.FirstOrDefault(f => e.Member.Id.ToString().Equals(f.User_ID));
                    Console.WriteLine(UserSettings.Kick_Count);
                    UserCheck = true;
                    UserSettings.Kick_Count++;
                    Console.WriteLine(UserSettings.Kick_Count);
                    DB.DBLists.UpdateServerRanks(new List<DB.ServerRanks> { UserSettings });
                    if (!UserCheck)
                    {
                        DB.ServerRanks newEntry = new DB.ServerRanks
                        {
                            Server_ID = e.Guild.Id.ToString(),
                            Ban_Count = 0,
                            Kick_Count = 1,
                            Warning_Level = 0,
                            User_ID = uid
                        };
                        DB.DBLists.InsertServerRanks(newEntry);
                    }
                }
            }
        }

        public static async Task User_Banned_Log(GuildBanAddEventArgs e)
        {
            var wkb_Settings = (from ss in DB.DBLists.ServerSettings
                                where ss.ID_Server == e.Guild.Id.ToString()
                                select ss).ToList();
            DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(wkb_Settings[0].ID_Server));
            if (wkb_Settings[0].WKB_Log != "0")
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
            var UserSettings = DB.DBLists.ServerRanks.FirstOrDefault(f => e.Member.Id.ToString().Equals(f.User_ID));
            bool UserCheck = false;
            UserCheck = true;
            UserSettings.Ban_Count += 1;
            UserSettings.Followers = 0;
            DB.DBLists.UpdateServerRanks(new List<DB.ServerRanks> { UserSettings });
            if (!UserCheck)
            {
                DB.ServerRanks newEntry = new DB.ServerRanks
                {
                    Server_ID = e.Guild.Id.ToString(),
                    Ban_Count = 1,
                    Kick_Count = 0,
                    Warning_Level = 0,
                    User_ID = e.Member.Id.ToString()
                };
                DB.DBLists.InsertServerRanks(newEntry);
            }
            await Task.Delay(0);
        }

        public static async Task User_Unbanned(GuildBanRemoveEventArgs e)
        {
            var wkb_Settings = (from ss in DB.DBLists.ServerSettings
                                where ss.ID_Server == e.Guild.Id.ToString()
                                select ss).ToList();
            DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(wkb_Settings[0].ID_Server));
            if (wkb_Settings[0].WKB_Log != "0")
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
    }
}