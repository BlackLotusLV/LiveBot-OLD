﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiveBot.Commands
{
    [Group("@")]
    [Description("Administrative commands")]
    [Hidden]
    [RequirePermissions(Permissions.KickMembers)]
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
        public async Task Say(CommandContext ctx, DiscordChannel channel, [Description("bot will repeat this")] [RemainingText] string word = "")
        {
            await ctx.Message.DeleteAsync();
            await channel.SendMessageAsync(word);
        }

        [Command("dm")]
        [Aliases("pm")]
        [Description("Bot sends a DM to the user")]
        public async Task DirrectMessage(CommandContext ctx, DiscordMember username, [RemainingText] string message)
        {
            await ctx.TriggerTypingAsync();
            DB.DBLists.LoadServerSettings();
            DB.ServerSettings ServerSettings = DB.DBLists.ServerSettings.FirstOrDefault(f => ctx.Guild.Id.ToString().Equals(f.ID_Server));

            if (ServerSettings.WKB_Log != "0")
            {
                if (username.Guild == ctx.Guild && message != "")
                {
                    DiscordChannel modlog = ctx.Guild.GetChannel(Convert.ToUInt64(ServerSettings.WKB_Log));
                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(0xf90707),
                        Author = new DiscordEmbedBuilder.EmbedAuthor
                        {
                            IconUrl = username.AvatarUrl,
                            Name = username.Username
                        },
                        Description = $"{username.Mention} was dirrect messaged via the bot by **{ctx.User.Username}**"
                    };
                    embed.AddField("Message", message);
                    try
                    {
                        await username.SendMessageAsync($"**Official message from a moderator**\n{message}\n*(This is an automated message, contact a moderator if you have any questions.)*");
                        await modlog.SendMessageAsync(embed: embed);
                    }
                    catch (Exception)
                    {
                        DiscordMessage msg = await ctx.RespondAsync("This user has dissabled DMs or has blocked the bot.");
                        await Task.Delay(5000).ContinueWith(t => msg.DeleteAsync());
                    }
                }
                else
                {
                    DiscordMessage msg = await ctx.RespondAsync($"{ctx.User.Mention} You didn't write a message.");
                    await Task.Delay(5000).ContinueWith(t => msg.DeleteAsync());
                }
            }
            else
            {
                DiscordMessage msg = await ctx.RespondAsync("This server has not set up this feature!");
                await Task.Delay(5000).ContinueWith(t => msg.DeleteAsync());
            }
            await ctx.Message.DeleteAsync();
        }

        [Command("warn")]
        [Description("Warns a user")]
        [Cooldown(1, 30, CooldownBucketType.Guild)]
        public async Task Warning(CommandContext ctx, DiscordMember username, [RemainingText] string reason = "Reason not specified")
        {
            await ctx.TriggerTypingAsync();
            await CustomMethod.WarnUserAsync(username, ctx.Member, ctx.Guild, ctx.Channel, reason, false);
            await ctx.Message.DeleteAsync();
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
            string MSGOut, modmsg = "";
            bool check = true;
            if (ServerSettings.WKB_Log != "0")
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
                        WarnedUserStats.Warning_Level -= 1;
                        MSGOut = $"Warning level lowered for {username.Username}";
                        var entry = Warnings.Where(f => f.Active is true).OrderBy(f => f.ID_Warning).FirstOrDefault();
                        entry.Active = false;
                        DB.DBLists.UpdateWarnings(new List<DB.Warnings> { entry });
                        DB.DBLists.UpdateServerRanks(new List<DB.ServerRanks> { WarnedUserStats });
                    }
                    if (check)
                    {
                        DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor(0xf90707),
                            Author = new DiscordEmbedBuilder.EmbedAuthor
                            {
                                IconUrl = username.AvatarUrl,
                                Name = username.Username
                            },
                            Description = $"{username.Mention} has been unwarned by {ctx.User.Mention}. Warning level now {WarnedUserStats.Warning_Level}"
                        };
                        try
                        {
                            await username.SendMessageAsync($"Your warning level in **{ctx.Guild.Name}** has been lowerd to {WarnedUserStats.Warning_Level} by {ctx.User.Mention}");
                        }
                        catch
                        {
                            modmsg = $":exclamation::exclamation:{username.Mention} could not be contacted via DM.";
                        }
                        await modlog.SendMessageAsync(modmsg, embed: embed);
                    }
                    DiscordMessage msg = await ctx.RespondAsync(MSGOut);
                    await Task.Delay(10000);
                    await msg.DeleteAsync();
                }
                DiscordMessage info = await ctx.Channel.SendMessageAsync($"{username.Username}, Has been un-warned!");
                await Task.Delay(10000).ContinueWith(t => info.DeleteAsync());
            }
            else
            {
                await ctx.RespondAsync("This server has not set up this feature!");
            }
        }

        [Command("getkicks")]
        [Description("Shows user warning, kick and ban history")]
        public async Task GetKicks(CommandContext ctx, DiscordUser username)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
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
                var WarningsList = warnings.Where(w => w.User_ID == uid && w.Server_ID == ctx.Guild.Id.ToString()).ToList();
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
                await ctx.RespondAsync($"{ctx.User.Mention}", embed: embed);
            }
        }

        [Command("vote")]
        [Description("starts a vote")]
        public async Task Vote(CommandContext ctx, [Description("What to vote about?")] [RemainingText] string topic)
        {
            await ctx.Message.DeleteAsync();
            DiscordMessage msg = await ctx.Message.RespondAsync(topic);
            DiscordEmoji up = DiscordEmoji.FromName(ctx.Client, ":thumbsup:");
            DiscordEmoji down = DiscordEmoji.FromName(ctx.Client, ":thumbsdown:");
            await msg.CreateReactionAsync(up);
            await Task.Delay(500);
            await msg.CreateReactionAsync(down);
        }

        [Command("poll")]
        [Description("creates a poll up to 10 choices. Delimiter \".\"")]
        public async Task Poll(CommandContext ctx, [Description("Options")] [RemainingText] string input)
        {
            await ctx.Message.DeleteAsync();
            char delimiter = '.';
            string[] options = input.Split(delimiter);
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
            foreach (var item in DB.DBLists.Warnings.GroupBy(w => w.Admin_ID)
                .Select(s => new
                {
                    Admin_ID = s.Key,
                    Count = s.Count()
                })
                .OrderByDescending(o => o.Count))
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
        [Cooldown(1, 180, CooldownBucketType.Channel)]
        public async Task Prune(CommandContext ctx, int MessageCount = 1)
        {
            await ctx.Message.DeleteAsync().ContinueWith(t => ctx.TriggerTypingAsync());
            var lid = 0ul;
            int count = 0;
            for (int i = 0; i < MessageCount; i += 100)
            {
                var msgs = await ctx.Channel.GetMessagesBeforeAsync(lid != 0 ? lid : ctx.Message.Id, Math.Min(MessageCount - i, 100)).ConfigureAwait(false);
                var lmsg = msgs.FirstOrDefault();
                if (lmsg == null)
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

        [Command("activity")]
        [Description("Used to check the users stream title and game, test command")]
        public async Task Activity(CommandContext ctx, DiscordUser user)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Game: {user.Presence.Activities.Where(w => w.Name.ToLower() == "twitch").FirstOrDefault().RichPresence.State}");
            sb.AppendLine($"Title: {user.Presence.Activities.Where(w => w.Name.ToLower() == "twitch").FirstOrDefault().RichPresence.Details}");
            //sb.AppendLine($"Game: {user.Presence.Activity.RichPresence.State}");
            //sb.AppendLine($"Title: {user.Presence.Activity.RichPresence.Details}");
            await ctx.RespondAsync(sb.ToString());
        }

        [Command("banword")]
        [Description("Adds a word to banned word list")]
        public async Task BanWord(CommandContext ctx,
            [Description("The word that is banned")] string BannedWord,
            [Description("What the user will be warned with when using such word")] [RemainingText] string warning)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            string info = "";
            DB.DBLists.LoadBannedWords();
            var duplicate = (from bw in DB.DBLists.AMBannedWords
                             where bw.Server_ID == ctx.Guild.Id.ToString()
                             where bw.Word == BannedWord.ToLower()
                             select bw).FirstOrDefault();
            if (duplicate is null)
            {
                DB.AMBannedWords newEntry = new DB.AMBannedWords()
                {
                    Word = BannedWord.ToLower(),
                    Offense = warning,
                    Server_ID = ctx.Guild.Id.ToString()
                };
                DB.DBLists.InsertBannedWords(newEntry);
                info = $"The word `{BannedWord.ToLower()}` has been added to the list. They will be warned with `{warning}`";
            }
            else
            {
                info = $"The word `{BannedWord.ToLower()}` is already in the database for this server.";
            }
            DiscordMessage msg = await ctx.RespondAsync(info);
            await Task.Delay(5000).ContinueWith(t => msg.DeleteAsync());
        }

        [Command("unbanword")]
        [Description("Removes a word from the banned word list")]
        public async Task UnbanWord(CommandContext ctx,
            [Description("The word you want to be removed from the list.")] string word)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            string info = "";
            DB.DBLists.LoadBannedWords();
            var DBEntry = (from bw in DB.DBLists.AMBannedWords
                           where bw.Server_ID == ctx.Guild.Id.ToString()
                           where bw.Word == word.ToLower()
                           select bw).FirstOrDefault();
            if (DBEntry != null)
            {
                var context = new DB.AMBannedWordsContext();
                context.Remove(DBEntry);
                context.SaveChanges();
                info = $"The word `{word.ToLower()}` has been removed from the list.";
            }
            else
            {
                info = $"The word `{word.ToLower()}` is not listed for this server.";
            }
            DiscordMessage msg = await ctx.RespondAsync(info);
            await Task.Delay(5000).ContinueWith(t => msg.DeleteAsync());
        }
    }
}