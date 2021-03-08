﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
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
            string changelog = "[FIX] MySummit command now should always indicate the right platform of which the stats are shown, when defaulting from a platform you have not linked.\n" +
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
        public async Task GetEmote(CommandContext ctx, params DiscordEmoji[] emotes)
        {
            StringBuilder sb = new();

            foreach (DiscordEmoji emoji in emotes)
            {
                sb.AppendLine($"{emoji} - {emoji.Id}");
            }
            await new DiscordMessageBuilder()
                .WithContent(sb.ToString())
                .WithReply(ctx.Message.Id, true)
                .SendAsync(ctx.Channel);
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
            FileStream ITImage = new("Assets/ITC.jpg", FileMode.Open);
            var msgBuilder = new DiscordMessageBuilder();
            msgBuilder.WithFile(ITImage);
            if (username == null)
            {
                msgBuilder.Content = $"{ctx.User.Mention}";
            }
            else
            {
                msgBuilder.Content = $"{username.Mention}";
            }
            await ctx.RespondAsync(msgBuilder);
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
            CultureInfo info = new("en-GB");
            string joinedstring = user.JoinedAt.ToString(format, info);
            string createdstring = user.CreationTimestamp.ToString(format, info);
            DiscordEmbedBuilder embed = new()
            {
                Color = new DiscordColor(0xFF6600),
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = user.Username,
                    IconUrl = user.AvatarUrl
                },
                Description = $"**ID**\t\t\t\t\t\t\t\t\t\t\t\t**Nickname**\n" +
                $"{user.Id}\t\t\t{user.Nickname ?? "*none*"}\n" +
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
            DiscordOverwriteBuilder user = new();
            DiscordOverwriteBuilder everyone = new();
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
        [RequireRoles(RoleCheckMode.Any, "Discord-Moderator", "Patreon", "Eldorado King", "Ubisoft", "Event Organizer", "Content Creator")]
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
            Random r = new();
            int row = 0;
            List<DB.VehicleList> SelectedVehicles = new();

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

            DiscordColor embedColour = new();

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

            DiscordEmbedBuilder embed = new()
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
            embed.AddField("MP exclusive?", $"{SelectedVehicles[row].IsMotorPassExclusive}", true);

            await ctx.RespondAsync($"*({SelectedVehicles.Count - 1} vehicles left in current rotation)*", embed);
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
            await new DiscordMessageBuilder()
                .WithContent(output)
                 .WithReply(ctx.Message.Id, true)
                 .SendAsync(ctx.Channel);
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
            await new DiscordMessageBuilder()
                .WithContent(output)
                 .WithReply(ctx.Message.Id, true)
                 .SendAsync(ctx.Channel);
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
            var msgBuilder = new DiscordMessageBuilder()
               .WithContent(CustomMethod.GetGlobalTop(ctx, (int)page))
                .WithReply(ctx.Message.Id, true);
            DiscordMessage TopMessage = await ctx.RespondAsync(msgBuilder);
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
            var msgBuilder = new DiscordMessageBuilder()
                .WithContent(CustomMethod.GetServerTop(ctx, (int)page))
                 .WithReply(ctx.Message.Id, true);
            DiscordMessage TopMessage = await ctx.RespondAsync(msgBuilder);
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
            var msgBuilder = new DiscordMessageBuilder()
                .WithContent(CustomMethod.GetBackgroundList(ctx, (int)page))
                 .WithReply(ctx.Message.Id, true);
            DiscordMessage TopMessage = await ctx.RespondAsync(msgBuilder);
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
                            DB.UserImages newEntry = new()
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
            await new DiscordMessageBuilder()
                .WithContent(output)
                 .WithReply(ctx.Message.Id, true)
                 .SendAsync(ctx.Channel);
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
            var receiver = DB.DBLists.Leaderboard.FirstOrDefault(f => f.ID_User == member.Id);
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
                    await new DiscordMessageBuilder()
                        .WithContent($"You have received {money} bucks")
                         .WithReply(ctx.Message.Id, true)
                         .SendAsync(ctx.Channel);
                }
                else
                {
                    Random r = new();
                    money += r.Next(200);
                    user.Daily_Used = DateTime.Now.ToString("ddMMyyyy");
                    receiver.Bucks += money;
                    DB.DBLists.UpdateLeaderboard(user);
                    DB.DBLists.UpdateLeaderboard(receiver);
                    await new DiscordMessageBuilder()
                        .WithContent($"{member.Mention}, You were given {money} bucks by {ctx.Member.Username}")
                         .WithReply(ctx.Message.Id)
                         .SendAsync(ctx.Channel);
                }
            }
            else
            {
                DateTime now = DateTime.Now;

                await new DiscordMessageBuilder()
                    .WithContent($"Time untill you can use daily {(24 - now.Hour) - 1}:{(60 - now.Minute) - 1}:{(60 - now.Second) - 1}.")
                     .WithReply(ctx.Message.Id, true)
                     .SendAsync(ctx.Channel);
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
            using (WebClient wc = new())
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

        [Command("roletag")]
        [Aliases("rt")]
        [Description("Tags a role ")]
        public async Task RoleTag(CommandContext ctx, DiscordEmoji Emoji)
        {
            List<DB.RoleTagSettings> Roles = DB.DBLists.RoleTagSettings.Where(w => w.Server_ID == ctx.Guild.Id && w.Emoji_ID == Emoji.Id).ToList();
            if (Roles != null)
            {
                DB.RoleTagSettings RT = Roles.FirstOrDefault(w => (ulong)w.Channel_ID == ctx.Channel.Id);
                if (RT != null)
                {
                    DiscordRole role = ctx.Guild.GetRole((ulong)RT.Role_ID);
                    if (RT.Last_Used < DateTime.Now - TimeSpan.FromMinutes(RT.Cooldown))
                    {
                        await ctx.RespondAsync($"{role.Mention} - {ctx.Member.Mention}: {RT.Message}");
                        RT.Last_Used = DateTime.Now;
                        DB.DBLists.UpdateRoleTagSettings(RT);
                    }
                    else
                    {
                        TimeSpan remainingTime = TimeSpan.FromMinutes(RT.Cooldown) - (DateTime.Now - RT.Last_Used);
                        await ctx.RespondAsync($"{ctx.Member.Mention}, This role can't be mentioned right now, cooldown has not passed yet. ({remainingTime.Hours} Hours {remainingTime.Minutes} Minutes {remainingTime.Seconds} Seconds left)");
                    }
                }
                else
                {
                    await ctx.RespondAsync($"{ctx.Member.Mention}, This channel does not allow this role to be pinged");
                }
            }
            else
            {
                await ctx.RespondAsync($"{ctx.Member.Mention}, This emoji does not represent a role in this server.");
            }
        }

        [Command("mysummit")]
        public async Task MySummitOld(CommandContext ctx)
        {
            await new DiscordMessageBuilder
            {
                Content = "The command has moved to `/hub mysummit` or `/hub sinfo`"
            }
            .WithReply(ctx.Message.Id, true)
            .SendAsync(ctx.Channel);
        }
    }
}