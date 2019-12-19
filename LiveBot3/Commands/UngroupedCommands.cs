using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LiveBot.Commands
{
    public class UngroupedCommands : BaseCommandModule
    {
        [Command("bot")]//list of Live bot changes
        [Description("Info about the bot. Latest changes, how to support, how long it has been up.")]
        public async Task Bot(CommandContext ctx)
        {
            DateTime current = DateTime.Now;
            TimeSpan time = current - Program.start;
            string changelog = "[Change] Random vehicle command will now ask for vehicle type when asking for street race vehicle.\n" +
                "[NEW] `/servertop` (`/top`) and `/globaltop` (`/gtop`) now have buttons to switch between pages. 20 seconds after executing the command, or clicking a button, they will disapear\n" +
                "[NEW] `/bg` - profile backgrounds list also has buttons now.\n" +
                "";
            DiscordUser user = ctx.Client.CurrentUser;
            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = user.AvatarUrl,
                    Name = user.Username,
                    Url = "https://www.patreon.com/BlackLotusLV"
                }
            };
            embed.AddField("Version:", Program.BotVersion, true);
            embed.AddField("Uptime:", $"{time.Days} Days {time.Hours}:{time.Minutes}.{time.Seconds}", true);

            embed.AddField("Programmed in:", "C#", true);
            embed.AddField("Programmed by:", "<@86725763428028416>", true);
            embed.AddField("LiveBot info", "General purpose bot with a level system, stream notifications, greeting people and various other functions related to The Crew franchise");
            embed.AddField("Patreon:", "You can support the development of Live Bot and The Crew Community Discord here: https://www.patreon.com/BlackLotusLV");
            embed.AddField("Change log:", changelog);
            await ctx.Message.RespondAsync(embed: embed);
        }

        [Command("getemote")]
        [Description("Returns the ID of an emote")]
        public async Task GetEmote(CommandContext ctx, DiscordEmoji emote)
        {
            await ctx.RespondAsync(emote.Id.ToString());
        }

        [Command("ping")]
        [Description("Shows that the bots response time")]
        [Aliases("pong")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync($"Pong! {ctx.Client.Ping}ms");
        }

        [Command("share")]
        [Description("Informs the user about the content sharing channels and how to get the share role")] // photomode info command
        public async Task Share(CommandContext ctx, [Description("Specifies the user the bot will mention, use ID or mention the user. If left blank, it will mention you.")] DiscordMember username = null, [Description("Specifies in what language the bot will respond. example, fr-french")] string language = null)
        {
            if (language == null)
            {
                language = CustomMethod.LanguageIfNull(ctx);
            }
            string content = (language) switch
            {
                ("gb") => File.ReadAllText(@"TextFiles/english/share.txt"),
                ("de") => File.ReadAllText(@"TextFiles/german/share.txt"),
                ("fr") => File.ReadAllText(@"TextFiles/french/share.txt"),
                ("nl") => File.ReadAllText(@"TextFiles/dutch/share.txt"),
                ("se") => File.ReadAllText(@"TextFiles/swedish/share.txt"),
                ("ru") => File.ReadAllText(@"TextFiles/russian/share.txt"),
                ("lv") => File.ReadAllText(@"TextFiles/latvian/share.txt"),
                _ => File.ReadAllText(@"TextFiles/english/share.txt")
            };
            await ctx.Message.DeleteAsync(); //deletes command message
            if (username is null) //checks if user name is not specified
            {
                await ctx.RespondAsync($"{ctx.Member.Mention}, {content}");
            }
            else // if user name specified
            {
                await ctx.RespondAsync($"{username.Mention}, {content}");
            }
        }

        [Command("platform")]
        [Description("Informs the user about the platform roles.")] //platform selection command
        public async Task Platform(CommandContext ctx, [Description("Specifies the user the bot will mention, use ID or mention the user. If left blank, it will mention you.")] DiscordMember username = null, [Description("Specifies in what language the bot will respond. example, fr-french")] string language = null)
        {
            if (language == null)
            {
                language = CustomMethod.LanguageIfNull(ctx);
            }
            string content = language switch
            {
                ("gb") => File.ReadAllText(@"TextFiles/english/platform.txt"),
                ("de") => File.ReadAllText(@"TextFiles/german/platform.txt"),
                ("fr") => File.ReadAllText(@"TextFiles/french/platform.txt"),
                ("nl") => File.ReadAllText(@"TextFiles/dutch/platform.txt"),
                ("se") => File.ReadAllText(@"TextFiles/swedish/platform.txt"),
                ("ru") => File.ReadAllText(@"TextFiles/russian/platform.txt"),
                ("lv") => File.ReadAllText(@"TextFiles/latvian/platform.txt"),
                _ => File.ReadAllText(@"TextFiles/english/platform.txt")
            };
            await ctx.Message.DeleteAsync();
            if (username is null)
            {
                await ctx.RespondAsync($"{ctx.Member.Mention}, {content}");
            }
            else
            {
                await ctx.RespondAsync($"{username.Mention}, {content}");
            }
        }

        [Command("maxlvl")]
        [Description("Explains how to get maximum car level in The Crew 1.")] // how to get max level for cars in TC1
        public async Task MaxCarlvl(CommandContext ctx, [Description("Specifies the user the bot will mention, use ID or mention the user. If left blank, it will mention you.")] DiscordMember username = null, [Description("Specifies in what language the bot will respond. example, fr-french")] string language = null)
        {
            if (language == null)
            {
                language = CustomMethod.LanguageIfNull(ctx);
            }
            string content = (language) switch
            {
                ("gb") => File.ReadAllText(@"TextFiles/english/maxlvl.txt"),
                ("de") => File.ReadAllText(@"TextFiles/german/maxlvl.txt"),
                ("fr") => File.ReadAllText(@"TextFiles/french/maxlvl.txt"),
                ("nl") => File.ReadAllText(@"TextFiles/dutch/maxlvl.txt"),
                ("se") => File.ReadAllText(@"TextFiles/swedish/maxlvl.txt"),
                ("ru") => File.ReadAllText(@"TextFiles/russian/maxlvl.txt"),
                ("lv") => File.ReadAllText(@"TextFiles/latvian/maxlvl.txt"),
                _ => File.ReadAllText(@"TextFiles/english/maxlvl.txt")
            };
            await ctx.Message.DeleteAsync();
            if (username is null)
            {
                await ctx.RespondAsync($"{ctx.Member.Mention}, {content}");
            }
            else
            {
                await ctx.RespondAsync($"{username.Mention}, {content}");
            }
        }

        [Command("tce")] //The Crew Exchange info command
        [Description("Informs the user about The Crew Exchange website.")]
        public async Task TCE(CommandContext ctx, [Description("Specifies the user the bot will mention, use ID or mention the user. If left blank, it will mention you.")] DiscordMember username = null, [Description("Specifies in what language the bot will respond. example, fr-french")] string language = null)
        {
            if (language == null)
            {
                language = CustomMethod.LanguageIfNull(ctx);
            }
            string content = language switch
            {
                ("gb") => File.ReadAllText(@"TextFiles/english/tce.txt"),
                ("de") => File.ReadAllText(@"TextFiles/german/tce.txt"),
                ("fr") => File.ReadAllText(@"TextFiles/french/tce.txt"),
                ("nl") => File.ReadAllText(@"TextFiles/dutch/tce.txt"),
                ("se") => File.ReadAllText(@"TextFiles/swedish/tce.txt"),
                ("ru") => File.ReadAllText(@"TextFiles/russian/tce.txt"),
                ("lv") => File.ReadAllText(@"TextFiles/latvian/tce.txt"),
                _ => File.ReadAllText(@"TextFiles/english/tce.txt")
            };
            await ctx.Message.DeleteAsync();
            if (username is null)
            {
                await ctx.RespondAsync($"{ctx.Member.Mention}, {content}");
            }
            else
            {
                await ctx.RespondAsync($"{username.Mention}, {content}");
            }
        }

        [Command("lfc")]
        [Description("Informs the user of using the LFC channels, or to get the platform role if they don't have it.")]
        public async Task LFC(CommandContext ctx, DiscordMember username = null)
        {
            string content = File.ReadAllText(@"TextFiles/english/lfcnorole.txt");
            DiscordRole pc = ctx.Guild.GetRole(223867454642716673);
            DiscordRole ps = ctx.Guild.GetRole(223867009484587008);
            DiscordRole xb = ctx.Guild.GetRole(223867264246611970);
            bool check = false;
            if (username == null)
            {
                username = ctx.Member;
            }
            foreach (var item in username.Roles)
            {
                if ((item == pc || item == ps || item == xb) && check == false)
                {
                    content = File.ReadAllText(@"TextFiles/english/lfcrole.txt");
                }
            }
            await ctx.RespondAsync($"{username.Mention}, {content}");
            await ctx.Message.DeleteAsync();
        }

        [Command("it")]
        [Description("Sends the IT Crowd image of \"have you tried turning it off and on again?\"")]
        public async Task IT(CommandContext ctx, DiscordMember username = null)
        {
            await ctx.Message.DeleteAsync();
            FileStream ITImage = new FileStream("ITC.jpg", FileMode.Open);
            if (username == null)
            {
                await ctx.RespondWithFileAsync(ITImage, ctx.User.Mention);
            }
            else
            {
                await ctx.RespondWithFileAsync(ITImage, username.Mention);
            }
        }

        [Command("support")]
        [Description("Gives the link to the support page")]
        public async Task Support(CommandContext ctx, DiscordMember username = null)
        {
            await ctx.Message.DeleteAsync();
            string content = "seems like you should contact support: https://support.ubi.com";
            if (username is null)
            {
                await ctx.RespondAsync($"{ctx.Member.Mention}, {content}");
            }
            else
            {
                await ctx.RespondAsync($"{username.Mention}, {content}");
            }
        }

        [Command("forums")]
        [Description("Gives the link to the forum")]
        public async Task Forums(CommandContext ctx, DiscordMember username = null)
        {
            await ctx.Message.DeleteAsync();
            string content = "seems like what you are discussing would better fit to be posted on the forums: https://forums.ubi.com/forumdisplay.php/497-The-Crew";
            if (username is null)
            {
                await ctx.RespondAsync($"{ctx.Member.Mention}, {content}");
            }
            else
            {
                await ctx.RespondAsync($"{username.Mention}, {content}");
            }
        }

        [Command("quote")]
        [Description("Quotes a message using its ID")]
        [Priority(10)]
        public async Task Quote(CommandContext ctx, [Description("message ID")] DiscordMessage msg)
        {
            await ctx.Message.DeleteAsync();
            string content = $"\"{msg.Content}\"";
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(0xFF6600),
                Description = $"{content}\n[go to message]({msg.JumpLink})",
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = msg.Author is DiscordMember mx ? mx.DisplayName : msg.Author.Username,
                    IconUrl = msg.Author.AvatarUrl
                }
            };
            await ctx.RespondAsync($"{ctx.User.Mention} is quoting:", embed: embed);
        }

        [Command("quote")]
        [Description("Cross channel message quoting. (ChannelID-MessageID)")]
        [Priority(9)]
        public async Task Quote(CommandContext ctx, [Description("Channel and Message ID seperated by -")] string ChannelMsg)
        {
            string[] strarr = ChannelMsg.Split("-");
            if (strarr.Length == 2)
            {
                ulong channelid = ulong.Parse(strarr[0]);
                ulong msgid = ulong.Parse(strarr[1]);
                await ctx.Message.DeleteAsync();
                DiscordChannel channel = ctx.Guild.GetChannel(channelid);
                DiscordMessage msg = await channel.GetMessageAsync(msgid);
                string content = $"\"{msg.Content}\"";
                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(0xFF6600),
                    Description = $"{content}\n[go to message]({msg.JumpLink})",
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = msg.Author is DiscordMember mx ? mx.DisplayName : msg.Author.Username,
                        IconUrl = msg.Author.AvatarUrl
                    }
                };
                await ctx.RespondAsync($"{ctx.User.Mention} is quoting:", embed: embed);
            }
        }

        [Command("info")]
        [Description("Shows users discord info")]
        public async Task Info(CommandContext ctx, [Description("users ID or mention")] DiscordMember user = null)
        {
            await Task.Delay(5);
            await ctx.Message.DeleteAsync();
            if (user == null)
            {
                user = ctx.Member;
            }
            string format = "dddd, MMM dd yyyy HH:mm:ss zzzz";
            CultureInfo info = new CultureInfo("en-GB");
            string joinedstring = user.JoinedAt.ToString(format, info);
            string createdstring = user.CreationTimestamp.ToString(format, info);
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(0xFF6600),
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = user.Username,
                    IconUrl = user.AvatarUrl
                },
                Description = $"**ID**\t\t\t\t\t\t\t\t\t\t\t\t**Nickname**\n" +
                $"{user.Id}\t\t\t{(user.Nickname ?? "*none*")}\n" +
                $"**Account created**\n" +
                $"{createdstring}\n" +
                $"**Join date**\n" +
                $"{joinedstring}\n",
                Title = "User info",
                ThumbnailUrl = user.AvatarUrl
            };
            await ctx.RespondAsync(embed: embed);
        }

        [Command("german"), RequireRoles(RoleCheckMode.Any, "German")]
        [Description("Give german role to user to access language voice channel. Requires the role to give the role")]
        public async Task GermanRole(CommandContext ctx, [Description("Who to give the role to.")] DiscordMember user)
        {
            DiscordRole role = ctx.Guild.GetRole(261616587923128330);
            await user.GrantRoleAsync(role);
            await ctx.RespondAsync($"German role given to {user.Mention}.");
        }

        [Command("russian"), RequireRoles(RoleCheckMode.Any, "Russian")]
        [Description("Give russian role to user to access language voice channel. Requires the role to give the role")]
        public async Task RussianRole(CommandContext ctx, [Description("Who to give the role to.")] DiscordMember user)
        {
            DiscordRole role = ctx.Guild.GetRole(458654242862006294);
            await user.GrantRoleAsync(role);
            await ctx.RespondAsync($"Russian role given to {user.Mention}.");
        }

        [Command("french"), RequireRoles(RoleCheckMode.Any, "French")]
        [Description("Give french role to user to access language voice channel. Requires the role to give the role")]
        public async Task FrenchRole(CommandContext ctx, [Description("Who to give the role to.")] DiscordMember user)
        {
            DiscordRole role = ctx.Guild.GetRole(326292304535224321);
            await user.GrantRoleAsync(role);
            await ctx.RespondAsync($"French role given to {user.Mention}.");
        }

        [Command("swedish"), RequireRoles(RoleCheckMode.Any, "Swedish")]
        [Description("Give swedish role to user to access language voice channel. Requires the role to give the role")]
        public async Task SwedishRole(CommandContext ctx, [Description("Who to give the role to.")] DiscordMember user)
        {
            DiscordRole role = ctx.Guild.GetRole(458660241731616796);
            await user.GrantRoleAsync(role);
            await ctx.RespondAsync($"Swedish role given to {user.Mention}.");
        }

        [Command("latvian"), RequireRoles(RoleCheckMode.Any, "Latvian")]
        [Description("Give latvian role to user to access language voice channel. Requires the role to give the role")]
        public async Task LatvianRole(CommandContext ctx, [Description("Who to give the role to.")] DiscordMember user)
        {
            DiscordRole role = ctx.Guild.GetRole(458660350842241034);
            await user.GrantRoleAsync(role);
            await ctx.RespondAsync($"Latvian role given to {user.Mention}.");
        }

        [Command("convert")]
        [Description("Converts M to KM and KM to M")]
        public async Task Convert(CommandContext ctx,
            [Description("value of the speed you want to convert")] double value,
            [Description("Mesurment of speed from what you convert")]string mesurement)
        {
            double result;
            mesurement = mesurement.ToLower();
            const double ToMiles = 0.621371192;
            const double ToKilometers = 1.609344;
            switch (mesurement)
            {
                case "km":
                case "kilometers":
                case "kmh":
                case "k":
                    result = value * ToMiles;
                    await ctx.RespondAsync($"{value} Kilometers = {result} Miles");
                    break;

                case "miles":
                case "mph":
                case "m":
                    result = value * ToKilometers;
                    await ctx.RespondAsync($"{value} Miles = {result} Kilometers");
                    break;
            }
        }

        [Command("private")]
        [Description("Creates a private voice chat, where only you can add and remove people from. Only patreons and Moderators can use this.")]
        [RequireRoles(RoleCheckMode.Any, "Tier 2")]
        public async Task Private(CommandContext ctx)
        {
            DiscordChannel category = ctx.Guild.GetChannel(477797862965772288);
            DiscordOverwriteBuilder user = new DiscordOverwriteBuilder();
            DiscordOverwriteBuilder everyone = new DiscordOverwriteBuilder();
            user.For(ctx.Member);
            user.Allow(Permissions.AccessChannels);
            user.Allow(Permissions.ManageChannels);
            everyone.For(ctx.Guild.EveryoneRole);
            everyone.Deny(Permissions.AccessChannels);
            IEnumerable<DiscordOverwriteBuilder> builder = new DiscordOverwriteBuilder[] { user, everyone };
            string name = $"{ctx.Member.Username} channel";
            await ctx.Guild.CreateVoiceChannelAsync(name, category, overwrites: builder);
            await ctx.RespondAsync($"Private \"{name}\" created.");
        }

        [Command("rvehicle")]
        [Aliases("rv")]
        [Description("Gives a random vehicle from a discipline. Street race gives both a bike and a car")]
        public async Task RandomVehicle(CommandContext ctx, DiscordEmoji discipline = null)
        {
            string disciplinename;
            if (discipline == null)
            {
                disciplinename = "Street Race";
            }
            disciplinename = (discipline.Id) switch
            {
                449686964757594123 => "Power Boat",
                449686964925628426 => "Alpha Grand Prix",
                449686964669513729 => "Air Race",
                449686965135212574 => "Touring Car",
                517383831398121473 => "Demolition Derby",
                449686964497547295 => "Monster Truck",
                449686964891811840 => "Aerobatics",
                449686964950794240 => "Jetsprint",
                481392539728084994 => "Hovercraft",
                449686964967309312 => "Rally Raid",
                449686964791410690 => "Rally Cross",
                449686964623638530 => "Moto Cross",
                449688867088367646 => "Hyper Car",
                449688867251945493 => "Drag Race",
                449688866870525963 => "Drift",
                449688867164127232 => "Street Race",
                _ => "Street Race"
            };

            List<DB.VehicleList> VehicleList = DB.DBLists.VehicleList;
            List<DB.DisciplineList> DisciplineList = DB.DBLists.DisciplineList.Where(w => w.Discipline_Name == disciplinename).ToList();
            Random r = new Random();
            int row = 0;
            int maxCount = (from vl in VehicleList
                            join dl in DisciplineList on vl.Discipline equals dl.ID_Discipline
                            where dl.Discipline_Name == disciplinename
                            select vl).ToList().Max(m => m.Selected_Count);

            List<DB.VehicleList> SelectedVehicles = new List<DB.VehicleList>();

            if (disciplinename=="Street Race")
            {

                DiscordMessage CarOrBike = await ctx.RespondAsync($"{ctx.Member.Mention} **Select vehicle type:**\n:one: - Car\n:two: - Bike");
                DiscordEmoji One = DiscordEmoji.FromName(ctx.Client, ":one:");
                DiscordEmoji Two = DiscordEmoji.FromName(ctx.Client, ":two:");

                await CarOrBike.CreateReactionAsync(One).ContinueWith(t=>CarOrBike.CreateReactionAsync(Two));

                var Result = await CarOrBike.WaitForReactionAsync(ctx.User, TimeSpan.FromSeconds(30));

                if (Result.TimedOut)
                {
                    await ctx.RespondAsync($"{ctx.Member.Mention} You didn't select vehicle type in time.");
                    return;
                }
                else if (Result.Result.Emoji== One)
                {
                    SelectedVehicles = (from vl in VehicleList
                                        join dl in DisciplineList on vl.Discipline equals dl.ID_Discipline
                                        where dl.Discipline_Name == disciplinename
                                        where vl.Type == "car"
                                        select vl).ToList();
                }
                else if (Result.Result.Emoji == Two)
                {
                    SelectedVehicles = (from vl in VehicleList
                                        join dl in DisciplineList on vl.Discipline equals dl.ID_Discipline
                                        where dl.Discipline_Name == disciplinename
                                        where vl.Type == "bike"
                                        select vl).ToList();
                }
            }
            else
            {
                SelectedVehicles = (from vl in VehicleList
                                    join dl in DisciplineList on vl.Discipline equals dl.ID_Discipline
                                    where dl.Discipline_Name == disciplinename
                                    select vl).ToList();
            }
            if (SelectedVehicles.Min(m=>m.Selected_Count)!=maxCount)
            {
                SelectedVehicles = (from sv in SelectedVehicles
                                    where sv.Selected_Count < maxCount
                                    select sv).ToList();
            }
            row = r.Next(SelectedVehicles.Count);
            DB.DBLists.UpdateVehicleList(CustomMethod.UpdateVehicle(VehicleList, SelectedVehicles, row));

            await ctx.RespondAsync($"{SelectedVehicles[row].Brand} | {SelectedVehicles[row].Model} | {SelectedVehicles[row].Year} ({SelectedVehicles[row].Type})");
        }

        [Command("rank")]
        [Description("Displays user server rank.")]
        public async Task Rank(CommandContext ctx, DiscordMember user = null)
        {
            if (user == null)
            {
                user = ctx.Member;
            }
            string output = "";
            int rank = 0;
            List<DB.ServerRanks> SR = DB.DBLists.ServerRanks;
            List<DB.ServerRanks> serverRanks = SR.Where(w => w.Server_ID == ctx.Guild.Id.ToString()).OrderByDescending(x => x.Followers).ToList();
            foreach (var item in serverRanks)
            {
                rank++;
                if (item.User_ID == user.Id.ToString())
                {
                    output = $"{user.Username}'s rank:{rank}\t Followers: {item.Followers}";
                }
            }
            await ctx.RespondAsync(output);
        }

        [Command("globalrank")]
        [Aliases("grank")]
        [Description("Displays users global rank")]
        public async Task GlobalRank(CommandContext ctx, DiscordMember user = null)
        {
            if (user == null)
            {
                user = ctx.Member;
            }
            string output = "";
            int rank = 0;
            List<DB.Leaderboard> LB = DB.DBLists.Leaderboard;
            List<DB.Leaderboard> leaderbaords = LB.OrderByDescending(x => x.Followers).ToList();
            foreach (var item in leaderbaords)
            {
                rank++;
                if (item.ID_User == user.Id.ToString())
                {
                    output = $"{user.Username}'s rank:{rank}\t Followers: {item.Followers}\t Level {item.Level}";
                }
            }
            await ctx.RespondAsync(output);
        }

        [Command("profile")]
        [Description("Shows users live bot profile.")]
        [Priority(10)]
        public async Task Profile(CommandContext ctx,
            [Description("Specify which user to show, if left empty, will take command caster")]DiscordMember user = null)
        {
            await ctx.TriggerTypingAsync();
            string json = "";
            using (var fs = File.OpenRead("Config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();
            Json.TCE tcejson = JsonConvert.DeserializeObject<Json.Config>(json).TCE;
            bool tcelink = false;
            if (user == null)
            {
                user = ctx.Member;
            }
            string link = $"https://thecrew-exchange.com/api/bot/isAccountLinked/{tcejson.Key}/{user.Id}";
            try
            {
                using WebClient wc = new WebClient();
                if (wc.DownloadString(link).ToLower().Contains("true"))
                {
                    tcelink = true;
                }
            }
            catch { }
            List<DB.Leaderboard> Leaderboard = DB.DBLists.Leaderboard;
            List<DB.UserSettings> USettings = DB.DBLists.UserSettings;
            var global = (from gl in Leaderboard
                          where gl.ID_User == user.Id.ToString()
                          select gl).ToList();
            var UserSettings = (from us in USettings
                                join ui in DB.DBLists.UserImages on us.Image_ID equals ui.ID_User_Images
                                join bi in DB.DBLists.BackgroundImage on ui.BG_ID equals bi.ID_BG
                                where us.User_ID == ui.User_ID
                                where us.User_ID == user.Id.ToString()
                                select new { us, ui, bi }).ToList();

            byte[] baseBG = (byte[])UserSettings[0].bi.Image;
            var webclinet = new WebClient();
            byte[] profilepic = webclinet.DownloadData(user.AvatarUrl);
            string username;
            StringBuilder sb = new StringBuilder();
            int usernameSize = 21;
            for (int i = 0; i < user.Username.Length; i++)
            {
                if (i == 20)
                {
                    sb.AppendLine();
                    usernameSize = 15;
                }
                else if (i == 41)
                {
                    break;
                }
                if ((int)user.Username[i] > short.MaxValue)
                {
                    sb.Append("?");
                }
                else
                {
                    sb.Append(user.Username[i]);
                }
            }
            username = sb.ToString();
            string level = global[0].Level.ToString();
            string followers = $"{global[0].Followers.ToString()}/{(int)global[0].Level * (300 * ((int)global[0].Level + 1) * 0.5)}";
            string bucks = global[0].Bucks.ToString();
            string bio = UserSettings[0].us.User_Info.ToString();

            double FBarLenght = 100 / (global[0].Level * (300 * ((int)global[0].Level + 1) * 0.5)) * global[0].Followers;

            Rgba32 bordercolour = CustomMethod.GetColour(UserSettings[0].us.Border_Colour.ToString());
            Rgba32 textcolour = CustomMethod.GetColour(UserSettings[0].us.Text_Colour.ToString());
            Rgba32 backfieldcolour = CustomMethod.GetColour(UserSettings[0].us.Background_Colour.ToString());

            using Image<Rgba32> pfp = Image.Load<Rgba32>(profilepic);
            using Image<Rgba32> picture = new Image<Rgba32>(600, 600);
            using Image<Rgba32> background = new Image<Rgba32>(560, 360);
            using Image<Rgba32> bg = Image.Load<Rgba32>(baseBG);
            using Image<Rgba32> FollowersBar = new Image<Rgba32>(System.Convert.ToInt32(Math.Floor((220 * FBarLenght) / 100)), 20);
            Font UsernameFont = SystemFonts.CreateFont("Consolas", usernameSize, FontStyle.Bold);
            Font Basefont = SystemFonts.CreateFont("Consolas", 21, FontStyle.Bold);
            Font LevelText = SystemFonts.CreateFont("Consolas", 30, FontStyle.Regular);
            Font LevelNumber = SystemFonts.CreateFont("Consolas", 50, FontStyle.Regular);
            Font InfoTextFont = SystemFonts.CreateFont("Consolas", 19, FontStyle.Regular);
            var AllignCenter = new TextGraphicsOptions(true)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            };
            Point bglocation = new Point(10, 10);
            Point pfplocation = new Point(440, 230);
            Point bgcolourlocation = new Point(20, 220);
            int BadgeX = 155, BadgeY = 260, BadgeCount = 0;
            background.Mutate(ctx => ctx
            .Fill(backfieldcolour)
            );
            string info = "";
            int LetterCount = 0, MaxInLine = 50, MaxLines = 8, LineCount = 1;
            foreach (var word in bio.Split(' '))
            {
                if (word.Length >= MaxInLine)
                {
                }
                else if (word == "$n")
                {
                    info += "\n";
                    LetterCount = 0;
                    LineCount++;
                }
                else if (LetterCount + word.Length + 1 <= MaxInLine)
                {
                    info += word + " ";
                    LetterCount += word.Length + 1;
                }
                else if (LetterCount + word.Length > MaxInLine && LineCount < MaxLines)
                {
                    info += "\n" + word + " ";
                    LetterCount = 0;
                    LineCount++;
                }
            }
            pfp.Mutate(ctx => ctx.Resize(128, 128));
            FollowersBar.Mutate(ctx => ctx.BackgroundColor(Color.Gray));
            picture.Mutate(ctx => ctx
                .DrawPolygon(bordercolour, 3, new PointF(10, 10), new PointF(590, 10), new PointF(590, 590), new PointF(10, 590)) //outside border
                .DrawImage(bg, bglocation, 1) //background image
                .DrawImage(background, bgcolourlocation, 0.6f) // base background for info
                .DrawPolygon(bordercolour, 3,
                new PointF(bgcolourlocation.X, bgcolourlocation.Y),
                new PointF(bgcolourlocation.X + background.Width, bgcolourlocation.Y),
                new PointF(bgcolourlocation.X + background.Width, bgcolourlocation.Y + background.Height),
                new PointF(bgcolourlocation.X, bgcolourlocation.Y + background.Height))
                .DrawImage(pfp, pfplocation, 1) // profile picture
                .DrawPolygon(bordercolour, 3,
                new PointF(pfplocation.X, pfplocation.Y),
                new PointF(pfplocation.X + pfp.Width, pfplocation.Y),
                new PointF(pfplocation.X + pfp.Width, pfplocation.Y + pfp.Height),
                new PointF(pfplocation.X, pfplocation.Y + pfp.Height)) //profile picture border
                .DrawImage(FollowersBar, new Point(152, 320), 1)
                .DrawPolygon(bordercolour, 1,
                new PointF(152, 320),
                new PointF(412, 320),
                new PointF(412, 340),
                new PointF(152, 340)) // follower bar border
                .DrawText(AllignCenter, username, UsernameFont, textcolour, new PointF(270, 225)) // username
                .DrawText($"LEVEL", LevelText, textcolour, new PointF(40, 230)) // levels
                .DrawText(level, LevelNumber, textcolour, new PointF(40, 260))
                .DrawText($"Followers:", Basefont, textcolour, new PointF(40, 320))
                .DrawText(followers, InfoTextFont, textcolour, new PointF(155, 322))
                .DrawText($"Bucks:", Basefont, textcolour, new PointF(40, 340))
                .DrawText(bucks, InfoTextFont, textcolour, new PointF(110, 342))
                .DrawText($"User info:", Basefont, textcolour, new PointF(40, 380))
                .DrawText(info, InfoTextFont, textcolour, new PointF(30, 400))
                .DrawLines(GraphicsOptions.Default, bordercolour, 1, new PointF(30, pfplocation.Y + pfp.Height + 10), new PointF(570, pfplocation.Y + pfp.Height + 10))
                );
            if (tcelink == true)
            {
                using Image<Rgba32> badge = Image.Load<Rgba32>("Badges/tce.png");
                Point badgeloc = new Point(BadgeX + (BadgeCount * badge.Width) + 2, BadgeY);
                picture.Mutate(ctx => ctx
                .DrawImage(badge, badgeloc, 1)
                );
                BadgeCount++;
            }
            if (ctx.Guild.Id == 150283740172517376) // checks if the crew commmunity server
            {
                foreach (DiscordRole role in user.Roles)
                {
                    if (role.Id == 473247655913455617) // checks if the role is patreon role
                    {
                        using Image<Rgba32> badge = Image.Load<Rgba32>("Badges/Patreon.png");
                        Point badgeloc = new Point(BadgeX + (BadgeCount * badge.Width) + 5, BadgeY);
                        picture.Mutate(ctx => ctx
                        .DrawImage(badge, badgeloc, 1)
                        );
                        BadgeCount++;
                    }
                    if (role.Id == 585537338491404290)
                    {
                        using Image<Rgba32> badge = Image.Load<Rgba32>("Badges/Booster.png");
                        Point badgeloc = new Point(BadgeX + (BadgeCount * badge.Width) + 5, BadgeY);
                        picture.Mutate(ctx => ctx
                        .DrawImage(badge, badgeloc, 1)
                        );
                        BadgeCount++;
                    }
                }
            }

            picture.Save("bg.png");
            using FileStream upFile = File.Open("bg.png", FileMode.Open);
            await ctx.RespondWithFileAsync(upFile);
        }

        [Command("profile")]
        [Description("Profile customisation command")]
        [Priority(9)]
        public async Task Profile(CommandContext ctx, [Description("profile settings command input, write `/profile help` for info")] params string[] input)
        {
            string output = "";
            if (input[0] == "update" && input.Length > 1)
            {
                List<DB.UserSettings> USettings = DB.DBLists.UserSettings;
                var UserSettings = (from us in USettings
                                    where us.User_ID == ctx.User.Id.ToString()
                                    select us).ToList();
                if (input.Length > 2 && (input[1] == "bg" || input[1] == "background"))
                {
                    List<DB.UserImages> UImages = DB.DBLists.UserImages;
                    var UserImages = (from ui in UImages
                                      where ui.User_ID == ctx.User.Id.ToString()
                                      where ui.BG_ID == Int32.Parse(input[2])
                                      select ui).ToList();
                    if (UserImages.Count == 0)
                    {
                        output = "You do not have this background!";
                    }
                    else if (UserImages.Count == 1)
                    {
                        UserSettings[0].Image_ID = UserImages[0].ID_User_Images;
                        DB.DBLists.UpdateUserSettings(USettings);
                        output = $"You have changed your profile background";
                    }
                }
                else if (input.Length > 2 && input[1] == "info")
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 2; i < input.Length; i++)
                    {
                        sb.Append(input[i] + " ");
                    }
                    UserSettings[0].User_Info = sb.ToString();

                    DB.DBLists.UpdateUserSettings(USettings);
                    output = "You have changed your user info.";
                }
                else
                {
                    output = "Wrong parameters, try again";
                }
            }
            else if (input[0] == "help" || input[0] != null)
            {
                output = "This is how to use the update command:\n" +
                    "1. Updating background - `/profile update background [image id]` write `/background` to see your aviable backgrounds\n" +
                    "2. Updating info - `/profile update info [your info]` If you want to make a new line in your info, write `$n`\n" +
                    "";
            }
            await ctx.RespondAsync(output);
        }

        //*/

        [Command("globaltop")]
        [Aliases("gtop")]
        [Description("Shows the global live bot leaderboard")]
        [RequireGuild]
        public async Task GlobalTop(CommandContext ctx, int? page = null)
        {
            if (page == null || page <= 0)
            {
                page = 1;
            }
            DiscordMessage TopMessage = await ctx.RespondAsync(CustomMethod.GetGlobalTop(ctx, (int)page));
            DiscordEmoji left = DiscordEmoji.FromName(ctx.Client, ":arrow_left:");
            DiscordEmoji right = DiscordEmoji.FromName(ctx.Client, ":arrow_right:");

            await TopMessage.CreateReactionAsync(left);
            await Task.Delay(300).ContinueWith(t => TopMessage.CreateReactionAsync(right));

            bool end = false;
            do
            {
                var result = TopMessage.WaitForReactionAsync(ctx.User, TimeSpan.FromSeconds(30));
                if (result.Result.TimedOut)
                {
                    end = result.Result.TimedOut;
                }
                else if (result.Result.Result.Emoji == left)
                {
                    await TopMessage.DeleteReactionAsync(result.Result.Result.Emoji, ctx.User);
                    if (page > 1)
                    {
                        page--;
                        await TopMessage.ModifyAsync(CustomMethod.GetGlobalTop(ctx, (int)page));
                    }
                }
                else if (result.Result.Result.Emoji == right)
                {
                    await TopMessage.DeleteReactionAsync(result.Result.Result.Emoji, ctx.User);
                    page++;
                    try
                    {
                        await TopMessage.ModifyAsync(CustomMethod.GetGlobalTop(ctx, (int)page));
                    }
                    catch (Exception)
                    {
                        page--;
                    }
                }
            } while (!end);
            await TopMessage.DeleteAllReactionsAsync();
        }

        [Command("servertop")]
        [Aliases("top")]
        [Description("Shows the server specific leaderboard")]
        [RequireGuild]
        public async Task ServerTop(CommandContext ctx, int? page = null)
        {
            if (page == null || page <= 0)
            {
                page = 1;
            }

            DiscordMessage TopMessage = await ctx.RespondAsync(CustomMethod.GetServerTop(ctx, (int)page));
            DiscordEmoji left = DiscordEmoji.FromName(ctx.Client, ":arrow_left:");
            DiscordEmoji right = DiscordEmoji.FromName(ctx.Client, ":arrow_right:");

            await TopMessage.CreateReactionAsync(left);
            await Task.Delay(300).ContinueWith(t => TopMessage.CreateReactionAsync(right));

            bool end = false;
            do
            {
                var result = TopMessage.WaitForReactionAsync(ctx.User, TimeSpan.FromSeconds(30));
                if (result.Result.TimedOut)
                {
                    end = result.Result.TimedOut;
                }
                else if (result.Result.Result.Emoji==left)
                {
                    await TopMessage.DeleteReactionAsync(result.Result.Result.Emoji, ctx.User);
                    if (page>1)
                    {
                        page--;
                        await TopMessage.ModifyAsync(CustomMethod.GetServerTop(ctx, (int)page));
                    }
                }
                else if (result.Result.Result.Emoji == right)
                {
                    await TopMessage.DeleteReactionAsync(result.Result.Result.Emoji, ctx.User);
                    page++;
                    try
                    {
                        await TopMessage.ModifyAsync(CustomMethod.GetServerTop(ctx, (int)page));
                    }
                    catch (Exception)
                    {
                        page--;
                    }
                }
            } while (!end);
            await TopMessage.DeleteAllReactionsAsync();
        }

        [Command("background")]
        [Aliases("bg")]
        [Description("Shows a list of backgrounds")]
        public async Task Background(CommandContext ctx, int? page = null)
        {
            if (page == null || page <= 0)
            {
                page = 1;
            }
            DiscordMessage TopMessage = await ctx.RespondAsync(CustomMethod.GetBackgroundList(ctx, (int)page));
            DiscordEmoji left = DiscordEmoji.FromName(ctx.Client, ":arrow_left:");
            DiscordEmoji right = DiscordEmoji.FromName(ctx.Client, ":arrow_right:");

            await TopMessage.CreateReactionAsync(left);
            await Task.Delay(300).ContinueWith(t => TopMessage.CreateReactionAsync(right));

            bool end = false;
            do
            {
                var result = TopMessage.WaitForReactionAsync(ctx.User, TimeSpan.FromSeconds(20));
                if (result.Result.TimedOut)
                {
                    end = result.Result.TimedOut;
                }
                else if (result.Result.Result.Emoji == left)
                {
                    await TopMessage.DeleteReactionAsync(result.Result.Result.Emoji, ctx.User);
                    if (page > 1)
                    {
                        page--;
                        await TopMessage.ModifyAsync(CustomMethod.GetBackgroundList(ctx, (int)page));
                    }
                }
                else if (result.Result.Result.Emoji == right)
                {
                    await TopMessage.DeleteReactionAsync(result.Result.Result.Emoji, ctx.User);
                    page++;
                    try
                    {
                        await TopMessage.ModifyAsync(CustomMethod.GetBackgroundList(ctx, (int)page));
                    }
                    catch (Exception)
                    {
                        page--;
                    }
                }
            } while (!end);
            await TopMessage.DeleteAllReactionsAsync();
        }

        [Command("buy")]
        [Cooldown(1, 60, CooldownBucketType.User)]
        [Description("Command to buy profile customisation.")]
        public async Task Buy(CommandContext ctx,
            [Description("What you want to buy")] string what,
            [Description("ID of what you want to buy")]int id)
        {
            string output = "";
            if (what == "background" || what == "bg")
            {
                List<DB.UserImages> UserImg = DB.DBLists.UserImages;
                var backgroundrow = (from ui in UserImg
                                     where ui.User_ID == ctx.User.Id.ToString()
                                     where ui.BG_ID == id
                                     select ui).ToList();
                List<DB.BackgroundImage> BG = DB.DBLists.BackgroundImage;
                var background = (from bg in BG
                                  where bg.ID_BG == id
                                  select bg).ToList();
                List<DB.Leaderboard> Leaderboard = DB.DBLists.Leaderboard;
                var user = (from lb in Leaderboard
                            where lb.ID_User == ctx.User.Id.ToString()
                            select lb).ToList();

                if (background.Count == 1)
                {
                    if (backgroundrow.Count == 0)
                    {
                        if ((long)background[0].Price <= (long)user[0].Bucks)
                        {
                            var idui = UserImg.Max(m => m.ID_User_Images);
                            user[0].Bucks -= (long)background[0].Price;
                            DB.UserImages newEntry = new DB.UserImages
                            {
                                User_ID = ctx.User.Id.ToString(),
                                BG_ID = id,
                                ID_User_Images = idui + 1
                            };
                            DB.DBLists.InsertUserImages(newEntry);
                            DB.DBLists.UpdateLeaderboard(user);
                            output = $"You have bought the \"{background[0].Name}\" background.";
                        }
                        else
                        {
                            output = $"You don't have enough bucks to buy this background.";
                        }
                    }
                    else if (backgroundrow.Count == 1)
                    {
                        output = $"You already have this background, use `/background` command to ";
                    }
                }
            }
            else
            {
                output = "Wrong parameters";
            }
            await ctx.RespondAsync(output);
        }

        [Command("summit")]
        [Cooldown(1, 120, CooldownBucketType.User)]
        [Description("Shows summit tier list and time left.")]
        public async Task Summit(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            string PCJson = "", XBJson = "", PSJson = "";
            byte[] SummitLogo;
            DateTime endtime;
            using (WebClient wc = new WebClient())
            {
                string JSummitString = wc.DownloadString("https://api.thecrew-hub.com/v1/data/summit");
                List<Json.Summit> JSummit = JsonConvert.DeserializeObject<List<Json.Summit>>(JSummitString);
                PCJson = wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/pc/profile/a92d844e-9c57-4b8c-a249-108ef42d4500");
                XBJson = wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/x1/profile/a92d844e-9c57-4b8c-a249-108ef42d4500");
                PSJson = wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/ps4/profile/a92d844e-9c57-4b8c-a249-108ef42d4500");

                SummitLogo = wc.DownloadData($"https://www.thecrew-hub.com/gen/assets/summits/{JSummit[0].Cover_Small}");

                endtime = CustomMethod.EpochConverter(JSummit[0].End_Date * 1000);
            }
            Json.Rank[] Events = new Json.Rank[3] { JsonConvert.DeserializeObject<Json.Rank>(PCJson), JsonConvert.DeserializeObject<Json.Rank>(PSJson), JsonConvert.DeserializeObject<Json.Rank>(XBJson) };

            string[,] pts = new string[3, 4];
            for (int i = 0; i < Events.Length; i++)
            {
                for (int j = 0; j < Events[i].Tier_entries.Length; j++)
                {
                    if (Events[i].Tier_entries[j].Points == 4294967295)
                    {
                        pts[i, j] = "";
                    }
                    else
                    {
                        pts[i, j] = Events[i].Tier_entries[j].Points.ToString();
                    }
                }
            }
            using (Image<Rgba32> PCImg = Image.Load<Rgba32>("Summit/PC.jpeg"))
            using (Image<Rgba32> PSImg = Image.Load<Rgba32>("Summit/PS.jpg"))
            using (Image<Rgba32> XBImg = Image.Load<Rgba32>("Summit/XB.png"))
            using (Image<Rgba32> BaseImg = new Image<Rgba32>(900, 643))
            {
                Image<Rgba32>[] PlatformImg = new Image<Rgba32>[3] { PCImg, PSImg, XBImg };
                for (int i = 0; i < Events.Length; i++)
                {
                    using (Image<Rgba32> TierImg = Image.Load<Rgba32>("Summit/SummitBase.png"))
                    using (Image<Rgba32> SummitImg = Image.Load<Rgba32>(SummitLogo))
                    using (Image<Rgba32> FooterImg = new Image<Rgba32>(300, 30))
                    {
                        Rgba32 TextColour = Rgba32.WhiteSmoke;
                        Point SummitLocation = new Point(0 + (300 * i), 0);
                        Font Basefont = Program.fonts.CreateFont("HurmeGeometricSans3W03-Blk", 30);
                        Font FooterFont = Program.fonts.CreateFont("HurmeGeometricSans3W03-Blk", 15);
                        FooterImg.Mutate(ctx => ctx
                        .Fill(Rgba32.Black)
                        .DrawText($"TOTAL PARTICIPANTS: {Events[i].Player_Count}", FooterFont, TextColour, new PointF(10, 10))
                        );
                        BaseImg.Mutate(ctx => ctx
                            .DrawImage(SummitImg, SummitLocation, 1)
                            .DrawImage(TierImg, SummitLocation, 1)
                            .DrawText(pts[i, 3], Basefont, TextColour, new PointF(80 + (300 * i), 370))
                            .DrawText(pts[i, 2], Basefont, TextColour, new PointF(80 + (300 * i), 440))
                            .DrawText(pts[i, 1], Basefont, TextColour, new PointF(80 + (300 * i), 510))
                            .DrawText(pts[i, 0], Basefont, TextColour, new PointF(80 + (300 * i), 580))
                            .DrawImage(FooterImg, new Point(0 + (300 * i), 613), 1)
                            .DrawImage(PlatformImg[i], new Point(0 + (300 * i), 0), 1)
                            );
                    }
                    BaseImg.Save("Summit/SummitUpload.png");
                }
            }
            using FileStream upFile = File.Open("Summit/SummitUpload.png", FileMode.Open);
            TimeSpan timeleft = endtime - DateTime.Now.ToUniversalTime();
            await ctx.RespondWithFileAsync(upFile, $"Summit tier lists.\n *Ends in {timeleft.Days} days, {timeleft.Hours} hours, {timeleft.Minutes} minutes.*");
        }

        [Command("mysummit")]
        [Cooldown(1, 300, CooldownBucketType.User)]
        [Aliases("sinfo", "summitinfo")]
        public async Task MySummit(CommandContext ctx, string platform = null)
        {
            await ctx.TriggerTypingAsync();

            string OutMessage = "";

            bool SendImage = false;

            DateTime endtime;

            string search = "";
            string json = "";
            using (var fs = File.OpenRead("Config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();
            Json.TCE tcejson = JsonConvert.DeserializeObject<Json.Config>(json).TCE;
            string link = $"https://thecrew-exchange.com/api/tchub/profileId/{tcejson.Key}/{ctx.Member.Id}";

            Json.TCESummit JTCE;
            using (WebClient wc = new WebClient())
            {
                string Jdown = wc.DownloadString(link);
                JTCE = JsonConvert.DeserializeObject<Json.TCESummit>(Jdown);
            }

            Json.TCESummitSubs UserInfo = new Json.TCESummitSubs();

            if (JTCE.Error != null)
            {
                if (JTCE.Error == "Unregistered user")
                {
                    OutMessage = $"{ctx.Member.Mention}, You have not linked your TCE account, please check out <#302818290336530434> on how to do so.";
                }
                else if (JTCE.Error == "Invalid API key !")
                {
                    OutMessage = $"{ctx.Member.Mention}, the API is down, please try again later.";
                }
            }
            else if (JTCE.Subs.Length == 1)
            {
                UserInfo = JTCE.Subs[0];
                search = UserInfo.Platform;
            }
            else if (JTCE.Subs.Length > 1)
            {
                switch (platform.ToLower())
                {
                    case null:
                    case "pc":
                    case "computer":
                        search = "pc";
                        break;

                    case "xbox":
                    case "xb1":
                    case "xb":
                    case "x1":
                        search = "x1";
                        break;

                    case "ps4":
                    case "playstation":
                    case "ps":
                        search = "ps4";
                        break;
                }
                if (JTCE.Subs.Where(w => w.Platform.Equals(search)).Count() == 1)
                {
                    UserInfo = JTCE.Subs.Where(w => w.Platform.Equals(search)).FirstOrDefault();
                }
                else if (JTCE.Subs.Where(w => w.Platform.Equals(search)).Count() != 1)
                {
                    UserInfo = JTCE.Subs[0];
                }
            }

            if (UserInfo.Profile_ID != null)
            {
                string SJson;
                List<Json.Summit> JSummit;
                byte[] EventLogoBit;
                using (WebClient wc = new WebClient())
                {
                    string JSummitString = wc.DownloadString("https://api.thecrew-hub.com/v1/data/summit");
                    JSummit = JsonConvert.DeserializeObject<List<Json.Summit>>(JSummitString);
                    SJson = wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/{UserInfo.Platform}/profile/{UserInfo.Profile_ID}");

                    endtime = CustomMethod.EpochConverter(JSummit[0].End_Date * 1000);
                }
                Json.Rank Events = JsonConvert.DeserializeObject<Json.Rank>(SJson);

                int[,] WidthHeight = new int[,] { { 0, 249 }, { 373, 249 }, { 0, 493 }, { 373, 493 }, { 747, 0 }, { 747, 249 }, { 0, 0 }, { 249, 0 }, { 498, 0 } };

                Font Basefont = Program.fonts.CreateFont("HurmeGeometricSans3W03-Blk", 18);
                Font SummitCaps15 = Program.fonts.CreateFont("HurmeGeometricSans3W03-Blk", 15);
                Font SummitCaps12 = Program.fonts.CreateFont("HurmeGeometricSans3W03-Blk", 12.5f);

                var AllignCenter = new TextGraphicsOptions(true)
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top
                };
                var AllignTopLeft = new TextGraphicsOptions(true)
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
                var AllignTopRight = new TextGraphicsOptions(true)
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top
                };

                if (Events.Points != 0)
                {
                    using Image<Rgba32> BaseImage = new Image<Rgba32>(1127, 765);
                    for (int i = 0; i < JSummit[0].Events.Length; i++)
                    {
                        var ThisEvent = JSummit[0].Events[i];
                        var Activity = Events.Activities.Where(w => w.Activity_ID.Equals(ThisEvent.ID.ToString())).ToArray();

                        using (WebClient wc = new WebClient())
                        {
                            EventLogoBit = wc.DownloadData($"https://www.thecrew-hub.com/gen/assets/summits/{ThisEvent.Img_Path}");
                        }
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
                            using WebClient wc = new WebClient();
                            string[] EventTitle = wc.DownloadString($"https://thecrew-exchange.com/api/tchub/event/{tcejson.Key}/{ThisEvent.ID}").Replace("\"", "").Split(' ');
                            Json.SummitLeaderboard leaderboard = JsonConvert.DeserializeObject<Json.SummitLeaderboard>(wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/leaderboard/{UserInfo.Platform}/{ThisEvent.ID}"));
                            StringBuilder sb = new StringBuilder();
                            for (int j = 0; j < EventTitle.Length; j++)
                            {
                                if (j == 3)
                                {
                                    sb.AppendLine();
                                }
                                sb.Append(EventTitle[j] + " ");
                            }
                            using (Image<Rgba32> TitleBar = new Image<Rgba32>(EventImage.Width, 40))
                            using (Image<Rgba32> ScoreBar = new Image<Rgba32>(EventImage.Width, 40))
                            {
                                ScoreBar.Mutate(ctx => ctx.Fill(Rgba32.Black));
                                TitleBar.Mutate(ctx => ctx.Fill(Rgba32.Black));
                                EventImage.Mutate(ctx => ctx
                                .DrawImage(ScoreBar, new Point(0, EventImage.Height - ScoreBar.Height), 0.7f)
                                .DrawImage(TitleBar, new Point(0, 0), 0.7f)
                                .DrawText(AllignTopLeft, sb.ToString(), SummitCaps15, Rgba32.White, new PointF(5, 0))
                                .DrawText(AllignTopLeft, $"Rank: {Activity[0].Rank + 1}", Basefont, Rgba32.White, new PointF(5, EventImage.Height - 22))
                                .DrawText(AllignTopRight, $"{(leaderboard.Score_Format == "time" ? $"Time: {CustomMethod.ScoreToTime(Activity[0].Score)}" : $"Score: {Activity[0].Score}")}", Basefont, Rgba32.White, new PointF(EventImage.Width - 5, EventImage.Height - 42))
                                .DrawText(AllignTopRight, $"Points: {Activity[0].Points}", Basefont, Rgba32.White, new PointF(EventImage.Width - 5, EventImage.Height - 22))
                                );
                            }
                            BaseImage.Mutate(ctx => ctx
                            .DrawImage(EventImage, new Point(WidthHeight[i, 0], WidthHeight[i, 1]), 1)
                            );
                        }
                        else
                        {
                            using Image<Rgba32> NotComplete = new Image<Rgba32>(EventImage.Width, EventImage.Height);
                            NotComplete.Mutate(ctx => ctx
                                .Fill(Rgba32.Black)
                                .DrawText(AllignCenter, "Event not completed!", Basefont, Rgba32.White, new PointF(NotComplete.Width / 2, NotComplete.Height / 2))
                                );
                            BaseImage.Mutate(ctx => ctx
                            .DrawImage(EventImage, new Point(WidthHeight[i, 0], WidthHeight[i, 1]), 1)
                            .DrawImage(NotComplete, new Point(WidthHeight[i, 0], WidthHeight[i, 1]), 0.8f)
                            );
                        }
                    }
                    using (Image<Rgba32> TierBar = Image.Load<Rgba32>("Summit/TierBar.png"))
                    {
                        TierBar.Mutate(ctx => ctx.DrawImage(new Image<Rgba32>(new Configuration(), TierBar.Width, TierBar.Height, backgroundColor: Rgba32.Black), new Point(0, 0), 0.35f));
                        int[] TierXPos = new int[4] { 845, 563, 281, 0 };
                        bool[] Tier = new bool[] { false, false, false, false };
                        for (int i = 0; i < Events.Tier_entries.Length; i++)
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
                                .DrawText(AllignTopLeft, $"Points Needed: {Events.Tier_entries[i].Points.ToString()}", SummitCaps12, Rgba32.White, new PointF(TierXPos[i] + 5, 15))
                                );
                            }
                        }

                        TierBar.Mutate(ctx => ctx
                                .DrawText(AllignTopLeft, $"Summit Rank: {Events.UserRank + 1} Score: {Events.Points}", SummitCaps15, Rgba32.White, new PointF(TierXPos[Tier.Count(c => c) - 1] + 5, 0))
                                );

                        BaseImage.Mutate(ctx => ctx
                        .DrawImage(TierBar, new Point(0, BaseImage.Height - 30), 1)
                        );
                    }

                    TimeSpan timeleft = endtime - DateTime.Now.ToUniversalTime();
                    BaseImage.Save("Summit/MySummitUpload.png");
                    OutMessage = $"{ctx.Member.Mention}, Here are your summit event stats for {(search == "x1" ? "Xbox" : search == "ps4" ? "PlayStation" : "PC")}.\n*Ends in {timeleft.Days} days, {timeleft.Hours} hours, {timeleft.Minutes} minutes. Scoreboard powered by The Crew Hub and The Crew Exchange!*";
                    SendImage = true;
                }
                else
                {
                    OutMessage = $"{ctx.Member.Mention}, You have not completed any summit event!";
                }
            }
            if (SendImage)
            {
                using FileStream upFile = File.Open("Summit/MySummitUpload.png", FileMode.Open);
                await ctx.RespondWithFileAsync(upFile, OutMessage);
            }
            else
            {
                await ctx.RespondAsync(OutMessage);
            }
        }

        [Command("daily")]
        [Cooldown(1, 60, CooldownBucketType.User)]
        [Description("Gives 200 bucks to yourself, or 200-400 if you give someone else.")]
        public async Task Daily(CommandContext ctx, DiscordMember member = null)
        {
            int money = 200;
            if (member == null)
            {
                member = ctx.Member;
            }
            var user = DB.DBLists.Leaderboard.FirstOrDefault(f => f.ID_User.Equals(ctx.Member.Id.ToString()));
            var reciever = DB.DBLists.Leaderboard.FirstOrDefault(f => f.ID_User.Equals(member.Id.ToString()));
            DateTime? dailyused = null;
            if (user.Daily_Used != null)
            {
                dailyused = DateTime.ParseExact(user.Daily_Used, "ddMMyyyy", CultureInfo.InvariantCulture);
            }
            if (dailyused == null || dailyused < DateTime.Now.Date)
            {
                if (member.Id == ctx.Member.Id)
                {
                    user.Daily_Used = DateTime.Now.ToString("ddMMyyyy");
                    user.Bucks += money;
                    DB.DBLists.UpdateLeaderboard(new List<DB.Leaderboard> { user });
                    await ctx.RespondAsync($"{ctx.Member.Mention}, You have received {money} bucks");
                }
                else
                {
                    Random r = new Random();
                    money += r.Next(200);
                    user.Daily_Used = DateTime.Now.ToString("ddMMyyyy");
                    reciever.Bucks += money;
                    DB.DBLists.UpdateLeaderboard(new List<DB.Leaderboard> { user });
                    DB.DBLists.UpdateLeaderboard(new List<DB.Leaderboard> { reciever });
                    await ctx.RespondAsync($"{member.Mention}, You were given {money} bucks by {ctx.Member.Username}");
                }
            }
            else
            {
                DateTime now = DateTime.Now;

                await ctx.RespondAsync($"Time untill you can use daily {(24 - now.Hour) - 1}:{(60 - now.Minute) - 1}:{(60 - now.Second) - 1}.");
            }
        }

        [Command("cookie")]
        [Priority(10)]
        [Description("Gives a cookie to someone.")]
        public async Task Cookie(CommandContext ctx, DiscordMember member)
        {
            string output = "";
            if (ctx.Member == member)
            {
                output = $"{ctx.Member.Mention}, you can't give a cookie to yourself";
            }
            else
            {
                var giver = DB.DBLists.Leaderboard.FirstOrDefault(f => f.ID_User.Equals(ctx.Member.Id.ToString()));
                var reciever = DB.DBLists.Leaderboard.FirstOrDefault(f => f.ID_User.Equals(member.Id.ToString()));
                DateTime? dailyused = null;
                if (giver.Cookies_Used != null)
                {
                    dailyused = DateTime.ParseExact(giver.Cookies_Used, "ddMMyyyy", CultureInfo.InvariantCulture);
                }
                if (dailyused == null || dailyused < DateTime.Now.Date)
                {
                    giver.Cookies_Used = DateTime.Now.ToString("ddMMyyyy");
                    giver.Cookies_Given += 1;
                    reciever.Cookies_Taken += 1;
                    output = $"{member.Mention}, {ctx.Member.Username} has given you a :cookie:";
                    DB.DBLists.UpdateLeaderboard(new List<DB.Leaderboard> { giver });
                    DB.DBLists.UpdateLeaderboard(new List<DB.Leaderboard> { reciever });
                }
                else
                {
                    DateTime now = DateTime.Now;

                    output = $"Time untill you can use cookie command again - {(24 - now.Hour) - 1}:{(60 - now.Minute) - 1}:{(60 - now.Second) - 1}.";
                }
            }
            await ctx.RespondAsync(output);
        }

        [Command("cookie")]
        [Priority(9)]
        [Description("See your cookie stats")]
        public async Task Cookie(CommandContext ctx)
        {
            var user = DB.DBLists.Leaderboard.FirstOrDefault(f => f.ID_User.Equals(ctx.Member.Id.ToString()));
            bool cookiecheck = false;
            DateTime? dailyused = null;
            if (user.Cookies_Used != null)
            {
                dailyused = DateTime.ParseExact(user.Cookies_Used, "ddMMyyyy", CultureInfo.InvariantCulture);
            }
            if (dailyused == null || dailyused < DateTime.Now.Date)
            {
                cookiecheck = true;
            }
            await ctx.RespondAsync($"{ctx.Member.Mention} you have given out {user.Cookies_Given}, and received {user.Cookies_Taken} :cookie:\n" +
                $"Can give cookie? {(cookiecheck ? "Yes" : "No")}.");
        }
    }
}