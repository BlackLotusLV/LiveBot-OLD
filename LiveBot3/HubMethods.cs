using LiveBot.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LiveBot
{
    internal static class HubMethods
    {
        private static DateTime TCHubLastUpdated;
        public static byte[][] EventLogoBitArr { get; set; } = new byte[9][];
        public static byte[,][] RewardsImageBitArr { get; set; } = new byte[4, 4][];

        public static void UpdateHubInfo(bool forced = false)
        {
            List<TCHubJson.Summit> JSummit;
            DateTime endtime;
            using WebClient wc = new();
            bool Connected = true;
            string JSummitString = string.Empty;
            try
            {
                JSummitString = wc.DownloadString(Program.TCHubJson.Summit);
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
                    Program.TCHubDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(wc.DownloadString(Program.TCHubJson.Dictionary));
                    Program.JSummit = JSummit;
                    Program.TCHub = JsonConvert.DeserializeObject<TCHubJson.TCHub>(wc.DownloadString(Program.TCHubJson.GameData));
                    for (int i = 0; i < JSummit[0].Events.Length; i++)
                    {
                        var ThisEvent = JSummit[0].Events[i];
                        EventLogoBitArr[i] = wc.DownloadData($"https://www.thecrew-hub.com/gen/assets/summits/{ThisEvent.Img_Path}");
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < JSummit[i].Rewards.Length; j++)
                        {
                            RewardsImageBitArr[i, j] = wc.DownloadData($"https://www.thecrew-hub.com/gen/assets/summits/{JSummit[i].Rewards[j].Img_Path}");
                        }
                    }
                    Program.Client.Logger.LogInformation(CustomLogEvents.TCHub, $"Info downloaded for {JSummit[0].Summit_ID} summit.");
                }
            }
        }

        public static void DownloadHubNews()
        {
            using WebClient wc = new();
            wc.Headers.Add("Ubi-AppId", "dda77324-f9d6-44ea-9ecb-30e57b286f6d");
            wc.Headers.Add("Ubi-localeCode", "us-en");
            string NewsString = string.Empty;
            bool Connected = true;

            try
            {
                NewsString = wc.DownloadString(Program.TCHubJson.News);
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

        public static Image<Rgba32> BuildEventImage(TCHubJson.Event Event, TCHubJson.Rank Rank, TCHubJson.TceSummitSubs UserInfo, byte[] EventImageBytes, bool isCorner = false, bool isSpecial = false)
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
                using WebClient wc = new();
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
                TCHubJson.SummitLeaderboard leaderboard = JsonConvert.DeserializeObject<TCHubJson.SummitLeaderboard>(wc.DownloadString($"https://api.thecrew-hub.com/v1/summit/{Program.JSummit[0].ID}/leaderboard/{UserInfo.Platform}/{Event.ID}?profile={UserInfo.Profile_ID}"));
                string
                    EventTitle = (NameIDLookup(ThisEventNameID)),
                    ActivityResult = $"Score: {Activity.Score}";
                TCHubJson.SummitLeaderboardEntries Entries = leaderboard.Entries.FirstOrDefault(w => w.Profile_ID == UserInfo.Profile_ID);
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
                    .DrawText(new DrawingOptions { TextOptions = EventTitleOptions }, $"{NameIDLookup(Brand != null ? Brand.Text_ID : "not found")} - {NameIDLookup(Model != null ? Model.Text_ID : "not found")}", VehicleFont, Color.White, new PointF(5, EventImage.Height - 62))
                    );
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