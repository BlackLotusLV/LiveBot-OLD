using LiveBot.Json;
using Newtonsoft.Json;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Net;
using System.Net.Http;

namespace LiveBot
{
    internal static class HubMethods
    {
        private static DateTime TCHubLastUpdated;
        public static byte[,][] RewardsImageBitArr { get; set; } = new byte[4, 4][];

        public static async Task UpdateHubInfo(bool forced = false)
        {
            List<TCHubJson.Summit> JSummit;
            DateTime endtime;
            using HttpClient wc = new();
            bool Connected = true;
            string JSummitString = string.Empty;
            try
            {
                JSummitString = await wc.GetStringAsync(Program.TCHubJson.Summit);
            }
            catch (WebException e)
            {
                Connected = false;
                Program.Client.Logger.LogInformation(CustomLogEvents.TCHub, e, "Connection error. Either wrong API link, or the Hub is down.");
            }
            if (Connected)
            {
                JSummit = JsonConvert.DeserializeObject<List<TCHubJson.Summit>>(JSummitString);
                if (forced)
                {
                    endtime = TCHubLastUpdated;
                }
                else
                {
                    endtime = CustomMethod.EpochConverter(JSummit[0].End_Date * 1000);
                }
                if (endtime != TCHubLastUpdated || forced)
                {
                    TCHubLastUpdated = endtime;
                    Program.TCHubDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(await wc.GetStringAsync(Program.TCHubJson.Dictionary));
                    Program.JSummit = JSummit;
                    Program.TCHub = JsonConvert.DeserializeObject<TCHubJson.TCHub>(await wc.GetStringAsync(Program.TCHubJson.GameData));
                    await Parallel.ForEachAsync(JSummit[0].Events, new ParallelOptions(), async (Event, Token) =>
                    {
                        JSummit[0].Events.FirstOrDefault(w => w.ID == Event.ID).Image_Byte = await wc.GetByteArrayAsync($"https://www.thecrew-hub.com/gen/assets/summits/{Event.Img_Path}", Token);
                    });
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < JSummit[i].Rewards.Length; j++)
                        {
                            RewardsImageBitArr[i, j] = await wc.GetByteArrayAsync($"https://www.thecrew-hub.com/gen/assets/summits/{JSummit[i].Rewards[j].Img_Path}");
                        }
                    }
                    Program.Client.Logger.LogInformation(CustomLogEvents.TCHub, "Info downloaded for {SummitId} summit.", JSummit[0].Summit_ID);
                }
            }
        }

        public static async Task DownloadHubNews()
        {
            using HttpClient wc = new();
            wc.DefaultRequestHeaders.Add("Ubi-AppId", "dda77324-f9d6-44ea-9ecb-30e57b286f6d");
            wc.DefaultRequestHeaders.Add("Ubi-localeCode", "us-en");
            string NewsString = string.Empty;
            bool Connected = true;

            try
            {
                NewsString = await wc.GetStringAsync(Program.TCHubJson.News);
            }
            catch (WebException e)
            {
                Connected = false;
                Program.Client.Logger.LogInformation(CustomLogEvents.TCHub, e, "Connection error. Either wrong API link, or the Hub is down.");
            }
            if (Connected)
            {
                Program.TCHub = JsonConvert.DeserializeObject<TCHubJson.TCHub>(NewsString);
            }
        }

        public static string NameIDLookup(string ID)
        {
            string HubText = Program.TCHubDictionary.FirstOrDefault(w => w.Key.Equals(ID)).Value ?? "[Item Name Missing]";
            HubText = HubText.Replace("&#8209;", "-");
            return HubText;
        }

        public static async Task<Image<Rgba32>> BuildEventImage(TCHubJson.Event Event, TCHubJson.Rank Rank, TCHubJson.TceSummitSubs UserInfo, byte[] EventImageBytes, bool isCorner = false, bool isSpecial = false)
        {
            Image<Rgba32> EventImage = Image.Load<Rgba32>(EventImageBytes);

            TCHubJson.Activities Activity = Rank.Activities.FirstOrDefault(w => w.Activity_ID.Equals(Event.ID.ToString()));
            if (Event.Is_Mission && !isSpecial && !isCorner)
            {
                EventImage.Mutate(ctx => ctx
                .Resize(368, 239)
                    );
            }
            else if (isCorner)
            {
                EventImage.Mutate(ctx => ctx
                .Resize(380, 245)
                );
            }
            else if (isSpecial)
            {
                EventImage.Mutate(ctx => ctx
                    .Resize(380, 483)
                    );
            }
            Font Basefont = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 18);
            Font SummitCaps15 = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 15);
            Font VehicleFont = Program.Fonts.CreateFont("HurmeGeometricSans3W03-Blk", 11.5f);
            if (Activity != null)
            {
                using HttpClient wc = new();
                TextOptions EventTitleOptions = new()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    WrapTextWidth = EventImage.Width - 10,
                    LineSpacing = 0.7f
                };
                TextOptions AllignTopLeft = new()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
                TextOptions AllignTopRight = new()
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top
                };

                string ThisEventNameID = string.Empty;
                if (Event.Is_Mission)
                {
                    ThisEventNameID = Program.TCHub.Missions.Where(w => w.ID == Event.ID).Select(s => s.Text_ID).FirstOrDefault();
                }
                else
                {
                    ThisEventNameID = Program.TCHub.Skills.Where(w => w.ID == Event.ID).Select(s => s.Text_ID).FirstOrDefault();
                }
                TCHubJson.SummitLeaderboard leaderboard = JsonConvert.DeserializeObject<TCHubJson.SummitLeaderboard>(await wc.GetStringAsync($"https://api.thecrew-hub.com/v1/summit/{Program.JSummit[0].ID}/leaderboard/{UserInfo.Platform}/{Event.ID}?profile={UserInfo.Profile_ID}"));
                string
                    EventTitle = (NameIDLookup(ThisEventNameID)),
                    ActivityResult = $"Score: {Activity.Score}",
                    VehicleInfo = string.Empty;
                TCHubJson.SummitLeaderboardEntries Entries = leaderboard.Entries.FirstOrDefault(w => w.Profile_ID == UserInfo.Profile_ID);
                if (Event.Constraint_Text_ID.Contains("60871"))
                {
                    VehicleInfo = "Forced Vehicle";
                }
                else
                {
                    TCHubJson.Model Model = Program.TCHub.Models.FirstOrDefault(w => w.ID == Entries.Vehicle_ID);
                    TCHubJson.Brand Brand;
                    if (Model != null)
                    {
                        Brand = Program.TCHub.Brands.FirstOrDefault(w => w.ID == Model.Brand_ID);
                    }
                    else
                    {
                        Brand = null;
                    }
                    VehicleInfo = $"{NameIDLookup(Brand != null ? Brand.Text_ID : "not found")} - {NameIDLookup(Model != null ? Model.Text_ID : "not found")}";
                }
                if (leaderboard.Score_Format == "time")
                {
                    ActivityResult = $"Time: {CustomMethod.ScoreToTime(Activity.Score)}";
                }
                else if (EventTitle.Contains("SPEEDTRAP"))
                {
                    ActivityResult = $"Speed: {Activity.Score.ToString().Insert(3, ".")} km/h";
                }
                else if (EventTitle.Contains("ESCAPE"))
                {
                    ActivityResult = $"Distance: {Activity.Score}m";
                }

                using Image<Rgba32> TitleBar = new(EventImage.Width, 40);
                using Image<Rgba32> ScoreBar = new(EventImage.Width, 60);
                ScoreBar.Mutate(ctx => ctx.Fill(Color.Black));
                TitleBar.Mutate(ctx => ctx.Fill(Color.Black));
                EventImage.Mutate(ctx => ctx
                    .DrawImage(ScoreBar, new Point(0, EventImage.Height - ScoreBar.Height), 0.7f)
                    .DrawImage(TitleBar, new Point(0, 0), 0.7f)
                    .DrawText(new DrawingOptions { TextOptions = EventTitleOptions }, EventTitle, SummitCaps15, Color.White, new PointF(5, 0))
                    .DrawText(new DrawingOptions { TextOptions = AllignTopLeft }, $"Rank: {Activity.Rank + 1}", Basefont, Color.White, new PointF(5, EventImage.Height - 22))
                    .DrawText(new DrawingOptions { TextOptions = AllignTopRight }, ActivityResult, Basefont, Color.White, new PointF(EventImage.Width - 5, EventImage.Height - 42))
                    .DrawText(new DrawingOptions { TextOptions = AllignTopRight }, $"Points: {Activity.Points}", Basefont, Color.White, new PointF(EventImage.Width - 5, EventImage.Height - 22))
                    .DrawText(new DrawingOptions { TextOptions = EventTitleOptions }, VehicleInfo, VehicleFont, Color.White, new PointF(5, EventImage.Height - 62))
                    );
                Parallel.For(0, Event.Modifiers.Length, (i, state) =>
                {
                    Image<Rgba32> ModifierImg = new(1, 1);
                    try
                    {
                        ModifierImg = Image.Load<Rgba32>($"Assets/Summit/Modifiers/{Event.Modifiers[i]}.png");
                    }
                    catch (Exception)
                    {
                        ModifierImg = Image.Load<Rgba32>($"Assets/Summit/Modifiers/unknown.png");
                    }
                    Image<Rgba32> ModifierBackground = new(ModifierImg.Width, ModifierImg.Height);
                    ModifierBackground.Mutate(ctx => ctx.Fill(Color.Black));
                    var modifierPoint = new Point(i * ModifierImg.Width + 20, TitleBar.Height + 10);
                    EventImage.Mutate(ctx => ctx
                    .DrawImage(ModifierBackground, modifierPoint, 0.7f)
                    .DrawImage(ModifierImg, modifierPoint, 1f));
                });
            }
            else
            {
                TextOptions AllignCenter = new()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top
                };
                using Image<Rgba32> NotComplete = new(EventImage.Width, EventImage.Height);
                NotComplete.Mutate(ctx => ctx
                    .Fill(Color.Black)
                    .DrawText(new DrawingOptions { TextOptions = AllignCenter }, "Event not completed!", Basefont, Color.White, new PointF(NotComplete.Width / 2, NotComplete.Height / 2))
                    );
                EventImage.Mutate(ctx => ctx
                .DrawImage(NotComplete, new Point(0, 0), 0.8f)
                );
            }

            return EventImage;
        }
    }
}