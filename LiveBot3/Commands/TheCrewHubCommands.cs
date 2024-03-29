﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using LiveBot.Json;
using Newtonsoft.Json;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace LiveBot.Commands
{
    [Group("hub")]
    [Aliases("h")]
    [Description("Commands that use The Crew Hub info. Together with TCE support.")]
    internal class TheCrewHubCommands : BaseCommandModule
    {
        [Command("summit")]
        [Aliases("s")]
        [Cooldown(1, 300, CooldownBucketType.User)]
        [Description("Shows summit tier list and time left.")]
        public async Task Summit(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            string PCJson = string.Empty, XBJson = string.Empty, PSJson = string.Empty, StadiaJson = string.Empty;
            string imageLoc = $"{Program.tmpLoc}{ctx.User.Id}-summit.png";
            float outlineSize = 0.7f;
            byte[] SummitLogo;
            int[,] TierCutoff = new int[,] { { 4000, 8000, 15000 }, { 11000, 21000, 41000 }, { 2100, 4200, 8500 }, { 100, 200, 400 } };
            List<TCHubJson.Summit> JSummit = Program.JSummit;

            int platforms = 4;

            using (HttpClient wc = new())
            {
                PCJson = await wc.GetStringAsync($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/pc/profile/a92d844e-9c57-4b8c-a249-108ef42d4500");
                XBJson = await wc.GetStringAsync($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/x1/profile/a92d844e-9c57-4b8c-a249-108ef42d4500");
                PSJson = await wc.GetStringAsync($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/ps4/profile/a92d844e-9c57-4b8c-a249-108ef42d4500");
                try
                {
                    StadiaJson = await wc.GetStringAsync($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/stadia/profile/a92d844e-9c57-4b8c-a249-108ef42d4500");
                }
                catch (Exception)
                {
                    platforms = 3;
                }

                try
                {
                    SummitLogo = await wc.GetByteArrayAsync($"https://www.thecrew-hub.com/gen/assets/summits/{JSummit[0].Cover_Small}");
                }
                catch (WebException e)
                {
                    Program.Client.Logger.LogError(CustomLogEvents.CommandError, e, $"Summit logo download failed, substituting image.");
                    SummitLogo = File.ReadAllBytes("Assets/Summit/summit_small");
                }
            }
            TCHubJson.Rank[] Events = Array.Empty<TCHubJson.Rank>();
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
            DrawingOptions TierCutoffOptions = new()
            {
                TextOptions = new TextOptions()
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top
                }
            };

            using (Image<Rgba32> PCImg = Image.Load<Rgba32>("Assets/Summit/PC.jpeg"))
            using (Image<Rgba32> PSImg = Image.Load<Rgba32>("Assets/Summit/PS.jpg"))
            using (Image<Rgba32> XBImg = Image.Load<Rgba32>("Assets/Summit/XB.png"))
            using (Image<Rgba32> StadiaImg = Image.Load<Rgba32>("Assets/Summit/STADIA.png"))
            using (Image<Rgba32> BaseImg = new(300 * platforms, 643))
            {
                Image<Rgba32>[] PlatformImg = new Image<Rgba32>[4] { PCImg, PSImg, XBImg, StadiaImg };
                Parallel.For(0, Events.Length, (i, state) =>
                {
                    using Image<Rgba32> TierImg = Image.Load<Rgba32>("Assets/Summit/SummitBase.png");
                    using Image<Rgba32> SummitImg = Image.Load<Rgba32>(SummitLogo);
                    using Image<Rgba32> FooterImg = new(300, 30);

                    SummitImg.Mutate(ctx => ctx.Crop(300, SummitImg.Height));
                    Color TextColour = Color.WhiteSmoke;
                    Color OutlineColour = Color.DarkSlateGray;

                    Point SummitLocation = new(0 + (300 * i), 0);
                    Font Basefont = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 30);
                    Font FooterFont = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 15);
                    Font CutoffFont = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 17);

                    TierImg.Mutate(ctx => ctx
                    .DrawText(TierCutoffOptions, $"Top {TierCutoff[i, 0]}", CutoffFont, Brushes.Solid(TextColour), Pens.Solid(OutlineColour, outlineSize), new PointF(295, 340))
                    .DrawText(TierCutoffOptions, $"Top {TierCutoff[i, 1]}", CutoffFont, Brushes.Solid(TextColour), Pens.Solid(OutlineColour, outlineSize), new PointF(295, 410))
                    .DrawText(TierCutoffOptions, $"Top {TierCutoff[i, 2]}", CutoffFont, Brushes.Solid(TextColour), Pens.Solid(OutlineColour, outlineSize), new PointF(295, 480))
                    .DrawText(TierCutoffOptions, "All Participants", CutoffFont, Brushes.Solid(TextColour), Pens.Solid(OutlineColour, outlineSize), new PointF(295, 550))
                    .DrawLines(Color.Black, 1.5f, new PointF(0, 0), new PointF(TierImg.Width, 0), new PointF(TierImg.Width, TierImg.Height), new PointF(0, TierImg.Height))
                    );
                    FooterImg.Mutate(ctx => ctx
                    .Fill(Color.Black)
                    .DrawText($"TOTAL PARTICIPANTS: {Events[i].Player_Count}", FooterFont, TextColour, new PointF(10, 10))
                    );
                    BaseImg.Mutate(ctx => ctx
                        .DrawImage(SummitImg, SummitLocation, 1)
                        .DrawImage(TierImg, SummitLocation, 1)
                        .DrawText(pts[i, 3], Basefont, Brushes.Solid(TextColour), Pens.Solid(OutlineColour, outlineSize), new PointF(80 + (300 * i), 365))
                        .DrawText(pts[i, 2], Basefont, Brushes.Solid(TextColour), Pens.Solid(OutlineColour, outlineSize), new PointF(80 + (300 * i), 435))
                        .DrawText(pts[i, 1], Basefont, Brushes.Solid(TextColour), Pens.Solid(OutlineColour, outlineSize), new PointF(80 + (300 * i), 505))
                        .DrawText(pts[i, 0], Basefont, Brushes.Solid(TextColour), Pens.Solid(OutlineColour, outlineSize), new PointF(80 + (300 * i), 575))
                        .DrawImage(FooterImg, new Point(0 + (300 * i), 613), 1)
                        .DrawImage(PlatformImg[i], new Point(0 + (300 * i), 0), 1)
                        );
                });
                BaseImg.Save(imageLoc);
            }
            using var upFile = new FileStream(imageLoc, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
            var msgBuilder = new DiscordMessageBuilder
            {
                Content = $"Summit tier lists.\n  *Summit ends on <t:{JSummit[0].End_Date}>.*"
            };
            msgBuilder.WithFile(upFile);
            await ctx.RespondAsync(msgBuilder);
        }

        [Command("mysummit")]
        [Description("Shows your summit scores.")]
        [Aliases("sinfo", "summitinfo", "ms")]
        public async Task MySummit(CommandContext ctx, string platform = null)
        {
            await ctx.TriggerTypingAsync();
            await HubMethods.UpdateHubInfo();

            string OutMessage = string.Empty;
            string imageLoc = $"{Program.tmpLoc}{ctx.User.Id}-mysummit.png";

            bool SendImage = false;

            string search = string.Empty;

            TCHubJson.TceSummit JTCE = await CustomMethod.GetTCEInfo(ctx.User.Id);

            TCHubJson.TceSummitSubs UserInfo = new();

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
                else if (JTCE.Error == "TCHub down")
                {
                    OutMessage = $"{ctx.User.Mention}, The Crew Hub seems to be down, there might be a maintenance. Please try again later.";
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
                    DiscordMessageBuilder platformMSGBuilder = new();
                    List<DiscordComponent> Buttons = new();
                    Buttons.Add(new DiscordButtonComponent(ButtonStyle.Primary, "ps4", "PlayStation", !JTCE.Subs.Any(w => w.Platform.Contains("ps4")), new DiscordComponentEmoji(445234294676258816)));
                    Buttons.Add(new DiscordButtonComponent(ButtonStyle.Primary, "stadia", "Stadia", !JTCE.Subs.Any(w => w.Platform.Contains("stadia")), new DiscordComponentEmoji(697798396584656896)));
                    Buttons.Add(new DiscordButtonComponent(ButtonStyle.Primary, "x1", "Xbox One", !JTCE.Subs.Any(w => w.Platform.Contains("x1")), new DiscordComponentEmoji(445234294915334146)));
                    Buttons.Add(new DiscordButtonComponent(ButtonStyle.Primary, "pc", "PC", !JTCE.Subs.Any(w => w.Platform.Contains("pc")), new DiscordComponentEmoji(445234293019770900)));

                    DiscordMessage platformMSG = await platformMSGBuilder
                        .AddComponents(Buttons)
                        .WithContent($"{ctx.User.Mention}, you have multiple platforms stored on TCE, please select platform you want to see the scores for.")
                        .SendAsync(ctx.Channel);

                    var Result = await platformMSG.WaitForButtonAsync(ctx.User, TimeSpan.FromSeconds(30));
                    await Result.Result.Interaction.CreateResponseAsync(DSharpPlus.InteractionResponseType.DeferredMessageUpdate);
                    if (Result.TimedOut)
                    {
                        await platformMSG.ModifyAsync("Nothing selected, defaulting to PC");
                        platform = "pc";
                    }
                    else
                    {
                        switch (Result.Result.Id)
                        {
                            case "ps4":
                                await platformMSG.ModifyAsync("Playstation Platform selected.");
                                platform = "ps";
                                break;

                            case "x1":
                                await platformMSG.ModifyAsync("Xbox Platform selected.");
                                platform = "x1";
                                break;

                            case "stadia":
                                await platformMSG.ModifyAsync("Stadia Platform selected.");
                                platform = "stadia";
                                break;

                            default:
                                await platformMSG.ModifyAsync("PC Platform selected.");
                                platform = "pc";
                                break;
                        }
                    }
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
                using (HttpClient wc = new())
                {
                    SJson = await wc.GetStringAsync($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/{UserInfo.Platform}/profile/{UserInfo.Profile_ID}");
                }
                TCHubJson.Rank Events = JsonConvert.DeserializeObject<TCHubJson.Rank>(SJson);

                if (Events.Points != 0)
                {
                    int[,] WidthHeight = new int[,] { { 0, 0 }, { 249, 0 }, { 498, 0 }, { 0, 249 }, { 373, 249 }, { 0, 493 }, { 373, 493 }, { 747, 0 }, { 747, 249 } };
                    Font SummitCaps15 = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 15);
                    Font SummitCaps12 = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 12.5f);
                    var AllignTopLeft = new TextOptions()
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    };

                    Image<Rgba32> BaseImage = new(1127, 765);
                    await Parallel.ForEachAsync(JSummit[0].Events, new ParallelOptions(), async (Event, token) =>
                    {
                        int i = JSummit[0].Events.Select((element, index) => new { element, index })
                               .FirstOrDefault(x => x.element.Equals(Event))?.index ?? -1;
                        Image image = await HubMethods.BuildEventImage(
                                Event,
                                Events,
                                new TCHubJson.TceSummitSubs { Platform = UserInfo.Platform, Profile_ID = UserInfo.Profile_ID },
                                Event.Image_Byte,
                                i == 7,
                                i == 8);
                        BaseImage.Mutate(ctx => ctx
                        .DrawImage(
                            image,
                            new Point(WidthHeight[i, 0], WidthHeight[i, 1]),
                        1)
                    );
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
                    BaseImage.Save(imageLoc);

                    OutMessage = $"{ctx.User.Mention}, Here are your summit event stats for {(UserInfo.Platform == "x1" ? "Xbox" : UserInfo.Platform == "ps4" ? "PlayStation" : UserInfo.Platform == "stadia" ? "Stadia" : "PC")}.\n*Summit ends on <t:{JSummit[0].End_Date}>. Scoreboard powered by The Crew Hub and The Crew Exchange!*";
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

                var msgBuilder = new DiscordMessageBuilder
                {
                    Content = OutMessage
                };
                msgBuilder.WithFile(upFile);
                await ctx.RespondAsync(msgBuilder);
            }
            else
            {
                await ctx.RespondAsync(OutMessage);
            }
        }

        [Command("topsummit")]
        [Aliases("ts")]
        [Description("Shows the summit board with all the world record scores.")]
        [Cooldown(1, 30, CooldownBucketType.User)]
        public async Task TopSummit(CommandContext ctx, string platform = null)
        {
            await ctx.TriggerTypingAsync();
            await HubMethods.UpdateHubInfo();

            int TotalPoints = 0;

            string OutMessage = string.Empty;
            string imageLoc = $"{Program.tmpLoc}{ctx.User.Id}-topsummit.png";

            bool alleventscompleted = true;

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

            int[,] WidthHeight = new int[,] { { 0, 0 }, { 249, 0 }, { 498, 0 }, { 0, 249 }, { 373, 249 }, { 0, 493 }, { 373, 493 }, { 747, 0 }, { 747, 249 } };

            using Image<Rgba32> BaseImage = new(1127, 735);
            await Parallel.ForEachAsync(JSummit[0].Events, new ParallelOptions(), async (Event, token) =>
            {
                using HttpClient wc = new();
                int i = JSummit[0].Events.Select((element, index) => new { element, index })
                       .FirstOrDefault(x => x.element.Equals(Event))?.index ?? -1;
                string ActivityString = await wc.GetStringAsync($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/leaderboard/{platform}/{Event.ID}?page_size=1", CancellationToken.None);
                TCHubJson.SummitLeaderboard Activity = JsonConvert.DeserializeObject<TCHubJson.SummitLeaderboard>(ActivityString);
                TCHubJson.Rank Rank = JsonConvert.DeserializeObject<TCHubJson.Rank>(await wc.GetStringAsync($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/{platform}/profile/{Activity.Entries[0].Profile_ID}", CancellationToken.None));
                Image image = await HubMethods.BuildEventImage(
                        Event,
                        Rank,
                        new TCHubJson.TceSummitSubs { Platform = platform, Profile_ID = Activity.Entries[0].Profile_ID },
                        Event.Image_Byte,
                        i == 7,
                        i == 8);
                BaseImage.Mutate(ctx => ctx
                .DrawImage(
                    image,
                    new Point(WidthHeight[i, 0], WidthHeight[i, 1]),
                    1)
                );
                TotalPoints += Activity.Entries[0].Points;
            });

            if (alleventscompleted)
            {
                TotalPoints += 100000;
            }
            BaseImage.Save(imageLoc);
            OutMessage = $"{ctx.User.Mention}, Here are the top summit scores for {(platform == "x1" ? "Xbox" : platform == "ps4" ? "PlayStation" : platform == "stadia" ? "Stadia" : "PC")}. Total event points: **{TotalPoints}**\n*Summit ends on <t:{JSummit[0].End_Date}>. Scoreboard powered by The Crew Hub and The Crew Exchange!*";

            using var upFile = new FileStream(imageLoc, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
            var msgBuilder = new DiscordMessageBuilder
            {
                Content = OutMessage
            };
            msgBuilder.WithFile(upFile);
            await ctx.RespondAsync(msgBuilder);
        }

        [Command("summitrewards")]
        [Aliases("srewards", "sr")]
        [Cooldown(1, 25, CooldownBucketType.User)]
        [Description("Shows this weeks summit rewards")]
        public async Task SummitRewards(CommandContext ctx, int summit = 1)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            await HubMethods.UpdateHubInfo();

            if (summit < 1 || summit > 4)
            {
                summit = 1;
            }
            summit--;

            Color[] RewardColours = new Color[] { Rgba32.ParseHex("#0060A9"), Rgba32.ParseHex("#D5A45F"), Rgba32.ParseHex("#C2C2C2"), Rgba32.ParseHex("#B07C4D") };

            string imageLoc = $"{Program.tmpLoc}{ctx.User.Id}-summitrewards.png";
            int RewardWidth = 412;
            TCHubJson.Reward[] Rewards = Program.JSummit[summit].Rewards;
            Font Font = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 25);
            using (Image<Rgba32> RewardsImage = new(4 * RewardWidth, 328))
            {
                Parallel.For(0, Rewards.Length, (i, state) =>
                {
                    string RewardTitle = string.Empty;

                    Image<Rgba32>
                                affix1 = new(1, 1),
                                affix2 = new(1, 1),
                                affixbonus = new(1, 1);
                    bool isParts = false;
                    switch (Rewards[i].Type)
                    {
                        case "phys_part":
                            string
                                   affix1name = Regex.Replace(Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("affix1")).Value ?? "unknown", "\\w{0,}_", string.Empty),
                                   affix2name = Regex.Replace(Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("affix2")).Value ?? "unknown", "\\w{0,}_", string.Empty),
                                   affixBonusName = Regex.Replace(Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("bonus_icon")).Value ?? "unknown", "\\w{0,}_", string.Empty);
                            try
                            {
                                affix1 = Image.Load<Rgba32>($"Assets/Affix/{affix1name.ToLower()}.png");
                            }
                            catch
                            {
                                affix1 = Image.Load<Rgba32>($"Assets/Affix/unknown.png");
                            }
                            try
                            {
                                affix2 = Image.Load<Rgba32>($"Assets/Affix/{affix2name.ToLower()}.png");
                            }
                            catch
                            {
                                affix2 = Image.Load<Rgba32>($"Assets/Affix/unknown.png");
                            }
                            try
                            {
                                affixbonus = Image.Load<Rgba32>($"Assets/Affix/{affixBonusName.ToLower()}.png");
                            }
                            catch
                            {
                                affixbonus = Image.Load<Rgba32>($"Assets/Affix/unknown.png");
                            }
                            string boosted = string.Empty;
                            if (Rewards[i].Debug_Subtitle == "BOOSTED")
                            {
                                boosted = "BOOSTED ";
                            }
                            RewardTitle = $"{boosted}{HubMethods.NameIDLookup(Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("quality_text_id")).Value)} " +
                            $"{affixBonusName} " +
                            $"{Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("type")).Value}" +
                            $"({Regex.Replace(Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("vcat_icon")).Value ?? "unknown", "\\w{0,}_", string.Empty)})";

                            isParts = true;
                            break;

                        case "vanity":
                            RewardTitle = HubMethods.NameIDLookup(Rewards[i].Title_Text_ID);
                            if (RewardTitle is null)
                            {
                                if (Rewards[i].Img_Path.Contains("emote"))
                                {
                                    RewardTitle = "Emote";
                                }
                                else
                                {
                                    RewardTitle = "[unknown]";
                                }
                            }
                            break;

                        case "generic":
                            RewardTitle = Rewards[i].Debug_Title;
                            break;

                        case "currency":
                            RewardTitle = $"{Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("currency_type")).Value} - {Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("currency_amount")).Value}";
                            break;

                        case "vehicle":
                            RewardTitle = $"{HubMethods.NameIDLookup(Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("brand_text_id")).Value)} - {HubMethods.NameIDLookup(Rewards[i].Extra.FirstOrDefault(w => w.Key.Equals("model_text_id")).Value)}";
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

                    using Image<Rgba32> RewardImage = Image.Load<Rgba32>(HubMethods.RewardsImageBitArr[summit, i]);
                    using Image<Rgba32> TopBar = new(RewardImage.Width, 20);
                    TopBar.Mutate(ctx => ctx.
                    Fill(RewardColours[i])
                    );
                    TextOptions TextOptions = new()
                    {
                        WrapTextWidth = RewardWidth
                    };
                    RewardsImage.Mutate(ctx => ctx
                    .DrawImage(RewardImage, new Point((4 - Rewards[i].Level) * RewardWidth, 0), 1)
                    .DrawImage(TopBar, new Point((4 - Rewards[i].Level) * RewardWidth, 0), 1)
                    .DrawText(new DrawingOptions { TextOptions = TextOptions }, RewardTitle, Font, Brushes.Solid(Color.White), Pens.Solid(Color.Black, 1f), new PointF(((4 - Rewards[i].Level) * RewardWidth) + 5, 15))
                    );
                    if (isParts)
                    {
                        RewardsImage.Mutate(ctx => ctx
                        .DrawImage(affix1, new Point((4 - Rewards[i].Level) * RewardWidth, RewardImage.Height - affix1.Height), 1)
                        .DrawImage(affix2, new Point((4 - Rewards[i].Level) * RewardWidth + affix1.Width, RewardImage.Height - affix2.Height), 1)
                        .DrawImage(affixbonus, new Point((4 - Rewards[i].Level) * RewardWidth + affix1.Width + affix2.Width, RewardImage.Height - affixbonus.Height), 1)
                        );
                    }
                });
                RewardsImage.Save(imageLoc);
            }
            using var upFile = new FileStream(imageLoc, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
            var msgBuilder = new DiscordMessageBuilder
            {
                Content = $"{ctx.User.Mention}, here are {(summit == 0 ? "this week" : summit == 1 ? "next week" : "future weeks")} summit rewards:"
            };
            msgBuilder.WithFile(upFile);
            await ctx.RespondAsync(msgBuilder);
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

            TCHubJson.TceSummit JTCE;
            using HttpClient wc = new();
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

            TCHubJson.TceSummitSubs UserInfo = new();

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
                TCHubJson.Fame Fame = JsonConvert.DeserializeObject<TCHubJson.Fame>(await wc.GetStringAsync($"https://api.thecrew-hub.com/v1/leaderboard/{UserInfo.Platform}/fame?profile_id={UserInfo.Profile_ID}"));
                var HubUserInfo = Fame.Scores.FirstOrDefault(w => w.Profile_ID.Equals(UserInfo.Profile_ID));
                OutMessage = $"{ctx.User.Mention}, Your follower count is **{HubUserInfo.Score}**. Your Icon Level is **[WIP]**. You are ranked **{HubUserInfo.Rank} on {search}**";
            }
            catch (WebException)
            {
                OutMessage = $"{ctx.User.Mention}, The Crew HUB API is currently unavailable.";
            }

            await ctx.RespondAsync(OutMessage);
        }

        [Command("events")]
        [RequireRoles(RoleCheckMode.Any, "Patreon", "Discord-Moderator", "Ubisoft")]
        public async Task HubEvent(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            StringBuilder Families = new();
            Families.AppendLine("```csharp");
            List<TCHubJson.Family> FamilyList = Program.TCHub.Families.OrderBy(w => w.ID).ToList();
            for (int i = 0; i < FamilyList.Count; i++)
            {
                Families.AppendLine($"#{i} - {HubMethods.NameIDLookup(FamilyList[i].Text_ID)}");
            }
            Families.AppendLine("```");
            await ctx.RespondAsync(Families.ToString());
            var FamilyIdMsg = await interactivity.WaitForMessageAsync(w => w.Author == ctx.User && w.Channel == ctx.Channel, TimeSpan.FromSeconds(30));
            if (!FamilyIdMsg.TimedOut)
            {
                bool isNumber = int.TryParse(FamilyIdMsg.Result.Content, out int familyID);
                if (isNumber && familyID >= 0 && familyID < FamilyList.Count)
                {
                    StringBuilder Disciplines = new();
                    List<TCHubJson.Discipline> DisciplinesList = Program.TCHub.Disciplines.Where(w => w.Family_ID == FamilyList[familyID].ID).OrderBy(w => w.ID).ToList();
                    Disciplines.AppendLine("```csharp");
                    for (int i = 0; i < DisciplinesList.Count; i++)
                    {
                        Disciplines.AppendLine($"#{i} - {HubMethods.NameIDLookup(DisciplinesList[i].Text_ID)}");
                    }
                    Disciplines.AppendLine("```");

                    await ctx.RespondAsync($"{ctx.User.Mention}\n{Disciplines}");

                    var DisciplineIdMsg = await interactivity.WaitForMessageAsync(w => w.Author == ctx.User && w.Channel == ctx.Channel, TimeSpan.FromSeconds(30));
                    if (!DisciplineIdMsg.TimedOut)
                    {
                        isNumber = int.TryParse(DisciplineIdMsg.Result.Content, out int disciplineID);
                        if (isNumber && disciplineID >= 0 && disciplineID < DisciplinesList.Count)
                        {
                            List<TCHubJson.Mission> MissionList = Program.TCHub.Missions.Where(w => w.Discipline_ID == DisciplinesList[disciplineID].ID).OrderBy(w => w.ID).ToList();
                            int page = 1;
                            bool end = false;

                            DiscordEmoji left = DiscordEmoji.FromName(ctx.Client, ":arrow_left:");
                            DiscordEmoji right = DiscordEmoji.FromName(ctx.Client, ":arrow_right:");
                            DiscordMessage MissionMsg = await ctx.RespondAsync($"{CustomMethod.GetMissionList(MissionList, page)}");
                            await MissionMsg.CreateReactionAsync(left);
                            await Task.Delay(300).ContinueWith(t => MissionMsg.CreateReactionAsync(right));
                            do
                            {
                                var reactionResult = MissionMsg.WaitForReactionAsync(ctx.User, TimeSpan.FromSeconds(30));
                                if (reactionResult.Result.TimedOut)
                                {
                                    end = reactionResult.Result.TimedOut;
                                }
                                else if (reactionResult.Result.Result.Emoji == left)
                                {
                                    await MissionMsg.DeleteReactionAsync(reactionResult.Result.Result.Emoji, ctx.User);
                                    if (page > 1)
                                    {
                                        page--;
                                        await MissionMsg.ModifyAsync(CustomMethod.GetMissionList(MissionList, page));
                                    }
                                }
                                else if (reactionResult.Result.Result.Emoji == right)
                                {
                                    await MissionMsg.DeleteReactionAsync(reactionResult.Result.Result.Emoji, ctx.User);
                                    page++;
                                    try
                                    {
                                        await MissionMsg.ModifyAsync(CustomMethod.GetMissionList(MissionList, page));
                                    }
                                    catch (Exception)
                                    {
                                        page--;
                                    }
                                }
                            }
                            while (!end);
                        }
                        else
                        {
                            await ctx.RespondAsync($"{ctx.User.Mention}, Oops, something went wrong. Wrong ID");
                        }
                    }
                    else
                    {
                        await ctx.RespondAsync($"{ctx.User.Mention}, The request timed out, you didn't specify the Discipline ID");
                    }
                }
                else
                {
                    await ctx.RespondAsync($"{ctx.User.Mention}, Oops, something went wrong. Wrong ID");
                }
            }
            else
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, The request timed out, you didn't specify the Family ID");
            }
        }

        [Command("weather")]
        [Aliases("w")]
        [Description("Outputs the weather schedule for the next hour")]
        public async Task Weather(CommandContext ctx)
        {
            StringBuilder sb = new();
            TimeSpan now = DateTime.UtcNow.TimeOfDay;
            TimeSpan CurrentTime = new(now.Hours, now.Minutes, 0);
            TimeSpan TimeNow = new(now.Hours, now.Minutes, 0);
            string weathercondition = string.Empty;

            var Weather = DB.DBLists.WeatherSchedule.Where(w => w.Time >= CurrentTime && w.Day.Equals((int)DateTime.Today.DayOfWeek)).OrderBy(o => o.Time).ToList();
            if (Weather.Count < 60)
            {
                int day = (int)DateTime.Today.DayOfWeek + 1;
                if (day is 7)
                {
                    day = 0;
                }
                try
                {
                    Weather.AddRange(DB.DBLists.WeatherSchedule.Where(w => w.Day.Equals(day)).OrderBy(o => o.Time).ToList().GetRange(0, 61 - Weather.Count));
                }
                catch (Exception)
                {
                    Weather.AddRange(DB.DBLists.WeatherSchedule.Where(w => w.Day.Equals(day)).OrderBy(o => o.Time).ToList());
                }
            }
            sb.AppendLine($"**--------------------------------------------------------**");
            CurrentTime = CurrentTime.Add(TimeSpan.FromMinutes(59));
            for (int i = 0; i < 60; i++)
            {
                var WeatherSpeciffic = Weather.FirstOrDefault(w => w.Time.Hours.Equals(CurrentTime.Hours) && w.Time.Minutes.Equals(CurrentTime.Minutes));
                if (WeatherSpeciffic is null)
                {
                    sb.AppendLine($"{CurrentTime:hh\\:mm} - Weather is unknown");
                }
                else
                {
                    switch (WeatherSpeciffic.Weather)
                    {
                        case "clear":
                            weathercondition = ":sunny: **Clear**";
                            break;

                        case "*":
                            weathercondition = ":fog: **Fog**";
                            break;

                        case "rain":
                            weathercondition = ":cloud_rain: **Rain**";
                            break;

                        case "rain*":
                            weathercondition = ":fog::cloud_rain: **Fog and Rain**";
                            break;

                        case "snow":
                            weathercondition = ":snowflake: **Snow**";
                            break;

                        case "snow*":
                            weathercondition = ":fog::snowflake: **Fog and Snow**";
                            break;
                    }
                    sb.AppendLine($"{WeatherSpeciffic.Time:hh\\:mm} - {weathercondition}");
                }
                CurrentTime -= TimeSpan.FromMinutes(1);
            }
            sb.AppendLine($"Current UTC time is {TimeNow:hh\\:mm}. Here is the weather for the upcoming hour!");
            sb.AppendLine($"**--------------------------------------------------------**");

            await new DiscordMessageBuilder()
                .WithContent(sb.ToString())
                .WithReply(ctx.Message.Id, true)
                .SendAsync(ctx.Channel);
        }
    }
}