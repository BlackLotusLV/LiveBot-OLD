﻿using DSharpPlus.SlashCommands;
using LiveBot.Json;
using Newtonsoft.Json;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace LiveBot.SlashCommands
{
    [SlashCommandGroup("Hub", "Commands in relation to the TheCrew-Hub leaderboards.")]
    public class SlashTheCrewHubCommands : ApplicationCommandModule
    {
        [SlashCommand("Summit", "Shows the tiers and current cut offs for the ongoing summit.")]
        public async Task Summit(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder { Content = "Gathering data and building image." }));
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
                    Program.Client.Logger.LogError(CustomLogEvents.CommandError, e.Message, $"Summit logo download failed, substituting image.");
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
            DiscordFollowupMessageBuilder msgBuilder = new()
            {
                Content = $"Summit tier lists.\n *Summit ends on <t:{JSummit[0].End_Date}>.*"
            };
            msgBuilder.AddFile(upFile);
            msgBuilder.AddMention(new UserMention());
            await ctx.FollowUpAsync(msgBuilder);
        }

        [SlashCommand("my-summit", "Shows your summit scores.")]
        public async Task MySummit(InteractionContext ctx, [Option("platform", "Which platform leaderboard you want to see")] Platforms platform = Platforms.pc)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder { Content = "Gathering data and building image." }));
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
                switch (platform)
                {
                    case Platforms.pc:
                        search = "pc";
                        break;

                    case Platforms.x1:
                        search = "x1";
                        break;

                    case Platforms.ps4:
                        search = "ps4";
                        break;

                    case Platforms.stadia:
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

                var msgBuilder = new DiscordFollowupMessageBuilder
                {
                    Content = OutMessage
                };
                msgBuilder.AddFile(upFile);
                msgBuilder.AddMention(new UserMention());
                await ctx.FollowUpAsync(msgBuilder);
            }
            else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder { Content = OutMessage });
            }
        }

        [SlashCommand("Top-Summit", "Shows the summit board with all the world record scores.")]
        public async Task TopSummit(InteractionContext ctx, [Option("platform", "Which platform leaderboard you want to see")] Platforms platform = Platforms.pc)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder { Content = "Gathering data and building image." }));
            await HubMethods.UpdateHubInfo();

            int TotalPoints = 0;

            string OutMessage = string.Empty;
            string imageLoc = $"{Program.tmpLoc}{ctx.User.Id}-topsummit.png";
            string search = string.Empty;

            bool alleventscompleted = true;

            switch (platform)
            {
                case Platforms.pc:
                    search = "pc";
                    break;

                case Platforms.x1:
                    search = "x1";
                    break;

                case Platforms.ps4:
                    search = "ps4";
                    break;

                case Platforms.stadia:
                    search = "stadia";
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
                TCHubJson.SummitLeaderboard Activity = JsonConvert.DeserializeObject<TCHubJson.SummitLeaderboard>(await wc.GetStringAsync($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/leaderboard/{search}/{Event.ID}?page_size=1", CancellationToken.None));
                TCHubJson.Rank Rank = JsonConvert.DeserializeObject<TCHubJson.Rank>(await wc.GetStringAsync($"https://api.thecrew-hub.com/v1/summit/{JSummit[0].ID}/score/{search}/profile/{Activity.Entries[0].Profile_ID}", CancellationToken.None));
                Image image = await HubMethods.BuildEventImage(
                        Event,
                        Rank,
                        new TCHubJson.TceSummitSubs { Platform = search, Profile_ID = Activity.Entries[0].Profile_ID },
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
            OutMessage = $"{ctx.User.Mention}, Here are the top summit scores for {(search == "x1" ? "Xbox" : search == "ps4" ? "PlayStation" : search == "stadia" ? "Stadia" : "PC")}. Total event points: **{TotalPoints}**\n*Summit ends on <t:{JSummit[0].End_Date}>. Scoreboard powered by The Crew Hub and The Crew Exchange!*";

            using var upFile = new FileStream(imageLoc, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
            var msgBuilder = new DiscordFollowupMessageBuilder
            {
                Content = OutMessage
            };
            msgBuilder.AddFile(upFile);
            msgBuilder.AddMention(new UserMention());
            await ctx.FollowUpAsync(msgBuilder);
        }

        public enum Platforms
        {
            [ChoiceName("PC")]
            pc,

            [ChoiceName("PlayStation")]
            ps4,

            [ChoiceName("Xbox")]
            x1,

            [ChoiceName("Stadia")]
            stadia,
        }
    }
}