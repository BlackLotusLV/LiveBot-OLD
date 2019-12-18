using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System;

namespace LiveBot
{
    internal class CustomMethod
    {
        public static string LanguageIfNull(CommandContext ctx)
        {
            DiscordChannel current = ctx.Channel;
            return (current.Id) switch
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

        public static string ParamsStringConverter(string[] words)
        {
            string fullmsg = "";
            foreach (string item in words)
            {
                fullmsg += item + " ";
            }
            return fullmsg;
        }

        public static List<string> WebToHTML(string url, string regex)
        {
            HttpWebRequest WebReq = (HttpWebRequest)HttpWebRequest.Create(url);
            WebReq.Method = "GET";
            HttpWebResponse WebRes = (HttpWebResponse)WebReq.GetResponse();
            StreamReader WebSource = new StreamReader(WebRes.GetResponseStream());
            string source = WebSource.ReadToEnd();
            WebRes.Close();
            List<string> list = new List<string>();
            foreach (Match item in Regex.Matches(source, regex))
            {
                list.Add(item.ToString());
            }
            return list;
        }

        public static Rgba32 GetColour(string incolour)
        {
            return (incolour) switch
            {
                "black" => Rgba32.Black,
                "white" => Rgba32.WhiteSmoke,
                "red" => Rgba32.Red,
                "green" => Rgba32.Green,
                _ => Rgba32.Transparent
            };
        }

        public static string GetConnString()
        {
            string json;
            using (var fs = File.OpenRead("Config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();
            var cfgjson = JsonConvert.DeserializeObject<Json.Config>(json).DataBase;
            return $"Host={cfgjson.Host};Username={cfgjson.Username};Password={cfgjson.Password};Database={cfgjson.Database}";
        }

        public static List<DB.VehicleList> UpdateVehicle(List<DB.VehicleList> Full, List<DB.VehicleList> Small, int row)
        {
            var vehicle = (from f in Full
                           where f.ID_Vehicle == Small[row].ID_Vehicle
                           select f).ToList();
            vehicle[0].Selected_Count++;
            return vehicle;
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
                ID_User = user.Id.ToString(),
                Followers = (long)0,
                Level = 0,
                Bucks = (long)0
            };
            DB.DBLists.InsertLeaderboard(newEntry);
            List<DB.UserImages> UserImg = DB.DBLists.UserImages;
            var idui = UserImg.Max(m => m.ID_User_Images);
            DB.UserImages newUImage = new DB.UserImages
            {
                User_ID = user.Id.ToString(),
                BG_ID = 1,
                ID_User_Images = idui + 1
            };
            DB.DBLists.InsertUserImages(newUImage);
            List<DB.UserSettings> UserSet = DB.DBLists.UserSettings;
            var us = UserSet.Max(m => m.ID_User_Settings);
            DB.UserSettings newUSettings = new DB.UserSettings
            {
                User_ID = user.Id.ToString(),
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
            List<DB.ServerRanks> Leaderboard = DB.DBLists.ServerRanks;
            var local = (from lb in Leaderboard
                         where lb.User_ID == user.Id.ToString()
                         where lb.Server_ID == guild.Id.ToString()
                         select lb).ToList();
            if (local.Count == 0)
            {
                DB.ServerRanks newEntry = new DB.ServerRanks
                {
                    User_ID = user.Id.ToString(),
                    Server_ID = guild.Id.ToString(),
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
                if (i<Time.ToString().Length-3)
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

            if (Total.Hours==0)
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
            user = user.Where(w => w.Server_ID == ctx.Guild.Id.ToString()).ToList();
            sb.AppendLine("```csharp\n📋 Rank | Username");
            for (int i = (int)(page * 10) - 10; i < page * 10; i++)
            {
                var duser = ctx.Client.GetUserAsync(System.Convert.ToUInt64(user[i].User_ID.ToString()));
                sb.AppendLine($"[{i + 1}]\t# {duser.Result.Username}\n\t\t\tFollowers:{user[i].Followers}");
                if (i == user.Count - 1)
                {
                    i = (int)page * 10;
                }
            }
            int rank = 0;
            List<DB.ServerRanks> LB = DB.DBLists.ServerRanks;
            List<DB.ServerRanks> leaderbaords = LB.OrderByDescending(x => x.Followers).ToList();
            leaderbaords = leaderbaords.Where(w => w.Server_ID == ctx.Guild.Id.ToString()).ToList();
            foreach (var item in leaderbaords)
            {
                rank++;
                if (item.User_ID == ctx.User.Id.ToString())
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
                var duser = ctx.Client.GetUserAsync(System.Convert.ToUInt64(user[i].ID_User.ToString()));
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
                if (item.ID_User == ctx.User.Id.ToString())
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
                        where ui.User_ID == ctx.User.Id.ToString()
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
    }
}