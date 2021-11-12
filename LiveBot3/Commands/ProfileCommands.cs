using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace LiveBot.Commands
{
    [Group("profile")]
    [Description("Profile commands")]
    internal class ProfileCommands : BaseCommandModule
    {
        [GroupCommand]
        [Description("Shows users live bot profile.")]
        [RequireGuild]
        public async Task Profile(CommandContext ctx, DiscordMember Member = null)
        {
            await ctx.TriggerTypingAsync();

            if (Member == null)
            {
                Member = ctx.Member;
            }

            DB.DBLists.LoadUserSettings();
            List<DB.Leaderboard> Leaderboard = DB.DBLists.Leaderboard;
            List<DB.UserSettings> USettings = DB.DBLists.UserSettings;
            var UserStats = (from gl in Leaderboard
                             where gl.ID_User == Member.Id
                             select gl).FirstOrDefault();
            var UserSettings = (from us in USettings
                                join ui in DB.DBLists.UserImages on us.Image_ID equals ui.ID_User_Images
                                join bi in DB.DBLists.BackgroundImage on ui.BG_ID equals bi.ID_BG
                                where us.User_ID == ui.User_ID
                                where us.User_ID == Member.Id
                                select new { us, ui, bi }).FirstOrDefault();

            string UsersName = Member.Username;
            if (Member.Nickname != null)
            {
                UsersName = Member.Nickname;
            }

            int
                usernameSize = 28;

            if (UsersName.Length > 16)
            {
                UsersName = $"{UsersName.Substring(0, UsersName.Length / 2)}\n{UsersName.Substring(UsersName.Length / 2, UsersName.Length / 2 + (UsersName.Length % 2 != 0 ? 1 : 0)) }";
                usernameSize = 19;
            }
            UsersName = Regex.Replace(UsersName, @"[^\u0000-\u007F]+", "�");

            string
                level = UserStats.Level.ToString(),
                followers = $"{UserStats.Followers}/{UserStats.Level * (300 * (UserStats.Level + 1) * 0.5)}",
                bucks = UserStats.Bucks.ToString(),
                bio = Regex.Replace(UserSettings.us.User_Info.ToString(), @"[^\u0000-\u007F]+", "�");
            string[] BioLines = bio.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            StringBuilder BioWrapper = new();
            foreach (string line in BioLines)
            {
                if (line.Length < 47)
                {
                    BioWrapper.AppendLine(line);
                }
                else
                {
                    StringBuilder newLine = new();
                    foreach (string word in line.Split(" "))
                    {
                        if (newLine.Length + word.Length < 47)
                        {
                            newLine.Append(word + " ");
                        }
                        else
                        {
                            BioWrapper.AppendLine(newLine.ToString());
                            newLine = new StringBuilder();
                            newLine.Append(word + " ");
                        }
                    }
                    BioWrapper.AppendLine(newLine.ToString());
                }
            }

            double
                FollowersBetweenLevels = ((UserStats.Level + 1) * (300 * (UserStats.Level + 2) * 0.5)) - (UserStats.Level * (300 * (UserStats.Level + 1) * 0.5)),
                FollowersToNextLevel = (UserStats.Level * (300 * (UserStats.Level + 1) * 0.5)) - UserStats.Followers,
                FBarLenght = 100 - (100 / FollowersBetweenLevels) * FollowersToNextLevel
                ;

            byte[] ProfilePicture = await new HttpClient().GetByteArrayAsync(Member.AvatarUrl);
            using Image<Rgba32>
                Base = new(580, 580),
                pfp = Image.Load<Rgba32>(ProfilePicture),
                background = Image.Load<Rgba32>(UserSettings.bi.Image)
                ;

            Image<Rgba32> Badge = new(1, 1);
            if (DateTimeOffset.Now - Member.JoinedAt > TimeSpan.FromDays(365.25) || Math.Floor((DateTimeOffset.Now - Member.JoinedAt) / TimeSpan.FromDays(30)) == 12)
            {
                int years = (int)Math.Floor((DateTimeOffset.Now - Member.JoinedAt) / TimeSpan.FromDays(365.25));
                if (years > 6)
                {
                    years = 6;
                }
                else if (years == 0)
                {
                    years = 1;
                }
                Badge = Image.Load<Rgba32>($"Assets/Badges/{years}Year.png");
            }
            else if (DateTimeOffset.Now - Member.JoinedAt < TimeSpan.FromDays(365.25) && DateTimeOffset.Now - Member.JoinedAt > TimeSpan.FromDays(30))
            {
                int months = (int)Math.Floor((DateTimeOffset.Now - Member.JoinedAt) / TimeSpan.FromDays(30));
                Badge = Image.Load<Rgba32>($"Assets/Badges/{months}Month.png");
            }

            Font
                UsernameFont = Program.Fonts.CreateFont("Roboto Mono", usernameSize, FontStyle.BoldItalic),
                BaseFont = Program.Fonts.CreateFont("Roboto Mono", 18, FontStyle.Italic),
                LevelNumberFont = Program.Fonts.CreateFont("Roboto Mono", 50, FontStyle.BoldItalic)
                ;

            var AlignCenter = new TextOptions()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            var AlignBottomLeft = new TextOptions()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            var BioGraphics = new TextOptions()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            int
                BGX = 0,
                BGY = 0,
                MarginWidth = 20,
                //MarginHeight = 30,
                FollowersY = BGY + 290,
                SmallStatBoxHeight = 35,
                BucksY = FollowersY + SmallStatBoxHeight + 5,
                StatBoxWidth = 300,
                StatBoxShift = 20,
                pfpX = BGX + 25,
                pfpY = BGY + 25,
                LevelBoxSize = (SmallStatBoxHeight * 2) + 5,
                LevelBoxX = background.Width - (MarginWidth + StatBoxShift + LevelBoxSize),
                LevelBoxY = FollowersY,
                BadgeX = (int)(LevelBoxX - (LevelBoxSize * 1.25)),
                BadgeY = LevelBoxY,
                NameWidth = 290,
                NameX = BGX + background.Width / 2 - NameWidth / 2,
                NameY = BGY + 230,
                NameYellowWidth = 5,
                NameHeight = 50,
                BioX = MarginWidth,
                BioY = BucksY + SmallStatBoxHeight + 10,
                BioHeight = 190,
                BioWidth = Base.Width - 3 * MarginWidth,
                FollowersBarX = MarginWidth + 10,
                FollowersBarY = FollowersY + SmallStatBoxHeight - 5,
                FollowersBarLenght = FollowersBarX + (int)(0.01 * FBarLenght * StatBoxWidth - 10)
                ;

            Color
                TC2Yellow = Color.ParseHex("FFDB15"),
                TC2Grey = Color.ParseHex("202328");

            pfp.Mutate(ctx => ctx
            .Resize(140, 140)
            );

            Base.Mutate(ctx => ctx
            .DrawImage(background, new Point(BGX, BGY), 1f)
            .FillPolygon(TC2Grey, new PointF[] { new PointF(BGX, BGY), new PointF(BGX + 250, BGY), new PointF(BGX, BGY + 170) })
            .FillPolygon(TC2Grey, new PointF[] { new PointF(background.Width + BGX, background.Height + BGY), new PointF(background.Width + BGX - 300, background.Height + BGY), new PointF(background.Width + BGX, background.Height + BGY - 220) })
            .FillPolygon(TC2Grey, new PointF[] { new PointF(pfpX - 5, pfpY - 5), new PointF(pfpX + pfp.Width + 5, pfpY - 5), new PointF(pfpX + pfp.Width + 5, pfpY + pfp.Height + 5), new PointF(pfpX - 5, pfpY + pfp.Height + 5) })
            .DrawImage(pfp, new Point(pfpX, pfpY), 1f)
            .FillPolygon(TC2Yellow, new PointF[] { new PointF(StatBoxShift + NameX - NameYellowWidth, NameY), new PointF(StatBoxShift + NameX, NameY), new PointF(NameX, NameY + NameHeight), new PointF(NameX - NameYellowWidth, NameY + NameHeight) })
            .FillPolygon(TC2Grey, new PointF[] { new PointF(StatBoxShift + NameX, NameY), new PointF(StatBoxShift + NameX + NameWidth, NameY), new PointF(NameX + NameWidth, NameY + NameHeight), new PointF(NameX, NameY + NameHeight) })
            .FillPolygon(TC2Yellow, new PointF[] { new PointF(MarginWidth + StatBoxShift, FollowersY), new PointF(MarginWidth + StatBoxShift + StatBoxWidth, FollowersY), new PointF(MarginWidth + StatBoxWidth, FollowersY + SmallStatBoxHeight), new PointF(MarginWidth, FollowersY + SmallStatBoxHeight) })
            .FillPolygon(TC2Yellow, new PointF[] { new PointF(MarginWidth + StatBoxShift, BucksY), new PointF(MarginWidth + StatBoxShift + StatBoxWidth, BucksY), new PointF(MarginWidth + StatBoxWidth, BucksY + SmallStatBoxHeight), new PointF(MarginWidth, BucksY + SmallStatBoxHeight) })
            .FillPolygon(TC2Yellow, new PointF[] { new PointF(LevelBoxX + StatBoxShift, LevelBoxY), new PointF(LevelBoxX + StatBoxShift + LevelBoxSize, LevelBoxY), new PointF(LevelBoxX + LevelBoxSize, LevelBoxY + LevelBoxSize), new PointF(LevelBoxX, LevelBoxY + LevelBoxSize) })
            .FillPolygon(TC2Yellow, new PointF[] { new PointF(BioX + StatBoxShift, BioY), new PointF(BioX + StatBoxShift + BioWidth, BioY), new PointF(BioX + BioWidth, BioY + BioHeight), new PointF(BioX, BioY + BioHeight) })
            .DrawImage(Badge, new Point(BadgeX, BadgeY), 1f)
            .DrawLines(TC2Grey, 2f, new PointF[] { new PointF(FollowersBarX, FollowersBarY), new PointF(FollowersBarLenght, FollowersBarY) })
            .DrawText(new DrawingOptions { TextOptions = AlignCenter }, UsersName, UsernameFont, TC2Yellow, new PointF(BGX + StatBoxShift / 2 + background.Width / 2, NameY + NameHeight / 2.3f))
            .DrawText(new DrawingOptions { TextOptions = AlignBottomLeft }, $"Followers: {followers}", BaseFont, TC2Grey, new PointF(MarginWidth + StatBoxShift, FollowersY + SmallStatBoxHeight / 1.5f))
            .DrawText(new DrawingOptions { TextOptions = AlignBottomLeft }, $"Bucks: {bucks}", BaseFont, TC2Grey, new PointF(MarginWidth + StatBoxShift, BucksY + SmallStatBoxHeight / 1.5f))
            .DrawText(new DrawingOptions { TextOptions = AlignCenter }, level, LevelNumberFont, TC2Grey, new PointF((LevelBoxX + StatBoxShift + LevelBoxX + LevelBoxSize) / 2, (LevelBoxY * 2 + LevelBoxSize / 1.5f) / 2))
            .DrawText(new DrawingOptions { TextOptions = BioGraphics }, BioWrapper.ToString(), BaseFont, TC2Grey, new PointF(BioX + StatBoxShift + 5, BioY + 7))
            );

            string imageLoc = $"{Program.tmpLoc}{Member.Id}-profile.png";
            Base.Save(imageLoc);
            using var upFile = new FileStream(imageLoc, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
            await new DiscordMessageBuilder()
                .WithFile(upFile)
                .WithReply(ctx.Message.Id, true)
                .SendAsync(ctx.Channel);
        }

        [Command("updatebackground")]
        [Aliases("updatebg", "bgupdate", "backgroundupdate")]
        public async Task UpdateBackground(CommandContext ctx, int BackgroundID)
        {
            var UserSettings = (from us in DB.DBLists.UserSettings
                                where us.User_ID == ctx.User.Id
                                select us).FirstOrDefault();
            string output = string.Empty;
            List<DB.UserImages> UImages = DB.DBLists.UserImages;
            var UserImages = (from ui in UImages
                              where ui.User_ID == ctx.User.Id
                              where ui.BG_ID == BackgroundID
                              select ui).ToList();
            if (UserImages.Count == 0)
            {
                output = "You do not have this background!";
            }
            else if (UserImages.Count == 1)
            {
                UserSettings.Image_ID = UserImages[0].ID_User_Images;
                DB.DBLists.UpdateUserSettings(UserSettings);
                output = $"You have changed your profile background";
            }
            await new DiscordMessageBuilder()
                .WithContent(output)
                 .WithReply(ctx.Message.Id, true)
                 .SendAsync(ctx.Channel);
        }

        [Command("updateinfo")]
        public async Task UpdateInfo(CommandContext ctx, [RemainingText] string Info)
        {
            var UserSettings = (from us in DB.DBLists.UserSettings
                                where us.User_ID == ctx.User.Id
                                select us).FirstOrDefault();
            UserSettings.User_Info = Info;

            DB.DBLists.UpdateUserSettings(UserSettings);
            await new DiscordMessageBuilder()
                .WithContent("You have changed your user info.")
                 .WithReply(ctx.Message.Id, true)
                 .SendAsync(ctx.Channel);
        }

        [Command("update")]
        [Description("Profile customisation command")]
        public async Task Update(CommandContext ctx)
        {
            await ctx.RespondAsync("Here is how to update your profile info:\n" +
                "1. Update background - Use the `/bg` command to view the backgrounds you own, then use `/profile updatebg [id]` command\n" +
                "2. Update info - `/profile updateinfo [your info here]`");
        }
    }
}