using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveBot
{
    internal class CustomMethod
    {
        public static Rgba32 GetColour(string incolour)
        {
            return (incolour) switch
            {
                "black" => Color.Black,
                "white" => Color.WhiteSmoke,
                "red" => Color.Red,
                "green" => Color.Green,
                _ => Color.Transparent
            };
        }

        public static string GetConnString()
        {
            string json;
            using (var fs = File.OpenRead("Config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();
            var cfgjson = JsonConvert.DeserializeObject<ConfigJson.Config>(json).DataBase;
            return $"Host={cfgjson.Host};Username={cfgjson.Username};Password={cfgjson.Password};Database={cfgjson.Database}; Port={cfgjson.Port}";
        }

        public static DateTime EpochConverter(long ms)
        {
            DateTime f = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return f.AddMilliseconds(ms);
        }

        public static void AddUserToLeaderboard(DiscordUser user)
        {
            DB.Leaderboard newEntry = new DB.Leaderboard
            {
                ID_User = user.Id,
                Followers = (long)0,
                Level = 0,
                Bucks = (long)0
            };
            DB.DBLists.InsertLeaderboard(newEntry);
            List<DB.UserImages> UserImg = DB.DBLists.UserImages;
            var idui = UserImg.Max(m => m.ID_User_Images);
            DB.UserImages newUImage = new DB.UserImages
            {
                User_ID = user.Id,
                BG_ID = 1,
                ID_User_Images = idui + 1
            };
            DB.DBLists.InsertUserImages(newUImage);
            List<DB.UserSettings> UserSet = DB.DBLists.UserSettings;
            var us = UserSet.Max(m => m.ID_User_Settings);
            DB.UserSettings newUSettings = new DB.UserSettings
            {
                User_ID = user.Id,
                Background_Colour = "white",
                Text_Colour = "black",
                Border_Colour = "black",
                User_Info = "Just a flesh wound",
                Image_ID = newUImage.ID_User_Images,
                ID_User_Settings = us + 1
            };
            DB.DBLists.InsertUserSettings(newUSettings);
        }

        public static void AddUserToServerRanks(DiscordUser user, DiscordGuild guild)
        {
            if (DB.DBLists.Leaderboard.FirstOrDefault(f=>f.ID_User==user.Id) == null)
            {
                AddUserToLeaderboard(user);
            }
            List<DB.ServerRanks> Leaderboard = DB.DBLists.ServerRanks;
            var local = (from lb in Leaderboard
                         where lb.User_ID == user.Id
                         where lb.Server_ID == guild.Id
                         select lb).ToList();
            if (local.Count == 0)
            {
                DB.ServerRanks newEntry = new DB.ServerRanks
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
            string[] sTime = new string[2];
            for (int i = 0; i < Time.ToString().Length; i++)
            {
                if (i < Time.ToString().Length - 3)
                {
                    sTime[0] += Time.ToString()[i];
                }
                else
                {
                    sTime[1] += Time.ToString()[i];
                }
            }
            TimeSpan seconds = TimeSpan.FromSeconds(double.Parse(sTime[0]));
            TimeSpan ms = TimeSpan.FromMilliseconds(double.Parse(sTime[1]));
            TimeSpan Total = seconds + ms;

            if (Total.Hours == 0)
            {
                return $"{Total.Minutes}:{Total.Seconds}.{Total.Milliseconds}";
            }

            return $"{Total.Hours}:{Total.Minutes}:{Total.Seconds}.{Total.Milliseconds}";
        }

        public static string GetServerTop(CommandContext ctx, int page)
        {
            StringBuilder sb = new StringBuilder();
            string personalscore = "";
            List<DB.ServerRanks> leaderboard = DB.DBLists.ServerRanks;
            var user = leaderboard.OrderByDescending(x => x.Followers).ToList();
            user = user.Where(w => w.Server_ID == ctx.Guild.Id).ToList();
            sb.AppendLine("```csharp\n📋 Rank | Username");
            for (int i = (int)(page * 10) - 10; i < page * 10; i++)
            {
                var duser = ctx.Client.GetUserAsync(Convert.ToUInt64(user[i].User_ID));
                sb.AppendLine($"[{i + 1}]\t# {duser.Result.Username}\n\t\t\tFollowers:{user[i].Followers}");
                if (i == user.Count - 1)
                {
                    i = (int)page * 10;
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
            StringBuilder sb = new StringBuilder();
            string list = "", personalscore = "";
            List<DB.Leaderboard> leaderboard = DB.DBLists.Leaderboard;
            var user = leaderboard.OrderByDescending(x => x.Followers).ToList();
            sb.AppendLine("```csharp\n📋 Rank | Username");
            for (int i = (int)(page * 10) - 10; i < page * 10; i++)
            {
                var duser = ctx.Client.GetUserAsync(System.Convert.ToUInt64(user[i].ID_User));
                list += $"[{i + 1}]\t# {duser.Result.Username}\n\t\t\tFollowers:{user[i].Followers}\tLevel:{user[i].Level}\n";
                sb.AppendLine($"[{i + 1}]\t# {duser.Result.Username}\n\t\t\tFollowers:{user[i].Followers}\tLevel:{user[i].Level}");
                if (i == user.Count - 1)
                {
                    i = (int)page * 10;
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

            StringBuilder sb = new StringBuilder();
            sb.Append("Visual representation of the backgrounds can be viewed here: <http://bit.ly/LiveBG>\n```csharp\n[ID]\tBackground Name\n");
            for (int i = (int)(page * 10) - 10; i < page * 10; i++)
            {
                bool check = false;
                foreach (var userimage in List)
                {
                    if (Backgrounds[i].ID_BG == (int)userimage.ID_BG)
                    {
                        sb.Append($"[{Backgrounds[i].ID_BG}]\t# {Backgrounds[i].Name}\n\t\t\t [OWNED]\n");
                        check = true;
                    }
                }
                if (check == false)
                {
                    sb.Append($"[{Backgrounds[i].ID_BG}]\t# {Backgrounds[i].Name}\n\t\t\t Price:{Backgrounds[i].Price} Bucks\n");
                }
                if (i == Backgrounds.Count - 1)
                {
                    i = (int)page * 10;
                }
            }
            sb.Append("```");

            return sb.ToString();
        }

        public static async Task WarnUserAsync(DiscordUser user, DiscordUser admin, DiscordGuild server, DiscordChannel channel, string reason, bool automsg)
        {
            DB.DBLists.LoadServerRanks();
            DB.DBLists.LoadServerSettings();
            DB.ServerRanks WarnedUserStats = DB.DBLists.ServerRanks.FirstOrDefault(f => server.Id==f.Server_ID && user.Id==f.User_ID);
            DB.ServerSettings ServerSettings = DB.DBLists.ServerSettings.FirstOrDefault(f => server.Id==f.ID_Server);

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
            StringBuilder SB = new StringBuilder();
            decimal uid = user.Id, aid = admin.Id;
            bool kick = false;
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            if (ServerSettings.WKB_Log != 0)
            {
                DiscordChannel modlog = server.GetChannel(Convert.ToUInt64(ServerSettings.WKB_Log));
                if (WarnedUserStats is null) // creates new entry in DB (Followers set to default value)
                {
                    DB.ServerRanks newEntry = new DB.ServerRanks
                    {
                        Server_ID = server.Id,
                        Ban_Count = 0,
                        Kick_Count = 0,
                        Warning_Level = 1,
                        User_ID = user.Id
                    };
                    DB.DBLists.InsertServerRanks(newEntry);
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
                    DB.DBLists.UpdateServerRanks(new List<DB.ServerRanks> { WarnedUserStats });
                }

                DB.Warnings newWarning = new DB.Warnings
                {
                    Reason = reason,
                    Active = true,
                    Date = DateTime.Now.ToString("yyyy-MM-dd"),
                    Admin_ID = aid,
                    User_ID = uid,
                    Server_ID = server.Id
                };
                DB.DBLists.InsertWarnings(newWarning);

                int warning_count = DB.DBLists.Warnings.Where(w => w.User_ID == user.Id && w.Server_ID == server.Id).Count();

                SB.AppendLine($"You have been warned by <@{admin.Id}>.\n**Warning Reason:**\t{reason}\n**Warning Level:** {WarnedUserStats.Warning_Level}\n**Server:** {server.Name}");
                embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(0xf90707),
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        IconUrl = user.AvatarUrl,
                        Name = user.Username
                    },
                    Description = $"**Warned user:**\t{user.Mention}\n**Warning level:**\t {WarnedUserStats.Warning_Level}\t**Warning count:**\t {warning_count}\n**Warned by**\t{admin.Username}\n**Reason:** {reason}"
                };

                if (WarnedUserStats.Warning_Level > 2)
                {
                    SB.AppendLine($"You have been kicked from **{server.Name}** by {admin.Mention} for exceeding the warning level threshold(2).");
                    kick = true;
                }

                if (automsg == true)
                {
                    SB.Append("*This warning is given out by a bot, contact an admin if you think this is a mistake*");
                }
                else
                {
                    SB.Append("*(This is an automated message, contact the moderator personally if you have any questions.)*");
                }

                try
                {
                    await member.SendMessageAsync(SB.ToString());
                }
                catch
                {
                    await modlog.SendMessageAsync($":exclamation::exclamation:{user.Mention} could not be contacted via DM. Reason not sent");
                }

                await modlog.SendMessageAsync(modinfo, embed: embed);

                if (kick == true && member != null)
                {
                    await member.RemoveAsync("Exceeded warning limit!");
                }

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
                    || role.CheckPermission(Permissions.Administrator) == PermissionLevel.Allowed)
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
                    _ => "gb"
                };
            }
            if (member is null)
            {
                member = ctx.Member;
            }

            var OutputEntry = DB.DBLists.BotOutputList.Where(w => w.Command.Equals(command) && w.Language.Equals(language)).FirstOrDefault();
            if (OutputEntry is null)
            {
                OutputEntry = DB.DBLists.BotOutputList.Where(w => w.Command.Equals(command) && w.Language.Equals("gb")).FirstOrDefault();
                if (OutputEntry is null)
                {
                    return $"{ctx.Member.Mention}, Command output not found. Contact an admin.";
                }
            }
            return $"{member.Mention}, {OutputEntry.Command_Text}";
        }
    }
}