using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiveBot.Automation
{
    internal static class Roles
    {
        public static List<ActivateRolesTimer> ActivateRolesTimer { get; set; } = new List<ActivateRolesTimer>();

        public static async Task Reaction_Roles(DiscordClient Client, MessageReactionAddEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Emoji.Id != 0 && !e.User.IsBot)
                {
                    new Thread(async () =>
                    {
                        DiscordEmoji used = e.Emoji;
                        DiscordMessage sourcemsg = e.Message;
                        DiscordUser username = e.User;

                        List<DB.ReactionRoles> ReactionRoles = DB.DBLists.ReactionRoles;
                        var RoleInfo = (from rr in ReactionRoles
                                        where rr.Server_ID == e.Channel.Guild.Id
                                        where rr.Message_ID == sourcemsg.Id
                                        where rr.Reaction_ID == used.Id
                                        select rr).ToList();
                        if (RoleInfo.Count == 1)
                        {
                            DiscordGuild guild = await Client.GetGuildAsync(Convert.ToUInt64(RoleInfo[0].Server_ID));
                            if (RoleInfo[0].Type == "acquire")
                            {
                                DiscordMember rolemember = await guild.GetMemberAsync(username.Id);
                                if (rolemember.Roles.Any(w => w.Id == Convert.ToUInt64(RoleInfo[0].Role_ID)))
                                {
                                    await rolemember.RevokeRoleAsync(guild.GetRole(Convert.ToUInt64(RoleInfo[0].Role_ID)));
                                }
                                else
                                {
                                    await rolemember.GrantRoleAsync(guild.GetRole(Convert.ToUInt64(RoleInfo[0].Role_ID)));
                                }

                                Thread.Sleep(5000);
                                await sourcemsg.DeleteReactionAsync(used, e.User, null);
                            }
                            else if (RoleInfo[0].Type == "activate")
                            {
                                DiscordRole role = guild.GetRole(Convert.ToUInt64(RoleInfo[0].Role_ID));
                                string msg = $"---";
                                if (role.IsMentionable)
                                {
                                    await role.ModifyAsync(mentionable: false);
                                    msg = $"{role.Name} ⨯";
                                    ActivateRolesTimer.RemoveAt(ActivateRolesTimer.FindIndex(a => a.Guild == e.Guild && a.Role == role));
                                }
                                else if (!role.IsMentionable)
                                {
                                    await role.ModifyAsync(mentionable: true);
                                    msg = $"{role.Name} ✓";
                                    ActivateRolesTimer newItem = new()
                                    {
                                        Guild = guild,
                                        Role = role,
                                        Time = DateTime.Now
                                    };
                                    ActivateRolesTimer.Add(newItem);
                                }
                                await sourcemsg.DeleteReactionAsync(used, e.User, null);
                                DiscordMessage m = await e.Channel.SendMessageAsync(msg);
                                Thread.Sleep(3000);
                                await m.DeleteAsync();
                            }
                        }
                    }).Start();
                    await Task.Delay(0);
                }
            });
            await Task.Delay(1);
        }
    }

    internal class ActivateRolesTimer
    {
        public DiscordGuild Guild { get; set; }
        public DiscordRole Role { get; set; }
        public DateTime Time { get; set; }
    }
}