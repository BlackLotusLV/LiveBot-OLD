using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
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
        public async Task Bot(CommandContext ctx)
        {
            DateTime current = DateTime.Now;
            TimeSpan time = current - Program.start;
            string changelog = "Bucks nerfs\n" +
                "New command, `/daily` - can be used once a day, gives you or someone else 200 bucks";
            string description = "LiveBot is a discord bot created for The Crew Community and used on few other discord servers as a stream announcement bot. " +
                "It allows people to select their role by simply clicking on a reaction on the designated messages and offers many tools for moderators to help people faster and to keep order in the server.";
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
            embed.AddField("LiveBot info", description);
            embed.AddField("Patreon:", "You can support the development of Live Bot and The Crew Community Discord here: https://www.patreon.com/BlackLotusLV");
            embed.AddField("Change log:", changelog);
            await ctx.Message.RespondAsync(embed: embed);
        }

        [Command("getemote")]
        public async Task GetEmote(CommandContext ctx, DiscordEmoji emote)
        {
            await ctx.RespondAsync(emote.Id.ToString());
        }

        [Command("say"), Description("Bot repeats whatever you tell it to repeat"), RequireRoles(RoleCheckMode.Any, "BotCMD1")] //word repeat command
        public async Task Say(CommandContext ctx, DiscordChannel channel = null, [Description("bot will repeat this")] params string[] word)
        {
            if (channel == null)
            {
                channel = ctx.Channel;
            }
            await ctx.Message.DeleteAsync();
            string f = CustomMethod.ParamsStringConverter(word);
            await channel.SendMessageAsync(f);
        }

        [Command("ping")]
        [Description("Shows that the bot is live")]
        [Aliases("pong")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync($"Pong! {ctx.Client.Ping}ms");
        }

        [Command("share"), Description("Informs the user about the content sharing channels and how to get the share role")] // photomode info command
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

        [Command("platform"), Description("Informs the user about the platform roles.")] //platform selection command
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

        [Command("maxlvl"), Description("Explains how to get maximum car level in The Crew 1.")] // how to get max level for cars in TC1
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

        [Command("magic"), RequireRoles(RoleCheckMode.Any, "TCE Patreon")] //tce patreon special command :D
        public async Task Magic(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync();
            await ctx.Message.RespondAsync($"{ctx.Member.Mention} loves TheCrew-Exhange and is a wonderful person because of it.");
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
        [Command("info"), Description("Shows user info")]
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
        public async Task Convert(CommandContext ctx, double value, string mesurement)
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
        [RequireRoles(RoleCheckMode.Any, "Tier 2", "BotCMD1")]
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

        [Command("getkicks")]
        [Description("Informs user of how many times they have been warned, kicked, banned and the reasons of warnings if there are any active warnings.")]
        public async Task GetKicks(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync();
            List<DB.UserWarnings> UWarnings = DB.DBLists.UserWarnings;
            List<DB.Warnings> Warn = DB.DBLists.Warnings;
            string uid = ctx.User.Id.ToString();
            bool UserCheck = false;
            int kcount = 0, bcount = 0, wlevel = 0, wcount = 0;
            string reason = "";
            var UserWarnings = (from uw in UWarnings
                                where uw.ID_User == ctx.User.Id.ToString()
                                select uw).ToList();
            var WarningList = (from w in Warn
                               where w.User_ID == ctx.User.Id.ToString()
                               select w).ToList();
            if (UserWarnings.Count == 1)
            {
                UserCheck = true;
                kcount = (int)UserWarnings[0].Kick_Count;
                bcount = (int)UserWarnings[0].Ban_Count;
                wcount = (int)UserWarnings[0].Warning_Count;
                wlevel = (int)UserWarnings[0].Warning_Level;
                foreach (var warning in WarningList)
                {
                    if (warning.Active == true)
                    {
                        reason += $"By: <@{warning.Admin_ID}>\t Reason: {warning.Reason}\n";
                    }
                }
            }
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(0xFF6600),
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = ctx.Member.Username,
                    IconUrl = ctx.Member.AvatarUrl
                },
                Description = $"",
                Title = "User kick Count",
                ThumbnailUrl = ctx.Member.AvatarUrl
            };
            embed.AddField("Warning level: ", $"{wlevel}", true);
            embed.AddField("Times warned: ", $"{wcount}", true);
            embed.AddField("Times banned: ", $"{bcount}", true);
            embed.AddField("Times kicked: ", $"{kcount}", true);
            embed.AddField("Warning reasons: ", $"-{reason}-", true);
            if (!UserCheck)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, This user has no warning, kick and/or ban history.");
            }
            else
            {
                await ctx.RespondAsync(embed: embed);
            }
        }

        [Command("rvehicle")]
        [Aliases("rv")]
        [Description("Gives a random vehicle from a discipline, if discipline allows both bikes and cars, it will give two vehicles")]
        public async Task RVehicle(CommandContext ctx, DiscordEmoji discipline = null)
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
            List<DB.DisciplineList> DisciplineList = DB.DBLists.DisciplineList.Where(w=>w.Discipline_Name==disciplinename).ToList();
            Random r = new Random();
            string output;
            bool check = true;
            int row=0;
            if (disciplinename == "Street Race")
            {
                var CarList = (from vl in VehicleList
                               join dl in DisciplineList on vl.Discipline equals dl.ID_Discipline
                               where dl.Discipline_Name == disciplinename
                               where vl.Type == "car"
                               select vl).ToList();
                var BikeList = (from vl in VehicleList
                                join dl in DisciplineList on vl.Discipline equals dl.ID_Discipline
                                where dl.Discipline_Name == disciplinename
                                where vl.Type == "bike"
                                select vl).ToList();
                
                if (Program.VehicleList.Where(w=>w.Discipline==DisciplineList[0].ID_Discipline).Count()>CarList.Count/2)
                {
                    Program.VehicleList.RemoveAll(r => r.Discipline == DisciplineList[0].ID_Discipline);
                }
                while (check)
                {
                    row = r.Next(CarList.Count);
                    if (!Program.VehicleList.Contains(CarList[row]))
                    {
                        check = false;
                        Program.VehicleList.Add(CarList[row]);
                    }
                }
                output = $"**Car:** {CarList[row].Brand} | {CarList[row].Model} | {CarList[row].Year}\n";
                row = r.Next(BikeList.Count);
                output += $"**Bike:** {BikeList[row].Brand} | {BikeList[row].Model} | {BikeList[row].Year}";
            }
            else
            {
                var Vlist = (from vl in VehicleList
                                      join dl in DisciplineList on vl.Discipline equals dl.ID_Discipline
                                      where dl.Discipline_Name == disciplinename
                                      select vl).ToList();
                if (Program.VehicleList.Where(w => w.Discipline == DisciplineList[0].ID_Discipline).Count() > Vlist.Count / 2)
                {
                    Program.VehicleList.RemoveAll(r => r.Discipline == DisciplineList[0].ID_Discipline);
                }
                do
                {
                    row = r.Next(Vlist.Count);
                    if (!Program.VehicleList.Contains(Vlist[row]))
                    {
                        check = false;
                        Program.VehicleList.Add(Vlist[row]);
                    }
                } while (check);
                output = $"{Vlist[row].Brand} | {Vlist[row].Model} | {Vlist[row].Year} ({Vlist[row].Type})";
            }
            await ctx.RespondAsync(output);
        }

        [Command("rank")]
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
        [Description("User profile command")]
        [Priority(10)]
        public async Task Profile(CommandContext ctx,
            [Description("Specify which user to show, if left empty, will take command caster")]DiscordMember user = null)
        {
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

            string username = user.Username;
            string level = global[0].Level.ToString();
            string followers = $"{global[0].Followers.ToString()}/{(int)global[0].Level * (300 * ((int)global[0].Level + 1) * 0.5)}";
            string bucks = global[0].Bucks.ToString();
            string bio = UserSettings[0].us.User_Info.ToString();
            Rgba32 bordercolour = CustomMethod.GetColour(UserSettings[0].us.Border_Colour.ToString());
            Rgba32 textcolour = CustomMethod.GetColour(UserSettings[0].us.Text_Colour.ToString());
            Rgba32 backfieldcolour = CustomMethod.GetColour(UserSettings[0].us.Background_Colour.ToString());

            using Image<Rgba32> pfp = Image.Load<Rgba32>(profilepic);
            using Image<Rgba32> picture = new Image<Rgba32>(600, 600);
            using Image<Rgba32> background = new Image<Rgba32>(560, 360);
            using Image<Rgba32> bg = Image.Load<Rgba32>(baseBG);
            Font Basefont = SystemFonts.CreateFont("Consolas", 21, FontStyle.Bold);
            Font LevelText = SystemFonts.CreateFont("Consolas", 30, FontStyle.Regular);
            Font LevelNumber = SystemFonts.CreateFont("Consolas", 50, FontStyle.Regular);
            Font InfoTextFont = SystemFonts.CreateFont("Consolas", 19, FontStyle.Regular);
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
                .DrawText(username, Basefont, textcolour, new PointF(200, 230)) // username
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
                    if (role.Id== 585537338491404290)
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

        [Command("profile")] // needs to be tested
        [Priority(9)]
        public async Task Profile(CommandContext ctx, [Description("profile settings command input, write help for info")] params string[] input)
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
            string list = "", personalscore = "";
            List<DB.Leaderboard> leaderboard = DB.DBLists.Leaderboard;
            var user = leaderboard.OrderByDescending(x => x.Followers).ToList();
            for (int i = (int)(page * 10) - 10; i < page * 10; i++)
            {
                var duser = ctx.Client.GetUserAsync(System.Convert.ToUInt64(user[i].ID_User.ToString()));
                list += $"[{i + 1}]\t# {duser.Result.Username}\n\t\t\tFollowers:{user[i].Followers}\tLevel:{user[i].Level}\n";
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
            await ctx.RespondAsync("```csharp\n" +
                "📋 Rank | Username\n" +
                $"{list}\n" +
                $"# Your Global Ranking\n" +
                $"{personalscore}" +
                "```");
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
            string list = "", personalscore = "";
            List<DB.ServerRanks> leaderboard = DB.DBLists.ServerRanks;
            var user = leaderboard.OrderByDescending(x => x.Followers).ToList();
            user = user.Where(w=>w.Server_ID==ctx.Guild.Id.ToString()).ToList();
            for (int i = (int)(page * 10) - 10; i < page * 10; i++)
            {
                var duser = ctx.Client.GetUserAsync(System.Convert.ToUInt64(user[i].User_ID.ToString()));
                list += $"[{i + 1}]\t# {duser.Result.Username}\n\t\t\tFollowers:{user[i].Followers}\n";
                if (i == user.Count - 1)
                {
                    i = (int)page * 10;
                }
            }
            int rank = 0;
            List<DB.ServerRanks> LB = DB.DBLists.ServerRanks;
            List<DB.ServerRanks> leaderbaords = LB.OrderByDescending(x => x.Followers).ToList();
            leaderbaords=leaderbaords.Where(w => w.Server_ID == ctx.Guild.Id.ToString()).ToList();
            foreach (var item in leaderbaords)
            {
                rank++;
                if (item.User_ID == ctx.User.Id.ToString())
                {
                    personalscore = $"⭐Rank: {rank}\t Followers: {item.Followers}\t";
                    break;
                }
            }
            await ctx.RespondAsync("```csharp\n" +
                "📋 Rank | Username\n" +
                $"{list}\n" +
                $"# Your Server Ranking\n" +
                $"{personalscore}" +
                "```");
        }

        [Command("background")]
        [Aliases("bg")]
        public async Task Background(CommandContext ctx, int? page = null)
        {
            if (page == null || page <= 0)
            {
                page = 1;
            }
            List<DB.UserImages> userImages = DB.DBLists.UserImages;
            List<DB.BackgroundImage> Backgrounds = DB.DBLists.BackgroundImage;
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
            await ctx.RespondAsync(sb.ToString());
        }

        [Command("buy")]
        public async Task Buy(CommandContext ctx, string what, int id)
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
                            user[0].Bucks = (long)user[0].Bucks - (long)background[0].Price;
                            DB.UserImages newEntry = new DB.UserImages
                            {
                                User_ID = ctx.User.Id.ToString(),
                                BG_ID = id
                            };
                            DB.DBLists.InsertUserImages(newEntry);
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
        [Cooldown(1, 60, CooldownBucketType.User)]
        public async Task Summit(CommandContext ctx, string platform = "pc")
        {
            switch (platform.ToLower())
            {
                case "xbox":
                case "xb1":
                case "xb":
                case "xbox1":
                case "x1":
                case "x":
                    platform = "x1";
                    break;

                case "ps4":
                case "ps":
                case "playstation":
                case "playstation4":
                    platform = "ps4";
                    break;

                case "pc":
                case null:
                default:
                    platform = "pc";
                    break;
            }
            string EventJsonString = "";
            byte[] SummitLogo;
            using (WebClient wc = new WebClient())
            {
                string JSummitString = wc.DownloadString("https://api.thecrew-hub.com/v1/data/summit");
                List<Json.Summit> JSummit = JsonConvert.DeserializeObject<List<Json.Summit>>(JSummitString);
                EventJsonString = wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/{platform}/profile/a92d844e-9c57-4b8c-a249-108ef42d4500");
                SummitLogo = wc.DownloadData($"https://www.thecrew-hub.com/gen/assets/summits/{JSummit[0].Cover_Small}");
            }
            Json.Rank JEvent = JsonConvert.DeserializeObject<Json.Rank>(EventJsonString);

            string[] pts = new string[4];
            int i = 0;
            foreach (var item in JEvent.Tier_entries)
            {
                if (item.Points == 4294967295)
                {
                    pts[i] = "";
                }
                else
                {
                    pts[i] = item.Points.ToString();
                }
                i++;
            }
            using (Image<Rgba32> BaseImg = new Image<Rgba32>(300, 643))
            using (Image<Rgba32> TierImg = Image.Load<Rgba32>("SummitBase.png"))
            using (Image<Rgba32> SummitImg = Image.Load<Rgba32>(SummitLogo))
            using (Image<Rgba32> FooterImg = new Image<Rgba32>(300, 30))
            {
                Rgba32 TextColour = Rgba32.WhiteSmoke;
                Point SummitLocation = new Point(0, 0);
                Font Basefont = SystemFonts.CreateFont("Consolas", 30, FontStyle.Bold);
                Font FooterFont = SystemFonts.CreateFont("Consolas", 15, FontStyle.Regular);
                FooterImg.Mutate(ctx => ctx
                .Fill(Rgba32.Black)
                .DrawText($"TOTAL PARTICIPANTS: {JEvent.Player_Count}", FooterFont, TextColour, new PointF(10, 10))
                );
                BaseImg.Mutate(ctx => ctx
                    .DrawImage(SummitImg, SummitLocation, 1)
                    .DrawImage(TierImg, SummitLocation, 1)
                    .DrawText(pts[3], Basefont, TextColour, new PointF(80, 370))
                    .DrawText(pts[2], Basefont, TextColour, new PointF(80, 440))
                    .DrawText(pts[1], Basefont, TextColour, new PointF(80, 510))
                    .DrawText(pts[0], Basefont, TextColour, new PointF(80, 580))
                    .DrawImage(FooterImg, new Point(0, 613), 1)
                    );
                BaseImg.Save("SummitUpload.png");
            }
            using FileStream upFile = File.Open("SummitUpload.png", FileMode.Open);
            string output = platform == "x1" ? "XBox One." : platform == "ps4" ? "Play Station 4." : platform == "pc" ? "PC" : "*error*";
            await ctx.RespondWithFileAsync(upFile, $"Summit tier list for {output}");
        }
        [Command("daily")]
        [Cooldown(1,60,CooldownBucketType.User)]
        public async Task Daily(CommandContext ctx,DiscordMember member = null)
        {
            if (member==null)
            {
                member = ctx.Member;
            }
            List<DB.Leaderboard> dbase = DB.DBLists.Leaderboard;
            var user = (from db in dbase
                         where db.ID_User == ctx.Member.Id.ToString()
                         select db).ToList();
            var receiver = (from db in dbase
                            where db.ID_User == member.Id.ToString()
                            select db).ToList();
            DateTime? dailyused=null;
            if (user[0].Daily_Used!=null)
            {
                dailyused = DateTime.ParseExact(user[0].Daily_Used, "ddMMyyyy", CultureInfo.InvariantCulture);
            }
            if (dailyused==null||dailyused>DateTime.Now)
            {
                if (member.Id==ctx.Member.Id)
                {
                    user[0].Daily_Used = DateTime.Now.ToString("ddMMyyyy");
                    user[0].Bucks += 200;
                    DB.DBLists.UpdateLeaderboard(user);
                    await ctx.RespondAsync($"{ctx.Member.Mention}, You have received 200 bucks");
                }
                else
                {
                    user[0].Daily_Used = DateTime.Now.ToString("ddMMyyyy");
                    receiver[0].Bucks += 200;
                    DB.DBLists.UpdateLeaderboard(user);
                    DB.DBLists.UpdateLeaderboard(receiver);
                    await ctx.RespondAsync($"{member.Mention}, You were given 200 bucks by {ctx.Member.Username}");
                }
            }
            else
            {
                DateTime now = DateTime.Now;

                await ctx.RespondAsync($"Time untill you can use daily {(24 - now.Hour) - 1}:{(60 - now.Minute) - 1}:{(60 - now.Second) - 1}.");
            }
        }
    }
}