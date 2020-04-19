using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using LiveBot.Automation;
using LiveBot.Json;
using Newtonsoft.Json;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveBot
{
    internal class Program
    {
        public static DiscordClient Client { get; set; }
        public InteractivityExtension Interactivity { get; set; }
        public CommandsNextExtension Commands { get; set; }
        public static DateTime start = DateTime.Now;
        public static string BotVersion = $"20200419_A";
        public static bool TestBuild;

        // TC Hub

        public static ConfigJson.TCHubAPI TCHubJson;
        public static Dictionary<string, string> TCHubDictionary;
        public static TCHubJson.TCHub TCHub;
        public static List<TCHubJson.Summit> JSummit;
        public static ConfigJson.TCE TCEJson;

        // Lists

        public static List<ulong> ServerIdList = new List<ulong>();

        // string

        public static string tmpLoc = Path.GetTempPath() + "/livebot-";

        //channels
        private DiscordChannel BotErrorLogChannel;

        // guild
        public static DiscordGuild TCGuild;

        // fonts
        public static FontCollection fonts = new FontCollection();

        private static void Main(string[] args)
        {
            var prog = new Program();
            prog.RunBotAsync(args).GetAwaiter().GetResult();
        }

        public async Task RunBotAsync(string[] args)
        {
            // fills all database lists
            DB.DBLists.LoadAllLists();
            fonts.Install("Assets/Fonts/Hurme_Geometric_Sans_3_W03_Blk.ttf");

            TestBuild = true;

            var json = string.Empty;
            using (var fs = File.OpenRead("Config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();
            TCEJson = JsonConvert.DeserializeObject<ConfigJson.Config>(json).TCE;
            ConfigJson.Bot cfgjson = JsonConvert.DeserializeObject<ConfigJson.Config>(json).DevBot;

            // TC Hub
            TCHubJson = JsonConvert.DeserializeObject<ConfigJson.Config>(json).TCHub;
            Thread HubThread = new Thread(TimerMethod.UpdateHubInfo);
            HubThread.Start();
            //

            if (args.Length == 1)
            {
                if (args[0] == "live") // Checks for command argument to be "live", if so, then launches the live version of the bot, not dev
                {
                    cfgjson = JsonConvert.DeserializeObject<ConfigJson.Config>(json).LiveBot;
                    Console.WriteLine($"Running live version: {BotVersion}");
                    TestBuild = false;
                }
            }
            var cfg = new DiscordConfiguration
            {
                Token = cfgjson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                ReconnectIndefinitely = false,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(cfg);
            Client.Ready += this.Client_Ready;
            Client.GuildAvailable += this.Client_GuildAvailable;
            Client.ClientErrored += this.Client_ClientError;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = DSharpPlus.Interactivity.Enums.PaginationBehaviour.Ignore,
                Timeout = TimeSpan.FromMinutes(2)
            });

            var ccfg = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { cfgjson.CommandPrefix },
                CaseSensitive = false,
                IgnoreExtraArguments = true
            };
            this.Commands = Client.UseCommandsNext(ccfg);

            this.Commands.CommandExecuted += this.Commands_CommandExecuted;
            this.Commands.CommandErrored += this.Commands_CommandErrored;

            this.Commands.RegisterCommands<Commands.UngroupedCommands>();
            this.Commands.RegisterCommands<Commands.AdminCommands>();
            this.Commands.RegisterCommands<Commands.OCommands>();

            // Servers
            TCGuild = await Client.GetGuildAsync(150283740172517376); //The Crew server
            DiscordGuild testserver = await Client.GetGuildAsync(282478449539678210);

            // Channels
            AutoMod.TC1Photomode = TCGuild.GetChannel(191567033064751104);
            AutoMod.TC2Photomode = TCGuild.GetChannel(447134224349134848);
            BotErrorLogChannel = testserver.GetChannel(673105806778040320);

            //*/
            Weather.StartTimer();
            Timer StreamTimer = new Timer(e => TimerMethod.StreamListCheck(LiveStream.LiveStreamerList, LiveStream.StreamCheckDelay), null, TimeSpan.Zero, TimeSpan.FromMinutes(2));
            Timer RoleTimer = new Timer(e => TimerMethod.ActivatedRolesCheck(Roles.ActivateRolesTimer), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            Timer UpdateTCHubInfo = new Timer(e => TimerMethod.UpdateHubInfo(), null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
            Timer ClearSpamMessageCache = new Timer(e => AutoMod.ClearMSGCache(), null, TimeSpan.Zero, TimeSpan.FromDays(1));

            if (!TestBuild) //Only enables these when using live version
            {
                Client.PresenceUpdated += LiveStream.Stream_Notification;

                Client.MessageCreated += Leveling.Update_User_Levels;
                Client.GuildMemberAdded += Leveling.Add_To_Leaderboards;

                Client.MessageCreated += AutoMod.Photomode_Cleanup;
                Client.MessageCreated += AutoMod.Auto_Moderator_Banned_Words;
                Client.MessageCreated += AutoMod.Spam_Protection;
                Client.MessageDeleted += AutoMod.Delete_Log;
                Client.MessagesBulkDeleted += AutoMod.Bulk_Delete_Log;
                Client.GuildMemberAdded += AutoMod.User_Join_Log;
                Client.GuildMemberRemoved += AutoMod.User_Leave_Log;
                Client.GuildMemberRemoved += AutoMod.User_Kicked_Log;
                Client.GuildBanAdded += AutoMod.User_Banned_Log;
                Client.GuildBanRemoved += AutoMod.User_Unbanned;

                Client.MessageReactionAdded += Roles.Reaction_Roles;

                Client.GuildMemberAdded += MemberFlow.Welcome_Member;
                Client.GuildMemberRemoved += MemberFlow.Say_Goodbye;
            }
            await Client.ConnectAsync();
            await Task.Delay(-1);

        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "LiveBot", "Client is ready to process events.", DateTime.Now);
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            ServerIdList.Add(e.Guild.Id);
            var list = (from ss in DB.DBLists.ServerSettings
                        where ss.ID_Server == e.Guild.Id.ToString()
                        select ss).ToList();
            if (list.Count == 0)
            {
                string[] arr = new string[] { "0", "0", "0" };
                var newEntry = new DB.ServerSettings()
                {
                    ID_Server = e.Guild.Id.ToString(),
                    Delete_Log = "0",
                    User_Traffic = "0",
                    Welcome_Settings = arr,
                    WKB_Log = "0",
                    Spam_Exception_Channels = new string[] {"0"}
                };
                DB.DBLists.InsertServerSettings(newEntry);
            }
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "LiveBot", $"Guild available: {e.Guild.Name}", DateTime.Now);
            return Task.CompletedTask;
        }

        private async Task<Task> Client_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "LiveBot", $"Exception occurred: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
            string errormsg = $"{DateTime.Now} {LogLevel.Error} LiveBot Exception Occured: {e.Exception.GetType()}: {e.Exception.Message}\n" +
                $"{e.Exception.InnerException}";
            if (errormsg.Length <= 1900)
            {
                await BotErrorLogChannel.SendMessageAsync(errormsg);
            }
            else
            {
                File.WriteAllText($"{tmpLoc}{errormsg.Length}-errorFile.txt", errormsg);
                using var upFile = new FileStream($"{tmpLoc}{errormsg.Length}-errorFile.txt", FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
                await BotErrorLogChannel.SendFileAsync(upFile);
            }
            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "LiveBot", $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

            DB.DBLists.LoadCUC();
            string CommandName = e.Command.Name;
            var DBEntry = DB.DBLists.CommandsUsedCount.Where(w => w.Name == CommandName).FirstOrDefault();
            if (DBEntry == null)
            {
                DB.CommandsUsedCount NewEntry = new DB.CommandsUsedCount()
                {
                    Name = e.Command.Name,
                    Used_Count = 1
                };
                DB.DBLists.InsertCUC(NewEntry);
            }
            else if (DBEntry != null)
            {
                DBEntry.Used_Count++;
                DB.DBLists.UpdateCUC(new List<DB.CommandsUsedCount> { DBEntry });
            }
            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "LiveBot", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}\n{e.Exception.InnerException}", DateTime.Now);
#pragma warning disable IDE0059 // Value assigned to symbol is never used
            if (e.Exception is ChecksFailedException ex)
#pragma warning restore IDE0059 // Value assigned to symbol is never used
            {
                var no_entry = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");
                var clock = DiscordEmoji.FromName(e.Context.Client, ":clock:");
                string msgContent;
                if (ex.FailedChecks[0].GetType() == typeof(CooldownAttribute))
                {
                    msgContent = $"{clock} You, {e.Context.Member.Mention}, tried to execute the command too fast, wait and try again later.";
                }
                else if (ex.FailedChecks[0].GetType() == typeof(RequireRolesAttribute))
                {
                    msgContent = $"{no_entry} You, {e.Context.Member.Mention}, don't have the required role for this command";
                }
                else
                {
                    msgContent = $"{no_entry} You, {e.Context.Member.Mention}, do not have the permissions required to execute this command.";
                }
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access denied",
                    Description = msgContent,
                    Color = new DiscordColor(0xFF0000) // red
                };
                DiscordMessage errorMSG = await e.Context.RespondAsync(string.Empty, embed: embed);
                await Task.Delay(10000).ContinueWith(t => errorMSG.DeleteAsync());
            }
        }
    }
}