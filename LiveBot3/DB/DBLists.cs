using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LiveBot.DB
{
    internal class DBLists
    {
        public static int TableCount = 17;
        public static int LoadedTableCount = 0;

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
        public static List<BotOutputList> BotOutputList;
        public static List<WeatherSchedule> WeatherSchedule;
        public static List<AMBannedWords> AMBannedWords;
        public static List<ModMail> ModMail;

        public static void LoadAllLists()
        {
            Console.WriteLine("[POSTGRESQL] Loading Database");
            CustomMethod.DBProgress(LoadedTableCount);
            LoadServerSettings(true);
            LoadWeatherSchedule(true);
            new Thread(() => LoadVehicleList(true)).Start();
            new Thread(() => LoadDisciplineList(true)).Start();
            new Thread(() => LoadReactionRoles(true)).Start();
            new Thread(() => LoadStreamNotifications(true)).Start();
            new Thread(() => LoadBackgroundImage(true)).Start();
            new Thread(() => LoadLeaderboard(true)).Start();
            new Thread(() => LoadServerRanks(true)).Start();
            new Thread(() => LoadUserImages(true)).Start();
            new Thread(() => LoadUserSettings(true)).Start();
            new Thread(() => LoadWarnings(true)).Start();
            new Thread(() => LoadRankRoles(true)).Start();
            new Thread(() => LoadCUC(true)).Start();
            new Thread(() => LoadBannedWords(true)).Start();
            new Thread(() => LoadBotOutputList(true)).Start();
            new Thread(() => LoadModMail(true)).Start();
        }

        #region Load Functions

        public static void LoadVehicleList(bool progress = false)
        {
            using var ctx = new VehicleListContext();
            VehicleList = (from c in ctx.VehicleList
                           select c).ToList();
            Console.WriteLine("[POSTGRESQL] Vehicle List Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadDisciplineList(bool progress = false)
        {
            using var ctx = new DisciplineListContext();
            DisciplineList = (from c in ctx.DisciplineList
                              select c).ToList();
            Console.WriteLine("[POSTGRESQL] Discipline List Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadReactionRoles(bool progress = false)
        {
            using var ctx = new ReactionRolesContext();
            ReactionRoles = (from c in ctx.ReactionRoles
                             select c).ToList();
            Console.WriteLine("[POSTGRESQL] Reaction Roles Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadStreamNotifications(bool progress = false)
        {
            using var ctx = new StreamNotificationsContext();
            StreamNotifications = (from c in ctx.StreamNotifications
                                   select c).ToList();
            Console.WriteLine("[POSTGRESQL] Stream Notifications Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadBackgroundImage(bool progress = false)
        {
            using var ctx = new BackgroundImageContext();
            BackgroundImage = (from c in ctx.BackgroundImage
                               select c).ToList();
            Console.WriteLine("[POSTGRESQL] Background Images Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadLeaderboard(bool progress = false)
        {
            using var ctx = new LeaderboardContext();
            Leaderboard = (from c in ctx.Leaderboard
                           select c).ToList();
            Console.WriteLine("[POSTGRESQL] Leaderboard Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadServerRanks(bool progress = false)
        {
            using var ctx = new ServerRanksContext();
            ServerRanks = (from c in ctx.ServerRanks
                           select c).ToList();
            Console.WriteLine("[POSTGRESQL] Server Ranks Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadUserSettings(bool progress = false)
        {
            using var ctx = new UserSettingsContext();
            UserSettings = (from c in ctx.UserSettings
                            select c).ToList();
            Console.WriteLine("[POSTGRESQL] User Settings Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadUserImages(bool progress = false)
        {
            using var ctx = new UserImagesContext();
            UserImages = (from c in ctx.UserImages
                          select c).ToList();
            Console.WriteLine("[POSTGRESQL] User Images Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadWarnings(bool progress = false)
        {
            using var ctx = new WarningsContext();
            Warnings = (from c in ctx.Warnings
                        select c).ToList();
            Console.WriteLine("[POSTGRESQL] Warnings Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadServerSettings(bool progress = false)
        {
            using var ctx = new ServerSettingsContext();
            ServerSettings = (from c in ctx.ServerSettings
                              select c).ToList();
            Console.WriteLine("[POSTGRESQL] Server Settings Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadRankRoles(bool progress = false)
        {
            using var ctx = new RankRolesContext();
            RankRoles = (from c in ctx.RankRoles
                         select c).ToList();
            Console.WriteLine("[POSTGRESQL] Rank Roles Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadCUC(bool progress = false)
        {
            using var ctx = new CommandsUsedCountContext();
            CommandsUsedCount = (from c in ctx.CommandsUsedCount
                                 select c).ToList();
            Console.WriteLine("[POSTGRESQL] Commands Used Count Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadBannedWords(bool progress = false)
        {
            using var ctx = new AMBannedWordsContext();
            AMBannedWords = (from c in ctx.AMBannedWords
                             select c).ToList();
            Console.WriteLine("[POSTGRESQL] Banned Words Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadBotOutputList(bool progress = false)
        {
            using var ctx = new BotOutputListContext();
            BotOutputList = (from c in ctx.BotOutputList
                             select c).ToList();
            Console.WriteLine("[POSTGRESQL] BotOutputList Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadWeatherSchedule(bool progress = false)
        {
            using var ctx = new WeatherScheduleContext();
            WeatherSchedule = (from c in ctx.WeatherSchedule
                               select c).ToList();
            Console.WriteLine("[POSTGRESQL] WeatherSchedule Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        public static void LoadModMail(bool progress = false)
        {
            using var ctx = new ModMailContext();
            ModMail = (from c in ctx.ModMail
                       select c).ToList();
            Console.WriteLine("[POSTGRESQL] ModMail Loaded");
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount);
            }
        }

        #endregion Load Functions

        #region Update Functions

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

        public static void UpdateBotOutputList(List<BotOutputList> o)
        {
            using var ctx = new BotOutputListContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateWeatherSchedule(List<WeatherSchedule> o)
        {
            using var ctx = new WeatherScheduleContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateModMail(List<ModMail> o)
        {
            using var ctx = new ModMailContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        #endregion Update Functions

        #region Insert Functions

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

        public static void InsertBotOutputList(BotOutputList o)
        {
            using var ctx = new BotOutputListContext();
            ctx.BotOutputList.Add(o);
            ctx.SaveChanges();
            LoadRankRoles();
        }

        public static void InsertWeatherSchedule(WeatherSchedule o)
        {
            using var ctx = new WeatherScheduleContext();
            ctx.WeatherSchedule.Add(o);
            ctx.SaveChanges();
            LoadRankRoles();
        }

        public static void InsertBackgroundImage(BackgroundImage o)
        {
            using var ctx = new BackgroundImageContext();
            ctx.BackgroundImage.Add(o);
            ctx.SaveChanges();
            LoadBackgroundImage();
        }

        public static void InsertModMail(ModMail o)
        {
            using var ctx = new ModMailContext();
            ctx.ModMail.Add(o);
            ctx.SaveChanges();
            LoadModMail();
        }

        #endregion Insert Functions
    }
}