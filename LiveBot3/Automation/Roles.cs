namespace LiveBot.Automation
{
    internal static class Roles
    {
        public static List<ActivateRolesTimer> ActivateRolesTimer { get; set; } = new List<ActivateRolesTimer>();

        public static async Task Button_Roles(object Client, InteractionCreateEventArgs e)
        {
            if (e.Interaction.Type == InteractionType.Component && !e.Interaction.User.IsBot && e.Interaction.Guild != null)
            {
                var ButtonRoleInfo = DB.DBLists.ButtonRoles.Where(w => w.Server_ID == e.Interaction.GuildId && w.Channel_ID == e.Interaction.ChannelId && e.Interaction.Guild.Roles.Any(f => f.Value.Id == Convert.ToUInt64(w.Button_ID))).ToList();
                if (ButtonRoleInfo.Count > 0)
                {
                    DiscordInteractionResponseBuilder response = new()
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