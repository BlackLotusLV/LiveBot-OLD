using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LiveBot.Automation
{
    internal static class MembershipScreening
    {
        public static async Task AcceptRules(DiscordClient Client, GuildMemberUpdateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.PendingBefore.Value && !e.PendingAfter.Value)
                {
                    var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                         where ss.ID_Server == e.Guild.Id
                                         select ss).FirstOrDefault();
                    var JoinRole = (from rr in DB.DBLists.RankRoles
                                    where rr.Server_ID == e.Guild.Id
                                    where rr.Server_Rank == 0
                                    select rr).FirstOrDefault();
                    DiscordGuild Guild = await Client.GetGuildAsync(Convert.ToUInt64(GuildSettings.ID_Server));
                    if (GuildSettings.Welcome_Settings[0] != "0" && GuildSettings.HasScreening)
                    {
                        DiscordChannel WelcomeChannel = Guild.GetChannel(Convert.ToUInt64(GuildSettings.Welcome_Settings[0]));
                        if (GuildSettings.Welcome_Settings[1] != "0")
                        {
                            string msg = GuildSettings.Welcome_Settings[1];
                            msg = msg.Replace("$Mention", $"{e.Member.Mention}");
                            await WelcomeChannel.SendMessageAsync(msg);
                            if (JoinRole != null)
                            {
                                DiscordRole role = Guild.GetRole(Convert.ToUInt64(JoinRole.Role_ID));
                                await e.Member.GrantRoleAsync(role);
                            }
                        }
                    }
                }
            });
            await Task.Delay(1);
        }
    }
}