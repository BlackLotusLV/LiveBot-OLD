using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using LiveBot.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LiveBot
{
    internal static class CustomMethod
    {
        public static string GetConnString()
        {
            string json;
            using (var sr = new StreamReader(File.OpenRead("Config.json"), new UTF8Encoding(false)))
                json = sr.ReadToEnd();
            var cfgjson = JsonConvert.DeserializeObject<ConfigJson.Config>(json).DataBase;
            return $"Host={cfgjson.Host};Username={cfgjson.Username};Password={cfgjson.Password};Database={cfgjson.Database}; Port={cfgjson.Port}";
        }

        public static DateTime EpochConverter(long ms)
        {
            DateTime f = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return f.AddMilliseconds(ms);
        }

        public static void AddUserToLeaderboard(DiscordUser user)
        {
            DB.Leaderboard newEntry = new()
            {
                ID_User = user.Id,
                Followers = (long)0,
                Level = 0,
                Bucks = (long)0
            };
            DB.DBLists.InsertLeaderboard(newEntry);
            List<DB.UserImages> UserImg = DB.DBLists.UserImages;
            var idui = UserImg.Max(m => m.ID_User_Images);
            DB.UserImages newUImage = new()
            {
                User_ID = user.Id,
                BG_ID = 1,
                ID_User_Images = idui + 1
            };
            DB.DBLists.InsertUserImages(newUImage);
            List<DB.UserSettings> UserSet = DB.DBLists.UserSettings;
            var us = UserSet.Max(m => m.ID_User_Settings);
            DB.UserSettings newUSettings = new()
            {
                User_ID = user.Id,
                User_Info = "There is a difference between knowing the path and walking the path.",
                Image_ID = newUImage.ID_User_Images,
                ID_User_Settings = us + 1
            };
            DB.DBLists.InsertUserSettings(newUSettings);
        }

        public static void AddUserToServerRanks(DiscordUser user, DiscordGuild guild)
        {
            if (DB.DBLists.Leaderboard.FirstOrDefault(f => f.ID_User == user.Id) == null)
            {
                AddUserToLeaderboard(user);
            }
            List<DB.ServerRanks> Leaderboard = DB.DBLists.ServerRanks;
            var local = (from lb in Leaderboard.AsParallel()
                         where lb.User_ID == user.Id
                         where lb.Server_ID == guild.Id
                         select lb).FirstOrDefault();
            if (local is null)
            {
                DB.ServerRanks newEntry = new()
                {
                    User_ID = user.Id,
                    Server_ID = guild.Id,
                    Followers = 0
                };
                DB.DBLists.InsertServerRanks(newEntry);
            }
        }

        public static string ScoreToTime(int Time)
        {
            StringBuilder[] sTime = { new StringBuilder(), new StringBuilder() };
            for (int i = 0; i < Time.ToString().Length; i++)
            {
                if (i < Time.ToString().Length - 3)
                {
                    sTime[0].Append(Time.ToString()[i]);
                }
                else
                {
                    sTime[1].Append(Time.ToString()[i]);
                }
            }
            if (sTime[0].Length == 0)
            {
                sTime[0].Append('0');
            }
            while (sTime[1].Length < 3)
            {
                sTime[1].Insert(0, '0');
            }
            TimeSpan seconds = TimeSpan.FromSeconds(double.Parse(sTime[0].ToString()));
            if (seconds.Hours == 0)
            {
                return $"{seconds.Minutes}:{seconds.Seconds}.{sTime[1]}";
            }

            return $"{seconds.Hours}:{seconds.Minutes}:{seconds.Seconds}.{sTime[1]}";
        }

        public static string GetServerTop(CommandContext ctx, int page)
        {
            StringBuilder sb = new();
            string personalscore = "";
            List<DB.ServerRanks> leaderboard = DB.DBLists.ServerRanks;
            var users = leaderboard.OrderByDescending(x => x.Followers).ToList();
            users = users.Where(w => w.Server_ID == ctx.Guild.Id).ToList();
            sb.AppendLine("```csharp\n📋 Rank | Username");
            for (int i = (page * 10) - 10; i < page * 10; i++)
            {
                var duser = ctx.Client.GetUserAsync(Convert.ToUInt64(users[i].User_ID));
                if (duser.Result.Username.StartsWith("Deleted User "))
                {
                    users[i].Followers = 0;
                    DB.DBLists.UpdateServerRanks(users[i]);
                    sb.AppendLine($"[{i + 1}]\tUser account deleted");
                }
                else
                {
                    sb.AppendLine($"[{i + 1}]\t# {duser.Result.Username}\n\t\t\tFollowers:{users[i].Followers}");
                    if (i == users.Count - 1)
                    {
                        i = page * 10;
                    }
                }
            }
            int rank = 0;
            List<DB.ServerRanks> LB = DB.DBLists.ServerRanks;
            List<DB.ServerRanks> leaderbaords = LB.OrderByDescending(x => x.Followers).ToList();
            leaderbaords = leaderbaords.Where(w => w.Server_ID == ctx.Guild.Id).ToList();
            foreach (var item in leaderbaords)
            {
                rank++;
                if (item.User_ID == ctx.User.Id)
                {
                    personalscore = $"⭐Rank: {rank}\t Followers: {item.Followers}\t";
                    break;
                }
            }
            sb.AppendLine($"\n# Your Server Ranking\n{personalscore}\n```");

            return sb.ToString();
        }

        public static string GetGlobalTop(CommandContext ctx, int page)
        {
            StringBuilder sb = new();
            string personalscore = "";
            List<DB.Leaderboard> leaderboard = DB.DBLists.Leaderboard;
            var users = leaderboard.OrderByDescending(x => x.Followers).ToList();
            sb.AppendLine("```csharp\n📋 Rank | Username");
            for (int i = (page * 10) - 10; i < page * 10; i++)
            {
                var duser = ctx.Client.GetUserAsync(Convert.ToUInt64(users[i].ID_User));
                if (duser.Result.Username.StartsWith("Deleted User "))
                {
                    users[i].Followers = 0;
                    users[i].Bucks = 0;
                    DB.DBLists.UpdateLeaderboard(users[i]);
                    sb.AppendLine($"[{i + 1}]\tUser account deleted");
                }
                else
                {
                    sb.AppendLine($"[{i + 1}]\t# {duser.Result.Username}\n\t\t\tFollowers:{users[i].Followers}\tLevel:{users[i].Level}");
                    if (i == users.Count - 1)
                    {
                        i = page * 10;
                    }
                }
            }
            int rank = 0;
            List<DB.Leaderboard> LB = DB.DBLists.Leaderboard;
            List<DB.Leaderboard> leaderbaords = LB.OrderByDescending(x => x.Followers).ToList();
            foreach (var item in leaderbaords)
            {
                rank++;
                if (item.ID_User == ctx.User.Id)
                {
                    personalscore = $"⭐Rank: {rank}\t Followers: {item.Followers}\t Level {item.Level}";
                }
            }
            sb.AppendLine($"\n# Your Global Ranking\n{personalscore}\n```");

            return sb.ToString();
        }

        public static string GetBackgroundList(CommandContext ctx, int page)
        {
            List<DB.UserImages> userImages = DB.DBLists.UserImages;
            List<DB.BackgroundImage> Backgrounds = DB.DBLists.BackgroundImage.OrderBy(o => o.ID_BG).ToList();
            var List = (from bi in Backgrounds
                        join ui in userImages on bi.ID_BG equals ui.BG_ID
                        where ui.User_ID == ctx.User.Id
                        select bi).ToList();

            StringBuilder sb = new();
            sb.Append("Visual representation of the backgrounds can be viewed here: <http://bit.ly/LiveBG>\n```csharp\n[ID]\tBackground Name\n");
            for (int i = (page * 10) - 10; i < page * 10; i++)
            {
                bool check = false;
                foreach (var userimage in List)
                {
                    if (Backgrounds[i].ID_BG == userimage.ID_BG)
                    {
                        sb.Append($"[{Backgrounds[i].ID_BG}]\t# {Backgrounds[i].Name}\n\t\t\t [OWNED]\n");
                        check = true;
                    }
                }
                if (!check)
                {
                    sb.Append($"[{Backgrounds[i].ID_BG}]\t# {Backgrounds[i].Name}\n\t\t\t Price:{Backgrounds[i].Price} Bucks\n");
                }
                if (i == Backgrounds.Count - 1)
                {
                    i = page * 10;
                }
            }
            sb.Append("```");

            return sb.ToString();
        }

        public static string GetMissionList(List<Json.TCHubJson.Mission> MissionList, int page)
        {
            StringBuilder Missions = new();
            Missions.AppendLine("```csharp");
            for (int i = (page * 10) - 10; i < page * 10; i++)
            {
                Missions.AppendLine($"{i}\t{MissionList[i].ID}\t{HubNameLookup(MissionList[i].Text_ID)}");
            }
            Missions.Append("```");
            return Missions.ToString();
        }

        public static async Task WarnUserAsync(DiscordUser user, DiscordUser admin, DiscordGuild server, DiscordChannel channel, string reason, bool automsg)
        {
            DB.ServerRanks WarnedUserStats = DB.DBLists.ServerRanks.FirstOrDefault(f => server.Id == f.Server_ID && user.Id == f.User_ID);
            DB.ServerSettings ServerSettings = DB.DBLists.ServerSettings.FirstOrDefault(f => server.Id == f.ID_Server);

            if (WarnedUserStats == null)
            {
                AddUserToServerRanks(user, server);
            }

            DiscordMember member = null;
            try
            {
                member = await server.GetMemberAsync(user.Id);
            }
            catch (Exception)
            {
                await channel.SendMessageAsync($"{user.Username} is no longer in the server.");
            }

            string modinfo = "";
            StringBuilder SB = new();
            decimal uid = user.Id, aid = admin.Id;
            bool kick = false, ban = false;
            DiscordEmbedBuilder embed;
            if (ServerSettings.WKB_Log != 0)
            {
                DiscordChannel modlog = server.GetChannel(Convert.ToUInt64(ServerSettings.WKB_Log));
                if (WarnedUserStats is null) // creates new entry in DB (Followers set to default value)
                {
                    DB.ServerRanks newEntry = new()
                    {
                        Server_ID = server.Id,
                        Ban_Count = 0,
                        Kick_Count = 0,
                        Warning_Level = 1,
                        User_ID = user.Id
                    };
                    DB.DBLists.InsertServerRanks(newEntry);
                    WarnedUserStats = newEntry;
                }
                else
                {
                    WarnedUserStats.Warning_Level++;
                    if (WarnedUserStats.Followers <= 1000 * WarnedUserStats.Warning_Level)
                    {
                        WarnedUserStats.Followers = 0;
                    }
                    else
                    {
                        WarnedUserStats.Followers -= (1000 * WarnedUserStats.Warning_Level);
                    }
                    DB.DBLists.UpdateServerRanks(WarnedUserStats);
                }

                DB.Warnings newWarning = new()
                {
                    Reason = reason,
                    Active = true,
                    Date = DateTime.Now.ToString("yyyy-MM-dd"),
                    Admin_ID = aid,
                    User_ID = uid,
                    Server_ID = server.Id,
                    Type = "warning"
                };
                DB.DBLists.InsertWarnings(newWarning);

                int warning_count = DB.DBLists.Warnings.Count(w => w.User_ID == user.Id && w.Server_ID == server.Id && w.Type == "warning");

                SB.AppendLine($"You have been warned by <@{admin.Id}>.\n**Warning Reason:**\t{reason}\n**Warning Level:** {WarnedUserStats.Warning_Level}\n**Server:** {server.Name}");
                embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(0xf90707),
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        IconUrl = user.AvatarUrl,
                        Name = $"{user.Username} ({user.Id})"
                    },
                    Description = $"**Warned user:**\t{user.Mention}\n**Warning level:**\t {WarnedUserStats.Warning_Level}\t**Warning count:**\t {warning_count}\n**Warned by**\t{admin.Username}\n**Reason:** {reason}"
                };
                if (WarnedUserStats.Warning_Level > 4)
                {
                    SB.AppendLine($"You have been banned from **{server.Name}** by {admin.Mention} for exceeding the warning level threshold(4).");
                    ban = true;
                }
                else if (WarnedUserStats.Warning_Level > 2 && WarnedUserStats.Warning_Level < 5)
                {
                    SB.AppendLine($"You have been kicked from **{server.Name}** by {admin.Mention} for exceeding the warning level threshold(2).");
                    kick = true;
                }

                if (automsg)
                {
                    SB.Append("*This warning is given out by a bot, contact an admin if you think this is a mistake*");
                }
                else
                {
                    SB.Append($"*(This is an automated message, use the `{Program.CFGJson.CommandPrefix}modmail` feature if you want to report a mistake.)*");
                }

                try
                {
                    await member.SendMessageAsync(SB.ToString());
                }
                catch
                {
                    modinfo = $":exclamation:{user.Mention} could not be contacted via DM. Reason not sent";
                }

                if (kick && member != null)
                {
                    await member.RemoveAsync("Exceeded warning limit!");
                }
                if (ban && user != null)
                {
                    await server.BanMemberAsync(user.Id, 0, "Exceeded warning limit!");
                }

                await modlog.SendMessageAsync(modinfo, embed: embed);

                DiscordMessage info = await channel.SendMessageAsync($"{user.Username}, Has been warned!");
                await Task.Delay(10000).ContinueWith(t => info.DeleteAsync());
            }
            else
            {
                await channel.SendMessageAsync("This server has not set up this feature!");
            }
        }

        public static bool CheckIfMemberAdmin(DiscordMember member)
        {
            foreach (DiscordRole role in member.Roles)
            {
                if (role.CheckPermission(Permissions.ManageMessages) == PermissionLevel.Allowed
                    || role.CheckPermission(Permissions.KickMembers) == PermissionLevel.Allowed
                    || role.CheckPermission(Permissions.BanMembers) == PermissionLevel.Allowed
                    || role.CheckPermission(Permissions.Administrator) == PermissionLevel.Allowed
                    || role.Id == 152156646402031627)
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetCommandOutput(CommandContext ctx, string command, string language, DiscordMember member)
        {
            DB.DBLists.LoadBotOutputList();

            if (language == null)
            {
                language = (ctx.Channel.Id) switch
                {
                    (150283740172517376) => "gb",
                    (249586001167515650) => "de",
                    (253231012492869632) => "fr",
                    (410790788738580480) => "nl",
                    (410835311602565121) => "se",
                    (363977914196295681) => "ru",
                    (423845614686699521) => "lv",
                    (585529567708446731) => "es",
                    (741656080051863662) => "jp",
                    _ => "gb"
                };
            }
            if (member is null)
            {
                member = ctx.Member;
            }

            var OutputEntry = DB.DBLists.BotOutputList.FirstOrDefault(w => w.Command.Equals(command) && w.Language.Equals(language));
            if (OutputEntry is null)
            {
                OutputEntry = DB.DBLists.BotOutputList.FirstOrDefault(w => w.Command.Equals(command) && w.Language.Equals("gb"));
                if (OutputEntry is null)
                {
                    return $"{ctx.Member.Mention}, Command output not found. Contact an admin.";
                }
            }
            return $"{member.Mention}, {OutputEntry.Command_Text}";
        }

        public static DiscordEmbed GetUserWarnings(DiscordGuild Guild, DiscordUser User, bool AdminCommand = false)
        {
            DB.DBLists.LoadServerRanks();
            DB.DBLists.LoadWarnings();
            List<DB.ServerRanks> ServerRanks = DB.DBLists.ServerRanks;
            List<DB.Warnings> warnings = DB.DBLists.Warnings;
            bool UserCheck = false;
            int kcount = 0,
                bcount = 0,
                wlevel = 0,
                wcount = 0,
                splitcount = 1;
            StringBuilder Reason = new();
            var UserStats = ServerRanks.FirstOrDefault(f => User.Id == f.User_ID && Guild.Id == f.Server_ID);
            if (UserStats != null)
            {
                UserCheck = true;
                kcount = UserStats.Kick_Count;
                bcount = UserStats.Ban_Count;
                wlevel = UserStats.Warning_Level;
                var WarningsList = warnings.Where(w => w.User_ID == User.Id && w.Server_ID == Guild.Id).ToList();
                if (!AdminCommand)
                {
                    WarningsList.RemoveAll(w => w.Type == "note");
                }
                wcount = WarningsList.Count(w => w.Type == "warning");
                foreach (var item in WarningsList)
                {
                    switch (item.Type)
                    {
                        case "ban":
                            Reason.Append("[🔨]");
                            break;
                        case "kick":
                            Reason.Append("[🥾]");
                            break;
                        case "note":
                            Reason.Append("[❔]");
                            break;
                        default: // warning
                            if (item.Active)
                            {
                                Reason.Append("[✅] ");
                            }
                            else
                            {

                                Reason.Append("[❌] ");
                            }
                            break;
                    }
                    string addedInfraction = $"**ID:**{item.ID_Warning}\t**By:** <@{item.Admin_ID}>\t**Date:** {item.Date}\n**Reason:** {item.Reason}\n **Type:**\t{item.Type}";
                    
                    if (Reason.Length+addedInfraction.Length>1023*splitcount)
                    {
                        Reason.Append("~split~");
                        splitcount++;
                    }
                    Reason.AppendLine(addedInfraction);
                }
                if (WarningsList.Count == 0)
                {
                    Reason.AppendLine("User has no warnings.");
                }
            }
            DiscordEmbedBuilder embed = new()
            {
                Color = new DiscordColor(0xFF6600),
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = User.Username,
                    IconUrl = User.AvatarUrl
                },
                Description = $"",
                Title = "User kick Count",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = User.AvatarUrl
                }
            };
            embed.AddField("Warning level: ", $"{wlevel}", true);
            embed.AddField("Times warned: ", $"{wcount}", true);
            embed.AddField("Times kicked: ", $"{kcount}", true);
            embed.AddField("Times banned: ", $"{bcount}", true);
            string[] SplitReason = Reason.ToString().Split("~split~");
            for (int i = 0; i < SplitReason.Length; i++)
            {
                embed.AddField($"Infraction({i + 1}/{SplitReason.Length})", SplitReason[i], false);
            }
            if (!UserCheck)
            {
                return new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(0xFF6600),
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = User.Username,
                        IconUrl = User.AvatarUrl
                    },
                    Description = "This user has no warning, kick and/or ban history in this server."
                };
            }
            else
            {
                return embed;
            }
        }

        public static void DBProgress(int LoadedTableCount, TimeSpan time, string DataTableName = null)
        {
            StringBuilder sb = new();
            sb.Append('[');
            for (int i = 1; i <= DB.DBLists.TableCount; i++)
            {
                if (i <= LoadedTableCount)
                {
                    sb.Append('#');
                }
                else
                {
                    sb.Append(' ');
                }
            }
            sb.Append(((float)LoadedTableCount / (float)DB.DBLists.TableCount).ToString($"] - [0.00%] [{time.Seconds}:{time.Milliseconds}]"));
            Program.Client.Logger.LogInformation(CustomLogEvents.POSTGRESQL, DataTableName is null ? "Starting to load Data Base" : $"{DataTableName} List Loaded");
            Program.Client.Logger.LogInformation(CustomLogEvents.POSTGRESQL, sb.ToString());
            if (LoadedTableCount == DB.DBLists.TableCount)
            {
                DB.DBLists.LoadedTableCount = 0;
            }
        }

        public static string HubNameLookup(string ID)
        {
            string HubText = Program.TCHubDictionary.FirstOrDefault(w => w.Key.Equals(ID)).Value ?? "[Item Name Missing]";
            HubText = HubText.Replace("&#8209;", "-");
            return HubText;
        }

        public static Image<Rgba32> BuildSummitImage(List<TCHubJson.Summit> JSummit, TCHubJson.Rank Events, TCHubJson.TceSummitSubs UserInfo)
        {
            int[,] WidthHeight = new int[,] { { 0, 249 }, { 373, 249 }, { 0, 493 }, { 373, 493 }, { 747, 0 }, { 747, 249 }, { 0, 0 }, { 249, 0 }, { 498, 0 } };
            Font Basefont = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 18);
            Font SummitCaps15 = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 15);
            Font SummitCaps12 = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 12.5f);

            var AllignCenter = new TextOptions()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            };
            var AllignTopLeft = new TextOptions()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            var AllignTopRight = new TextOptions()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };

            Image<Rgba32> BaseImage = new(1127, 765);
            Parallel.For(0, 9, (i, state) =>
            {
                var ThisEvent = JSummit[0].Events[i];
                var Activity = Events.Activities.Where(w => w.Activity_ID.Equals(ThisEvent.ID.ToString())).ToArray();

                byte[] EventLogoBit = TimerMethod.EventLogoBitArr[i];
                using Image<Rgba32> EventImage = Image.Load<Rgba32>(EventLogoBit);
                if (i == 5)
                {
                    EventImage.Mutate(ctx => ctx.
                    Resize(380, 483)
                    );
                }
                else if (i >= 0 && i <= 3)
                {
                    EventImage.Mutate(ctx => ctx.
                    Resize(368, 239)
                    );
                }
                if (Activity.Length > 0)
                {
                    using WebClient wc = new();
                    string ThisEventNameID = string.Empty;
                    if (ThisEvent.Is_Mission)
                    {
                        ThisEventNameID = Program.TCHub.Missions.Where(w => w.ID == ThisEvent.ID).Select(s => s.Text_ID).FirstOrDefault();
                    }
                    else
                    {
                        ThisEventNameID = Program.TCHub.Skills.Where(w => w.ID == ThisEvent.ID).Select(s => s.Text_ID).FirstOrDefault();
                    }

                    string[] EventTitle = (CustomMethod.HubNameLookup(ThisEventNameID)).Replace("\"", string.Empty).Split(' ');
                    TCHubJson.SummitLeaderboard leaderboard = JsonConvert.DeserializeObject<TCHubJson.SummitLeaderboard>(wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/leaderboard/{UserInfo.Platform}/{ThisEvent.ID}?page_size=1"));
                    StringBuilder sb = new();
                    for (int j = 0; j < EventTitle.Length; j++)
                    {
                        if (j == 3)
                        {
                            sb.AppendLine();
                        }
                        sb.Append(EventTitle[j] + " ");
                    }
                    string ActivityResult = $"Score: {Activity[0].Score}";
                    if (leaderboard.Score_Format == "time")
                    {
                        ActivityResult = $"Time: {CustomMethod.ScoreToTime(Activity[0].Score)}";
                    }
                    else if (sb.ToString().Contains("SPEEDTRAP"))
                    {
                        ActivityResult = $"Speed: {Activity[0].Score.ToString().Insert(3, ".")} km/h";
                    }
                    else if (sb.ToString().Contains("ESCAPE"))
                    {
                        ActivityResult = $"Distance: {Activity[0].Score}m";
                    }
                    using (Image<Rgba32> TitleBar = new(EventImage.Width, 40))
                    using (Image<Rgba32> ScoreBar = new(EventImage.Width, 40))
                    {
                        ScoreBar.Mutate(ctx => ctx.Fill(Color.Black));
                        TitleBar.Mutate(ctx => ctx.Fill(Color.Black));
                        EventImage.Mutate(ctx => ctx
                        .DrawImage(ScoreBar, new Point(0, EventImage.Height - ScoreBar.Height), 0.7f)
                        .DrawImage(TitleBar, new Point(0, 0), 0.7f)
                        .DrawText(new DrawingOptions { TextOptions = AllignTopLeft }, sb.ToString(), SummitCaps15, Color.White, new PointF(5, 0))
                        .DrawText(new DrawingOptions { TextOptions = AllignTopLeft }, $"Rank: {Activity[0].Rank + 1}", Basefont, Color.White, new PointF(5, EventImage.Height - 22))
                        .DrawText(new DrawingOptions { TextOptions = AllignTopRight }, ActivityResult, Basefont, Color.White, new PointF(EventImage.Width - 5, EventImage.Height - 42))
                        .DrawText(new DrawingOptions { TextOptions = AllignTopRight }, $"Points: {Activity[0].Points}", Basefont, Color.White, new PointF(EventImage.Width - 5, EventImage.Height - 22))
                        );
                    }
                    BaseImage.Mutate(ctx => ctx
                    .DrawImage(EventImage, new Point(WidthHeight[i, 0], WidthHeight[i, 1]), 1)
                    );
                }
                else
                {
                    using Image<Rgba32> NotComplete = new(EventImage.Width, EventImage.Height);
                    NotComplete.Mutate(ctx => ctx
                        .Fill(Color.Black)
                        .DrawText(new DrawingOptions { TextOptions = AllignCenter }, "Event not completed!", Basefont, Color.White, new PointF(NotComplete.Width / 2, NotComplete.Height / 2))
                        );
                    BaseImage.Mutate(ctx => ctx
                    .DrawImage(EventImage, new Point(WidthHeight[i, 0], WidthHeight[i, 1]), 1)
                    .DrawImage(NotComplete, new Point(WidthHeight[i, 0], WidthHeight[i, 1]), 0.8f)
                    );
                }
            });
            using (Image<Rgba32> TierBar = Image.Load<Rgba32>("Assets/Summit/TierBar.png"))
            {
                TierBar.Mutate(ctx => ctx.DrawImage(new Image<Rgba32>(new Configuration(), TierBar.Width, TierBar.Height, backgroundColor: Color.Black), new Point(0, 0), 0.35f));
                int[] TierXPos = new int[4] { 845, 563, 281, 0 };
                bool[] Tier = new bool[] { false, false, false, false };
                Parallel.For(0, Events.Tier_entries.Length, (i, state) =>
                {
                    if (Events.Tier_entries[i].Points == 4294967295)
                    {
                        Tier[i] = true;
                    }
                    else
                    {
                        if (Events.Tier_entries[i].Points <= Events.Points)
                        {
                            Tier[i] = true;
                        }

                        TierBar.Mutate(ctx => ctx
                        .DrawText(new DrawingOptions { TextOptions = AllignTopLeft }, $"Points Needed: {Events.Tier_entries[i].Points}", SummitCaps12, Color.White, new PointF(TierXPos[i] + 5, 15))
                        );
                    }
                });

                TierBar.Mutate(ctx => ctx
                        .DrawText(new DrawingOptions { TextOptions = AllignTopLeft }, $"Summit Rank: {Events.UserRank + 1} Score: {Events.Points}", SummitCaps15, Color.White, new PointF(TierXPos[Tier.Count(c => c) - 1] + 5, 0))
                        );

                BaseImage.Mutate(ctx => ctx
                .DrawImage(TierBar, new Point(0, BaseImage.Height - 30), 1)
                );
            }
            return BaseImage;
        }

        public static TCHubJson.TceSummit GetTCEInfo(CommandContext ctx)
        {
            string link = $"{Program.TCEJson.Link}api/tchub/profileId/{Program.TCEJson.Key}/{ctx.User.Id}";

            TCHubJson.TceSummit JTCE;
            using (WebClient wc = new())
            {
                try
                {
                    string Jdown = wc.DownloadString(link);
                    JTCE = JsonConvert.DeserializeObject<TCHubJson.TceSummit>(Jdown);
                }
                catch (Exception)
                {
                    JTCE = new TCHubJson.TceSummit
                    {
                        Error = "No Connection."
                    };
                }
            }
            return JTCE;
        }
    }
}