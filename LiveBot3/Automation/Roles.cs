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
                                        select rr).FirstOrDefault();
                        if (RoleInfo != null)
                        {
                            DiscordGuild guild = await Client.GetGuildAsync(Convert.ToUInt64(RoleInfo.Server_ID));
                            if (RoleInfo.Type == "acquire")
                            {
                                DiscordMember rolemember = username as DiscordMember;
                                if (rolemember.Roles.Any(w => w.Id == Convert.ToUInt64(RoleInfo.Role_ID)))
                                {
                                    await rolemember.RevokeRoleAsync(guild.GetRole(Convert.ToUInt64(RoleInfo.Role_ID)));
                                }
                                else
                                {
                                    await rolemember.GrantRoleAsync(guild.GetRole(Convert.ToUInt64(RoleInfo.Role_ID)));
                                }

                                Thread.Sleep(20000);
                                await sourcemsg.DeleteReactionAsync(used, e.User, null);
                            }
                            else if (RoleInfo.Type == "activate")
                            {
                                DiscordRole role = guild.GetRole(Convert.ToUInt64(RoleInfo.Role_ID));
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

        public static async Task Button_Roles(object Client, InteractionCreateEventArgs e)
        {
            if (e.Interaction.Type == InteractionType.Component && !e.Interaction.User.IsBot && e.Interaction.Guild != null)
            {
                var ButtonRoleInfo = DB.DBLists.ButtonRoles.Where(w => w.Server_ID == e.Interaction.GuildId && w.Channel_ID == e.Interaction.ChannelId && e.Interaction.Guild.Roles.Any(f => f.Value.Id == Convert.ToUInt64(w.Button_ID))).ToList();
                if (ButtonRoleInfo.Count > 0)
                {
                    DiscordInteractionResponseBuilder response = new DiscordInteractionResponseBuilder()
                    {
                        IsEphemeral = true
                    };
                    DiscordMember member = e.Interaction.User as DiscordMember;
                    DiscordRole role = e.Interaction.Guild.Roles.FirstOrDefault(w => w.Value.Id == Convert.ToUInt64(e.Interaction.Data.CustomId)).Value;
                    if (member.Roles.Any(w => w.Id == Convert.ToUInt64(e.Interaction.Data.CustomId)))
                    {
                        await member.RevokeRoleAsync(role);
                        response.Content = $"{member.Mention} the role {role.Mention} has been removed.";
                    }
                    else
                    {
                        await member.GrantRoleAsync(role);
                        response.Content = $"{member.Mention} you have been given the {role.Mention}.";
                    }
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
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