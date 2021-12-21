using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using LiveBot.Json;
using Newtonsoft.Json;
using System.Net.Http;

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
                Missions.AppendLine($"{i}\t{MissionList[i].ID}\t{HubMethods.NameIDLookup(MissionList[i].Text_ID)}");
            }
            Missions.Append("```");
            return Missions.ToString();
        }

        public static async Task SendModLog(DiscordChannel ModLogChannel, DiscordUser TargetUser, string Description, ModLogType type, string Content=null)
        {
            DiscordColor color = DiscordColor.NotQuiteBlack;
            string FooterText = string.Empty;
            switch (type)
            {
                case ModLogType.Kick:
                    color = new DiscordColor(0xf90707);
                    FooterText = "User Kicked";
                    break;
                case ModLogType.Ban:
                    color = new DiscordColor(0xf90707);
                    FooterText = "User Banned";
                    break;
                case ModLogType.Info:
                    color = new DiscordColor(0x59bfff);
                    FooterText = "Info";
                    break;
                case ModLogType.Warning:
                    color = new DiscordColor(0xFFBA01);
                    FooterText = "User Warned";
                    break;
                case ModLogType.Unwarn:
                    FooterText = "User Unwarned";
                    break;
                case ModLogType.Unban:
                    FooterText = "User Unbanned";
                    break;
                default:
                    break;
            }
            DiscordMessageBuilder discordMessageBuilder = new();
            DiscordEmbedBuilder discordEmbedBuilder = new()
            {
                Color = color,
                Description = Description,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl=TargetUser.AvatarUrl,
                    Name=$"{TargetUser.Username} ({TargetUser.Id})"
                },
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl=TargetUser.AvatarUrl,
                    Text=FooterText
                }
            };

            discordMessageBuilder.AddEmbed(discordEmbedBuilder);
            discordMessageBuilder.Content = Content;

            await ModLogChannel.SendMessageAsync(discordMessageBuilder);
        }

        public static async Task WarnUserAsync(DiscordUser user, DiscordUser admin, DiscordGuild server, DiscordChannel channel, string reason, bool automsg, InteractionContext ctx = null)
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
                    Time_Created = DateTime.UtcNow,
                    Admin_ID = aid,
                    User_ID = uid,
                    Server_ID = server.Id,
                    Type = "warning"
                };
                DB.DBLists.InsertWarnings(newWarning);

                int warning_count = DB.DBLists.Warnings.Count(w => w.User_ID == user.Id && w.Server_ID == server.Id && w.Type == "warning");

                SB.AppendLine($"You have been warned by <@{admin.Id}>.\n**Warning Reason:**\t{reason}\n**Warning Level:** {WarnedUserStats.Warning_Level}\n**Server:** {server.Name}");

                string warningDescription = $"**Warned user:**\t{user.Mention}\n**Warning level:**\t {WarnedUserStats.Warning_Level}\t**Warning count:**\t {warning_count}\n**Warned by**\t{admin.Username}\n**Reason:** {reason}";

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

                await SendModLog(modlog, user, warningDescription, ModLogType.Warning, modinfo);

                if (ctx == null)
                {
                    DiscordMessage info = await channel.SendMessageAsync($"{user.Username}, Has been warned!");
                    await Task.Delay(10000).ContinueWith(t => info.DeleteAsync());
                }
                else
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{user.Username}, Has been warned!"));
                    await Task.Delay(10000).ContinueWith(t => ctx.DeleteResponseAsync());
                }
            }
            else
            {
                if (ctx == null)
                {
                    await channel.SendMessageAsync("This server has not set up this feature!");
                }
                else
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("This server has not set up this feature!"));
                }
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
                var WarningsList = warnings.Where(w => w.User_ID == User.Id && w.Server_ID == Guild.Id).OrderBy(w=>w.Time_Created).ToList();
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
                    string addedInfraction = $"**ID:**{item.ID_Warning}\t**By:** <@{item.Admin_ID}>\t**Date:** <t:{(int)(item.Time_Created-new DateTime(1970,1,1)).TotalSeconds}>\n**Reason:** {item.Reason}\n **Type:**\t{item.Type}";

                    if (Reason.Length + addedInfraction.Length > 1023 * splitcount)
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
                    Name = $"{User.Username}({User.Id})",
                    IconUrl = User.AvatarUrl
                },
                Description = $"",
                Title = "Infraction Count",
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
            Program.Client.Logger.LogInformation(CustomLogEvents.POSTGRESQL, "{DataBase}",DataTableName is null ? "Starting to load Data Base" : $"{DataTableName} List Loaded");
            Program.Client.Logger.LogInformation(CustomLogEvents.POSTGRESQL, "{LoadBar}",sb.ToString());
            if (LoadedTableCount == DB.DBLists.TableCount)
            {
                DB.DBLists.LoadedTableCount = 0;
            }
        }

        public static async Task<TCHubJson.TceSummit> GetTCEInfo(ulong UserID)
        {
            string link = $"{Program.TCEJson.Link}api/tchub/profileId/{Program.TCEJson.Key}/{UserID}";

            TCHubJson.TceSummit JTCE;
            using (HttpClient wc = new())
            {
                try
                {
                    string Jdown = await wc.GetStringAsync(link);
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

        public enum ModLogType
        {
            Kick,
            Ban,
            Info,
            Warning,
            Unwarn,
            Unban
        }
    }
}