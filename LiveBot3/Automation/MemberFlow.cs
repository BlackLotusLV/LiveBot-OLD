namespace LiveBot.Automation
{
    internal static class MemberFlow
    {
        public static async Task Welcome_Member(object Client, GuildMemberAddEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                var WelcomeSettings = DB.DBLists.ServerWelcomeSettings.FirstOrDefault(w => w.Server_ID == e.Guild.Id);
                var JoinRole = (from rr in DB.DBLists.RankRoles
                                where rr.Server_ID == e.Guild.Id
                                where rr.Server_Rank == 0
                                select rr).FirstOrDefault();
                if (WelcomeSettings.Channel_ID != 0 && !WelcomeSettings.HasScreening)
                {
                    DiscordChannel WelcomeChannel = e.Guild.GetChannel(Convert.ToUInt64(WelcomeSettings.Channel_ID));
                    if (WelcomeSettings.Welcome_Message != null)
                    {
                        string msg = WelcomeSettings.Welcome_Message;
                        msg = msg.Replace("$Mention", $"{e.Member.Mention}");
                        await WelcomeChannel.SendMessageAsync(msg);
                        if (JoinRole != null)
                        {
                            DiscordRole role = e.Guild.GetRole(Convert.ToUInt64(JoinRole.Role_ID));
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
                var WelcomeSettings = DB.DBLists.ServerWelcomeSettings.FirstOrDefault(w => w.Server_ID == e.Guild.Id);
                bool pendingcheck = true;
                if (WelcomeSettings.HasScreening && e.Member.IsPending == true)
                {
                    pendingcheck = false;
                }
                if (WelcomeSettings.Channel_ID != 0 && pendingcheck)
                {
                    DiscordChannel WelcomeChannel = e.Guild.GetChannel(Convert.ToUInt64(WelcomeSettings.Channel_ID));
                    if (WelcomeSettings.Goodbye_Message != null)
                    {
                        string msg = WelcomeSettings.Goodbye_Message;
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