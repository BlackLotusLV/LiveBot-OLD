using System.Collections.Generic;
using System.Linq;

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
        public static List<UserWarnings> UserWarnings;
        public static List<Warnings> Warnings;
        public static List<ServerSettings> ServerSettings;
        public static List<RankRoles> RankRoles;

        public static void LoadAllLists()
        {
            LoadVehicleList();
            LoadDisciplineList();
            LoadReactionRoles();
            LoadStreamNotifications();
            LoadBackgroundImage();
            LoadLeaderboard();
            LoadServerRanks();
            LoadUserImages();
            LoadUserSettings();
            LoadUserWarnings();
            LoadWarnings();
            LoadServerSettings();
            LoadRankRoles();
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

        public static void LoadUserWarnings()
        {
            using var ctx = new UserWarningsContext();
            UserWarnings = (from c in ctx.UserWarnings
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

        public static void UpdateUserWarnings(List<UserWarnings> o)
        {
            using var ctx = new UserWarningsContext();
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

        public static void InsertUserWarnings(UserWarnings o)
        {
            using var ctx = new UserWarningsContext();
            ctx.UserWarnings.Add(o);
            ctx.SaveChanges();
            LoadUserWarnings();
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
    }
}