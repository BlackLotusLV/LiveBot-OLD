using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LiveBot.Automation
{
    internal class MemberFlow
    {
        public static async Task Welcome_Member(GuildMemberAddEventArgs e)
        {
            var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                 where ss.ID_Server == e.Guild.Id.ToString()
                                 select ss).ToList();
            var JoinRole = (from rr in DB.DBLists.RankRoles
                            where rr.Server_ID == e.Guild.Id.ToString()
                            where rr.Server_Rank == 0
                            select rr).ToList();
            DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(GuildSettings[0].ID_Server));
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
        }

        public static async Task Say_Goodbye(GuildMemberRemoveEventArgs e)
        {
            var GuildSettings = (from ss in DB.DBLists.ServerSettings
                                 where ss.ID_Server == e.Guild.Id.ToString()
                                 select ss).ToList();
            DiscordGuild Guild = await Program.Client.GetGuildAsync(Convert.ToUInt64(GuildSettings[0].ID_Server));
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
        }
    }
}