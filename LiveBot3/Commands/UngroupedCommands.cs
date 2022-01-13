using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Globalization;
using System.Net.Http;

namespace LiveBot.Commands
{
    public class UngroupedCommands : BaseCommandModule
    {
        [Command("bot")]//list of Live bot changes
        [Description("Info about the bot. Latest changes, how to support, how long it has been up.")]
        public async Task Bot(CommandContext ctx)
        {
            DateTime current = DateTime.UtcNow;
            TimeSpan time = current - Program.start;
            string changelog = "[NEW] everyone tag protection in testing.";
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
            await ctx.RespondAsync(embed: embed);
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
        public async Task Ping(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
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
            await ctx.TriggerTypingAsync();
            string content;
            if (username is null) //checks if user name is not specified
            {
                content = CustomMethod.GetCommandOutput(ctx, "share", language, ctx.Member);
            }
            else // if user name specified
            {
                content = CustomMethod.GetCommandOutput(ctx, "share", language, username);
            }
            await new DiscordMessageBuilder()
                .WithContent(content)
                .WithAllowedMention(new UserMention())
                .SendAsync(ctx.Channel);
        }

        [Command("platform")]
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        [Description("Informs the user about the platform roles.")] //platform selection command
        public async Task Platform(CommandContext ctx, [Description("Specifies the user the bot will mention, use ID or mention the user. If left blank, it will mention you.")] DiscordMember username = null, [Description("Specifies in what language the bot will respond. example, fr-french")] string language = null)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            string content;
            if (username is null)
            {
                content = CustomMethod.GetCommandOutput(ctx, "platform", language, ctx.Member);
            }
            else
            {
                content = CustomMethod.GetCommandOutput(ctx, "platform", language, username);
            }
            await new DiscordMessageBuilder()
                .WithContent(content)
                .WithAllowedMention(new UserMention())
                .SendAsync(ctx.Channel);
        }

        [Command("maxlvl")]
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        [Description("Explains how to get maximum car level in The Crew 1.")] // how to get max level for cars in TC1
        public async Task MaxCarlvl(CommandContext ctx, [Description("Specifies the user the bot will mention, use ID or mention the user. If left blank, it will mention you.")] DiscordMember username = null, [Description("Specifies in what language the bot will respond. example, fr-french")] string language = null)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            string content;
            if (username is null)
            {
                content = CustomMethod.GetCommandOutput(ctx, "maxlvl", language, ctx.Member);
            }
            else
            {
                content = CustomMethod.GetCommandOutput(ctx, "maxlvl", language, username);
            }
            await new DiscordMessageBuilder()
                .WithContent(content)
                .WithAllowedMention(new UserMention())
                .SendAsync(ctx.Channel);
        }

        [Command("tce")] //The Crew Exchange info command
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        [Description("Informs the user about The Crew Exchange website.")]
        public async Task TCE(CommandContext ctx, [Description("Specifies the user the bot will mention, use ID or mention the user. If left blank, it will mention you.")] DiscordMember username = null, [Description("Specifies in what language the bot will respond. example, fr-french")] string language = null)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            string content;
            if (username is null)
            {
                content = CustomMethod.GetCommandOutput(ctx, "tce", language, ctx.Member);
            }
            else
            {
                content = CustomMethod.GetCommandOutput(ctx, "tce", language, username);
            }
            await new DiscordMessageBuilder()
                .WithContent(content)
                .WithAllowedMention(new UserMention())
                .SendAsync(ctx.Channel);
        }

        [Command("useigc")]
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        public async Task UseIGC(CommandContext ctx, params DiscordMember[] username)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            string content;
            if (username.Length == 0)
            {
                content = CustomMethod.GetCommandOutput(ctx, "useigc", null, ctx.Member);
            }
            else if (username.Length == 1)
            {
                content = CustomMethod.GetCommandOutput(ctx, "useigc", null, username[0]);
            }
            else
            {
                StringBuilder sb = new();
                foreach (DiscordMember member in username)
                {
                    sb.Append($"{member.Mention},");
                }
                sb.Append(DB.DBLists.BotOutputList.FirstOrDefault(w => w.Command.Equals("useigc") && w.Language.Equals("gb")).Command_Text);
                content = sb.ToString();
            }
            await new DiscordMessageBuilder()
                .WithContent(content)
                .WithAllowedMention(new UserMention())
                .SendAsync(ctx.Channel);
        }

        [Command("lfc")]
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        [Description("Informs the user of using the LFC channels, or to get the platform role if they don't have it.")]
        public async Task LFC(CommandContext ctx, DiscordMember username = null, [Description("Specifies in what language the bot will respond. example, fr-french")] string language = null)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            string content = CustomMethod.GetCommandOutput(ctx, "lfc2", language, username);
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
                    content = CustomMethod.GetCommandOutput(ctx, "lfc1", language, username);
                    check = true;
                }
            }
            await new DiscordMessageBuilder()
                .WithContent(content)
                .WithAllowedMention(new UserMention())
                .SendAsync(ctx.Channel);
        }

        [Command("it")]
        [Description("Sends the IT Crowd image of \"have you tried turning it off and on again?\"")]
        public async Task IT(CommandContext ctx, DiscordMember username = null)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            FileStream ITImage = new("Assets/img/ITC.jpg", FileMode.Open);
            string content;
            if (username == null)
            {
                content = $"{ctx.User.Mention}";
            }
            else
            {
                content = $"{username.Mention}";
            }

            await new DiscordMessageBuilder()
                .WithFile(ITImage)
                .WithContent(content)
                .WithAllowedMention(new UserMention())
                .SendAsync(ctx.Channel);
        }

        [Command("bs")]
        public async Task BS(CommandContext ctx, DiscordMember discordMember = null)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            FileStream image = new("Assets/img/bs.gif", FileMode.Open);
            await new DiscordMessageBuilder()
                .WithFile(image)
                .WithAllowedMention(new UserMention())
                .WithContent($"{(discordMember == null ? ctx.Member.Mention : discordMember.Mention)}")
                .SendAsync(ctx.Channel);
        }

        [Command("supra")]
        [Description("Sends the supra gif.")]
        [Cooldown(1, 60, CooldownBucketType.Channel)]
        public async Task Supra(CommandContext ctx, DiscordMember username = null)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            string content;
            if (username is null)
            {
                content = CustomMethod.GetCommandOutput(ctx, "supra", null, ctx.Member);
            }
            else
            {
                content = CustomMethod.GetCommandOutput(ctx, "supra", null, username);
            }
            await new DiscordMessageBuilder()
                .WithContent(content)
                .WithAllowedMention(new UserMention())
                .SendAsync(ctx.Channel);
        }

        [Command("support")]
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        [Description("Gives the link to the support page")]
        public async Task Support(CommandContext ctx, DiscordMember username = null)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            string content;
            if (username is null)
            {
                content = CustomMethod.GetCommandOutput(ctx, "support", null, ctx.Member);
            }
            else
            {
                content = CustomMethod.GetCommandOutput(ctx, "support", null, username);
            }
            await new DiscordMessageBuilder()
                .WithContent(content)
                .WithAllowedMention(new UserMention())
                .SendAsync(ctx.Channel);
        }

        [Command("forums")]
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        [Description("Gives the link to the forum")]
        public async Task Forums(CommandContext ctx, DiscordMember username = null)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            string content;
            if (username is null)
            {
                content = CustomMethod.GetCommandOutput(ctx, "forums", null, ctx.Member);
            }
            else
            {
                content = CustomMethod.GetCommandOutput(ctx, "forums", null, username);
            }
            await new DiscordMessageBuilder()
                .WithContent(content)
                .WithAllowedMention(new UserMention())
                .SendAsync(ctx.Channel);
        }

        [Command("prosettings")]
        [Aliases("psettings")]
        [Cooldown(1, 10, CooldownBucketType.Channel)]
        [Description("Explains how to find pro settings")]
        public async Task ProSettings(CommandContext ctx, DiscordMember username = null)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            string content;
            if (username is null)
            {
                content = CustomMethod.GetCommandOutput(ctx, "prosettings", null, ctx.Member);
            }
            else
            {
                content = CustomMethod.GetCommandOutput(ctx, "prosettings", null, username);
            }
            await new DiscordMessageBuilder()
                .WithContent(content)
                .WithAllowedMention(new UserMention())
                .SendAsync(ctx.Channel);
        }

        [Command("info")]
        [Description("Shows users discord info")]
        public async Task Info(CommandContext ctx, [Description("users ID or mention")] DiscordMember user = null)
        {
            await Task.Delay(5);
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
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
            Random r = new();
            int row = 0;
            List<DB.VehicleList> SelectedVehicles = new();
            DiscordMessageBuilder MsgBuilder = null;
            DiscordMessage ChoiceMsg = null;

            if (disciplinename == "Street Race")
            {
                DiscordButtonComponent carButton = new(ButtonStyle.Primary, "CarButton", "Car", false, new DiscordComponentEmoji("🏎"));
                DiscordButtonComponent bikeButton = new(ButtonStyle.Primary, "BikeButton", "Bike", false, new DiscordComponentEmoji("🏍"));
                MsgBuilder = new DiscordMessageBuilder()
                    .WithContent($"{ctx.Member.Mention} **Select vehicle type!**")
                    .AddComponents(carButton, bikeButton);

                ChoiceMsg = await MsgBuilder.SendAsync(ctx.Channel);

                var Result = await ChoiceMsg.WaitForButtonAsync(ctx.User, TimeSpan.FromSeconds(30));

                if (Result.TimedOut)
                {
                    await ctx.RespondAsync($"{ctx.Member.Mention} You didn't select vehicle type in time.");
                    return;
                }
                else if (Result.Result.Id == "CarButton")
                {
                    SelectedVehicles = (from vl in VehicleList
                                        join dl in DisciplineList on vl.Discipline equals dl.ID_Discipline
                                        where dl.Discipline_Name == disciplinename
                                        where vl.Type == "car"
                                        select vl).ToList();
                    await Result.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                }
                else if (Result.Result.Id == "BikeButton")
                {
                    SelectedVehicles = (from vl in VehicleList
                                        join dl in DisciplineList on vl.Discipline equals dl.ID_Discipline
                                        where dl.Discipline_Name == disciplinename
                                        where vl.Type == "bike"
                                        select vl).ToList();
                    await Result.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
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
            embed.AddField("Crew Credits only?", $"{(SelectedVehicles[row].IsCCOnly ? "✅" : "❌")}", true);
            embed.AddField("Summit exclusive?", $"{(SelectedVehicles[row].IsSummitVehicle ? "✅" : "❌")}", true);
            embed.AddField("MP exclusive?", $"{(SelectedVehicles[row].IsMotorPassExclusive ? "✅" : "❌")}", true);
            if (MsgBuilder != null)
            {
                MsgBuilder.ClearComponents();
                MsgBuilder.AddEmbed(embed);
                MsgBuilder.WithContent($"*({SelectedVehicles.Count - 1} vehicles left in current rotation)*");
                await ChoiceMsg.ModifyAsync(MsgBuilder);
            }
            else
            {
                await ctx.RespondAsync($"*({SelectedVehicles.Count - 1} vehicles left in current rotation)*", embed);
            }
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

            List<DiscordButtonComponent> buttons = new() { new DiscordButtonComponent(ButtonStyle.Primary, "left", "Previous Page"), new DiscordButtonComponent(ButtonStyle.Primary, "right", "Next Page") };

            DiscordMessage TopMessage = await new DiscordMessageBuilder()
                .WithContent(CustomMethod.GetGlobalTop(ctx, (int)page))
                .WithReply(ctx.Message.Id, true)
                .AddComponents(buttons)
                .SendAsync(ctx.Channel);

            bool end = false;
            do
            {
                var result = TopMessage.WaitForButtonAsync(ctx.User, TimeSpan.FromSeconds(30));
                if (result.Result.TimedOut)
                {
                    end = result.Result.TimedOut;
                }
                else if (result.Result.Result.Id == "left")
                {
                    if (page > 1)
                    {
                        page--;
                        await TopMessage.ModifyAsync(CustomMethod.GetGlobalTop(ctx, (int)page));
                    }
                    await result.Result.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                }
                else if (result.Result.Result.Id == "right")
                {
                    page++;
                    try
                    {
                        await TopMessage.ModifyAsync(CustomMethod.GetGlobalTop(ctx, (int)page));
                    }
                    catch (Exception)
                    {
                        page--;
                    }
                    await result.Result.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                }
            } while (!end);
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
            List<DiscordButtonComponent> buttons = new() { new DiscordButtonComponent(ButtonStyle.Primary, "left", "Previous Page"), new DiscordButtonComponent(ButtonStyle.Primary, "right", "Next Page") };
            DiscordMessage TopMessage = await new DiscordMessageBuilder()
                .WithContent(CustomMethod.GetServerTop(ctx, (int)page))
                .WithReply(ctx.Message.Id, true)
                .AddComponents(buttons)
                .SendAsync(ctx.Channel);

            bool end = false;
            do
            {
                var result = TopMessage.WaitForButtonAsync(ctx.User, TimeSpan.FromSeconds(30));
                if (result.Result.TimedOut)
                {
                    end = result.Result.TimedOut;
                }
                else if (result.Result.Result.Id == "left")
                {
                    if (page > 1)
                    {
                        page--;
                        await TopMessage.ModifyAsync(CustomMethod.GetServerTop(ctx, (int)page));
                    }
                    await result.Result.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                }
                else if (result.Result.Result.Id == "right")
                {
                    page++;
                    try
                    {
                        await TopMessage.ModifyAsync(CustomMethod.GetServerTop(ctx, (int)page));
                    }
                    catch (Exception)
                    {
                        page--;
                    }
                    await result.Result.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
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
            List<DiscordButtonComponent> buttons = new() { new DiscordButtonComponent(ButtonStyle.Primary, "left", "Previous Page"), new DiscordButtonComponent(ButtonStyle.Primary, "right", "Next Page") };
            DiscordMessage TopMessage = await new DiscordMessageBuilder()
                .WithContent(CustomMethod.GetBackgroundList(ctx, (int)page))
                .AddComponents(buttons)
                .WithReply(ctx.Message.Id, true)
                .SendAsync(ctx.Channel);

            bool end = false;
            do
            {
                var result = TopMessage.WaitForButtonAsync(ctx.User, TimeSpan.FromSeconds(20));

                if (result.Result.TimedOut)
                {
                    end = result.Result.TimedOut;
                }
                else if (result.Result.Result.Id == "left")
                {
                    if (page > 1)
                    {
                        page--;
                        await TopMessage.ModifyAsync(CustomMethod.GetBackgroundList(ctx, (int)page));
                    }
                    await result.Result.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                }
                else if (result.Result.Result.Id == "right")
                {
                    page++;
                    try
                    {
                        await TopMessage.ModifyAsync(CustomMethod.GetBackgroundList(ctx, (int)page));
                    }
                    catch (Exception)
                    {
                        page--;
                    }
                    await result.Result.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
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
            if (dailyused == null || dailyused < DateTime.UtcNow.Date)
            {
                if (member.Id == ctx.Member.Id)
                {
                    user.Daily_Used = DateTime.UtcNow.ToString("ddMMyyyy");
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
                    user.Daily_Used = DateTime.UtcNow.ToString("ddMMyyyy");
                    receiver.Bucks += money;
                    DB.DBLists.UpdateLeaderboard(user);
                    DB.DBLists.UpdateLeaderboard(receiver);
                    await new DiscordMessageBuilder()
                        .WithContent($"{member.Mention}, You were given {money} bucks by {ctx.Member.Username}")
                         .WithReply(ctx.Message.Id)
                         .WithAllowedMention(new UserMention())
                         .SendAsync(ctx.Channel);
                }
            }
            else
            {
                DateTime now = DateTime.UtcNow;

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
                if (dailyused == null || dailyused < DateTime.UtcNow.Date)
                {
                    giver.Cookies_Used = DateTime.UtcNow.ToString("ddMMyyyy");
                    giver.Cookies_Given += 1;
                    reciever.Cookies_Taken += 1;
                    output = $"{member.Mention}, {ctx.Member.Username} has given you a :cookie:";
                    DB.DBLists.UpdateLeaderboard(giver);
                    DB.DBLists.UpdateLeaderboard(reciever);
                }
                else
                {
                    DateTime now = DateTime.UtcNow;

                    output = $"Time untill you can use cookie command again - {(24 - now.Hour) - 1}:{(60 - now.Minute) - 1}:{(60 - now.Second) - 1}.";
                }
            }
            await new DiscordMessageBuilder()
                .WithContent(output)
                .WithReply(ctx.Message.Id, false)
                .WithAllowedMention(new UserMention())
                .SendAsync(ctx.Channel);
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
            if (dailyused == null || dailyused < DateTime.UtcNow.Date)
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
            using (HttpClient wc = new())
            {
                HTML = await wc.GetStringAsync($"https://ubistatic-a.akamaihd.net/0115/tc2/status.html");
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
                await new DiscordMessageBuilder()
                    .WithContent($"{ctx.Member.Mention}, Could not contact you through DMs.")
                    .WithAllowedMention(new UserMention())
                    .SendAsync(ctx.Channel);
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
                DB.RoleTagSettings RT = Roles.FirstOrDefault(w => w.Channel_ID == ctx.Channel.Id);
                if (RT != null)
                {
                    DiscordRole role = ctx.Guild.GetRole(RT.Role_ID);
                    if (RT.Last_Used < DateTime.UtcNow - TimeSpan.FromMinutes(RT.Cooldown))
                    {
                        await new DiscordMessageBuilder()
                            .WithContent($"{role.Mention} - {ctx.Member.Mention}: {RT.Message}")
                            .WithReply(ctx.Message.Id, false)
                            .WithAllowedMention(new RoleMention(role))
                            .SendAsync(ctx.Channel);
                        RT.Last_Used = DateTime.UtcNow;
                        DB.DBLists.UpdateRoleTagSettings(RT);
                    }
                    else
                    {
                        TimeSpan remainingTime = TimeSpan.FromMinutes(RT.Cooldown) - (DateTime.UtcNow - RT.Last_Used);
                        await new DiscordMessageBuilder()
                            .WithContent($"This role can't be mentioned right now, cooldown has not passed yet. ({remainingTime.Hours} Hours {remainingTime.Minutes} Minutes {remainingTime.Seconds} Seconds left)")
                            .WithReply(ctx.Message.Id, true)
                            .SendAsync(ctx.Channel);
                    }
                }
                else
                {
                    await new DiscordMessageBuilder()
                        .WithContent($"This channel does not allow this role to be pinged")
                        .WithReply(ctx.Message.Id, true)
                        .SendAsync(ctx.Channel);
                }
            }
            else
            {
                await new DiscordMessageBuilder()
                    .WithContent($"This emoji does not represent a role in this server.")
                    .WithReply(ctx.Message.Id, true)
                    .SendAsync(ctx.Channel);
            }
        }
    }
}