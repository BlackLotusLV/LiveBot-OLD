using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text;

namespace LiveBot.Commands
{
    [Group("@")]
    [Description("Administrative commands")]
    [Hidden]
    [RequirePermissions(DSharpPlus.Permissions.KickMembers)]
    public class AdminCommands : BaseCommandModule
    {
        [Command("uptime")] //uptime command
        [Aliases("live")]
        public async Task Uptime(CommandContext ctx)
        {
            DateTime current = DateTime.Now;
            TimeSpan time = current - Program.start;
            await ctx.Message.RespondAsync($"{time.Days} Days {time.Hours}:{time.Minutes}.{time.Seconds}");
        }

        [Command("say")]
        [Description("Bot repeats whatever you tell it to repeat")]
        public async Task Say(CommandContext ctx, DiscordChannel channel, [Description("bot will repeat this")] [RemainingText] string word)
        {
            await ctx.Message.DeleteAsync();
            await channel.SendMessageAsync(word);
        }

        [Command("Warn")]
        [Description("Warns a user")]
        public async Task Warning(CommandContext ctx, DiscordMember username, [RemainingText] string reason="Reason not specified")
        {
            await ctx.TriggerTypingAsync();
            await ctx.Message.DeleteAsync();
            var WarnedUserStats = DB.DBLists.ServerRanks.FirstOrDefault(f => ctx.Guild.Id.ToString().Equals(f.Server_ID) && username.Id.ToString().Equals(f.User_ID));
            var ServerSettings = DB.DBLists.ServerSettings.FirstOrDefault(f => ctx.Guild.Id.ToString().Equals(f.ID_Server));
            string modmsg,DM;
            string uid = username.Id.ToString(), aid = ctx.User.Id.ToString();
            bool kick = false;
            if (ServerSettings.WKB_Log!="0")
            {
                DiscordChannel modlog = ctx.Guild.GetChannel(Convert.ToUInt64(ServerSettings.WKB_Log));
                if (WarnedUserStats is null) // creates new entry in DB (Followers set to default value)
                {
                    DB.ServerRanks newEntry = new DB.ServerRanks
                    {
                        Server_ID = ctx.Guild.Id.ToString(),
                        Ban_Count = 0,
                        Kick_Count = 0,
                        Warning_Level = 1,
                        User_ID = username.Id.ToString()
                    };
                    DB.DBLists.InsertServerRanks(newEntry);
                }
                else
                {
                    WarnedUserStats.Warning_Level++;
                    if (WarnedUserStats.Warning_Level>2)
                    {
                        DM = $"You have been kicked by {ctx.User.Mention} for exceeding the warning level(2). Your level: {WarnedUserStats.Warning_Level}\n" +
                            $"Warning reason: {reason}";
                        try
                        {
                            await username.SendMessageAsync(DM);
                        }
                        catch
                        {
                            await modlog.SendMessageAsync($"{username.Mention} could not be contacted via DM. Reason not sent");
                        }
                        kick = true;
                        await username.RemoveAsync("Exceeded warning limit!");
                    }

                    DB.DBLists.UpdateServerRanks(new List<DB.ServerRanks> { WarnedUserStats });
                }
                DB.Warnings newWarning = new DB.Warnings
                {
                    Reason = reason,
                    Active = true,
                    Date = DateTime.Now.ToString("yyyy-MM-dd"),
                    Admin_ID = aid,
                    User_ID = uid,
                    Server_ID = ctx.Guild.Id.ToString()
                };
                DB.DBLists.InsertWarnings(newWarning);

                int warning_count = DB.DBLists.Warnings.Where(w => w.User_ID == username.Id.ToString() && w.Server_ID==ctx.Guild.Id.ToString()).Count();

                modmsg = $"**Warned user:**\t{username.Mention}\n**Warning level:**\t {WarnedUserStats.Warning_Level}\t**Warning count:**\t {warning_count}\n**Warned by**\t{ctx.User.Username}\n**Reason:** {reason}";
                DM = $"You have been warned by <@{ctx.User.Id}>.\n**Warning Reason:**\t{reason}\n**Warning Level:** {WarnedUserStats.Warning_Level}";
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor(0xf90707),
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        IconUrl = username.AvatarUrl,
                        Name = username.Username
                    },
                    Description = modmsg
                };
                if (!kick)
                {
                    try
                    {
                        await username.SendMessageAsync(DM);
                    }
                    catch
                    {
                        await modlog.SendMessageAsync($":exclamation::exclamation:{username.Mention} could not be contacted via DM. Reason not sent");
                    }
                }
                await modlog.SendMessageAsync(embed: embed);
            }
            else
            {
                await ctx.RespondAsync("This server has not set up this feature!");
            }
            DiscordMessage info = await ctx.Channel.SendMessageAsync($"{username.Username}, Has been warned!");
            await Task.Delay(10000).ContinueWith(t => info.DeleteAsync());
        }

        [Command("unwarn")]
        [Description("Removes a warning from a user")]
        public async Task Unwarning(CommandContext ctx, DiscordMember username)
        {
            await ctx.TriggerTypingAsync();
            await ctx.Message.DeleteAsync();
            var WarnedUserStats = DB.DBLists.ServerRanks.FirstOrDefault(f => ctx.Guild.Id.ToString().Equals(f.Server_ID) && username.Id.ToString().Equals(f.User_ID));
            var ServerSettings = DB.DBLists.ServerSettings.FirstOrDefault(f => ctx.Guild.Id.ToString().Equals(f.ID_Server));
            var Warnings = DB.DBLists.Warnings.Where(f => ctx.Guild.Id.ToString().Equals(f.Server_ID) && username.Id.ToString().Equals(f.User_ID)).ToList();
            string MSGOut,modmsg;
            bool check = true;
            if (ServerSettings.WKB_Log!="0")
            {
                DiscordChannel modlog = ctx.Guild.GetChannel(Convert.ToUInt64(ServerSettings.WKB_Log));
                if (WarnedUserStats is null)
                {
                    MSGOut = $"This user, {username.Username}, has no warning history.";
                }
                else
                {
                    if (WarnedUserStats.Warning_Level is 0)
                    {
                        MSGOut = "This user warning level is already 0";
                        check = false;
                    }
                    else
                    {
                        WarnedUserStats.Warning_Level-=1;
                        MSGOut = $"Warning level lowered for {username.Username}";
                        var entry = Warnings.Where(f=>f.Active is true).OrderBy(f => f.ID_Warning).FirstOrDefault();
                        entry.Active = false;
                        DB.DBLists.UpdateWarnings(new List<DB.Warnings> { entry });
                        DB.DBLists.UpdateServerRanks(new List<DB.ServerRanks> { WarnedUserStats });
                    }
                    if (check)
                    {
                        modmsg = $"{username.Mention} has been unwarned by {ctx.User.Mention}. Warning level now {WarnedUserStats.Warning_Level}";
                        DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor(0xf90707),
                            Author = new DiscordEmbedBuilder.EmbedAuthor
                            {
                                IconUrl = username.AvatarUrl,
                                Name = username.Username
                            },
                            Description = modmsg
                        };
                        try
                        {
                            await username.SendMessageAsync($"Your warning level has been lowerd to {WarnedUserStats.Warning_Level} by {ctx.User.Mention}");
                        }
                        catch
                        {
                            await modlog.SendMessageAsync($":exclamation::exclamation:{username.Mention} could not be contacted via DM. Reason not sent");
                        }
                        await modlog.SendMessageAsync(embed: embed);
                    }
                    DiscordMessage msg = await ctx.RespondAsync(MSGOut);
                    await Task.Delay(10000);
                    await msg.DeleteAsync();
                }
            }
            else
            {
                await ctx.RespondAsync("This server has not set up this feature!");
            }
            DiscordMessage info = await ctx.Channel.SendMessageAsync($"{username.Username}, Has been un-warned!");
            await Task.Delay(10000).ContinueWith(t => info.DeleteAsync());
        }
        [Command("getkicks")]
        [Description("Shows user warning, kick and ban history")]
        public async Task GetKicks(CommandContext ctx, DiscordUser username)
        {
            await ctx.Message.DeleteAsync();
            DB.DBLists.LoadServerRanks();
            DB.DBLists.LoadWarnings();
            List<DB.ServerRanks> ServerRanks = DB.DBLists.ServerRanks;
            List<DB.Warnings> warnings = DB.DBLists.Warnings;
            string uid = username.Id.ToString();
            bool UserCheck = false;
            int kcount = 0, bcount = 0, wlevel = 0, wcount = 0;
            string reason = "";
            var UserStats = ServerRanks.FirstOrDefault(f => uid.Equals(f.User_ID) && ctx.Guild.Id.ToString().Equals(f.Server_ID));
            if (UserStats != null)
            {
                UserCheck = true;
                kcount = UserStats.Kick_Count;
                bcount = UserStats.Ban_Count;
                wlevel = UserStats.Warning_Level;
                var WarningsList = warnings.Where(w => w.User_ID == uid && w.Server_ID==ctx.Guild.Id.ToString()).ToList();
                foreach (var item in WarningsList)
                {
                    if ((bool)item.Active == true)
                    {
                        reason += $"By: <@{item.Admin_ID.ToString()}>\t Reason: {item.Reason.ToString()}\n";
                    }
                    wcount++;
                }
            }
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(0xFF6600),
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = username.Username,
                    IconUrl = username.AvatarUrl
                },
                Description = $"",
                Title = "User kick Count",
                ThumbnailUrl = username.AvatarUrl
            };
            embed.AddField("Warning level: ", $"{wlevel}", true);
            embed.AddField("Times warned: ", $"{wcount}", true);
            embed.AddField("Times banned: ", $"{bcount}", true);
            embed.AddField("Times kicked: ", $"{kcount}", true);
            embed.AddField("Warning reasons: ", $"-{reason}-", true);
            if (!UserCheck)
            {
                await ctx.RespondAsync($"{ctx.User.Mention}, This user has no warning, kick and/or ban history in this server.");
            }
            else
            {
                await ctx.RespondAsync(embed: embed);
            }
        }

        [Command("vote")]
        [Description("starts a timed vote")]
        public async Task Vote(CommandContext ctx, [Description("What to vote about?")] params string[] topic)
        {
            await ctx.Message.DeleteAsync();
            string f = CustomMethod.ParamsStringConverter(topic);
            DiscordMessage msg = await ctx.Message.RespondAsync(f);
            DiscordEmoji up = DiscordEmoji.FromName(ctx.Client, ":thumbsup:");
            DiscordEmoji down = DiscordEmoji.FromName(ctx.Client, ":thumbsdown:");
            await msg.CreateReactionAsync(up);
            await Task.Delay(500);
            await msg.CreateReactionAsync(down);
        }

        [Command("poll")]
        [Description("creates a poll up to 10 choices. Delimiter \".\"")]
        public async Task Poll(CommandContext ctx, [Description("Options")]params string[] input)
        {
            await ctx.Message.DeleteAsync();
            string f = CustomMethod.ParamsStringConverter(input);
            char delimiter = '.';
            string[] options = f.Split(delimiter);
            string final = "";
            string[] emotename = new string[] { ":zero:", ":one:", ":two:", ":three:", ":four:", ":five:", ":six:", ":seven:", ":eight:", ":nine:" };
            int i = 0;
            foreach (var item in options)
            {
                final = $"{final}{emotename[i]} {item} \n";
                i++;
            }
            DiscordMessage msg = await ctx.Message.RespondAsync(final);
            DiscordEmoji zero = DiscordEmoji.FromName(ctx.Client, ":zero:");
            DiscordEmoji one = DiscordEmoji.FromName(ctx.Client, ":one:");
            DiscordEmoji two = DiscordEmoji.FromName(ctx.Client, ":two:");
            DiscordEmoji three = DiscordEmoji.FromName(ctx.Client, ":three:");
            DiscordEmoji four = DiscordEmoji.FromName(ctx.Client, ":four:");
            DiscordEmoji five = DiscordEmoji.FromName(ctx.Client, ":five:");
            DiscordEmoji six = DiscordEmoji.FromName(ctx.Client, ":six:");
            DiscordEmoji seven = DiscordEmoji.FromName(ctx.Client, ":seven:");
            DiscordEmoji eight = DiscordEmoji.FromName(ctx.Client, ":eight:");
            DiscordEmoji nine = DiscordEmoji.FromName(ctx.Client, ":nine:");
            DiscordEmoji[] emotes = new DiscordEmoji[] { zero, one, two, three, four, five, six, seven, eight, nine };
            for (int j = 0; j < i; j++)
            {
                await msg.CreateReactionAsync(emotes[j]);
                await Task.Delay(300);
            }
        }

        [Command("rinfo")]
        [Aliases("roleinfo")]
        [Description("give the ID of the role that was mentioned with the command")]
        public async Task RInfo(CommandContext ctx, DiscordRole role)
        {
            await ctx.RespondAsync($"**Role ID:**{role.Id}");
        }

        [Command("faq")]
        [RequirePermissions(DSharpPlus.Permissions.BanMembers)]
        [Description("Edits the existing FAQ message")]
        [Priority(10)]
        public async Task FAQ(CommandContext ctx, DiscordMessage faqMsg, string type, [RemainingText] string str1)
        {
            string og = faqMsg.Content;
            og = og.Replace("*", string.Empty);
            string[] str2 = og.Split("A: ");
            str2[0] = str2[0].Remove(str2[0].Length - 2, 2);
            if (type.ToLower() == "q")
            {
                str2[0] = $"Q: {str1}";
            }
            else if (type.ToLower() == "a")
            {
                str2[1] = str1;
            }
            await faqMsg.ModifyAsync($"**{str2[0]}**\n *A: {str2[1].TrimEnd()}*");
            await ctx.Message.DeleteAsync();
        }

        [Command("faq")]
        [RequirePermissions(DSharpPlus.Permissions.BanMembers)]
        [Description("Creates an FAQ post. Delimiter is `|`")]
        [Priority(9)]
        public async Task FAQ(CommandContext ctx, [RemainingText] string str1)
        {
            string[] str2 = str1.Split('|');
            await ctx.RespondAsync($"**Q: {str2[0]}**\n*A: {str2[1].TrimEnd()}*");
            await ctx.Message.DeleteAsync();
        }
        [Command("warncount")]
        [Description("Shows the count of warnings issues by each admin")]
        public async Task WarnCount(CommandContext ctx)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.AppendLine("```csharp\n\t\tUsername");
            foreach (var item in DB.DBLists.Warnings.GroupBy(w=>w.Admin_ID)
                .Select(s=>new { 
                    Admin_ID = s.Key,
                    Count = s.Count() 
                })
                .OrderByDescending(o=>o.Count))
            {
                i++;
                DiscordUser user = await Program.Client.GetUserAsync(Convert.ToUInt64(item.Admin_ID));
                sb.AppendLine($"[{i}]\t# {user.Username}\n\tWarnings Given {item.Count}");
            }
            sb.AppendLine("```");
            await ctx.RespondAsync(sb.ToString());
        }
        [Command("prune")]
        [Description("Deletes chat history, up to 100 messages per use")]
        [Cooldown(1,180,CooldownBucketType.Channel)]
        public async Task Prune(CommandContext ctx, int MessageCount = 1)
        {
            await ctx.Message.DeleteAsync().ContinueWith(t => ctx.TriggerTypingAsync());
            var lid = 0ul;
            int count = 0;
            Console.WriteLine(0);
            for (int i = 0; i < MessageCount; i+=100)
            {
                var msgs = await ctx.Channel.GetMessagesBeforeAsync(lid != 0 ? lid : ctx.Message.Id, Math.Min(MessageCount - i, 100)).ConfigureAwait(false);
                var lmsg = msgs.FirstOrDefault();
                if (lmsg==null)
                {
                    break;
                }
                lid = lmsg.Id;
                count++;
                await ctx.Channel.DeleteMessagesAsync(msgs).ConfigureAwait(false);
            }
            DiscordMessage info = await ctx.RespondAsync($"{MessageCount} messages deleted");
            await Task.Delay(5000).ContinueWith(t => info.DeleteAsync());
        }
    }
}