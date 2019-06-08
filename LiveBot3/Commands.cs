using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
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
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiveBot
{
    public class UngroupedCommands : BaseCommandModule
    {
        [Command("bot")]//list of Live bot changes
        public async Task Bot(CommandContext ctx)
        {
            DateTime current = DateTime.Now;
            TimeSpan time = current - Program.start;
            string changelog = "Added `/faq` command (moderator use)\n" +
                "";
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
        public async Task Quote(CommandContext ctx, [Description("message ID")] string ChannelMsg)
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

        [Command("quote")]
        [Description("Quotes a message using its ID")]
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
            DataBase.UpdateTables();
            DataTable User_warnings = DataBase.User_warnings;
            DataTable Warnings = DataBase.Warnings;
            string uid = ctx.User.Id.ToString();
            bool UserCheck = false;
            int kcount = 0, bcount = 0, wlevel = 0, wcount = 0;
            string reason = "";
            foreach (DataRow row in User_warnings.Rows)
            {
                if (row["id_user"].ToString() == uid)
                {
                    UserCheck = true;
                    kcount = (int)row["kick_count"];
                    bcount = (int)row["ban_count"];
                    wcount = (int)row["warning_count"];
                    wlevel = (int)row["warning_level"];
                    foreach (DataRow item in Warnings.Rows)
                    {
                        if (item["user_id"].ToString() == row["id_user"].ToString())
                        {
                            if ((bool)item["active"] == true)
                            {
                                reason += $"ID: {item["id_warning"].ToString()}\t By: <@{item["admin_id"].ToString()}>\t Reason: {item["reason"].ToString()}\n";
                            }
                        }
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
            string disciplinename = (discipline.Id) switch
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
                _ => "Street Race",
            };
            Random r = new Random();
            string output;
            if (disciplinename == "Street Race")
            {
                DataRow[] carlist = DataBase.vehicle_list.Select($"discipline_name='Street Race' and type='car'");
                DataRow[] bikelist = DataBase.vehicle_list.Select($"discipline_name='Street Race' and type='bike'");

                int row = r.Next(0, carlist.Length);
                output = $"**Car:** {carlist[row]["brand"]} {carlist[row]["model"]} {carlist[row]["year"]}\n";
                row = r.Next(0, bikelist.Length);
                output += $"**Bike:** {bikelist[row]["brand"]} {bikelist[row]["model"]} {bikelist[row]["year"]}";
            }
            else if (discipline == null)
            {
                DataRow[] disciplinelist = DataBase.vehicle_list.Select();
                int row = r.Next(0, disciplinelist.Length);
                output = $"{disciplinelist[row]["discipline_name"]} - {disciplinelist[row]["model"]} {disciplinelist[row]["year"]}";
            }
            else
            {
                DataRow[] disciplinelist = DataBase.vehicle_list.Select($"discipline_name='{disciplinename}'");
                int row = r.Next(0, disciplinelist.Length);
                output = $"{disciplinelist[row]["brand"]} {disciplinelist[row]["model"]} {disciplinelist[row]["year"]}";
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
            string rank = "";
            NpgsqlConnection conn = DataBase.DBConnection();
            conn.Open();
            NpgsqlCommand selectrank = new NpgsqlCommand($"with r as (select *,row_number() over(order by followers desc) as rank from server_ranks where server_id='{ctx.Channel.Guild.Id}') select * from r where user_id='{user.Id}'", conn);
            using (NpgsqlDataReader dr = selectrank.ExecuteReader())
            {
                while (dr.Read())
                {
                    rank = $"{user.Username}'s rank: {dr["rank"]}\tFollowers: {dr["followers"]}\t";
                }
            }
            conn.Close();
            await ctx.RespondAsync(rank);
        }

        [Command("globalrank")]
        [Aliases("grank")]
        public async Task GlobalRank(CommandContext ctx, DiscordMember user = null)
        {
            if (user == null)
            {
                user = ctx.Member;
            }
            string rank = "";
            NpgsqlConnection conn = DataBase.DBConnection();
            conn.Open();
            NpgsqlCommand selectrank = new NpgsqlCommand($"with r as (select *,row_number() over(order by followers desc) as rank from leaderboard) select * from r where id_user='{user.Id}'", conn);
            using (NpgsqlDataReader dr = selectrank.ExecuteReader())
            {
                while (dr.Read())
                {
                    rank = $"{user.Username}'s rank: {dr["rank"]}\tFollowers: {dr["followers"]}\tLevel: {dr["level"]}";
                }
            }
            conn.Close();
            await ctx.RespondAsync(rank);
        }

        [Command("profile")]
        [Description("User profile command")]
        public async Task Profile(CommandContext ctx,
            [Description("Specify which user to show, if left empty, will take command caster")]DiscordMember user = null)
        {
            string json = "";
            using (var fs = File.OpenRead("TCECFG.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();
            TCEJson tcejson = JsonConvert.DeserializeObject<TCEJson>(json);
            bool tcelink = false;
            string readoutput = "";
            if (user == null)
            {
                user = ctx.Member;
            }
            string link = $"https://thecrew-exchange.com/api/bot/isAccountLinked/{tcejson.Key}/{user.Id}";
            HttpWebRequest r = (HttpWebRequest)WebRequest.Create(link);
            r.AutomaticDecompression = DecompressionMethods.GZip;
            using (HttpWebResponse response = (HttpWebResponse)r.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                readoutput = reader.ReadToEnd();
                if (readoutput == "True")
                {
                    tcelink = true;
                }
            }

            DataRow[] global = DataBase.Leaderboard.Select($"id_user='{user.Id}'");

            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
            NpgsqlCommand GetUserSettings = new NpgsqlCommand($"select us.user_id as uid, background_colour, text_colour, border_colour, user_info, bi.image from user_settings us inner join user_images ui on us.image_id = ui.id_user_images and us.user_id = ui.user_id inner join background_image bi on ui.bg_id = bi.id_bg where us.user_id = '{user.Id}'", DataBase.DBConnection());
            adapter.SelectCommand = GetUserSettings;
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            byte[] baseBG = (byte[])dt.Rows[0]["image"];
            var webclinet = new WebClient();
            byte[] profilepic = webclinet.DownloadData(user.AvatarUrl);

            string username = user.Username;
            string level = global[0]["level"].ToString();
            string followers = $"{global[0]["followers"].ToString()}/{(int)global[0]["level"] * (300 * ((int)global[0]["level"] + 1) * 0.5)}";
            string bucks = global[0]["bucks"].ToString();
            string bio = dt.Rows[0]["user_info"].ToString();
            Rgba32 bordercolour = CustomMethod.GetColour(dt.Rows[0]["border_colour"].ToString());
            Rgba32 textcolour = CustomMethod.GetColour(dt.Rows[0]["text_colour"].ToString());
            Rgba32 backfieldcolour = CustomMethod.GetColour(dt.Rows[0]["background_colour"].ToString());

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
                }
            }

            picture.Save("bg.png");
            using FileStream upFile = File.Open("bg.png", FileMode.Open);
            await ctx.RespondWithFileAsync(upFile);
        }

        [Command("profile")] // needs to be tested
        public async Task Profile(CommandContext ctx, [Description("profile settings command input, write help for info")] params string[] input)
        {
            string output = "";
            if (input[0] == "update" && input.Length > 1)
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
                NpgsqlCommand GetUserSettings = new NpgsqlCommand($"select * from user_settings where user_id='{ctx.User.Id}'", DataBase.DBConnection());
                adapter.SelectCommand = GetUserSettings;
                NpgsqlCommand SetUserSettings = new NpgsqlCommand($"update user_settings set image_id=@iid,background_colour=@bgc, text_colour=@txtc, border_colour=@bc, user_info=@ui where user_id='{ctx.User.Id}'", DataBase.DBConnection());
                SetUserSettings.Parameters.Add(new NpgsqlParameter("@iid", NpgsqlDbType.Integer, sizeof(int), "image_id"));
                SetUserSettings.Parameters.Add(new NpgsqlParameter("@bgc", NpgsqlDbType.Text, 500, "background_colour"));
                SetUserSettings.Parameters.Add(new NpgsqlParameter("@txtc", NpgsqlDbType.Text, 500, "text_colour"));
                SetUserSettings.Parameters.Add(new NpgsqlParameter("@bc", NpgsqlDbType.Text, 500, "border_colour"));
                SetUserSettings.Parameters.Add(new NpgsqlParameter("@ui", NpgsqlDbType.Text, 500, "user_info"));
                adapter.UpdateCommand = SetUserSettings;
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (input.Length > 2 && (input[1] == "bg" || input[1] == "background"))
                {
                    DataBase.UpdateUserBackground("table");
                    DataRow[] row = DataBase.User_Background.Select($"user_id='{ctx.User.Id}' and bg_id={Int32.Parse(input[2])}");
                    if (row.Length == 0)
                    {
                        output = "You do not have this background!";
                    }
                    else if (row.Length == 1)
                    {
                        dt.Rows[0]["image_id"] = row[0]["id_user_images"];
                        adapter.Update(dt);
                        output = $"You have changed your profile background";
                    }
                }
                else if (input.Length > 2 && input[1] == "info")
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 2; i < input.Length; i++)
                    {
                        /*
                        if (input[i]=="$n")
                        {
                            sb.Append("\n");
                        }
                        else
                        {
                            sb.Append(input[i] + " ");
                        }
                        */
                        sb.Append(input[i] + " ");
                    }
                    dt.Rows[0]["user_info"] = sb.ToString();
                    adapter.Update(dt);
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
        public async Task GlobalTop(CommandContext ctx, int? page = null)
        {
            NpgsqlConnection conn = DataBase.DBConnection();
            if (page == null || page <= 0)
            {
                page = 1;
            }
            string list = "", personalscore = "";
            DataRow[] user = DataBase.Leaderboard.Select("", "followers desc");
            for (int i = (int)(page * 10) - 10; i < page * 10; i++)
            {
                var duser = ctx.Client.GetUserAsync(System.Convert.ToUInt64(user[i]["id_user"].ToString()));
                list += $"[{i + 1}]\t# {duser.Result.Username}\n\t\t\tFollowers:{user[i]["followers"]}\tLevel:{user[i]["level"]}\n";
                if (i == user.Length - 1)
                {
                    i = (int)page * 10;
                }
            }
            conn.Open();
            NpgsqlCommand selectrank = new NpgsqlCommand($"with r as (select *,row_number() over(order by followers desc) as rank from leaderboard) select * from r where id_user='{ctx.User.Id}'", conn);
            using (NpgsqlDataReader dr = selectrank.ExecuteReader())
            {
                while (dr.Read())
                {
                    personalscore = $"⭐Rank: {dr["rank"]}\tFollowers: {dr["followers"]}\tLevel: {dr["level"]}";
                }
            }
            conn.Close();
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
        public async Task ServerTop(CommandContext ctx, int? page = null)
        {
            NpgsqlConnection conn = DataBase.DBConnection();
            if (page == null || page <= 0)
            {
                page = 1;
            }
            string list = "", personalscore = "";
            DataRow[] user = DataBase.Server_Leaderboard.Select($"server_id={ctx.Guild.Id.ToString()}", "followers desc");
            for (int i = (int)(page * 10) - 10; i < page * 10; i++)
            {
                var duser = ctx.Client.GetUserAsync(System.Convert.ToUInt64(user[i]["user_id"].ToString()));
                list += $"[{i + 1}]\t# {duser.Result.Username}\n\t\t\tFollowers:{user[i]["followers"]}\n";
                if (i == user.Length - 1)
                {
                    i = (int)page * 10;
                }
            }
            conn.Open();
            NpgsqlCommand selectrank = new NpgsqlCommand($"with r as (select *,row_number() over(order by followers desc) as rank from server_ranks where server_id='{ctx.Channel.Guild.Id}') select * from r where user_id='{ctx.User.Id}'", conn);
            using (NpgsqlDataReader dr = selectrank.ExecuteReader())
            {
                while (dr.Read())
                {
                    personalscore = $"⭐Rank: {dr["rank"]}\tFollowers: {dr["followers"]}\t";
                }
            }
            conn.Close();
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
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
            NpgsqlCommand GetUserSettings = new NpgsqlCommand($"select b.id_bg,b.name from background_image b inner join user_images ui on b.id_bg=ui.bg_id where ui.user_id='{ctx.User.Id}'", DataBase.DBConnection());
            adapter.SelectCommand = GetUserSettings;
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            StringBuilder sb = new StringBuilder();
            sb.Append("Visual representation of the backgrounds can be viewed here: <http://bit.ly/LiveBG>\n```csharp\n[ID]\tBackground Name\n");
            for (int i = (int)(page * 10) - 10; i < page * 10; i++)
            {
                bool check = false;
                foreach (DataRow userimage in dt.Rows)
                {
                    if ((int)DataBase.Backgrounds.Rows[i]["id_bg"] == (int)userimage["id_bg"])
                    {
                        sb.Append($"[{DataBase.Backgrounds.Rows[i]["id_bg"]}]\t# {DataBase.Backgrounds.Rows[i]["name"]}\n\t\t\t [OWNED]\n");
                        check = true;
                    }
                }
                if (check == false)
                {
                    sb.Append($"[{DataBase.Backgrounds.Rows[i]["id_bg"]}]\t# {DataBase.Backgrounds.Rows[i]["name"]}\n\t\t\t Price:{DataBase.Backgrounds.Rows[i]["price"]} Bucks\n");
                }
                if (i == DataBase.Backgrounds.Rows.Count - 1)
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
                DataRow[] backgroundrow = DataBase.User_Background.Select($"user_id='{ctx.User.Id}' and bg_id={id}");
                DataRow[] background = DataBase.Backgrounds.Select($"id_bg={id}");
                DataRow[] user = DataBase.Leaderboard.Select($"id_user='{ctx.User.Id}'");
                if (background.Length == 1)
                {
                    if (backgroundrow.Length == 0)
                    {
                        if ((long)background[0]["price"] <= (long)user[0]["bucks"])
                        {
                            user[0]["bucks"] = (long)user[0]["bucks"] - (long)background[0]["price"];
                            DataRow newrow = DataBase.User_Background.NewRow();
                            newrow["user_id"] = ctx.User.Id.ToString();
                            newrow["bg_id"] = id;
                            DataBase.User_Background.Rows.Add(newrow);
                            DataBase.UpdateLeaderboards("base");
                            DataBase.UpdateUserBackground("base");
                            output = $"You have bought the \"{background[0]["name"]}\" background.";
                        }
                        else
                        {
                            output = $"You don't have enough bucks to buy this background.";
                        }
                    }
                    else if (backgroundrow.Length == 1)
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
        [Cooldown(1,60,CooldownBucketType.User)]
        public async Task Summit(CommandContext ctx, string platform="pc")
        {
            switch (platform.ToLower())
            {
                case"xbox":
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
            string EventJsonString="";
            byte[] SummitLogo;
            using (WebClient wc=new WebClient())
            {
                string JSummitString = wc.DownloadString("https://api.thecrew-hub.com/v1/data/summit");
                var JSummit = JsonConvert.DeserializeObject<List<SummitJson>>(JSummitString);
                EventJsonString = wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].Summit_ID}/score/{platform}/profile/a92d844e-9c57-4b8c-a249-108ef42d4500");
                SummitLogo = wc.DownloadData($"https://www.thecrew-hub.com/gen/assets/summits/{JSummit[0].LinkEnd}");
            }
            var JEvent = JsonConvert.DeserializeObject<EventJson>(EventJsonString);

            string[] pts = new string[4];
            int i = 0;
            foreach (var item in JEvent.Tier_entries)
            {
                
                if (item.Points==4294967295)
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
                .DrawText($"TOTAL PARTICIPANTS: {JEvent.Player_Count}",FooterFont,TextColour,new PointF(10,10))
                );
                BaseImg.Mutate(ctx => ctx
                    .DrawImage(SummitImg, SummitLocation, 1)
                    .DrawImage(TierImg, SummitLocation, 1)
                    .DrawText(pts[3], Basefont, TextColour, new PointF(80, 370))
                    .DrawText(pts[2], Basefont, TextColour, new PointF(80, 440))
                    .DrawText(pts[1], Basefont, TextColour, new PointF(80, 510))
                    .DrawText(pts[0], Basefont, TextColour, new PointF(80, 580))
                    .DrawImage(FooterImg,new Point(0,613),1)
                    );
                BaseImg.Save("SummitUpload.png");
            }
            using FileStream upFile = File.Open("SummitUpload.png", FileMode.Open);
            string output = platform == "x1" ? "XBox One." : platform == "ps4" ? "Play Station 4." : platform == "pc" ? "PC" : "*error*";
            await ctx.RespondWithFileAsync(upFile, $"Summit tier list for {output}");
        }
    }

    [Group("@")]
    [Description("Administrative commands")]
    [Hidden]
    [RequireRoles(RoleCheckMode.Any, "BotCMD1")]
    public class BotCMD1Commands : BaseCommandModule
    {
        [Command("uptime")] //uptime command
        [Aliases("live")]
        public async Task Uptime(CommandContext ctx)
        {
            DateTime current = DateTime.Now;
            TimeSpan time = current - Program.start;
            await ctx.Message.RespondAsync($"{time.Days} Days {time.Hours}:{time.Minutes}.{time.Seconds}");
        }

        [Command("Warn")]
        public async Task Warning(CommandContext ctx, DiscordMember username, params string[] reason)
        {
            DataBase.UpdateTables();
            await ctx.Message.DeleteAsync();
            DataTable User_warnings = DataBase.User_warnings;
            DataTable Warnings = DataBase.Warnings;
            DiscordChannel modlog = ctx.Guild.GetChannel(440365270893330432);
            string f = CustomMethod.ParamsStringConverter(reason);
            string modmsg, DM;
            string uid = username.Id.ToString(), aid = ctx.User.Id.ToString();
            bool UserCheck = false, kick = false;
            int level = 1, count = 1;
            foreach (DataRow item in User_warnings.Rows)
            {
                if (item["id_user"].ToString() == uid)
                {
                    UserCheck = true;
                    level = (int)item["warning_level"] + 1;
                    count = (int)item["warning_count"] + 1;
                    if (level > 2)
                    {
                        DM = $"You have been kicked by {ctx.User.Mention} for exceeding the warning level(2). Your level: {level}\n" +
                            $"Warning reason: {f}";
                        try
                        {
                            await username.SendMessageAsync(DM);
                        }
                        catch
                        {
                            await modlog.SendMessageAsync($"{username.Mention} could not be contacted via DM. Reason not sent");
                        }
                        await username.RemoveAsync();
                        kick = true;
                    }
                    item["warning_level"] = level;
                    item["warning_count"] = count;
                }
            }
            if (!UserCheck)
            {
                DataRow NewUser = User_warnings.NewRow();
                NewUser["warning_level"] = level;
                NewUser["warning_count"] = level;
                NewUser["kick_count"] = 0;
                NewUser["ban_count"] = 0;
                NewUser["id_user"] = uid;
                User_warnings.Rows.Add(NewUser);
            }
            DataRow row = Warnings.NewRow();
            row["reason"] = f;
            row["active"] = true;
            row["date"] = DateTime.Now.ToString("yyyy-MM-dd");
            row["admin_id"] = aid;
            row["user_id"] = uid;
            Warnings.Rows.Add(row);

            modmsg = $"**Warned user:**\t{username.Mention}\n**Warning level:**\t {level}\t**Warning count:\t {count}\n**Warned by**\t{ctx.User.Username}\n**Reason:** {f}";
            DM = $"You have been warned by <@{ctx.User.Id}>.\n**Warning Reason:**\t{f}\n**Warning Level:** {level}";
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(0xf90707),
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = username.AvatarUrl,
                    Name = username.Username
                },
                Description = modmsg
            };
            if (!kick)
            {
                try
                {
                    await username.SendMessageAsync(DM);
                }
                catch
                {
                    await modlog.SendMessageAsync($":exclamation::exclamation:{username.Mention} could not be contacted via DM. Reason not sent");
                }
            }
            await Task.Delay(1000);
            DataBase.UserWarnAdapter.Update(User_warnings);
            DataBase.WarnAdapter.Update(Warnings);
            await modlog.SendMessageAsync(embed: embed);
        }

        [Command("unwarn")]
        public async Task Unwarning(CommandContext ctx, DiscordMember username)
        {
            await ctx.Message.DeleteAsync();
            DataBase.UpdateTables();
            DataTable User_warnings = DataBase.User_warnings;
            DataTable Warnings = DataBase.Warnings;
            DiscordChannel modlog = ctx.Guild.GetChannel(440365270893330432);
            string modmsg;
            string uid = username.Id.ToString(), aid = ctx.User.Id.ToString();
            bool UserCheck = false;
            int level = 0;
            foreach (DataRow item in User_warnings.Rows)
            {
                if (item["id_user"].ToString() == uid && (int)item["warning_level"] > 0)
                {
                    level = (int)item["warning_level"] - 1;
                    item["warning_level"] = level;
                    bool check = false;
                    foreach (DataRow row in Warnings.Rows)
                    {
                        if (check == false && row["user_id"].ToString() == uid && (bool)row["active"] == true)
                        {
                            row["active"] = false;
                            check = true;
                        }
                    }
                }
                else if (item["id_user"].ToString() == uid && (int)item["warning_level"] == 0)
                {
                    await ctx.RespondAsync($"{ctx.User.Mention}, this user warning level is already at 0");
                    UserCheck = true;
                }
            }
            modmsg = $"{username.Mention} has been unwarned by <@{aid}>. Warning level now {level}";
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(0xf90707),
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = username.AvatarUrl,
                    Name = username.Username
                },
                Description = modmsg
            };
            await Task.Delay(1000);
            DataBase.UserWarnAdapter.Update(User_warnings);
            DataBase.WarnAdapter.Update(Warnings);
            if (!UserCheck)
            {
                try
                {
                    await username.SendMessageAsync($"Your warning level has been lowerd to {level} by {ctx.User.Mention}");
                }
                catch
                {
                    await modlog.SendMessageAsync($":exclamation::exclamation:{username.Mention} could not be contacted via DM. Reason not sent");
                }
            }
            await modlog.SendMessageAsync(embed: embed);
        }

        [Command("getkicks")]
        public async Task GetKicks(CommandContext ctx, DiscordMember username)
        {
            await ctx.Message.DeleteAsync();
            DataBase.UpdateTables();
            DataTable User_warnings = DataBase.User_warnings;
            DataTable Warnings = DataBase.Warnings;
            string uid = username.Id.ToString();
            bool UserCheck = false;
            int kcount = 0, bcount = 0, wlevel = 0, wcount = 0;
            string reason = "";
            foreach (DataRow row in User_warnings.Rows)
            {
                if (row["id_user"].ToString() == uid)
                {
                    UserCheck = true;
                    kcount = (int)row["kick_count"];
                    bcount = (int)row["ban_count"];
                    wcount = (int)row["warning_count"];
                    wlevel = (int)row["warning_level"];
                    foreach (DataRow item in Warnings.Rows)
                    {
                        if (item["user_id"].ToString() == row["id_user"].ToString())
                        {
                            if ((bool)item["active"] == true)
                            {
                                reason += $"ID: {item["id_warning"].ToString()}\t By: <@{item["admin_id"].ToString()}>\t Reason: {item["reason"].ToString()}\n";
                            }
                        }
                    }
                }
            }
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(0xFF6600),
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = username.Username,
                    IconUrl = username.AvatarUrl
                },
                Description = $"",
                Title = "User kick Count",
                ThumbnailUrl = username.AvatarUrl
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

        [Command("vote")]
        [Description("starts a timed vote")]
        public async Task Vote(CommandContext ctx, [Description("What to vote about?")] params string[] topic)
        {
            await ctx.Message.DeleteAsync();
            string f = CustomMethod.ParamsStringConverter(topic);
            DiscordMessage msg = await ctx.Message.RespondAsync(f);
            DiscordEmoji up = DiscordEmoji.FromName(ctx.Client, ":thumbsup:");
            DiscordEmoji down = DiscordEmoji.FromName(ctx.Client, ":thumbsdown:");
            await msg.CreateReactionAsync(up);
            await Task.Delay(500);
            await msg.CreateReactionAsync(down);
        }

        [Command("poll")]
        [Description("creates a poll up to 10 choices. Delimiter \".\"")]
        public async Task Poll(CommandContext ctx, [Description("Options")]params string[] input)
        {
            //var interactivity = ctx.Client.GetInteractivity();
            //var interactivity = ctx.Client.GetInteractivityModule();
            await ctx.Message.DeleteAsync();
            string f = CustomMethod.ParamsStringConverter(input);
            char delimiter = '.';
            string[] options = f.Split(delimiter);
            string final = "";
            string[] emotename = new string[] { ":zero:", ":one:", ":two:", ":three:", ":four:", ":five:", ":six:", ":seven:", ":eight:", ":nine:" };
            int i = 0;
            foreach (var item in options)
            {
                final = $"{final}{emotename[i]} {item} \n";
                i++;
            }
            DiscordMessage msg = await ctx.Message.RespondAsync(final);
            DiscordEmoji zero = DiscordEmoji.FromName(ctx.Client, ":zero:");
            DiscordEmoji one = DiscordEmoji.FromName(ctx.Client, ":one:");
            DiscordEmoji two = DiscordEmoji.FromName(ctx.Client, ":two:");
            DiscordEmoji three = DiscordEmoji.FromName(ctx.Client, ":three:");
            DiscordEmoji four = DiscordEmoji.FromName(ctx.Client, ":four:");
            DiscordEmoji five = DiscordEmoji.FromName(ctx.Client, ":five:");
            DiscordEmoji six = DiscordEmoji.FromName(ctx.Client, ":six:");
            DiscordEmoji seven = DiscordEmoji.FromName(ctx.Client, ":seven:");
            DiscordEmoji eight = DiscordEmoji.FromName(ctx.Client, ":eight:");
            DiscordEmoji nine = DiscordEmoji.FromName(ctx.Client, ":nine:");
            DiscordEmoji[] emotes = new DiscordEmoji[] { zero, one, two, three, four, five, six, seven, eight, nine };
            for (int j = 0; j < i; j++)
            {
                await msg.CreateReactionAsync(emotes[j]);
                await Task.Delay(300);
            }
        }

        [Command("rinfo")]
        [Aliases("roleinfo")]
        [Description("give the ID of the role that was mentioned with the command")]
        public async Task RInfo(CommandContext ctx, DiscordRole role)
        {
            await ctx.RespondAsync($"**Role ID:**{role.Id}");
        }
        [Command("faq")]
        public async Task FAQ(CommandContext ctx, DiscordMessage faqMsg, string type, params string[] input)
        {
            string str1 = CustomMethod.ParamsStringConverter(input);
            string og = faqMsg.Content;
            og = og.Replace("*", string.Empty);
            og = og.Replace("\n", string.Empty);
            string[] str2 = og.Split("A: ");
            if (type.ToLower() == "q")
            {
                str2[0] = $"Q: {str1}";
            }
            else if (type.ToLower() == "a")
            {
                str2[1] = str1;
            }
            await faqMsg.ModifyAsync($"**{str2[0]}**\n *A: {str2[1].TrimEnd()}*");
            await ctx.Message.DeleteAsync();
        }
        [Command("faq")]
        public async Task FAQ(CommandContext ctx, params string[] input)
        {
            string str1 = CustomMethod.ParamsStringConverter(input);
            string[] str2 = str1.Split('|');
            await ctx.RespondAsync($"**Q: {str2[0]}**\n*A: {str2[1].TrimEnd()}*");
            await ctx.Message.DeleteAsync();
        }
        /*
        [Command("event")]
        [RequireRoles(RoleCheckMode.Any, "BotCMD1")]
        public async Task Event(CommandContext ctx, string url)
        {
            await ctx.TriggerTypingAsync();
            string newline = "\n";
            List<string> titlelist = DataBase.WebToHTML(url, @"((Weekly|Special) Event: )(\w{1,}[ ]?){1,} [|]");
            string titleone = DataBase.ParamsStringConverter(titlelist.ToArray());
            string title = Regex.Replace(titleone, @"[|]", ""); // removes | from the title
            List<string> stringlist = DataBase.WebToHTML(url, @"(<p>|<strong>|<em>){1,}(\w{1,}([ .#,-’!()]|.){0,}){1,}(<strong>|</strong>){0,}|(<br)");
            string rawstring = DataBase.ParamsStringConverter(stringlist.ToArray());
            string[] main = new string[8];
            main[0] = Regex.Replace(rawstring, @"<p>For more information.{0,}", "");//removes not needed part from photo events
            main[1] = Regex.Replace(main[0], @"</?strong>|</?b>", "**"); //html bold to markdown bold
            main[2] = Regex.Replace(main[1], @"</?em>", "*"); // italics to italics
            main[3] = Regex.Replace(main[2], @"<p>", newline); //paragraph to new line
            main[4] = Regex.Replace(main[3], @"(<a href=(.https?://(\w{0,}[-./]){0,}\w{0,}.>))|</a>", "**"); //bolds the link words and removes the link
            main[5] = Regex.Replace(main[4], @"[*][*]#TC2Weekly[*][*]", "`#TC2Weekly`");
            main[6] = Regex.Replace(main[5], @"<u>|</u>", "__");
            main[7] = Regex.Replace(main[6], @"</?sup>|</p>| <br ", "");//cleans up not used formating
            DiscordGuild guild = await Program.Client.GetGuildAsync(150283740172517376);
            DiscordRole EventsRole = guild.GetRole(486531401089417226);
            string response = $"**--------------------------------------------------------------**" +
                $"\n**{title}** {EventsRole.Mention}\n" +
                $"{main[7]}\n" +
                $"**--------------------------------------------------------------**";

            await ctx.RespondAsync(response);
            await ctx.Message.DeleteAsync();
        }

        [Command("event2")]
        [RequireRoles(RoleCheckMode.Any, "BotCMD1")]
        public async Task Event2(CommandContext ctx, string url)
        {
            int i = 0;
            Console.WriteLine(i++);
            string newline = "\n";
            List<string> titlelist = DataBase.WebToHTML(url, @"((Weekly|Special) Event: )(\w{1,}[ ]?){1,} [|]");
            string titleone = DataBase.ParamsStringConverter(titlelist.ToArray());
            string title = Regex.Replace(titleone, @"[|]", ""); // removes | from the title
            List<string> stringlist = DataBase.WebToHTML(url, @"(<td([ >="":;-]|\w){0,}|<strong>|<p>|<em>){1,}(\w{1,}([ -.#,'!()]|.){0,}){1,}(<strong>|</stron>){0,}(</td>)?");
            Console.WriteLine(i++);
            string rawstring = DataBase.ParamsStringConverter(stringlist.ToArray());
            string[] main = new string[10];
            main[0] = Regex.Replace(rawstring, @"<p>For more information.{0,}", "");//removes not needed part from photo events
            main[1] = Regex.Replace(main[0], @"</?strong>|</?b>", "**"); //html bold to markdown bold
            main[2] = Regex.Replace(main[1], @"</?em>", "*"); // italics to italics
            main[3] = Regex.Replace(main[2], @"<p>", newline); //paragraph to new line
            main[4] = Regex.Replace(main[3], @"(<a href=(.https?://(\w{0,}[-./]){0,}\w{0,}.>))|</a>", "**"); //bolds the link words and removes the link
            main[5] = Regex.Replace(main[4], @"[*][*]#TC2Weekly[*][*]", "`#TC2Weekly`");
            main[6] = Regex.Replace(main[5], @"<u>|</u>", "__");
            main[7] = Regex.Replace(main[6], @".{2}Buy Now.{2}|<t(\w|[ :="";-]){1,}>|</?sup>|</p>| <br ", "");//cleans up not used formating
            main[8] = Regex.Replace(main[7], @"</td>", "\t");//
            main[9] = Regex.Replace(main[8], @"", "");
            DiscordGuild guild = await Program.Client.GetGuildAsync(150283740172517376);
            DiscordRole EventsRole = guild.GetRole(486531401089417226);
            string response = $"**--------------------------------------------------------------**" +
                $"\n**{title}** {EventsRole.Mention}\n" +
                $"{main[9].Trim()}\n" +
                $"**--------------------------------------------------------------**";

            Console.WriteLine(i++);
            Console.WriteLine(response);
            await ctx.RespondAsync(response);
            await ctx.Message.DeleteAsync();
        }
        //*/
    }

    [Group("!")]
    [Description("Owner commands")]
    [Hidden]
    [RequireOwner]
    public class OwnerCommands : BaseCommandModule
    {
        [Command("react")]
        public async Task React(CommandContext ctx, DiscordMessage message, params DiscordEmoji[] emotes)
        {
            foreach (DiscordEmoji emote in emotes)
            {
                await message.CreateReactionAsync(emote);
                await Task.Delay(300);
            }
            await ctx.Message.DeleteAsync();
        }

        [Command("textupdate")]
        [Aliases("txtup")]
        public async Task TextUpdate(CommandContext ctx, string language, string command, params string[] text)
        {
            string location = "TextFiles/" + language.ToLower() + "/" + command.ToLower() + ".txt";
            string f = CustomMethod.ParamsStringConverter(text);
            if (File.Exists(location))
            {
                File.WriteAllText(location, f);
                await ctx.RespondAsync("Command updated");
            }
            else
            {
                await ctx.RespondAsync("Specified file locations incorrect.");
            }
        }

        [Command("refresh")]
        [Description("Updates data tables with the current date in the data base. Add info parameter if need more info")]
        public async Task Refresh(CommandContext ctx, string what = null)
        {
#pragma warning disable IDE0059 // Value assigned to symbol is never used
            string output = "";
#pragma warning restore IDE0059 // Value assigned to symbol is never used
            DiscordMessage msg;
            if (what == null)
            {
                what = "info";
            }
            switch (what)
            {
                case "roles":
                    DataBase.Reaction_Roles_DT.Clear();
                    DataBase.Reaction_Roles_Adapter.Fill(DataBase.Reaction_Roles_DT);
                    output = "Roles list updated";
                    break;

                case "vehicle":
                    DataBase.UpdateVehicleList();
                    output = "Vehicle list updated";
                    break;

                case "ranks":
                    DataBase.UpdateLeaderboards("table");
                    output = "Leaderboards have been updated";
                    break;

                case "background":
                    DataBase.UpdateBackgroundTable();
                    output = "Background table has been updated";
                    break;

                case "stream":
                    DataBase.UpdateStreamNotifications("table");
                    output = "Stream notification settings have been updated";
                    break;

                case "info":
                default:
                    output = "roles - updates role selector table\n" +
                    "vehicle - updates vehicle list table\n" +
                    "ranks - updates leaderboard tables\n" +
                    "background - udpates background tables\n" +
                    "stream - updates stream notification table";
                    break;
            }
            msg = await ctx.RespondAsync(output);
            await Task.Delay(1000);
            await msg.DeleteAsync();
        }
    }
}