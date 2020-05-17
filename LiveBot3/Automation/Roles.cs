using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveBot.Automation
{
    internal class Roles
    {
        public static List<ActivateRolesTimer> ActivateRolesTimer = new List<ActivateRolesTimer>();

        public static async Task Reaction_Roles(MessageReactionAddEventArgs e)
        {
            if (e.Emoji.Id != 0 && !e.User.IsBot)
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
                    DiscordGuild guild = await Program.Client.GetGuildAsync(UInt64.Parse(RoleInfo[0].Server_ID.ToString()));
                    if (RoleInfo[0].Type == "acquire")
                    {
                        DiscordMember rolemember = await guild.GetMemberAsync(username.Id);
                        if (rolemember.Roles.Where(w => w.Id == UInt64.Parse(RoleInfo[0].Role_ID.ToString())).Count() > 0)
                        {
                            await rolemember.RevokeRoleAsync(guild.GetRole(UInt64.Parse(RoleInfo[0].Role_ID.ToString())));
                        }
                        else
                        {
                            await rolemember.GrantRoleAsync(guild.GetRole(UInt64.Parse(RoleInfo[0].Role_ID.ToString())));
                        }

                        await Task.Delay(5000).ContinueWith(t => sourcemsg.DeleteReactionAsync(used, e.User, null));
                    }
                    else if (RoleInfo[0].Type == "activate")
                    {
                        DiscordRole role = guild.GetRole(UInt64.Parse(RoleInfo[0].Role_ID.ToString()));
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
                            ActivateRolesTimer newItem = new ActivateRolesTimer
                            {
                                Guild = guild,
                                Role = role,
                                Time = DateTime.Now
                            };
                            ActivateRolesTimer.Add(newItem);
                        }
                        await sourcemsg.DeleteReactionAsync(used, e.User, null);
                        DiscordMessage m = await e.Channel.SendMessageAsync(msg);
                        await Task.Delay(3000).ContinueWith(t => m.DeleteAsync());
                    }
                }
            }
        }
    }

    internal class ActivateRolesTimer
    {
        public DiscordGuild Guild { get; set; }
        public DiscordRole Role { get; set; }
        public DateTime Time { get; set; }
    }
}