using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveBot.Automation
{
    internal static class Leveling
    {
        public static List<LevelTimer> UserLevelTimer { get; set; } = new List<LevelTimer>();
        public static List<ServerLevelTimer> ServerUserLevelTimer { get; set; } = new List<ServerLevelTimer>();

        public static async Task Update_User_Levels(object o, MessageCreateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (!e.Author.IsBot && e.Guild != null)
                {
                    bool checkglobal = false, checklocal = false;
                    Random r = new Random();
                    int
                        MinIntervalA = 10,
                        MaxIntervalA = 30,
                        CharCountSmall = 100,
                        CharCountBig = 500,
                        MinIntervalB = 55,
                        MaxIntervalB = 85;
                    if (e.Message.Content.Length > CharCountBig)
                    {
                        MinIntervalA = MinIntervalB;
                        MaxIntervalA = MaxIntervalB;
                    }
                    else if (e.Message.Content.Length > CharCountSmall && e.Message.Content.Length <= CharCountBig)
                    {
                        MinIntervalA += (MinIntervalB - MinIntervalA - 1 / CharCountBig) * e.Message.Content.Length;
                        MaxIntervalA += (MaxIntervalB - MaxIntervalA - 1 / CharCountBig) * e.Message.Content.Length;
                    }
                    int MinMoney = 2, MaxMoney = 5;
                    int points_added = r.Next(MinIntervalA, MaxIntervalA);
                    int money_added = r.Next(MinMoney, MaxMoney);
                    Parallel.ForEach(UserLevelTimer, Guser =>
                    {
                        if (Guser.User.Id == e.Author.Id)
                        {
                            checkglobal = true;
                            if (Guser.Time.AddMinutes(2) <= DateTime.Now)
                            {
                                List<DB.Leaderboard> Leaderboard = DB.DBLists.Leaderboard;
                                var global = (from lb in Leaderboard.AsParallel()
                                              where lb.ID_User == e.Author.Id
                                              select lb).FirstOrDefault();
                                global.Followers += points_added;
                                global.Bucks += money_added;
                                if (global.Level < global.Followers / (300 * (global.Level + 1) * 0.5))
                                {
                                    global.Level += 1;
                                }
                                Guser.Time = DateTime.Now;
                                DB.DBLists.UpdateLeaderboard(global);
                            }
                        }
                    });
                    Parallel.ForEach(ServerUserLevelTimer, Suser =>
                    {
                        if (Suser.User.Id == e.Author.Id && Suser.Guild.Id == e.Guild.Id)
                        {
                            checklocal = true;
                            if (Suser.Time.AddMinutes(2) <= DateTime.Now)
                            {
                                List<DB.ServerRanks> Leaderboard = DB.DBLists.ServerRanks;
                                var local = (from lb in Leaderboard.AsParallel()
                                             where lb.User_ID == e.Author.Id
                                             where lb.Server_ID == e.Channel.Guild.Id
                                             select lb).FirstOrDefault();
                                local.Followers += points_added;
                                Suser.Time = DateTime.Now; DB.DBLists.UpdateServerRanks(local);
                            }
                        }
                    });
                    if (!checkglobal)
                    {
                        List<DB.Leaderboard> Leaderboard = DB.DBLists.Leaderboard;
                        var global = (from lb in Leaderboard
                                      where lb.ID_User == e.Author.Id
                                      select lb).FirstOrDefault();
                        if (global is null)
                        {
                            CustomMethod.AddUserToLeaderboard(e.Author);
                        }
                        global = (from lb in Leaderboard.AsParallel()
                                  where lb.ID_User == e.Author.Id
                                  select lb).FirstOrDefault();
                        points_added = r.Next(MinIntervalA, MaxIntervalA);

                        global.Followers += points_added;
                        global.Bucks += money_added;
                        if (global.Level < global.Followers / (300 * (global.Level + 1) * 0.5))
                        {
                            global.Level += 1;
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
                        var local = (from lb in Leaderboard.AsParallel()
                                     where lb.User_ID == e.Author.Id
                                     where lb.Server_ID == e.Channel.Guild.Id
                                     select lb).FirstOrDefault();
                        if (local != null)
                        {
                            local.Followers += points_added;
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
                    var userrank = (from sr in DB.DBLists.ServerRanks.AsParallel()
                                    where sr.Server_ID == e.Guild.Id
                                    where sr.User_ID == e.Author.Id
                                    select sr).FirstOrDefault().Followers;
                    var rankedroles = (from rr in DB.DBLists.RankRoles
                                       where rr.Server_Rank != 0
                                       where rr.Server_Rank <= userrank
                                       where rr.Server_ID == e.Guild.Id
                                       select rr).ToList();
                    List<DiscordRole> roles = new List<DiscordRole>();
                    foreach (var item in rankedroles)
                    {
                        if (item.Server_ID == e.Guild.Id)
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
            });
            await Task.Delay(1);
        }

        public static async Task Add_To_Leaderboards(object O, GuildMemberAddEventArgs e)
        {
            _ = Task.Run(() =>
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
             });
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