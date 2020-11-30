using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LiveBot.Automation
{
    internal static class MemberFlow
    {
        public static async Task Welcome_Member(DiscordClient Client, GuildMemberAddEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                     where ss.ID_Server == e.Guild.Id
                                     select ss).ToList();
                var JoinRole = (from rr in DB.DBLists.RankRoles
                                where rr.Server_ID == e.Guild.Id
                                where rr.Server_Rank == 0
                                select rr).ToList();
                DiscordGuild Guild = await Client.GetGuildAsync(Convert.ToUInt64(GuildSettings[0].ID_Server));
                if (GuildSettings[0].Welcome_Settings[0] != "0")
                {
                    DiscordChannel WelcomeChannel = Guild.GetChannel(Convert.ToUInt64(GuildSettings[0].Welcome_Settings[0]));
                    if (GuildSettings[0].Welcome_Settings[1] != "0")
                    {
                        string msg = GuildSettings[0].Welcome_Settings[1];
                        msg = msg.Replace("$Mention", $"{e.Member.Mention}");
                        await WelcomeChannel.SendMessageAsync(msg);
                        if (JoinRole.Count != 0)
                        {
                            DiscordRole role = Guild.GetRole(Convert.ToUInt64(JoinRole[0].Role_ID));
                            await e.Member.GrantRoleAsync(role);
                        }
                    }
                }
            });
            await Task.Delay(1);
        }

        public static async Task Say_Goodbye(DiscordClient Client, GuildMemberRemoveEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                     where ss.ID_Server == e.Guild.Id
                                     select ss).ToList();
                DiscordGuild Guild = await Client.GetGuildAsync(Convert.ToUInt64(GuildSettings[0].ID_Server));
                if (GuildSettings[0].Welcome_Settings[0] != "0")
                {
                    DiscordChannel WelcomeChannel = Guild.GetChannel(Convert.ToUInt64(GuildSettings[0].Welcome_Settings[0]));
                    if (GuildSettings[0].Welcome_Settings[2] != "0")
                    {
                        string msg = GuildSettings[0].Welcome_Settings[2];
                        msg = msg.Replace("$Username", $"{e.Member.Username}");
                        await WelcomeChannel.SendMessageAsync(msg);
                    }
                }
                var ModMailEntry = DB.DBLists.ModMail.FirstOrDefault(w => w.User_ID == e.Member.Id && w.Server_ID == e.Guild.Id && w.IsActive);
                if (ModMailEntry != null)
                {
                    await ModMail.CloseModMail(ModMailEntry, await Client.GetUserAsync(e.Member.Id), "Mod Mail entry closed due to user leaving", "**Mod Mail closed!\n----------------------------------------------------**");
                }
            });
            await Task.Delay(1);
        }
    }
}