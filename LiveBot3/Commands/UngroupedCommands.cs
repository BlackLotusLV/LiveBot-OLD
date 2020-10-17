using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using LiveBot.Json;
using Newtonsoft.Json;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Point = SixLabors.ImageSharp.Point;
using PointF = SixLabors.ImageSharp.PointF;

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
            string changelog = "[FIX] Mod Mail auto closer fix\n" +
                "[FIX] Mod Mail closing message for admins sent to user and vice versa.\n" +
                "[INTERNAL] Continuation of code cleanup";
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
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync($"Pong! {ctx.Client.Ping}ms");
        }

        [Command("share")]
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        [Description("Informs the user, how to get access to the share channels.")] // photomode info command
        public async Task Share(CommandContext ctx,
            [Description("Specifies the user the bot will mention, use ID or mention the user. If left blank, it will mention you.")] DiscordMember username = null,
            [Description("Specifies in what language the bot will respond. example, fr-french")] string language = null)
        {
            await ctx.Message.DeleteAsync(); //deletes command message
            if (username is null) //checks if user name is not specified
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "share", language, ctx.Member));
            }
            else // if user name specified
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "share", language, username));
            }
        }

        [Command("platform")]
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        [Description("Informs the user about the platform roles.")] //platform selection command
        public async Task Platform(CommandContext ctx, [Description("Specifies the user the bot will mention, use ID or mention the user. If left blank, it will mention you.")] DiscordMember username = null, [Description("Specifies in what language the bot will respond. example, fr-french")] string language = null)
        {
            await ctx.Message.DeleteAsync();
            if (username is null)
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "platform", language, ctx.Member));
            }
            else
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "platform", language, username));
            }
        }

        [Command("maxlvl")]
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        [Description("Explains how to get maximum car level in The Crew 1.")] // how to get max level for cars in TC1
        public async Task MaxCarlvl(CommandContext ctx, [Description("Specifies the user the bot will mention, use ID or mention the user. If left blank, it will mention you.")] DiscordMember username = null, [Description("Specifies in what language the bot will respond. example, fr-french")] string language = null)
        {
            await ctx.Message.DeleteAsync();
            if (username is null)
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "maxlvl", language, ctx.Member));
            }
            else
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "maxlvl", language, username));
            }
        }

        [Command("tce")] //The Crew Exchange info command
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        [Description("Informs the user about The Crew Exchange website.")]
        public async Task TCE(CommandContext ctx, [Description("Specifies the user the bot will mention, use ID or mention the user. If left blank, it will mention you.")] DiscordMember username = null, [Description("Specifies in what language the bot will respond. example, fr-french")] string language = null)
        {
            await ctx.Message.DeleteAsync();
            if (username is null)
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "tce", language, ctx.Member));
            }
            else
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "tce", language, username));
            }
        }

        [Command("lfc")]
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        [Description("Informs the user of using the LFC channels, or to get the platform role if they don't have it.")]
        public async Task LFC(CommandContext ctx, DiscordMember username = null)
        {
            string content = CustomMethod.GetCommandOutput(ctx, "lfc2", null, username);
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
                if ((item == pc || item == ps || item == xb) && !check)
                {
                    content = CustomMethod.GetCommandOutput(ctx, "lfc1", null, username);
                    check = true;
                }
            }
            await ctx.RespondAsync(content);
            await ctx.Message.DeleteAsync();
        }

        [Command("it")]
        [Description("Sends the IT Crowd image of \"have you tried turning it off and on again?\"")]
        public async Task IT(CommandContext ctx, DiscordMember username = null)
        {
            await ctx.Message.DeleteAsync();
            FileStream ITImage = new FileStream("Assets/ITC.jpg", FileMode.Open);
            if (username == null)
            {
                await ctx.RespondWithFileAsync(ITImage, ctx.User.Mention);
            }
            else
            {
                await ctx.RespondWithFileAsync(ITImage, username.Mention);
            }
        }

        [Command("supra")]
        [Description("Sends the supra gif.")]
        [Cooldown(1, 60, CooldownBucketType.Channel)]
        public async Task Supra(CommandContext ctx, DiscordMember username = null)
        {
            if (username is null)
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "supra", null, ctx.Member));
            }
            else
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "supra", null, username));
            }
        }

        [Command("support")]
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        [Description("Gives the link to the support page")]
        public async Task Support(CommandContext ctx, DiscordMember username = null)
        {
            await ctx.Message.DeleteAsync();
            if (username is null)
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "support", null, ctx.Member));
            }
            else
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "support", null, username));
            }
        }

        [Command("forums")]
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        [Description("Gives the link to the forum")]
        public async Task Forums(CommandContext ctx, DiscordMember username = null)
        {
            await ctx.Message.DeleteAsync();
            if (username is null)
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "forums", null, ctx.Member));
            }
            else
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "forums", null, username));
            }
        }

        [Command("prosettings")]
        [Aliases("psettings")]
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        [Description("Explains how to find pro settings")]
        public async Task ProSettings(CommandContext ctx, DiscordMember username = null)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();

            if (username is null)
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "prosettings", null, ctx.Member));
            }
            else
            {
                await ctx.RespondAsync(CustomMethod.GetCommandOutput(ctx, "prosettings", null, username));
            }
        }

        [Command("quote")]
        [Description("Quotes a message using its ID")]
        [Priority(10)]
        public async Task Quote(CommandContext ctx,
            [Description("message ID")] DiscordMessage QuotedMessage,
            [RemainingText] string YourMessage)
        {
            await ctx.Message.DeleteAsync();
            string content = $"\"{QuotedMessage.Content}\"";
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(0xFF6600)
            };
            embed.AddField($"Quoted {QuotedMessage.Author.Username}'s message:", $"{content}\n[go to message]({QuotedMessage.JumpLink})");
            if (YourMessage == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention} is quoting:", embed: embed);
            }
            else
            {
                embed.AddField($"{ctx.Member.Username} Says:", YourMessage);
                await ctx.RespondAsync($"{ctx.User.Mention} is quoting:", embed: embed);
            }
        }

        [Command("quote")]
        [Description("Cross channel message quoting. (ChannelID-MessageID)")]
        [Priority(9)]
        public async Task Quote(CommandContext ctx,
            [Description("Channel and Message ID seperated by -")] string ChannelMsg,
            [RemainingText] string YourMessage)
        {
            string[] strarr = ChannelMsg.Split("-");
            if (strarr.Length == 2)
            {
                ulong channelid = ulong.Parse(strarr[0]);
                ulong msgid = ulong.Parse(strarr[1]);
                await ctx.Message.DeleteAsync();
                DiscordChannel channel = ctx.Guild.GetChannel(channelid);
                DiscordMessage QuotedMessage = await channel.GetMessageAsync(msgid);
                string content = $"\"{QuotedMessage.Content}\"";
                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(0xFF6600)
                };
                embed.AddField($"Quoted {QuotedMessage.Author.Username}'s message:", $"{content}\n[go to message]({QuotedMessage.JumpLink})");
                if (YourMessage == null)
                {
                    await ctx.RespondAsync($"{ctx.User.Mention} is quoting:", embed: embed);
                }
                else
                {
                    embed.AddField($"{ctx.Member.Username} Says:", YourMessage);
                    await ctx.RespondAsync($"{ctx.User.Mention} is quoting:", embed: embed);
                }
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
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = user.AvatarUrl
                }
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
            [Description("Mesurment of speed from what you convert")] string mesurement)
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
            else
            {
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
            }

            List<DB.VehicleList> VehicleList = DB.DBLists.VehicleList;
            List<DB.DisciplineList> DisciplineList = DB.DBLists.DisciplineList.Where(w => w.Discipline_Name == disciplinename).ToList();
            Random r = new Random();
            int row = 0;
            List<DB.VehicleList> SelectedVehicles = new List<DB.VehicleList>();

            if (disciplinename == "Street Race")
            {
                DiscordMessage CarOrBike = await ctx.RespondAsync($"{ctx.Member.Mention} **Select vehicle type:**\n:one: - Car\n:two: - Bike");
                DiscordEmoji One = DiscordEmoji.FromName(ctx.Client, ":one:");
                DiscordEmoji Two = DiscordEmoji.FromName(ctx.Client, ":two:");

                await CarOrBike.CreateReactionAsync(One);
                await Task.Delay(300).ContinueWith(t => CarOrBike.CreateReactionAsync(Two));

                var Result = await CarOrBike.WaitForReactionAsync(ctx.User, TimeSpan.FromSeconds(30));

                if (Result.TimedOut)
                {
                    await ctx.RespondAsync($"{ctx.Member.Mention} You didn't select vehicle type in time.");
                    return;
                }
                else if (Result.Result.Emoji == One)
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

            if (SelectedVehicles.Count(c => c.IsSelected is true) == SelectedVehicles.Count)
            {
                DB.DBLists.UpdateVehicleList(SelectedVehicles.Select(s => { s.IsSelected = false; return s; }).ToArray());
            }

            SelectedVehicles = (from sv in SelectedVehicles
                                where sv.IsSelected is false
                                select sv).ToList();
            row = r.Next(SelectedVehicles.Count);

            DB.DBLists.UpdateVehicleList(SelectedVehicles.Where(w => w.ID_Vehicle.Equals(SelectedVehicles[row].ID_Vehicle)).Select(s => { s.IsSelected = true; return s; }).ToArray());

            DiscordColor embedColour = new DiscordColor();

            switch (SelectedVehicles[row].VehicleTier)
            {
                case 'S':
                    embedColour = new DiscordColor(0x00ffff);
                    break;

                case 'A':
                    embedColour = new DiscordColor(0x00ff00);
                    break;

                case 'B':
                    embedColour = new DiscordColor(0xffff00);
                    break;

                case 'C':
                    embedColour = new DiscordColor(0xff9900);
                    break;

                case 'D':
                    embedColour = new DiscordColor(0xda5b5b);
                    break;

                case 'E':
                    embedColour = new DiscordColor(0x9f4ad8);
                    break;
            }

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Color = embedColour,
                Title = $"{SelectedVehicles[row].Brand} | {SelectedVehicles[row].Model} | {SelectedVehicles[row].Year} ({SelectedVehicles[row].Type})"
            };
            embed.AddField("Brand", $"{SelectedVehicles[row].Brand}", true);
            embed.AddField("Model", $"{SelectedVehicles[row].Model}", true);
            embed.AddField("Year", $"{SelectedVehicles[row].Year}", true);
            embed.AddField("Type", $"{SelectedVehicles[row].Type}", false);
            embed.AddField("Vehicle Tier", $"{SelectedVehicles[row].VehicleTier}", true);
            embed.AddField("Crew Credits only?", $"{SelectedVehicles[row].IsCCOnly}", true);
            embed.AddField("Summit exclusive?", $"{SelectedVehicles[row].IsSummitVehicle}", true);

            await ctx.RespondAsync($"*({SelectedVehicles.Count - 1} vehicles left in current rotation)*", false, embed);
        }

        [Command("rank")]
        [Description("Displays user server rank.")]
        [RequireGuild]
        public async Task Rank(CommandContext ctx, DiscordMember user = null)
        {
            if (user == null)
            {
                user = ctx.Member;
            }
            string output = string.Empty;
            int rank = 0;
            List<DB.ServerRanks> SR = DB.DBLists.ServerRanks;
            List<DB.ServerRanks> serverRanks = SR.Where(w => w.Server_ID == ctx.Guild.Id).OrderByDescending(x => x.Followers).ToList();
            foreach (var item in serverRanks)
            {
                rank++;
                if (item.User_ID == user.Id)
                {
                    output = $"{user.Username}'s rank:{rank}\t Followers: {item.Followers}";
                }
            }
            await ctx.RespondAsync(output);
        }

        [Command("globalrank")]
        [Aliases("grank")]
        [Description("Displays users global rank")]
        [RequireGuild]
        public async Task GlobalRank(CommandContext ctx, DiscordMember user = null)
        {
            if (user == null)
            {
                user = ctx.Member;
            }
            string output = string.Empty;
            int rank = 0;
            List<DB.Leaderboard> LB = DB.DBLists.Leaderboard;
            List<DB.Leaderboard> leaderbaords = LB.OrderByDescending(x => x.Followers).ToList();
            foreach (var item in leaderbaords)
            {
                rank++;
                if (item.ID_User == user.Id)
                {
                    output = $"{user.Username}'s rank:{rank}\t Followers: {item.Followers}\t Level {item.Level}";
                }
            }
            await ctx.RespondAsync(output);
        }

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
                else if (result.Result.Result.Emoji == left)
                {
                    await TopMessage.DeleteReactionAsync(result.Result.Result.Emoji, ctx.User);
                    if (page > 1)
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
        [Description("Command to buy profile customisation.")]
        public async Task Buy(CommandContext ctx,
            [Description("What you want to buy")] string what,
            [Description("ID of what you want to buy")] int id)
        {
            string output = string.Empty;
            if (what == "background" || what == "bg")
            {
                List<DB.UserImages> UserImg = DB.DBLists.UserImages;
                var backgroundrow = (from ui in UserImg
                                     where ui.User_ID == ctx.User.Id
                                     where ui.BG_ID == id
                                     select ui).ToList();
                List<DB.BackgroundImage> BG = DB.DBLists.BackgroundImage;
                var background = (from bg in BG
                                  where bg.ID_BG == id
                                  select bg).ToList();
                List<DB.Leaderboard> Leaderboard = DB.DBLists.Leaderboard;
                var user = (from lb in Leaderboard
                            where lb.ID_User == ctx.User.Id
                            select lb).FirstOrDefault();

                if (background.Count == 1)
                {
                    if (backgroundrow.Count == 0)
                    {
                        if ((long)background[0].Price <= user.Bucks)
                        {
                            var idui = UserImg.Max(m => m.ID_User_Images);
                            user.Bucks -= (long)background[0].Price;
                            DB.UserImages newEntry = new DB.UserImages
                            {
                                User_ID = ctx.User.Id,
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
        [Cooldown(1, 300, CooldownBucketType.User)]
        [Description("Shows summit tier list and time left.")]
        public async Task Summit(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            string PCJson = string.Empty, XBJson = string.Empty, PSJson = string.Empty, StadiaJson = string.Empty;
            string imageLoc = $"{Program.tmpLoc}{ctx.User.Id}-summit.png";
            byte[] SummitLogo;
            DateTime endtime;

            int platforms = 4;

            using (WebClient wc = new WebClient())
            {
                List<TCHubJson.Summit> JSummit = Program.JSummit;
                PCJson = wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/pc/profile/a92d844e-9c57-4b8c-a249-108ef42d4500");
                XBJson = wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/x1/profile/a92d844e-9c57-4b8c-a249-108ef42d4500");
                PSJson = wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/ps4/profile/a92d844e-9c57-4b8c-a249-108ef42d4500");
                try
                {
                    StadiaJson = wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/stadia/profile/a92d844e-9c57-4b8c-a249-108ef42d4500");
                }
                catch (Exception)
                {
                    platforms = 3;
                }

                SummitLogo = wc.DownloadData($"https://www.thecrew-hub.com/gen/assets/summits/{JSummit[0].Cover_Small}");

                endtime = CustomMethod.EpochConverter(JSummit[0].End_Date * 1000);
            }
            TCHubJson.Rank[] Events = new TCHubJson.Rank[0];
            if (platforms == 4)
            {
                Events = new TCHubJson.Rank[4] { JsonConvert.DeserializeObject<TCHubJson.Rank>(PCJson), JsonConvert.DeserializeObject<TCHubJson.Rank>(PSJson), JsonConvert.DeserializeObject<TCHubJson.Rank>(XBJson), JsonConvert.DeserializeObject<TCHubJson.Rank>(StadiaJson) };
            }
            else
            {
                Events = new TCHubJson.Rank[3] { JsonConvert.DeserializeObject<TCHubJson.Rank>(PCJson), JsonConvert.DeserializeObject<TCHubJson.Rank>(PSJson), JsonConvert.DeserializeObject<TCHubJson.Rank>(XBJson) };
            }
            string[,] pts = new string[platforms, 4];
            for (int i = 0; i < Events.Length; i++)
            {
                for (int j = 0; j < Events[i].Tier_entries.Length; j++)
                {
                    if (Events[i].Tier_entries[j].Points == 4294967295)
                    {
                        pts[i, j] = "-";
                    }
                    else
                    {
                        pts[i, j] = Events[i].Tier_entries[j].Points.ToString();
                    }
                }
            }
            using (Image<Rgba32> PCImg = Image.Load<Rgba32>("Assets/Summit/PC.jpeg"))
            using (Image<Rgba32> PSImg = Image.Load<Rgba32>("Assets/Summit/PS.jpg"))
            using (Image<Rgba32> XBImg = Image.Load<Rgba32>("Assets/Summit/XB.png"))
            using (Image<Rgba32> StadiaImg = Image.Load<Rgba32>("Assets/Summit/STADIA.png"))
            using (Image<Rgba32> BaseImg = new Image<Rgba32>(300 * platforms, 643))
            {
                Image<Rgba32>[] PlatformImg = new Image<Rgba32>[4] { PCImg, PSImg, XBImg, StadiaImg };
                Parallel.For(0, Events.Length, (i, state) =>
                {
                    using Image<Rgba32> TierImg = Image.Load<Rgba32>("Assets/Summit/SummitBase.png");
                    using Image<Rgba32> SummitImg = Image.Load<Rgba32>(SummitLogo);
                    using Image<Rgba32> FooterImg = new Image<Rgba32>(300, 30);
                    SummitImg.Mutate(ctx => ctx.Crop(300, SummitImg.Height));
                    Color TextColour = Color.WhiteSmoke;
                    Point SummitLocation = new Point(0 + (300 * i), 0);
                    Font Basefont = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 30);
                    Font FooterFont = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 15);
                    FooterImg.Mutate(ctx => ctx
                    .Fill(Color.Black)
                    .DrawText($"TOTAL PARTICIPANTS: {Events[i].Player_Count}", FooterFont, TextColour, new PointF(10, 10))
                    );
                    BaseImg.Mutate(ctx => ctx
                        .DrawImage(SummitImg, SummitLocation, 1)
                        .DrawImage(TierImg, SummitLocation, 1)
                        .DrawText(pts[i, 3], Basefont, TextColour, new PointF(80 + (300 * i), 365))
                        .DrawText(pts[i, 2], Basefont, TextColour, new PointF(80 + (300 * i), 435))
                        .DrawText(pts[i, 1], Basefont, TextColour, new PointF(80 + (300 * i), 505))
                        .DrawText(pts[i, 0], Basefont, TextColour, new PointF(80 + (300 * i), 575))
                        .DrawImage(FooterImg, new Point(0 + (300 * i), 613), 1)
                        .DrawImage(PlatformImg[i], new Point(0 + (300 * i), 0), 1)
                        );
                });
                BaseImg.Save(imageLoc);
            }
            TimeSpan timeleft = endtime - DateTime.Now.ToUniversalTime();
            using var upFile = new FileStream(imageLoc, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
            await ctx.RespondWithFileAsync(upFile, $"Summit tier lists.\n *Ends in {timeleft.Days} days, {timeleft.Hours} hours, {timeleft.Minutes} minutes.*");
        }

        [Command("mysummit")]
        [Description("Shows your summit scores.")]
        [Cooldown(1, 30, CooldownBucketType.User)]
        [Aliases("sinfo", "summitinfo")]
        public async Task MySummit(CommandContext ctx, string platform = null)
        {
            await ctx.TriggerTypingAsync();
            TimerMethod.UpdateHubInfo();

            string OutMessage = string.Empty;
            string imageLoc = $"{Program.tmpLoc}{ctx.User.Id}-mysummit.png";

            bool SendImage = false;

            DateTime endtime;

            string search = string.Empty;
            string link = $"{Program.TCEJson.Link}api/tchub/profileId/{Program.TCEJson.Key}/{ctx.User.Id}";

            TCHubJson.TCESummit JTCE;
            using (WebClient wc = new WebClient())
            {
                try
                {
                    string Jdown = wc.DownloadString(link);
                    JTCE = JsonConvert.DeserializeObject<TCHubJson.TCESummit>(Jdown);
                }
                catch (Exception)
                {
                    JTCE = new TCHubJson.TCESummit
                    {
                        Error = "No Connection."
                    };
                }
            }

            TCHubJson.TCESummitSubs UserInfo = new TCHubJson.TCESummitSubs();

            if (JTCE.Error != null)
            {
                if (JTCE.Error == "Unregistered user")
                {
                    OutMessage = $"{ctx.User.Mention}, You have not linked your TCE account, please check out <#302818290336530434> on how to do so.";
                }
                else if (JTCE.Error == "Invalid API key !" || JTCE.Error == "No Connection.")
                {
                    OutMessage = $"{ctx.User.Mention}, the API is down, check <#257513574061178881> and please try again later.\n" +
                        $"<@85017957343694848> Rip API";
                }
            }
            else if (JTCE.Subs.Length == 1)
            {
                UserInfo = JTCE.Subs[0];
                search = UserInfo.Platform;
            }
            else if (JTCE.Subs.Length > 1)
            {
                if (platform == null)
                {
                    DiscordMessage platformMSG = await ctx.RespondAsync($"{ctx.User.Mention}, you have multiple platforms stored on TCE, please select platform you want to see the scores for.");
                    DiscordEmoji X1Emoji = await Program.TCGuild.GetEmojiAsync(445234294915334146);
                    DiscordEmoji PCEmoji = await Program.TCGuild.GetEmojiAsync(445234293019770900);
                    DiscordEmoji PSEmoji = await Program.TCGuild.GetEmojiAsync(445234294676258816);
                    DiscordEmoji STADIAEmoji = await Program.TCGuild.GetEmojiAsync(697798396584656896);

                    foreach (var sub in JTCE.Subs)
                    {
                        if (sub.Platform.Contains("ps4"))
                        {
                            await platformMSG.CreateReactionAsync(PSEmoji);
                        }
                        if (sub.Platform.Contains("stadia"))
                        {
                            await platformMSG.CreateReactionAsync(STADIAEmoji);
                        }
                        if (sub.Platform.Contains("x1"))
                        {
                            await platformMSG.CreateReactionAsync(X1Emoji);
                        }
                        if (sub.Platform.Contains("pc"))
                        {
                            await platformMSG.CreateReactionAsync(PCEmoji);
                        }
                    }

                    var Result = await platformMSG.WaitForReactionAsync(ctx.User, TimeSpan.FromSeconds(30));

                    if (Result.TimedOut)
                    {
                        await platformMSG.ModifyAsync("Nothing selected, defaulting to PC");
                        platform = "pc";
                    }
                    else if (Result.Result.Emoji == PCEmoji)
                    {
                        await platformMSG.ModifyAsync("PC Platform selected.");
                        platform = "pc";
                    }
                    else if (Result.Result.Emoji == PSEmoji)
                    {
                        await platformMSG.ModifyAsync("Playstation Platform selected.");
                        platform = "ps";
                    }
                    else if (Result.Result.Emoji == X1Emoji)
                    {
                        await platformMSG.ModifyAsync("Xbox Platform selected.");
                        platform = "x1";
                    }
                    else if (Result.Result.Emoji == STADIAEmoji)
                    {
                        await platformMSG.ModifyAsync("Stadia Platform selected.");
                        platform = "stadia";
                    }
                    await platformMSG.DeleteAllReactionsAsync();
                }
                switch (platform.ToLower())
                {
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

                    case "stadia":
                        search = "stadia";
                        break;
                }
                if (JTCE.Subs.Count(w => w.Platform.Equals(search)) == 1)
                {
                    UserInfo = JTCE.Subs.FirstOrDefault(w => w.Platform.Equals(search));
                }
                else if (JTCE.Subs.Count(w => w.Platform.Equals(search)) != 1)
                {
                    UserInfo = JTCE.Subs[0];
                }
            }

            if (UserInfo.Profile_ID != null)
            {
                string SJson;
                List<TCHubJson.Summit> JSummit = Program.JSummit;
                byte[] EventLogoBit;
                using (WebClient wc = new WebClient())
                {
                    SJson = wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/{UserInfo.Platform}/profile/{UserInfo.Profile_ID}");

                    endtime = CustomMethod.EpochConverter(JSummit[0].End_Date * 1000);
                }
                TCHubJson.Rank Events = JsonConvert.DeserializeObject<TCHubJson.Rank>(SJson);

                int[,] WidthHeight = new int[,] { { 0, 249 }, { 373, 249 }, { 0, 493 }, { 373, 493 }, { 747, 0 }, { 747, 249 }, { 0, 0 }, { 249, 0 }, { 498, 0 } };

                Font Basefont = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 18);
                Font SummitCaps15 = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 15);
                Font SummitCaps12 = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 12.5f);

                var AllignCenter = new TextGraphicsOptions()
                {
                    TextOptions =
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Top
                    }
                };
                var AllignTopLeft = new TextGraphicsOptions()
                {
                    TextOptions =
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    }
                };
                var AllignTopRight = new TextGraphicsOptions()
                {
                    TextOptions =
                    {
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top
                    }
                };

                if (Events.Points != 0)
                {
                    using Image<Rgba32> BaseImage = new Image<Rgba32>(1127, 765);
                    Parallel.For(0, 9, (i, state) =>
                    {
                        var ThisEvent = JSummit[0].Events[i];
                        var Activity = Events.Activities.Where(w => w.Activity_ID.Equals(ThisEvent.ID.ToString())).ToArray();

                        EventLogoBit = TimerMethod.EventLogoBitArr[i];
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
                            string ThisEventNameID = string.Empty;
                            if (ThisEvent.Is_Mission)
                            {
                                ThisEventNameID = Program.TCHub.Missions.Where(w => w.ID == ThisEvent.ID).Select(s => s.Text_ID).FirstOrDefault();
                            }
                            else
                            {
                                ThisEventNameID = Program.TCHub.Skills.Where(w => w.ID == ThisEvent.ID).Select(s => s.Text_ID).FirstOrDefault();
                            }
                            string[] EventTitle = Program.TCHubDictionary.FirstOrDefault(w => w.Key.Equals(ThisEventNameID)).Value.Replace("\"", string.Empty).Split(' ');
                            TCHubJson.SummitLeaderboard leaderboard = JsonConvert.DeserializeObject<TCHubJson.SummitLeaderboard>(wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/leaderboard/{UserInfo.Platform}/{ThisEvent.ID}?page_size=1"));
                            StringBuilder sb = new StringBuilder();
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
                            using (Image<Rgba32> TitleBar = new Image<Rgba32>(EventImage.Width, 40))
                            using (Image<Rgba32> ScoreBar = new Image<Rgba32>(EventImage.Width, 40))
                            {
                                ScoreBar.Mutate(ctx => ctx.Fill(Color.Black));
                                TitleBar.Mutate(ctx => ctx.Fill(Color.Black));
                                EventImage.Mutate(ctx => ctx
                                .DrawImage(ScoreBar, new Point(0, EventImage.Height - ScoreBar.Height), 0.7f)
                                .DrawImage(TitleBar, new Point(0, 0), 0.7f)
                                .DrawText(AllignTopLeft, sb.ToString(), SummitCaps15, Color.White, new PointF(5, 0))
                                .DrawText(AllignTopLeft, $"Rank: {Activity[0].Rank + 1}", Basefont, Color.White, new PointF(5, EventImage.Height - 22))
                                .DrawText(AllignTopRight, ActivityResult, Basefont, Color.White, new PointF(EventImage.Width - 5, EventImage.Height - 42))
                                .DrawText(AllignTopRight, $"Points: {Activity[0].Points}", Basefont, Color.White, new PointF(EventImage.Width - 5, EventImage.Height - 22))
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
                                .Fill(Color.Black)
                                .DrawText(AllignCenter, "Event not completed!", Basefont, Color.White, new PointF(NotComplete.Width / 2, NotComplete.Height / 2))
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
                                .DrawText(AllignTopLeft, $"Points Needed: {Events.Tier_entries[i].Points}", SummitCaps12, Color.White, new PointF(TierXPos[i] + 5, 15))
                                );
                            }
                        });

                        TierBar.Mutate(ctx => ctx
                                .DrawText(AllignTopLeft, $"Summit Rank: {Events.UserRank + 1} Score: {Events.Points}", SummitCaps15, Color.White, new PointF(TierXPos[Tier.Count(c => c) - 1] + 5, 0))
                                );

                        BaseImage.Mutate(ctx => ctx
                        .DrawImage(TierBar, new Point(0, BaseImage.Height - 30), 1)
                        );
                    }

                    TimeSpan timeleft = endtime - DateTime.Now.ToUniversalTime();
                    BaseImage.Save(imageLoc);
                    OutMessage = $"{ctx.User.Mention}, Here are your summit event stats for {(search == "x1" ? "Xbox" : search == "ps4" ? "PlayStation" : search == "stadia" ? "Stadia" : "PC")}.\n*Ends in {timeleft.Days} days, {timeleft.Hours} hours, {timeleft.Minutes} minutes. Scoreboard powered by The Crew Hub and The Crew Exchange!*";
                    SendImage = true;
                }
                else
                {
                    OutMessage = $"{ctx.User.Mention}, You have not completed any summit event!";
                }
            }

            if (SendImage)
            {
                using var upFile = new FileStream(imageLoc, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
                await ctx.RespondWithFileAsync(upFile, OutMessage);
            }
            else
            {
                await ctx.RespondAsync(OutMessage);
            }
        }

        [Command("topsummit")]
        [Description("Shows the summit board with all the world record scores.")]
        [Cooldown(1, 30, CooldownBucketType.User)]
        public async Task TopSummit(CommandContext ctx, string platform = null)
        {
            await ctx.TriggerTypingAsync();
            TimerMethod.UpdateHubInfo();

            int TotalPoints = 0;

            string OutMessage = string.Empty;
            string imageLoc = $"{Program.tmpLoc}{ctx.User.Id}-topsummit.png";

            bool alleventscompleted = true;

            DateTime endtime;

            if (platform == null)
            {
                platform = "pc";
            }

            switch (platform.ToLower())
            {
                case null:
                case "pc":
                case "computer":
                    platform = "pc";
                    break;

                case "xbox":
                case "xb1":
                case "xb":
                case "x1":
                    platform = "x1";
                    break;

                case "ps4":
                case "playstation":
                case "ps":
                    platform = "ps4";
                    break;

                case "stadia":
                    platform = "stadia";
                    break;
            }

            List<TCHubJson.Summit> JSummit = Program.JSummit;
            byte[] EventLogoBit;

            endtime = CustomMethod.EpochConverter(JSummit[0].End_Date * 1000);

            int[,] WidthHeight = new int[,] { { 0, 249 }, { 373, 249 }, { 0, 493 }, { 373, 493 }, { 747, 0 }, { 747, 249 }, { 0, 0 }, { 249, 0 }, { 498, 0 } };

            Font Basefont = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 18);
            Font SummitCaps15 = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 15);

            var AllignCenter = new TextGraphicsOptions()
            {
                TextOptions =
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top
                }
            };
            var AllignTopLeft = new TextGraphicsOptions()
            {
                TextOptions =
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                }
            };
            var AllignTopRight = new TextGraphicsOptions()
            {
                TextOptions =
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top
                }
            };

            using Image<Rgba32> BaseImage = new Image<Rgba32>(1127, 735);
            Parallel.For(0, 9, (i, state) =>
            {
                using WebClient wc = new WebClient();
                var ThisEvent = JSummit[0].Events[i];
                var Activity = JsonConvert.DeserializeObject<TCHubJson.SummitLeaderboard>(wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/leaderboard/{platform}/{JSummit[0].Events[i].ID}?page_size=1"));

                EventLogoBit = TimerMethod.EventLogoBitArr[i];
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
                if (Activity.Entries.Length > 0)
                {
                    string ThisEventNameID = string.Empty;
                    if (ThisEvent.Is_Mission)
                    {
                        ThisEventNameID = Program.TCHub.Missions.Where(w => w.ID == ThisEvent.ID).Select(s => s.Text_ID).FirstOrDefault();
                    }
                    else
                    {
                        ThisEventNameID = Program.TCHub.Skills.Where(w => w.ID == ThisEvent.ID).Select(s => s.Text_ID).FirstOrDefault();
                    }
                    string[] EventTitle = Program.TCHubDictionary.FirstOrDefault(w => w.Key.Equals(ThisEventNameID)).Value.Replace("\"", string.Empty).Split(' ');
                    TCHubJson.SummitLeaderboard leaderboard = JsonConvert.DeserializeObject<TCHubJson.SummitLeaderboard>(wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/leaderboard/{platform}/{ThisEvent.ID}?page_size=1"));
                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < EventTitle.Length; j++)
                    {
                        if (j == 3)
                        {
                            sb.AppendLine();
                        }
                        sb.Append(EventTitle[j] + " ");
                    }
                    string ActivityResult = $"Score: {Activity.Entries[0].Score}";
                    if (leaderboard.Score_Format == "time")
                    {
                        ActivityResult = $"Time: {CustomMethod.ScoreToTime(Activity.Entries[0].Score)}";
                    }
                    else if (sb.ToString().Contains("SPEEDTRAP"))
                    {
                        ActivityResult = $"Speed: {Activity.Entries[0].Score.ToString().Insert(3, ".")} km/h";
                    }
                    else if (sb.ToString().Contains("ESCAPE"))
                    {
                        ActivityResult = $"Distance: {Activity.Entries[0].Score}m";
                    }
                    using (Image<Rgba32> TitleBar = new Image<Rgba32>(EventImage.Width, 40))
                    using (Image<Rgba32> ScoreBar = new Image<Rgba32>(EventImage.Width, 40))
                    {
                        ScoreBar.Mutate(ctx => ctx.Fill(Color.Black));
                        TitleBar.Mutate(ctx => ctx.Fill(Color.Black));
                        EventImage.Mutate(ctx => ctx
                        .DrawImage(ScoreBar, new Point(0, EventImage.Height - ScoreBar.Height), 0.7f)
                        .DrawImage(TitleBar, new Point(0, 0), 0.7f)
                        .DrawText(AllignTopLeft, sb.ToString(), SummitCaps15, Color.White, new PointF(5, 0))
                        .DrawText(AllignTopLeft, $"Rank: 1", Basefont, Color.White, new PointF(5, EventImage.Height - 22))
                        .DrawText(AllignTopRight, ActivityResult, Basefont, Color.White, new PointF(EventImage.Width - 5, EventImage.Height - 42))
                        .DrawText(AllignTopRight, $"Points: {Activity.Entries[0].Points}", Basefont, Color.White, new PointF(EventImage.Width - 5, EventImage.Height - 22))
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
                        .Fill(Color.Black)
                        .DrawText(AllignCenter, "Event not completed!", Basefont, Color.White, new PointF(NotComplete.Width / 2, NotComplete.Height / 2))
                        );
                    BaseImage.Mutate(ctx => ctx
                    .DrawImage(EventImage, new Point(WidthHeight[i, 0], WidthHeight[i, 1]), 1)
                    .DrawImage(NotComplete, new Point(WidthHeight[i, 0], WidthHeight[i, 1]), 0.8f)
                    );
                    alleventscompleted = false;
                }
                TotalPoints += Activity.Entries[0].Points;
            });

            if (alleventscompleted)
            {
                TotalPoints += 100000;
            }

            TimeSpan timeleft = endtime - DateTime.Now.ToUniversalTime();
            BaseImage.Save(imageLoc);
            OutMessage = $"{ctx.User.Mention}, Here are the top summit scores for {(platform == "x1" ? "Xbox" : platform == "ps4" ? "PlayStation" : platform == "stadia" ? "Stadia" : "PC")}. Total event points: **{TotalPoints}**\n*Ends in {timeleft.Days} days, {timeleft.Hours} hours, {timeleft.Minutes} minutes. Scoreboard powered by The Crew Hub and The Crew Exchange!*";
            
            using var upFile = new FileStream(imageLoc, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
            await ctx.RespondWithFileAsync(upFile, OutMessage);
        }

        [Command("summitrewards")]
        [Aliases("srewards")]
        [Cooldown(1, 60, CooldownBucketType.User)]
        [Description("Shows this weeks summit rewards")]
        public async Task SummitRewards(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            TimerMethod.UpdateHubInfo();

            Color[] RewardColours = new Color[] { Rgba32.ParseHex("#0060A9"), Rgba32.ParseHex("#D5A45F"), Rgba32.ParseHex("#C2C2C2"), Rgba32.ParseHex("#B07C4D") };

            string imageLoc = $"{Program.tmpLoc}{ctx.User.Id}-summitrewards.png";
            int RewardWidth = 412;
            TCHubJson.Reward[] Rewards = Program.JSummit[0].Rewards;
            Font Font = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 15);
            using (Image<Rgba32> RewardsImage = new Image<Rgba32>(4 * RewardWidth, 328))
            {
                Parallel.For(0, Rewards.Length, (i, state) =>
                 {
                     string RewardTitle = string.Empty;
                     switch (Rewards[i].Type)
                     {
                         case "phys_part":
                             RewardTitle = $"{Program.TCHubDictionary.FirstOrDefault(w => w.Key.Equals(Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("quality_text_id")).Value)).Value}" +
                             $" {Regex.Replace(Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("bonus_icon")).Value, "\\w{0,}_", "")} {Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("type")).Value}" +
                             $"({Regex.Replace(Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("vcat_icon")).Value, "\\w{0,}_", "")})";
                             break;

                         case "vanity":
                             RewardTitle = Program.TCHubDictionary.FirstOrDefault(w => w.Key.Equals(Rewards[i].Title_Text_ID)).Value;
                             if (RewardTitle is null)
                             {
                                 if (Rewards[i].Img_Path.Contains("emote"))
                                 {
                                     RewardTitle = "Emote";
                                 }
                             }
                             break;

                         case "generic":
                             RewardTitle = Rewards[i].Debug_Title;
                             break;

                         case "currency":
                             RewardTitle = $"{Rewards[i].Debug_Title} - {Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("currency_amount")).Value}";
                             break;

                         case "vehicle":
                             RewardTitle = $"{Program.TCHubDictionary.FirstOrDefault(w => w.Key.Equals(Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("brand_text_id")).Value)).Value} - {Program.TCHubDictionary.FirstOrDefault(w => w.Key.Equals(Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("model_text_id")).Value)).Value}";
                             break;

                         default:
                             RewardTitle = "LiveBot needs to be updated to view this reward!";
                             break;
                     }
                     if (RewardTitle is null)
                     {
                         RewardTitle = "LiveBot needs to be updated to view this reward!";
                     }

                     RewardTitle = Regex.Replace(RewardTitle, "((<(\\w||[/=\"'#\\ ]){0,}>)||(&#\\d{0,}; )){0,}", "").ToUpper();

                     using Image<Rgba32> RewardImage = Image.Load<Rgba32>(TimerMethod.RewardsImageBitArr[i]);
                     using Image<Rgba32> TopBar = new Image<Rgba32>(RewardImage.Width, 20);
                     TopBar.Mutate(ctx => ctx.
                     Fill(RewardColours[i])
                     );
                     RewardsImage.Mutate(ctx => ctx
                     .DrawImage(RewardImage, new Point((4 - Rewards[i].Level) * RewardWidth, 0), 1)
                     .DrawImage(TopBar, new Point((4 - Rewards[i].Level) * RewardWidth, 0), 1)
                     .DrawText(RewardTitle, Font, Color.White, new PointF(((4 - Rewards[i].Level) * RewardWidth) + 5, 0))
                     );
                 });
                RewardsImage.Save(imageLoc);
            }
            using var upFile = new FileStream(imageLoc, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
            await ctx.RespondWithFileAsync(upFile, $"{ctx.User.Mention}, here are this weeks summit rewards:");
        }

        [Command("myfame")]
        [Cooldown(1, 60, CooldownBucketType.User)]
        [Description("Tells your followers and rank on the leaderboard")]
        [RequireRoles(RoleCheckMode.Any, "Patreon", "TCE Patreon", "Ubisoft", "Discord-Moderator")]
        public async Task MyFame(CommandContext ctx, string platform = null)
        {
            await ctx.TriggerTypingAsync();

            string OutMessage = string.Empty;

            string search = string.Empty;

            string link = $"{Program.TCEJson.Link}api/tchub/profileId/{Program.TCEJson.Key}/{ctx.User.Id}";

            TCHubJson.TCESummit JTCE;
            using WebClient wc = new WebClient();
            try
            {
                string Jdown = wc.DownloadString(link);
                JTCE = JsonConvert.DeserializeObject<TCHubJson.TCESummit>(Jdown);
            }
            catch (Exception)
            {
                JTCE = new TCHubJson.TCESummit
                {
                    Error = "No Connection."
                };
            }

            TCHubJson.TCESummitSubs UserInfo = new TCHubJson.TCESummitSubs();

            if (JTCE.Error != null)
            {
                if (JTCE.Error == "Unregistered user")
                {
                    await ctx.RespondAsync($"{ctx.User.Mention}, You have not linked your TCE account, please check out <#302818290336530434> on how to do so.");
                }
                else if (JTCE.Error == "Invalid API key !" || JTCE.Error == "No Connection.")
                {
                    await ctx.RespondAsync($"{ctx.User.Mention}, the API is down, check <#257513574061178881> and please try again later.\n" +
                        $"<@85017957343694848> Rip API");
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

                    case "stadia":
                        search = "stadia";
                        break;
                }
                if (JTCE.Subs.Count(w => w.Platform.Equals(search)) == 1)
                {
                    UserInfo = JTCE.Subs.FirstOrDefault(w => w.Platform.Equals(search));
                }
                else if (JTCE.Subs.Count(w => w.Platform.Equals(search)) != 1)
                {
                    UserInfo = JTCE.Subs[0];
                }
            }

            try
            {
                TCHubJson.Fame Fame = JsonConvert.DeserializeObject<TCHubJson.Fame>(wc.DownloadString($"https://api.thecrew-hub.com/v1/leaderboard/{UserInfo.Platform}/fame?profile_id={UserInfo.Profile_ID}"));
                var HubUserInfo = Fame.Scores.FirstOrDefault(w => w.Profile_ID.Equals(UserInfo.Profile_ID));
                OutMessage = $"{ctx.User.Mention}, Your follower count is **{HubUserInfo.Score}**. Your Icon Level is **[WIP]**. You are ranked **{HubUserInfo.Rank} on {search}**";
            }
            catch (WebException)
            {
                OutMessage = $"{ctx.User.Mention}, The Crew HUB API is currently unavailable.";
            }

            await ctx.RespondAsync(OutMessage);
        }

        [Command("daily")]
        [Aliases("dailies")]
        [Cooldown(1, 60, CooldownBucketType.User)]
        [Description("Gives 200 bucks to yourself, or 200-400 if you give someone else.")]
        [RequireGuild]
        public async Task Daily(CommandContext ctx, DiscordMember member = null)
        {
            int money = 200;
            if (member == null)
            {
                member = ctx.Member;
            }
            var user = DB.DBLists.Leaderboard.FirstOrDefault(f => f.ID_User == ctx.Member.Id);
            var reciever = DB.DBLists.Leaderboard.FirstOrDefault(f => f.ID_User == member.Id);
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
                    DB.DBLists.UpdateLeaderboard(user);
                    await ctx.RespondAsync($"{ctx.Member.Mention}, You have received {money} bucks");
                }
                else
                {
                    Random r = new Random();
                    money += r.Next(200);
                    user.Daily_Used = DateTime.Now.ToString("ddMMyyyy");
                    reciever.Bucks += money;
                    DB.DBLists.UpdateLeaderboard(user);
                    DB.DBLists.UpdateLeaderboard(reciever);
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
        [RequireGuild]
        public async Task Cookie(CommandContext ctx, DiscordMember member)
        {
            string output = string.Empty;
            if (ctx.Member == member)
            {
                output = $"{ctx.Member.Mention}, you can't give a cookie to yourself";
            }
            else
            {
                var giver = DB.DBLists.Leaderboard.FirstOrDefault(f => f.ID_User == ctx.Member.Id);
                var reciever = DB.DBLists.Leaderboard.FirstOrDefault(f => f.ID_User == member.Id);
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
                    DB.DBLists.UpdateLeaderboard(giver);
                    DB.DBLists.UpdateLeaderboard(reciever);
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
            var user = DB.DBLists.Leaderboard.FirstOrDefault(f => f.ID_User == ctx.Member.Id);
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

        [Command("status")]
        [Description("The Crew 2 Server status.")]
        [Cooldown(1, 120, CooldownBucketType.User)]
        public async Task Status(CommandContext ctx)
        {
            string HTML;
            using (WebClient wc = new WebClient())
            {
                HTML = wc.DownloadString("https://ubistatic-a.akamaihd.net/0115/tc2/status.html");
            }
            if (HTML.Contains("STATUS OK"))
            {
                await ctx.RespondAsync("The Crew 2 Server is Online");
            }
            else
            {
                await ctx.RespondAsync("The Crew 2 Server is Offline");
            }
        }
        
        [Command("mywarnings")]
        [RequireGuild]
        [Description("Shows your warning, kick and ban history.")]
        public async Task MyWarnings(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            try
            {
                await ctx.Member.SendMessageAsync(embed: CustomMethod.GetUserWarnings(ctx.Guild, ctx.User));
            }
            catch
            {
                await ctx.RespondAsync($"{ctx.Member.Mention}, Could not contact you through DMs.");
            }
        }
    }
}