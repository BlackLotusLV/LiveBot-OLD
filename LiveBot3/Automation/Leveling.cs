namespace LiveBot.Automation
{
    internal static class Leveling
    {
        private static List<LevelTimer> GlobalLevelTimer { get; set; } = new List<LevelTimer>();
        private static List<ServerLevelTimer> ServerLevelTimer { get; set; } = new List<ServerLevelTimer>();

        private static readonly int
            MinimalSmallInterval = 10,
            MaximalSmallInterval = 30,
            MinimalBigInterval = 55,
            MaximalBigInterval = 85,
            SmallMSGLenght = 100,
            BigMSGLenght = 500,
            MinimalMoney = 2,
            MaximalMoney = 5,
            MSGCooldown = 2;

        public static async Task Level_Gaining_System(object Client, MessageCreateEventArgs e)
        {
            if (e.Author.IsBot || e.Guild == null) return;
            Random r = new();
            int FollowersMin = MinimalSmallInterval,
                FollowersMax = MaximalSmallInterval;
            if (e.Message.Content.Length > BigMSGLenght)
            {
                FollowersMin = MinimalBigInterval;
                FollowersMax = MaximalBigInterval;
            }
            else if (e.Message.Content.Length > SmallMSGLenght && e.Message.Content.Length < BigMSGLenght)
            {
                float diff = MinimalBigInterval - MinimalSmallInterval - 1;
                diff /= BigMSGLenght;
                diff *= e.Message.Content.Length;
                FollowersMin += (int)diff;
                diff = MaximalBigInterval - MaximalSmallInterval - 1;
                diff /= BigMSGLenght;
                diff *= e.Message.Content.Length;
                FollowersMax += (int)diff;
            }
            int FollowersAdded = r.Next(FollowersMin, FollowersMax);
            int MoneyAdded = r.Next(MinimalMoney, MaximalMoney);
            LevelTimer GlobalUser = GlobalLevelTimer.FirstOrDefault(w => w.User.Id == e.Author.Id);
            if (GlobalUser != null && GlobalUser.Time.AddMinutes(MSGCooldown) <= DateTime.UtcNow)
            {
                var dbEntry = DB.DBLists.Leaderboard.AsParallel().FirstOrDefault(w => w.ID_User == e.Author.Id);
                dbEntry.Followers += FollowersAdded;
                dbEntry.Bucks += MoneyAdded;
                if (dbEntry.Level < dbEntry.Followers / (300 * (dbEntry.Level + 1) * 0.5))
                {
                    dbEntry.Level += 1;
                }
                GlobalUser.Time = DateTime.UtcNow;
                DB.DBLists.UpdateLeaderboard(dbEntry);
            }
            else
            {
                if (DB.DBLists.Leaderboard.AsParallel().FirstOrDefault(w => w.ID_User == e.Author.Id) == null)
                {
                    CustomMethod.AddUserToLeaderboard(e.Author);
                }
                var dbEntry = DB.DBLists.Leaderboard.AsParallel().FirstOrDefault(w => w.ID_User == e.Author.Id);
                dbEntry.Followers += FollowersAdded;
                dbEntry.Bucks += MoneyAdded;
                if (dbEntry.Level < dbEntry.Followers / (300 * (dbEntry.Level + 1) * 0.5))
                {
                    dbEntry.Level += 1;
                }

                LevelTimer NewToList = new()
                {
                    Time = DateTime.UtcNow,
                    User = e.Author
                };
                GlobalLevelTimer.Add(NewToList);

                DB.DBLists.UpdateLeaderboard(dbEntry);
            }
            ServerLevelTimer ServerUser = ServerLevelTimer.FirstOrDefault(w => w.User.Id == e.Author.Id);
            if (ServerUser != null && ServerUser.Time.AddMinutes(MSGCooldown) <= DateTime.UtcNow)
            {
                var dbEntry = DB.DBLists.ServerRanks.AsParallel().FirstOrDefault(w => w.User_ID == e.Author.Id);
                dbEntry.Followers += FollowersAdded;
                ServerUser.Time = DateTime.UtcNow;
                DB.DBLists.UpdateServerRanks(dbEntry);
            }
            else
            {
                if (DB.DBLists.ServerRanks.AsParallel().FirstOrDefault(w => w.User_ID == e.Author.Id && w.Server_ID == e.Guild.Id) == null)
                {
                    CustomMethod.AddUserToServerRanks(e.Author, e.Guild);
                }
                var dbEntry = DB.DBLists.ServerRanks.AsParallel().FirstOrDefault(w => w.User_ID == e.Author.Id && w.Server_ID == e.Guild.Id);
                dbEntry.Followers += FollowersAdded;

                ServerLevelTimer NewToList = new()
                {
                    Time = DateTime.UtcNow,
                    Guild = e.Guild,
                    User = e.Author
                };
                ServerLevelTimer.Add(NewToList);

                DB.DBLists.UpdateServerRanks(dbEntry);
            }

            long UserServerFollowers = DB.DBLists.ServerRanks.AsParallel().FirstOrDefault(w => w.User_ID == e.Author.Id && w.Server_ID == e.Guild.Id).Followers;
            var RankRolesUnder = DB.DBLists.RankRoles.AsParallel().Where(w => w.Server_ID == e.Guild.Id && w.Server_Rank <= UserServerFollowers && w.Server_Rank != 0).OrderByDescending(w => w.Server_Rank).ToList();
            if (RankRolesUnder.Count != 0 && !(e.Author as DiscordMember).Roles.Any(w => w.Id == RankRolesUnder[0].Role_ID))
            {
                DiscordMember ServerMember = (e.Author as DiscordMember);
                await ServerMember.GrantRoleAsync(e.Guild.Roles.Values.FirstOrDefault(w => w.Id == RankRolesUnder[0].Role_ID));
                if (RankRolesUnder.Count > 1)
                {
                    await ServerMember.RevokeRoleAsync(e.Guild.Roles.Values.FirstOrDefault(w => w.Id == RankRolesUnder[1].Role_ID));
                }
            }
        }

        public static async Task Add_To_Leaderboards(object O, GuildMemberAddEventArgs e)
        {
            var global = (from lb in DB.DBLists.Leaderboard.AsParallel()
                          where lb.ID_User == e.Member.Id
                          select lb).FirstOrDefault();
            if (global is null)
            {
                CustomMethod.AddUserToLeaderboard(e.Member);
            }
            var local = (from lb in DB.DBLists.ServerRanks.AsParallel()
                         where lb.User_ID == e.Member.Id
                         where lb.Server_ID == e.Guild.Id
                         select lb).FirstOrDefault();
            if (local is null)
            {
                CustomMethod.AddUserToServerRanks(e.Member, e.Guild);
            }
        }
    }

    internal class LevelTimer
    {
        public DiscordUser User { get; set; }
        public DateTime Time { get; set; }
    }

    internal class ServerLevelTimer
    {
        public DiscordUser User { get; set; }
        public DiscordGuild Guild { get; set; }
        public DateTime Time { get; set; }
    }
}