﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveBot
{
    internal class Program
    {
        public static DiscordClient Client { get; set; }
        public CommandsNextExtension Commands { get; set; }
        public static DateTime start = DateTime.Now;
        public static string BotVersion = $"20190911_A";

        // numbers
        public int StreamCheckDelay = 5;

        //lists
        public List<LiveStreamer> LiveStreamerList = new List<LiveStreamer>();

        public List<LevelTimer> UserLevelTimer = new List<LevelTimer>();
        public List<ServerLevelTimer> ServerUserLevelTimer = new List<ServerLevelTimer>();

        //channels
        public DiscordChannel TC1Photomode;

        public DiscordChannel TC2Photomode;

        // guild
        public static DiscordGuild TCGuild;

        // fonts
        public static FontCollection fonts = new FontCollection();

        private static void Main(string[] args)
        {
            var prog = new Program();
            prog.RunBotAsync(args).GetAwaiter().GetResult();
        }

        public async Task RunBotAsync(string[] args)
        {
            // fills all database lists
            DB.DBLists.LoadAllLists();
            fonts.Install("Fonts/Hurme_Geometric_Sans_3_W03_Blk.ttf");

            var json = "";
            using (var fs = File.OpenRead("Config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();
            Json.Bot cfgjson = JsonConvert.DeserializeObject<Json.Config>(json).DevBot;
            if (args.Length == 1)
            {
                if (args[0] == "live")
                {
                    cfgjson = JsonConvert.DeserializeObject<Json.Config>(json).LiveBot;
                    Console.WriteLine($"Running live version: {BotVersion}");
                }
            }
            var cfg = new DiscordConfiguration
            {
                Token = cfgjson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                ReconnectIndefinitely = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(cfg);
            Client.Ready += this.Client_Ready;
            Client.GuildAvailable += this.Client_GuildAvailable;
            Client.ClientErrored += this.Client_ClientError;

            var ccfg = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { cfgjson.CommandPrefix },
                CaseSensitive = false
            };
            this.Commands = Client.UseCommandsNext(ccfg);

            this.Commands.CommandExecuted += this.Commands_CommandExecuted;
            this.Commands.CommandErrored += this.Commands_CommandErrored;

            this.Commands.RegisterCommands<Commands.UngroupedCommands>();
            this.Commands.RegisterCommands<Commands.AdminCommands>();
            this.Commands.RegisterCommands<Commands.OCommands>();

            // Servers
            TCGuild = await Client.GetGuildAsync(150283740172517376); //The Crew server

            // Channels
            TC1Photomode = TCGuild.GetChannel(191567033064751104);
            TC2Photomode = TCGuild.GetChannel(447134224349134848);
            //*/
            void StreamListCheck(List<LiveStreamer> list)
            {
                try
                {
                    foreach (var item in list)
                    {
                        if (item.Time.AddHours(StreamCheckDelay) < DateTime.Now && item.User.Presence.Activity.ActivityType != ActivityType.Streaming)
                        {
                            Console.WriteLine($"{item.User.Username} removed for time out");
                            list.Remove(item);
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("list empty");
                }
            }
            Timer StreamTimer = new Timer(e => StreamListCheck(LiveStreamerList), null, TimeSpan.Zero, TimeSpan.FromMinutes(2));
            //*/ comment this when testing features
            Client.PresenceUpdated += this.Presence_Updated;
            Client.MessageCreated += this.Message_Created;
            Client.MessageReactionAdded += this.Reaction_Role_Added;
            Client.MessageReactionRemoved += this.Reaction_Roles_Removed;
            Client.MessageDeleted += this.Message_Deleted;
            Client.GuildMemberAdded += this.Member_Joined;
            Client.GuildMemberRemoved += this.Memmber_Leave;
            Client.GuildBanAdded += this.Ban_Counter;
            Client.GuildBanRemoved += this.Ban_Removed;
            //*/
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private async Task Presence_Updated(PresenceUpdateEventArgs e)
        {
            List<DB.StreamNotifications> StreamNotifications = DB.DBLists.StreamNotifications;
            foreach (var row in StreamNotifications)
            {
                DiscordGuild guild = await Client.GetGuildAsync(Convert.ToUInt64(row.Server_ID.ToString()));
                DiscordChannel channel = guild.GetChannel(Convert.ToUInt64(row.Channel_ID.ToString()));
                if (e.User.Presence.Guild.Id == guild.Id)
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
                        ItemIndex = LiveStreamerList.FindIndex(a => a.User.Id == e.User.Id && a.Guild.Id == e.User.Presence.Guild.Id);
                    }
                    catch (Exception)
                    {
                        ItemIndex = -1;
                    }
                    if (ItemIndex >= 0 && e.Activity.ActivityType != ActivityType.Streaming)
                    {
                        //removes user from list
                        if (LiveStreamerList[ItemIndex].Time.AddHours(StreamCheckDelay) < DateTime.Now && e.Activity == LiveStreamerList[ItemIndex].User.Presence.Activity)
                        {
                            LiveStreamerList.RemoveAt(ItemIndex);
                        }
                    }
                    else if (ItemIndex == -1 && e.Activity.ActivityType == ActivityType.Streaming)
                    {
                        DiscordMember StreamMember = await guild.GetMemberAsync(e.User.Id);
                        bool role = false, game = false;
                        if (row.Roles_ID != null)
                        {
                            foreach (DiscordRole urole in StreamMember.Roles)
                            {
                                foreach (string roleid in (string[])row.Roles_ID)
                                {
                                    if (urole.Id.ToString() == roleid)
                                    {
                                        role = true;
                                    }
                                }
                            }
                        }
                        else if (row.Roles_ID == null)
                        {
                            role = true;
                        }
                        if (row.Games != null)
                        {
                            foreach (string ugame in row.Games)
                            {
                                try
                                {
                                    if (e.User.Presence.Activity.RichPresence.Details == ugame)
                                    {
                                        game = true;
                                    }
                                }
                                catch { }
                            }
                        }
                        else if (row.Games == null)
                        {
                            game = true;
                        }
                        if (game == true && role == true)
                        {
                            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor(0x6441A5),
                                Author = new DiscordEmbedBuilder.EmbedAuthor
                                {
                                    IconUrl = e.User.AvatarUrl,
                                    Name = "STREAM",
                                    Url = e.User.Presence.Activity.StreamUrl
                                },
                                Description = $"**Streamer:**\n {e.User.Mention}\n\n" +
                        $"**Game:**\n{e.User.Presence.Activity.RichPresence.Details}\n\n" +
                        $"**Stream title:**\n{e.User.Presence.Activity.Name}\n\n" +
                        $"**Stream Link:**\n{e.User.Presence.Activity.StreamUrl}",
                                ThumbnailUrl = e.User.AvatarUrl,
                                Title = $"Check out {e.User.Username} is now Streaming!"
                            };
                            await channel.SendMessageAsync(embed: embed);
                            //adds user to list
                            LiveStreamerList.Add(streamer);
                        }
                    }
                }
            }
        }

        private async Task Message_Created(MessageCreateEventArgs e)
        {
            if ((e.Channel == TC1Photomode || e.Channel == TC2Photomode) && !e.Author.IsBot) // deletes regular messages in photo mode channels
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
            if (!e.Author.IsBot)
            {
                bool checkglobal = false, checklocal = false;
                Random r = new Random();
                int MinInterval = 10, MaxInterval = 30, MinMoney = 2, MaxMoney = 5;
                int points_added = r.Next(MinInterval, MaxInterval);
                int money_added = r.Next(MinMoney, MaxMoney);
                foreach (var Guser in UserLevelTimer)
                {
                    if (Guser.User.Id == e.Author.Id)
                    {
                        checkglobal = true;
                        if (Guser.Time.AddMinutes(2) <= DateTime.Now)
                        {
                            List<DB.Leaderboard> Leaderboard = DB.DBLists.Leaderboard;
                            var global = (from lb in Leaderboard
                                          where lb.ID_User == e.Author.Id.ToString()
                                          select lb).ToList();
                            global[0].Followers = (long)global[0].Followers + points_added;
                            global[0].Bucks = (long)global[0].Bucks + money_added;
                            if ((int)global[0].Level < (long)global[0].Followers / (300 * ((int)global[0].Level + 1) * 0.5))
                            {
                                global[0].Level = (int)global[0].Level + 1;
                            }
                            Guser.Time = DateTime.Now;
                            DB.DBLists.UpdateLeaderboard(global);
                        }
                    }
                }
                foreach (var Suser in ServerUserLevelTimer)
                {
                    if (Suser.User.Id == e.Author.Id)
                    {
                        if (Suser.Guild.Id == e.Guild.Id)
                        {
                            checklocal = true;
                            if (Suser.Time.AddMinutes(2) <= DateTime.Now)
                            {
                                List<DB.ServerRanks> Leaderboard = DB.DBLists.ServerRanks;
                                var local = (from lb in Leaderboard
                                             where lb.User_ID == e.Author.Id.ToString()
                                             where lb.Server_ID == e.Channel.Guild.Id.ToString()
                                             select lb).ToList();
                                local[0].Followers = (long)local[0].Followers + points_added;
                                Suser.Time = DateTime.Now; DB.DBLists.UpdateServerRanks(local);
                            }
                        }
                    }
                }
                if (!checkglobal)
                {
                    List<DB.Leaderboard> Leaderboard = DB.DBLists.Leaderboard;
                    var global = (from lb in Leaderboard
                                  where lb.ID_User == e.Author.Id.ToString()
                                  select lb).ToList();
                    if (global.Count == 0)
                    {
                        DB.Leaderboard newEntry = new DB.Leaderboard
                        {
                            ID_User = e.Author.Id.ToString(),
                            Followers = (long)0,
                            Level = 0,
                            Bucks = (long)0
                        };
                        DB.DBLists.InsertLeaderboard(newEntry);
                        List<DB.UserImages> UserImg = DB.DBLists.UserImages;
                        var idui = UserImg.Max(m => m.ID_User_Images);
                        DB.UserImages newUImage = new DB.UserImages
                        {
                            User_ID = e.Author.Id.ToString(),
                            BG_ID = 1,
                            ID_User_Images = idui + 1
                        };
                        DB.DBLists.InsertUserImages(newUImage);
                        List<DB.UserSettings> UserSet = DB.DBLists.UserSettings;
                        var us = UserSet.Max(m => m.ID_User_Settings);
                        DB.UserSettings newUSettings = new DB.UserSettings
                        {
                            User_ID = e.Author.Id.ToString(),
                            Background_Colour = "white",
                            Text_Colour = "black",
                            Border_Colour = "black",
                            User_Info = "Just a flesh wound",
                            Image_ID = newUImage.ID_User_Images,
                            ID_User_Settings = us + 1
                        };
                        DB.DBLists.InsertUserSettings(newUSettings);
                    }
                    global = (from lb in Leaderboard
                              where lb.ID_User == e.Author.Id.ToString()
                              select lb).ToList();
                    points_added = r.Next(MinInterval, MaxInterval);
                    if (global.Count == 1)
                    {
                        global[0].Followers = (long)global[0].Followers + points_added;
                        global[0].Bucks = (long)global[0].Bucks + money_added;
                        if ((int)global[0].Level < (long)global[0].Followers / (300 * ((int)global[0].Level + 1) * 0.5))
                        {
                            global[0].Level = (int)global[0].Level + 1;
                        }
                    }
                    LevelTimer NewToList = new LevelTimer
                    {
                        Time = DateTime.Now,
                        User = e.Author
                    };
                    UserLevelTimer.Add(NewToList);
                    DB.DBLists.UpdateLeaderboard(global);
                }
                if (!checklocal)
                {
                    List<DB.ServerRanks> Leaderboard = DB.DBLists.ServerRanks;
                    var local = (from lb in Leaderboard
                                 where lb.User_ID == e.Author.Id.ToString()
                                 where lb.Server_ID == e.Channel.Guild.Id.ToString()
                                 select lb).ToList();
                    if (local.Count == 0)
                    {
                        DB.ServerRanks newEntry = new DB.ServerRanks
                        {
                            User_ID = e.Author.Id.ToString(),
                            Server_ID = e.Channel.Guild.Id.ToString(),
                            Followers = 0
                        };
                        DB.DBLists.InsertServerRanks(newEntry);
                    }
                    local = (from lb in Leaderboard
                             where lb.User_ID == e.Author.Id.ToString()
                             where lb.Server_ID == e.Channel.Guild.Id.ToString()
                             select lb).ToList();
                    if (local.Count > 0)
                    {
                        local[0].Followers = (long)local[0].Followers + points_added;
                    }
                    ServerLevelTimer NewToList = new ServerLevelTimer
                    {
                        Time = DateTime.Now,
                        User = e.Author,
                        Guild = e.Guild
                    };
                    ServerUserLevelTimer.Add(NewToList);
                    DB.DBLists.UpdateServerRanks(local);
                }
                //*/
                var userrank = (from sr in DB.DBLists.ServerRanks
                                where sr.Server_ID == e.Guild.Id.ToString()
                                where sr.User_ID == e.Author.Id.ToString()
                                select sr).ToList()[0].Followers;
                var rankedroles = (from rr in DB.DBLists.RankRoles
                                   where rr.Server_Rank != 0
                                   where rr.Server_Rank <= userrank
                                   where rr.Server_ID == e.Guild.Id.ToString()
                                   select rr).ToList();
                List<DiscordRole> roles = new List<DiscordRole>();
                foreach (var item in rankedroles)
                {
                    if (item.Server_ID == e.Guild.Id.ToString())
                    {
                        roles.Add(e.Guild.GetRole(Convert.ToUInt64(item.Role_ID)));
                    }
                }
                DiscordMember member = await e.Guild.GetMemberAsync(e.Author.Id);
                for (int i = 0; i < roles.Count; i++)
                {
                    if (i != roles.Count - 1 && member.Roles.Contains(roles[i]))
                    {
                        await member.RevokeRoleAsync(roles[i]);
                    }
                    else if (i == roles.Count - 1 && !member.Roles.Contains(roles[i]))
                    {
                        await member.GrantRoleAsync(roles[i]);
                    }
                }
            }
        }

        private async Task Reaction_Role_Added(MessageReactionAddEventArgs e)
        {
            if (e.Emoji.Id != 0)
            {
                DiscordEmoji used = e.Emoji;
                DiscordMessage sourcemsg = e.Message;
                DiscordUser username = e.User;
                //ulong f = e.User.Id;

                List<DB.ReactionRoles> ReactionRoles = DB.DBLists.ReactionRoles;
                var RR = (from rr in ReactionRoles
                          where rr.Server_ID == e.Channel.Guild.Id.ToString()
                          where rr.Message_ID == sourcemsg.Id.ToString()
                          where rr.Reaction_ID == used.Id.ToString()
                          select rr).ToList();
                if (RR.Count == 1)
                {
                    DiscordGuild guild = await Client.GetGuildAsync(UInt64.Parse(RR[0].Server_ID.ToString()));
                    DiscordMember rolemember = await guild.GetMemberAsync(username.Id);
                    await rolemember.GrantRoleAsync(guild.GetRole(UInt64.Parse(RR[0].Role_ID.ToString())));
                }
            }
        }

        private async Task Reaction_Roles_Removed(MessageReactionRemoveEventArgs e)
        {
            if (e.Emoji.Id != 0)
            {
                DiscordEmoji used = e.Emoji;
                DiscordMessage sourcemsg = e.Message;
                DiscordUser username = e.User;
                //ulong f = e.User.Id;

                List<DB.ReactionRoles> ReactionRoles = DB.DBLists.ReactionRoles;
                var RR = (from rr in ReactionRoles
                          where rr.Server_ID == e.Channel.Guild.Id.ToString()
                          where rr.Message_ID == sourcemsg.Id.ToString()
                          where rr.Reaction_ID == used.Id.ToString()
                          select rr).ToList();
                if (RR.Count == 1)
                {
                    DiscordGuild guild = await Client.GetGuildAsync(UInt64.Parse(RR[0].Server_ID.ToString()));
                    DiscordMember rolemember = await guild.GetMemberAsync(username.Id);
                    await rolemember.RevokeRoleAsync(guild.GetRole(UInt64.Parse(RR[0].Role_ID.ToString())));
                }
            }
        }

        private async Task Message_Deleted(MessageDeleteEventArgs e)
        {
            DiscordMessage msg = e.Message;
            DiscordUser author = msg.Author;
            var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                 where ss.ID_Server == e.Guild.Id.ToString()
                                 select ss).ToList();
            if (GuildSettings[0].Delete_Log != "0")
            {
                DiscordGuild Guild = await Client.GetGuildAsync(Convert.ToUInt64(GuildSettings[0].ID_Server));
                DiscordChannel DeleteLog = Guild.GetChannel(Convert.ToUInt64(GuildSettings[0].Delete_Log));
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

        private async Task Member_Joined(GuildMemberAddEventArgs e)
        {
            var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                 where ss.ID_Server == e.Guild.Id.ToString()
                                 select ss).ToList();
            var JoinRole = (from rr in DB.DBLists.RankRoles
                            where rr.Server_ID == e.Guild.Id.ToString()
                            where rr.Server_Rank == 0
                            select rr).ToList();
            DiscordGuild Guild = await Client.GetGuildAsync(Convert.ToUInt64(GuildSettings[0].ID_Server));
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
            if (GuildSettings[0].Welcome_Settings[0] != "0")
            {
                DiscordChannel WelcomeChannel = Guild.GetChannel(Convert.ToUInt64(GuildSettings[0].Welcome_Settings[0]));
                if (GuildSettings[0].Welcome_Settings[1] != "0")
                {
                    string msg = GuildSettings[0].Welcome_Settings[1];
                    msg = msg.Replace("$Mention", $"{e.Member.Mention}");
                    await WelcomeChannel.SendMessageAsync(msg);
                    if (JoinRole.Count != 0)
                    {
                        DiscordRole role = Guild.GetRole(Convert.ToUInt64(JoinRole[0].Role_ID));
                        await e.Member.GrantRoleAsync(role);
                    }
                }
            }
        }

        private async Task Memmber_Leave(GuildMemberRemoveEventArgs e)
        {
            DateTimeOffset time = DateTimeOffset.Now.UtcDateTime;
            DateTimeOffset beforetime = time.AddSeconds(-5);
            DateTimeOffset aftertime = time.AddSeconds(10);
            List<DB.UserWarnings> userWarnings = DB.DBLists.UserWarnings;
            string uid = e.Member.Id.ToString();
            bool UserCheck = false;
            var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                 where ss.ID_Server == e.Guild.Id.ToString()
                                 select ss).ToList();
            DiscordGuild Guild = await Client.GetGuildAsync(Convert.ToUInt64(GuildSettings[0].ID_Server));
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
            if (GuildSettings[0].Welcome_Settings[0] != "0")
            {
                DiscordChannel WelcomeChannel = Guild.GetChannel(Convert.ToUInt64(GuildSettings[0].Welcome_Settings[0]));
                if (GuildSettings[0].Welcome_Settings[2] != "0")
                {
                    string msg = GuildSettings[0].Welcome_Settings[2];
                    msg = msg.Replace("$Username", $"{e.Member.Username}");
                    await WelcomeChannel.SendMessageAsync(msg);
                }
            }
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
            if (e.Guild == TCGuild)
            {
                // Checks if user was kicked.
                foreach (var item in logs)
                {
                    if (item.CreationTimestamp >= beforetime && item.CreationTimestamp <= aftertime)
                    {
                        foreach (var rows in userWarnings)
                        {
                            if (rows.ID_User.ToString() == uid)
                            {
                                UserCheck = true;
                                rows.Kick_Count = (int)rows.Kick_Count + 1;
                                DB.DBLists.UpdateUserWarnings(userWarnings);
                            }
                        }
                        if (!UserCheck)
                        {
                            DB.UserWarnings newEntry = new DB.UserWarnings
                            {
                                Warning_Level = 0,
                                Warning_Count = 0,
                                Kick_Count = 1,
                                Ban_Count = 0,
                                ID_User = uid
                            };
                            DB.DBLists.InsertUserWarnings(newEntry);
                        }
                    }
                }
            }
        }

        private async Task Ban_Counter(GuildBanAddEventArgs e)
        {
            var wkb_Settings = (from ss in DB.DBLists.ServerSettings
                                where ss.ID_Server == e.Guild.Id.ToString()
                                select ss).ToList();
            DiscordGuild Guild = await Client.GetGuildAsync(Convert.ToUInt64(wkb_Settings[0].ID_Server));
            if (wkb_Settings[0].WKB_Log != "0")
            {
                await Task.Delay(1000);
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
            List<DB.UserWarnings> userWarnings = DB.DBLists.UserWarnings;
            string uid = e.Member.Id.ToString();
            bool UserCheck = false;
            if (e.Guild == TCGuild)
            {
                foreach (var rows in userWarnings)
                {
                    if (rows.ID_User.ToString() == uid)
                    {
                        UserCheck = true;
                        rows.Ban_Count = (int)rows.Ban_Count + 1;
                        DB.DBLists.UpdateUserWarnings(userWarnings);
                    }
                }
                if (!UserCheck)
                {
                    DB.UserWarnings newEntry = new DB.UserWarnings
                    {
                        Warning_Level = 0,
                        Warning_Count = 0,
                        Kick_Count = 0,
                        Ban_Count = 1,
                        ID_User = uid
                    };
                    DB.DBLists.InsertUserWarnings(newEntry);
                }
            }
            await Task.Delay(0);
        }

        private async Task Ban_Removed(GuildBanRemoveEventArgs e)
        {
            var wkb_Settings = (from ss in DB.DBLists.ServerSettings
                                where ss.ID_Server == e.Guild.Id.ToString()
                                select ss).ToList();
            DiscordGuild Guild = await Client.GetGuildAsync(Convert.ToUInt64(wkb_Settings[0].ID_Server));
            if (wkb_Settings[0].WKB_Log != "0")
            {
                await Task.Delay(1000);
                var logs = await Guild.GetAuditLogsAsync(1, action_type: AuditLogActionType.Ban);
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

        private Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "LiveBot", "Client is ready to process events.", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            var list = (from ss in DB.DBLists.ServerSettings
                        where ss.ID_Server == e.Guild.Id.ToString()
                        select ss).ToList();
            if (list.Count == 0)
            {
                string[] arr = new string[] { "0", "0", "0" };
                var newEntry = new DB.ServerSettings()
                {
                    ID_Server = e.Guild.Id.ToString(),
                    Delete_Log = "0",
                    User_Traffic = "0",
                    Welcome_Settings = arr,
                    WKB_Log = "0"
                };
                DB.DBLists.InsertServerSettings(newEntry);
            }
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "LiveBot", $"Guild available: {e.Guild.Name}", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "LiveBot", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "LiveBot", $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);
            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "LiveBot", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);
            if (e.Exception.InnerException != null)
            {
                e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "LiveBot", $"{e.Exception.InnerException.Message}", DateTime.Now);
            }
#pragma warning disable IDE0059 // Value assigned to symbol is never used
            if (e.Exception is ChecksFailedException ex)
#pragma warning restore IDE0059 // Value assigned to symbol is never used
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = new DiscordColor(0xFF0000) // red
                };
                await e.Context.RespondAsync("", embed: embed);
            }
        }
    }

    internal class LiveStreamer
    {
        public DiscordUser User { get; set; }
        public DateTime Time { get; set; }
        public DiscordGuild Guild { get; set; }
    }

    internal class LevelTimer
    {
        public DiscordUser User { get; set; }
        public DateTime Time { get; set; }
    }

    internal class ServerLevelTimer
    {
        public DiscordUser User { get; set; }
        public DiscordGuild Guild { get; set; }
        public DateTime Time { get; set; }
    }

    public class Tier_Entries
    {
        public ulong Points { get; set; }
        public ulong Rank { get; set; }
    }
}