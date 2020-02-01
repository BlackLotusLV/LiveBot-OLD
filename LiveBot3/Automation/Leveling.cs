using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveBot.Automation
{
    internal class Leveling
    {
        public static List<LevelTimer> UserLevelTimer = new List<LevelTimer>();
        public static List<ServerLevelTimer> ServerUserLevelTimer = new List<ServerLevelTimer>();

        public static async Task Update_User_Levels(MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot)
            {
                DB.DBLists.LoadServerRanks();
                DB.DBLists.LoadLeaderboard();
                bool checkglobal = false, checklocal = false;
                Random r = new Random();
                int MinInterval = 10, MaxInterval = 30;
                if (e.Message.Content.Length > 500)
                {
                    MinInterval = 40;
                    MaxInterval = 70;
                }
                else if (e.Message.Content.Length > 100 && e.Message.Content.Length <= 500)
                {
                    MinInterval += (29 / 500) * e.Message.Content.Length;
                    MaxInterval += (29 / 500) * e.Message.Content.Length;
                }
                int MinMoney = 2, MaxMoney = 5;
                int points_added = r.Next(MinInterval, MaxInterval);
                int money_added = r.Next(MinMoney, MaxMoney);
                foreach (var Guser in UserLevelTimer)
                {
                    if (Guser.User.Id == e.Author.Id)
                    {
                        checkglobal = true;
                        if (Guser.Time.AddMinutes(2) <= DateTime.Now)
                        {
                            List<DB.Leaderboard> Leaderboard = DB.DBLists.Leaderboard;
                            var global = (from lb in Leaderboard
                                          where lb.ID_User == e.Author.Id.ToString()
                                          select lb).ToList();
                            global[0].Followers = (long)global[0].Followers + points_added;
                            global[0].Bucks = (long)global[0].Bucks + money_added;
                            if ((int)global[0].Level < (long)global[0].Followers / (300 * ((int)global[0].Level + 1) * 0.5))
                            {
                                global[0].Level = (int)global[0].Level + 1;
                            }
                            Guser.Time = DateTime.Now;
                            DB.DBLists.UpdateLeaderboard(global);
                        }
                    }
                }
                foreach (var Suser in ServerUserLevelTimer)
                {
                    if (Suser.User.Id == e.Author.Id)
                    {
                        if (Suser.Guild.Id == e.Guild.Id)
                        {
                            checklocal = true;
                            if (Suser.Time.AddMinutes(2) <= DateTime.Now)
                            {
                                List<DB.ServerRanks> Leaderboard = DB.DBLists.ServerRanks;
                                var local = (from lb in Leaderboard
                                             where lb.User_ID == e.Author.Id.ToString()
                                             where lb.Server_ID == e.Channel.Guild.Id.ToString()
                                             select lb).ToList();
                                local[0].Followers = (long)local[0].Followers + points_added;
                                Suser.Time = DateTime.Now; DB.DBLists.UpdateServerRanks(local);
                            }
                        }
                    }
                }
                if (!checkglobal)
                {
                    List<DB.Leaderboard> Leaderboard = DB.DBLists.Leaderboard;
                    var global = (from lb in Leaderboard
                                  where lb.ID_User == e.Author.Id.ToString()
                                  select lb).ToList();
                    if (global.Count == 0)
                    {
                        CustomMethod.AddUserToLeaderboard(e.Author);
                    }
                    global = (from lb in Leaderboard
                              where lb.ID_User == e.Author.Id.ToString()
                              select lb).ToList();
                    points_added = r.Next(MinInterval, MaxInterval);
                    if (global.Count == 1)
                    {
                        global[0].Followers = (long)global[0].Followers + points_added;
                        global[0].Bucks = (long)global[0].Bucks + money_added;
                        if ((int)global[0].Level < (long)global[0].Followers / (300 * ((int)global[0].Level + 1) * 0.5))
                        {
                            global[0].Level = (int)global[0].Level + 1;
                        }
                    }
                    LevelTimer NewToList = new LevelTimer
                    {
                        Time = DateTime.Now,
                        User = e.Author
                    };
                    UserLevelTimer.Add(NewToList);
                    DB.DBLists.UpdateLeaderboard(global);
                }
                if (!checklocal)
                {
                    List<DB.ServerRanks> Leaderboard = DB.DBLists.ServerRanks;
                    CustomMethod.AddUserToServerRanks(e.Author, e.Guild);
                    var local = (from lb in Leaderboard
                                 where lb.User_ID == e.Author.Id.ToString()
                                 where lb.Server_ID == e.Channel.Guild.Id.ToString()
                                 select lb).ToList();
                    if (local.Count > 0)
                    {
                        local[0].Followers = (long)local[0].Followers + points_added;
                    }
                    ServerLevelTimer NewToList = new ServerLevelTimer
                    {
                        Time = DateTime.Now,
                        User = e.Author,
                        Guild = e.Guild
                    };
                    ServerUserLevelTimer.Add(NewToList);
                    DB.DBLists.UpdateServerRanks(local);
                }
                //*/
                var userrank = (from sr in DB.DBLists.ServerRanks
                                where sr.Server_ID == e.Guild.Id.ToString()
                                where sr.User_ID == e.Author.Id.ToString()
                                select sr).ToList()[0].Followers;
                var rankedroles = (from rr in DB.DBLists.RankRoles
                                   where rr.Server_Rank != 0
                                   where rr.Server_Rank <= userrank
                                   where rr.Server_ID == e.Guild.Id.ToString()
                                   select rr).ToList();
                List<DiscordRole> roles = new List<DiscordRole>();
                foreach (var item in rankedroles)
                {
                    if (item.Server_ID == e.Guild.Id.ToString())
                    {
                        roles.Add(e.Guild.GetRole(Convert.ToUInt64(item.Role_ID)));
                    }
                }
                DiscordMember member = await e.Guild.GetMemberAsync(e.Author.Id);
                for (int i = 0; i < roles.Count; i++)
                {
                    if (i != roles.Count - 1 && member.Roles.Contains(roles[i]))
                    {
                        await member.RevokeRoleAsync(roles[i]);
                    }
                    else if (i == roles.Count - 1 && !member.Roles.Contains(roles[i]))
                    {
                        await member.GrantRoleAsync(roles[i]);
                    }
                }
            }
        }

        public static async Task Add_To_Leaderboards(GuildMemberAddEventArgs e)
        {
            var global = (from lb in DB.DBLists.Leaderboard
                          where lb.ID_User == e.Member.Id.ToString()
                          select lb).ToList();
            if (global.Count == 0)
            {
                CustomMethod.AddUserToLeaderboard(e.Member);
            }
            var local = (from lb in DB.DBLists.ServerRanks
                         where lb.User_ID == e.Member.Id.ToString()
                         where lb.Server_ID == e.Guild.Id.ToString()
                         select lb).ToList();
            if (local.Count == 0)
            {
                CustomMethod.AddUserToServerRanks(e.Member, e.Guild);
            }
            await Task.Delay(1);
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