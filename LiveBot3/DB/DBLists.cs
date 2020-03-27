using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LiveBot.DB
{
    internal class DBLists
    {
        public static List<VehicleList> VehicleList;
        public static List<DisciplineList> DisciplineList;
        public static List<ReactionRoles> ReactionRoles;
        public static List<StreamNotifications> StreamNotifications;
        public static List<BackgroundImage> BackgroundImage;
        public static List<Leaderboard> Leaderboard;
        public static List<ServerRanks> ServerRanks;
        public static List<UserImages> UserImages;
        public static List<UserSettings> UserSettings;
        public static List<Warnings> Warnings;
        public static List<ServerSettings> ServerSettings;
        public static List<RankRoles> RankRoles;
        public static List<CommandsUsedCount> CommandsUsedCount;

        public static List<AMBannedWords> AMBannedWords;

        public static void LoadAllLists()
        {
            LoadServerSettings();
            new Thread(LoadVehicleList).Start();
            new Thread(LoadDisciplineList).Start();
            new Thread(LoadReactionRoles).Start();
            new Thread(LoadStreamNotifications).Start();
            new Thread(LoadBackgroundImage).Start();
            new Thread(LoadLeaderboard).Start();
            new Thread(LoadServerRanks).Start();
            new Thread(LoadUserImages).Start();
            new Thread(LoadUserSettings).Start();
            new Thread(LoadWarnings).Start();
            new Thread(LoadRankRoles).Start();
            new Thread(LoadCUC).Start();
            new Thread(LoadBannedWords).Start();

            Console.WriteLine($"[Postgres] Latest database data downloaded");
        }

        public static void LoadVehicleList()
        {
            using var ctx = new VehicleListContext();
            VehicleList = (from c in ctx.VehicleList
                           select c).ToList();
        }

        public static void LoadDisciplineList()
        {
            using var ctx = new DisciplineListContext();
            DisciplineList = (from c in ctx.DisciplineList
                              select c).ToList();
        }

        public static void LoadReactionRoles()
        {
            using var ctx = new ReactionRolesContext();
            ReactionRoles = (from c in ctx.ReactionRoles
                             select c).ToList();
        }

        public static void LoadStreamNotifications()
        {
            using var ctx = new StreamNotificationsContext();
            StreamNotifications = (from c in ctx.StreamNotifications
                                   select c).ToList();
        }

        public static void LoadBackgroundImage()
        {
            using var ctx = new BackgroundImageContext();
            BackgroundImage = (from c in ctx.BackgroundImage
                               select c).ToList();
        }

        public static void LoadLeaderboard()
        {
            using var ctx = new LeaderboardContext();
            Leaderboard = (from c in ctx.Leaderboard
                           select c).ToList();
        }

        public static void LoadServerRanks()
        {
            using var ctx = new ServerRanksContext();
            ServerRanks = (from c in ctx.ServerRanks
                           select c).ToList();
        }

        public static void LoadUserSettings()
        {
            using var ctx = new UserSettingsContext();
            UserSettings = (from c in ctx.UserSettings
                            select c).ToList();
        }

        public static void LoadUserImages()
        {
            using var ctx = new UserImagesContext();
            UserImages = (from c in ctx.UserImages
                          select c).ToList();
        }

        public static void LoadWarnings()
        {
            using var ctx = new WarningsContext();
            Warnings = (from c in ctx.Warnings
                        select c).ToList();
        }

        public static void LoadServerSettings()
        {
            using var ctx = new ServerSettingsContext();
            ServerSettings = (from c in ctx.ServerSettings
                              select c).ToList();
        }

        public static void LoadRankRoles()
        {
            using var ctx = new RankRolesContext();
            RankRoles = (from c in ctx.RankRoles
                         select c).ToList();
        }

        public static void LoadCUC()
        {
            using var ctx = new CommandsUsedCountContext();
            CommandsUsedCount = (from c in ctx.CommandsUsedCount
                                 select c).ToList();
        }

        public static void LoadBannedWords()
        {
            using var ctx = new AMBannedWordsContext();
            AMBannedWords = (from c in ctx.AMBannedWords
                             select c).ToList();
        }

        public static void UpdateLeaderboard(List<Leaderboard> o)
        {
            using var ctx = new LeaderboardContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateServerRanks(List<ServerRanks> o)
        {
            using var ctx = new ServerRanksContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateUserSettings(List<UserSettings> o)
        {
            using var ctx = new UserSettingsContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateUserImages(List<UserImages> o)
        {
            using var ctx = new UserImagesContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateWarnings(List<Warnings> o)
        {
            using var ctx = new WarningsContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateVehicleList(List<VehicleList> o)
        {
            using var ctx = new VehicleListContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateServerSettings(List<ServerSettings> o)
        {
            using var ctx = new ServerSettingsContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateRankRoles(List<RankRoles> o)
        {
            using var ctx = new RankRolesContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateCUC(List<CommandsUsedCount> o)
        {
            using var ctx = new CommandsUsedCountContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateBannedWords(List<AMBannedWords> o)
        {
            using var ctx = new AMBannedWordsContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void InsertUserImages(UserImages o)
        {
            using var ctx = new UserImagesContext();
            ctx.UserImages.Add(o);
            ctx.SaveChanges();
            LoadUserImages();
        }

        public static void InsertUserSettings(UserSettings o)
        {
            using var ctx = new UserSettingsContext();
            ctx.UserSettings.Add(o);
            ctx.SaveChanges();
            LoadUserSettings();
        }

        public static void InsertLeaderboard(Leaderboard o)
        {
            using var ctx = new LeaderboardContext();
            ctx.Leaderboard.Add(o);
            ctx.SaveChanges();
            LoadLeaderboard();
        }

        public static void InsertServerRanks(ServerRanks o)
        {
            using var ctx = new ServerRanksContext();
            ctx.ServerRanks.Add(o);
            ctx.SaveChanges();
            LoadServerRanks();
        }

        public static void InsertWarnings(Warnings o)
        {
            using var ctx = new WarningsContext();
            ctx.Warnings.Add(o);
            ctx.SaveChanges();
            LoadWarnings();
        }

        public static void InsertServerSettings(ServerSettings o)
        {
            using var ctx = new ServerSettingsContext();
            ctx.ServerSettings.Add(o);
            ctx.SaveChanges();
            LoadServerSettings();
        }

        public static void InsertRankRoles(RankRoles o)
        {
            using var ctx = new RankRolesContext();
            ctx.RankRoles.Add(o);
            ctx.SaveChanges();
            LoadRankRoles();
        }

        public static void InsertCUC(CommandsUsedCount o)
        {
            using var ctx = new CommandsUsedCountContext();
            ctx.CommandsUsedCount.Add(o);
            ctx.SaveChanges();
            LoadRankRoles();
        }

        public static void InsertBannedWords(AMBannedWords o)
        {
            using var ctx = new AMBannedWordsContext();
            ctx.AMBannedWords.Add(o);
            ctx.SaveChanges();
            LoadRankRoles();
        }
    }
}