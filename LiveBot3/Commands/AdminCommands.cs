using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        public async Task Warning(CommandContext ctx, DiscordMember username, params string[] reason)
        {
            await ctx.Message.DeleteAsync();
            List<DB.UserWarnings> userWarnings = DB.DBLists.UserWarnings;
            DiscordChannel modlog = ctx.Guild.GetChannel(440365270893330432);
            string f = CustomMethod.ParamsStringConverter(reason);
            string modmsg, DM;
            string uid = username.Id.ToString(), aid = ctx.User.Id.ToString();
            bool UserCheck = false, kick = false;
            int level = 1, count = 1;
            foreach (var item in userWarnings)
            {
                if (item.ID_User.ToString() == uid)
                {
                    UserCheck = true;
                    level = (int)item.Warning_Level + 1;
                    count = (int)item.Warning_Count + 1;
                    if (level > 2)
                    {
                        DM = $"You have been kicked by {ctx.User.Mention} for exceeding the warning level(2). Your level: {level}\n" +
                            $"Warning reason: {f}";
                        try
                        {
                            await username.SendMessageAsync(DM);
                        }
                        catch
                        {
                            await modlog.SendMessageAsync($"{username.Mention} could not be contacted via DM. Reason not sent");
                        }
                        await username.RemoveAsync();
                        kick = true;
                    }
                    item.Warning_Level = level;
                    item.Warning_Count = count;
                    DB.DBLists.UpdateUserWarnings(userWarnings);
                }
            }
            if (!UserCheck)
            {
                DB.UserWarnings newEntry = new DB.UserWarnings
                {
                    Warning_Level = level,
                    Warning_Count = level,
                    Kick_Count = 0,
                    Ban_Count = 0,
                    ID_User = uid
                };
                DB.DBLists.InsertUserWarnings(newEntry);
            }
            DB.Warnings newWarning = new DB.Warnings
            {
                Reason = f,
                Active = true,
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Admin_ID = aid,
                User_ID = uid
            };
            DB.DBLists.InsertWarnings(newWarning);

            modmsg = $"**Warned user:**\t{username.Mention}\n**Warning level:**\t {level}\t**Warning count:\t {count}\n**Warned by**\t{ctx.User.Username}\n**Reason:** {f}";
            DM = $"You have been warned by <@{ctx.User.Id}>.\n**Warning Reason:**\t{f}\n**Warning Level:** {level}";
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

        [Command("unwarn")]
        [Description("Removes a warning from a user")]
        public async Task Unwarning(CommandContext ctx, DiscordMember username)
        {
            await ctx.Message.DeleteAsync();

            List<DB.UserWarnings> userWarnings = DB.DBLists.UserWarnings;
            List<DB.Warnings> warnings = DB.DBLists.Warnings;
            DiscordChannel modlog = ctx.Guild.GetChannel(440365270893330432);
            string modmsg;
            string uid = username.Id.ToString(), aid = ctx.User.Id.ToString();
            bool UserCheck = false;
            int level = 0;
            foreach (var item in userWarnings)
            {
                if (item.ID_User.ToString() == uid && (int)item.Warning_Level > 0)
                {
                    level = (int)item.Warning_Level - 1;
                    item.Warning_Level = level;
                    bool check = false;
                    DB.DBLists.UpdateUserWarnings(userWarnings);
                    foreach (var row in warnings)
                    {
                        if (check == false && row.User_ID.ToString() == uid && (bool)row.Active == true)
                        {
                            row.Active = false;
                            check = true;
                            DB.DBLists.UpdateWarnings(warnings);
                        }
                    }
                }
                else if (item.ID_User.ToString() == uid && (int)item.Warning_Level == 0)
                {
                    await ctx.RespondAsync($"{ctx.User.Mention}, this user warning level is already at 0");
                    UserCheck = true;
                }
            }
            modmsg = $"{username.Mention} has been unwarned by <@{aid}>. Warning level now {level}";
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
            if (!UserCheck)
            {
                try
                {
                    await username.SendMessageAsync($"Your warning level has been lowerd to {level} by {ctx.User.Mention}");
                }
                catch
                {
                    await modlog.SendMessageAsync($":exclamation::exclamation:{username.Mention} could not be contacted via DM. Reason not sent");
                }
            }
            await modlog.SendMessageAsync(embed: embed);
        }

        [Command("getkicks")]
        [Description("Shows user warning, kick and ban history")]
        public async Task GetKicks(CommandContext ctx, DiscordUser username)
        {
            await ctx.Message.DeleteAsync();
            List<DB.UserWarnings> userWarnings = DB.DBLists.UserWarnings;
            List<DB.Warnings> warnings = DB.DBLists.Warnings;
            string uid = username.Id.ToString();
            bool UserCheck = false;
            int kcount = 0, bcount = 0, wlevel = 0, wcount = 0;
            string reason = "";
            var selectedUser = userWarnings.Where(w => w.ID_User == uid).ToList();
            if (selectedUser.Count == 1)
            {
                UserCheck = true;
                kcount = (int)selectedUser[0].Kick_Count;
                bcount = (int)selectedUser[0].Ban_Count;
                wcount = (int)selectedUser[0].Warning_Count;
                wlevel = (int)selectedUser[0].Warning_Level;
                var WarningsList = warnings.Where(w => w.User_ID == uid).ToList();
                foreach (var item in WarningsList)
                {
                    if ((bool)item.Active == true)
                    {
                        reason += $"By: <@{item.Admin_ID.ToString()}>\t Reason: {item.Reason.ToString()}\n";
                    }
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
                await ctx.RespondAsync($"{ctx.User.Mention}, This user has no warning, kick and/or ban history.");
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
            og = og.Replace("\n", string.Empty);
            string[] str2 = og.Split("A: ");
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
    }
}