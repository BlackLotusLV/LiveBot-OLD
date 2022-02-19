using DSharpPlus.SlashCommands;
using System.Collections.Concurrent;

namespace LiveBot.Services
{
    internal static class WarningService
    {
        private static readonly ConcurrentQueue<WarningItem> _warnings = new();

        private static readonly Thread Warningthread = new(async () =>
             {
                 while (true)
                 {
                     while (_warnings.TryDequeue(out WarningItem item))
                     {
                         await WarnUserAsync(item.User, item.Admin, item.Guild, item.Channel, item.Reason, item.Automsg, item.Ctx);
                     }
                     Thread.Sleep(100);
                 }
             });

        public static void StartService()
        {
            Warningthread.Start();
            Program.Client.Logger.LogInformation(CustomLogEvents.LiveBot, "Warning Service started!");
        }

        public static void QueueWarning(DiscordUser user, DiscordUser admin, DiscordGuild server, DiscordChannel channel, string reason, bool automsg, InteractionContext ctx = null)
        {
            _warnings.Enqueue(new WarningItem(user, admin, server, channel, reason, automsg, ctx));
        }

        public static async Task WarnUserAsync(DiscordUser user, DiscordUser admin, DiscordGuild server, DiscordChannel channel, string reason, bool automsg, InteractionContext ctx = null)
        {
            DB.ServerRanks WarnedUserStats = DB.DBLists.ServerRanks.FirstOrDefault(f => server.Id == f.Server_ID && user.Id == f.User_ID);
            DB.ServerSettings ServerSettings = DB.DBLists.ServerSettings.FirstOrDefault(f => server.Id == f.ID_Server);

            if (WarnedUserStats == null)
            {
                CustomMethod.AddUserToServerRanks(user, server);
            }

            DiscordMember member = null;
            try
            {
                member = await server.GetMemberAsync(user.Id);
            }
            catch (Exception)
            {
                if (automsg) return;
                await channel.SendMessageAsync($"{user.Username} is no longer in the server.");
            }

            string modinfo = "";
            StringBuilder SB = new();
            bool kick = false, ban = false;
            if (ServerSettings.WKB_Log != 0)
            {
                DiscordChannel modlog = server.GetChannel(Convert.ToUInt64(ServerSettings.WKB_Log));
                if (WarnedUserStats is null) // creates new entry in DB (Followers set to default value)
                {
                    DB.ServerRanks newEntry = new()
                    {
                        Server_ID = server.Id,
                        Ban_Count = 0,
                        Kick_Count = 0,
                        Warning_Level = 1,
                        User_ID = user.Id
                    };
                    DB.DBLists.InsertServerRanks(newEntry);
                    WarnedUserStats = newEntry;
                }
                else
                {
                    WarnedUserStats.Warning_Level++;
                    if (WarnedUserStats.Followers <= 1000 * WarnedUserStats.Warning_Level)
                    {
                        WarnedUserStats.Followers = 0;
                    }
                    else
                    {
                        WarnedUserStats.Followers -= (1000 * WarnedUserStats.Warning_Level);
                    }
                    DB.DBLists.UpdateServerRanks(WarnedUserStats);
                }

                DB.Warnings newWarning = new()
                {
                    Reason = reason,
                    Active = true,
                    Time_Created = DateTime.UtcNow,
                    Admin_ID = admin.Id,
                    User_ID = user.Id,
                    Server_ID = server.Id,
                    Type = "warning"
                };
                DB.DBLists.InsertWarnings(newWarning);

                int warning_count = DB.DBLists.Warnings.Count(w => w.User_ID == user.Id && w.Server_ID == server.Id && w.Type == "warning");

                SB.AppendLine($"You have been warned by <@{admin.Id}>.\n**Warning Reason:**\t{reason}\n**Warning Level:** {WarnedUserStats.Warning_Level}\n**Server:** {server.Name}");

                string warningDescription = $"**Warned user:**\t{user.Mention}\n**Warning level:**\t {WarnedUserStats.Warning_Level}\t**Warning count:**\t {warning_count}\n**Warned by**\t{admin.Username}\n**Reason:** {reason}";

                if (WarnedUserStats.Warning_Level > 4)
                {
                    SB.AppendLine($"You have been banned from **{server.Name}** by {admin.Mention} for exceeding the warning level threshold(4).");
                    ban = true;
                }
                else if (WarnedUserStats.Warning_Level > 2 && WarnedUserStats.Warning_Level < 5)
                {
                    SB.AppendLine($"You have been kicked from **{server.Name}** by {admin.Mention} for exceeding the warning level threshold(2).");
                    kick = true;
                }

                if (automsg)
                {
                    SB.Append("*This warning is given out by a bot, contact an admin if you think this is a mistake*");
                }
                else
                {
                    SB.Append($"*(This is an automated message, use the `{Program.CFGJson.CommandPrefix}modmail` feature if you want to report a mistake.)*");
                }

                try
                {
                    await member.SendMessageAsync(SB.ToString());
                }
                catch
                {
                    modinfo = $":exclamation:{user.Mention} could not be contacted via DM. Reason not sent";
                }

                if (kick && member != null)
                {
                    await member.RemoveAsync("Exceeded warning limit!");
                }
                if (ban && user != null)
                {
                    await server.BanMemberAsync(user.Id, 0, "Exceeded warning limit!");
                }

                await CustomMethod.SendModLog(modlog, user, warningDescription, CustomMethod.ModLogType.Warning, modinfo);

                if (ctx == null)
                {
                    DiscordMessage info = await channel.SendMessageAsync($"{user.Username}, Has been warned!");
                    await Task.Delay(10000).ContinueWith(t => info.DeleteAsync());
                }
                else
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{user.Username}, Has been warned!"));
                    await Task.Delay(10000).ContinueWith(t => ctx.DeleteResponseAsync());
                }
            }
            else
            {
                if (ctx == null)
                {
                    await channel.SendMessageAsync("This server has not set up this feature!");
                }
                else
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("This server has not set up this feature!"));
                }
            }
        }
    }

    public class WarningItem
    {
        public DiscordUser User { get; set; }
        public DiscordUser Admin { get; set; }
        public DiscordGuild Guild { get; set; }
        public DiscordChannel Channel { get; set; }
        public string Reason { get; set; }
        public bool Automsg { get; set; }
        public InteractionContext Ctx { get; set; }

        public WarningItem(DiscordUser user, DiscordUser admin, DiscordGuild server, DiscordChannel channel, string reason, bool automsg, InteractionContext ctx = null)
        {
            User = user;
            Admin = admin;
            Guild = server;
            Channel = channel;
            Reason = reason;
            Automsg = automsg;
            this.Ctx = ctx;
        }
    }
}