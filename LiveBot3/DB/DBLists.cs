﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace LiveBot.DB
{
    internal static class DBLists
    {
        public readonly static int TableCount = 18;
        public static int LoadedTableCount { get; set; } = 0;

        public static List<VehicleList> VehicleList { get; set; } = new List<VehicleList>();
        public static List<DisciplineList> DisciplineList { get; set; } = new List<DisciplineList>();
        public static List<ReactionRoles> ReactionRoles { get; set; } = new List<ReactionRoles>();
        public static List<StreamNotifications> StreamNotifications { get; set; } = new List<StreamNotifications>();
        public static List<BackgroundImage> BackgroundImage { get; set; } = new List<BackgroundImage>();
        public static List<Leaderboard> Leaderboard { get; set; } = new List<Leaderboard>();
        public static List<ServerRanks> ServerRanks { get; set; } = new List<ServerRanks>();
        public static List<UserImages> UserImages { get; set; } = new List<UserImages>();
        public static List<UserSettings> UserSettings { get; set; } = new List<UserSettings>();
        public static List<Warnings> Warnings { get; set; } = new List<Warnings>();
        public static List<ServerSettings> ServerSettings { get; set; } = new List<ServerSettings>();
        public static List<RankRoles> RankRoles { get; set; } = new List<RankRoles>();
        public static List<CommandsUsedCount> CommandsUsedCount { get; set; } = new List<CommandsUsedCount>();
        public static List<BotOutputList> BotOutputList { get; set; } = new List<BotOutputList>();
        public static List<WeatherSchedule> WeatherSchedule { get; set; } = new List<WeatherSchedule>();
        public static List<AMBannedWords> AMBannedWords { get; set; } = new List<AMBannedWords>();
        public static List<ModMail> ModMail { get; set; } = new List<ModMail>();
        public static List<RoleTagSettings> RoleTagSettings { get; set; } = new List<RoleTagSettings>();

        public static void LoadAllLists()
        {
            CustomMethod.DBProgress(LoadedTableCount, TimeSpan.Zero);
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
            new Thread(() => LoadRoleTagSettings(true)).Start();
        }

        #region Load Functions

        public static void LoadVehicleList(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new VehicleListContext();
            VehicleList = (from c in ctx.VehicleList
                           select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "Vehicle");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "Vehicle List Loaded");
            }
        }

        public static void LoadDisciplineList(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new DisciplineListContext();
            DisciplineList = (from c in ctx.DisciplineList
                              select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "Discipline");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "Discipline List Loaded");
            }
        }

        public static void LoadReactionRoles(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new ReactionRolesContext();
            ReactionRoles = (from c in ctx.ReactionRoles
                             select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "Reaction Roles");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "Reaction Roles List Loaded");
            }
        }

        public static void LoadStreamNotifications(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new StreamNotificationsContext();
            StreamNotifications = (from c in ctx.StreamNotifications
                                   select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "Stream Notifications");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "Stream Notifications List Loaded");
            }
        }

        public static void LoadBackgroundImage(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new BackgroundImageContext();
            BackgroundImage = (from c in ctx.BackgroundImage
                               select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "Background Images");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "Background Images List Loaded");
            }
        }

        public static void LoadLeaderboard(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new LeaderboardContext();
            Leaderboard = (from c in ctx.Leaderboard
                           select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "Leaderboard");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "Leaderboard List Loaded");
            }
        }

        public static void LoadServerRanks(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new ServerRanksContext();
            ServerRanks = (from c in ctx.ServerRanks
                           select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "Server Ranks");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "Server Ranks List Loaded");
            }
        }

        public static void LoadUserSettings(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new UserSettingsContext();
            UserSettings = (from c in ctx.UserSettings
                            select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "User Settings");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "User Settings List Loaded");
            }
        }

        public static void LoadUserImages(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new UserImagesContext();
            UserImages = (from c in ctx.UserImages
                          select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "User Images");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "User Images List Loaded");
            }
        }

        public static void LoadWarnings(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new WarningsContext();
            Warnings = (from c in ctx.Warnings
                        select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "Warnings");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "Warnings List Loaded");
            }
        }

        public static void LoadServerSettings(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new ServerSettingsContext();
            ServerSettings = (from c in ctx.ServerSettings
                              select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "Server Settings");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "Server Settings List Loaded");
            }
        }

        public static void LoadRankRoles(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new RankRolesContext();
            RankRoles = (from c in ctx.RankRoles
                         select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "Rank Roles");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "Rank Roles List Loaded");
            }
        }

        public static void LoadCUC(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new CommandsUsedCountContext();
            CommandsUsedCount = (from c in ctx.CommandsUsedCount
                                 select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "Commands Used Count");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "Commands Used Count List Loaded");
            }
        }

        public static void LoadBannedWords(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new AMBannedWordsContext();
            AMBannedWords = (from c in ctx.AMBannedWords
                             select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "Banned Words");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "Banned Words List Loaded");
            }
        }

        public static void LoadBotOutputList(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new BotOutputListContext();
            BotOutputList = (from c in ctx.BotOutputList
                             select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "BotOutputList");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "BotOutputList List Loaded");
            }
        }

        public static void LoadWeatherSchedule(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new WeatherScheduleContext();
            WeatherSchedule = (from c in ctx.WeatherSchedule
                               select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "WeatherSchedule");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "WeatherSchedule List Loaded");
            }
        }

        public static void LoadModMail(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new ModMailContext();
            ModMail = (from c in ctx.ModMail
                       select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "ModMail");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "ModMail List Loaded");
            }
        }

        public static void LoadRoleTagSettings(bool progress = false)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            using var ctx = new RoleTagSettingsContext();
            RoleTagSettings = (from c in ctx.RoleTagSettings
                               select c).ToList();
            timer.Stop();
            if (progress)
            {
                LoadedTableCount++;
                CustomMethod.DBProgress(LoadedTableCount, timer.Elapsed, "RoleTagSettings");
            }
            else
            {
                Program.Client.Logger.LogInformation(CustomLogEvents.TableLoaded, "RoleTag Settings Loaded");
            }
        }

        #endregion Load Functions

        #region Update Functions

        public static void UpdateLeaderboard(params Leaderboard[] o)
        {
            using var ctx = new LeaderboardContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateServerRanks(params ServerRanks[] o)
        {
            using var ctx = new ServerRanksContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateUserSettings(params UserSettings[] o)
        {
            using var ctx = new UserSettingsContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateUserImages(params UserImages[] o)
        {
            using var ctx = new UserImagesContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateWarnings(params Warnings[] o)
        {
            using var ctx = new WarningsContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateVehicleList(params VehicleList[] o)
        {
            using var ctx = new VehicleListContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateServerSettings(params ServerSettings[] o)
        {
            using var ctx = new ServerSettingsContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateRankRoles(params RankRoles[] o)
        {
            using var ctx = new RankRolesContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateCUC(params CommandsUsedCount[] o)
        {
            using var ctx = new CommandsUsedCountContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateBannedWords(params AMBannedWords[] o)
        {
            using var ctx = new AMBannedWordsContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateBotOutputList(params BotOutputList[] o)
        {
            using var ctx = new BotOutputListContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateWeatherSchedule(params WeatherSchedule[] o)
        {
            using var ctx = new WeatherScheduleContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateModMail(params ModMail[] o)
        {
            using var ctx = new ModMailContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateRoleTagSettings(params RoleTagSettings[] o)
        {
            using var ctx = new RoleTagSettingsContext();
            ctx.UpdateRange(o);
            ctx.SaveChanges();
        }

        public static void UpdateBackgroundImages(params BackgroundImage[] o)
        {
            using var ctx = new BackgroundImageContext();
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

        public static void InsertRoleTagSettings(RoleTagSettings o)
        {
            using var ctx = new RoleTagSettingsContext();
            ctx.RoleTagSettings.Add(o);
            ctx.SaveChanges();
            LoadRoleTagSettings();
        }

        #endregion Insert Functions
    }
}