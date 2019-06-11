using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiveBot.Commands
{
    [Group("@")]
    [Description("Administrative commands")]
    [Hidden]
    [RequireRoles(RoleCheckMode.Any, "BotCMD1")]
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

        [Command("Warn")]
        public async Task Warning(CommandContext ctx, DiscordMember username, params string[] reason)
        {
            await ctx.Message.DeleteAsync();
            List<DB.UserWarnings> userWarnings = DB.DBLists.UserWarnings;
            //List<DB.Warnings> warnings = DB.DBLists.Warnings;
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
        public async Task GetKicks(CommandContext ctx, DiscordMember username)
        {
            await ctx.Message.DeleteAsync();
            List<DB.UserWarnings> userWarnings = DB.DBLists.UserWarnings;
            List<DB.Warnings> warnings = DB.DBLists.Warnings;
            string uid = username.Id.ToString();
            bool UserCheck = false;
            int kcount = 0, bcount = 0, wlevel = 0, wcount = 0;
            string reason = "";
            foreach (var row in userWarnings)
            {
                if (row.ID_User.ToString() == uid)
                {
                    UserCheck = true;
                    kcount = (int)row.Kick_Count;
                    bcount = (int)row.Ban_Count;
                    wcount = (int)row.Warning_Count;
                    wlevel = (int)row.Warning_Level;
                    foreach (var item in warnings)
                    {
                        if (item.User_ID.ToString() == row.ID_User.ToString())
                        {
                            if ((bool)item.Active == true)
                            {
                                reason += $"By: <@{item.Admin_ID.ToString()}>\t Reason: {item.Reason.ToString()}\n";
                            }
                        }
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
            //var interactivity = ctx.Client.GetInteractivity();
            //var interactivity = ctx.Client.GetInteractivityModule();
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
        public async Task FAQ(CommandContext ctx, DiscordMessage faqMsg, string type, params string[] input)
        {
            string str1 = CustomMethod.ParamsStringConverter(input);
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
        public async Task FAQ(CommandContext ctx, params string[] input)
        {
            string str1 = CustomMethod.ParamsStringConverter(input);
            string[] str2 = str1.Split('|');
            await ctx.RespondAsync($"**Q: {str2[0]}**\n*A: {str2[1].TrimEnd()}*");
            await ctx.Message.DeleteAsync();
        }

        /*
        [Command("event")]
        [RequireRoles(RoleCheckMode.Any, "BotCMD1")]
        public async Task Event(CommandContext ctx, string url)
        {
            await ctx.TriggerTypingAsync();
            string newline = "\n";
            List<string> titlelist = DataBase.WebToHTML(url, @"((Weekly|Special) Event: )(\w{1,}[ ]?){1,} [|]");
            string titleone = DataBase.ParamsStringConverter(titlelist.ToArray());
            string title = Regex.Replace(titleone, @"[|]", ""); // removes | from the title
            List<string> stringlist = DataBase.WebToHTML(url, @"(<p>|<strong>|<em>){1,}(\w{1,}([ .#,-’!()]|.){0,}){1,}(<strong>|</strong>){0,}|(<br)");
            string rawstring = DataBase.ParamsStringConverter(stringlist.ToArray());
            string[] main = new string[8];
            main[0] = Regex.Replace(rawstring, @"<p>For more information.{0,}", "");//removes not needed part from photo events
            main[1] = Regex.Replace(main[0], @"</?strong>|</?b>", "**"); //html bold to markdown bold
            main[2] = Regex.Replace(main[1], @"</?em>", "*"); // italics to italics
            main[3] = Regex.Replace(main[2], @"<p>", newline); //paragraph to new line
            main[4] = Regex.Replace(main[3], @"(<a href=(.https?://(\w{0,}[-./]){0,}\w{0,}.>))|</a>", "**"); //bolds the link words and removes the link
            main[5] = Regex.Replace(main[4], @"[*][*]#TC2Weekly[*][*]", "`#TC2Weekly`");
            main[6] = Regex.Replace(main[5], @"<u>|</u>", "__");
            main[7] = Regex.Replace(main[6], @"</?sup>|</p>| <br ", "");//cleans up not used formating
            DiscordGuild guild = await Program.Client.GetGuildAsync(150283740172517376);
            DiscordRole EventsRole = guild.GetRole(486531401089417226);
            string response = $"**--------------------------------------------------------------**" +
                $"\n**{title}** {EventsRole.Mention}\n" +
                $"{main[7]}\n" +
                $"**--------------------------------------------------------------**";

            await ctx.RespondAsync(response);
            await ctx.Message.DeleteAsync();
        }

        [Command("event2")]
        [RequireRoles(RoleCheckMode.Any, "BotCMD1")]
        public async Task Event2(CommandContext ctx, string url)
        {
            int i = 0;
            Console.WriteLine(i++);
            string newline = "\n";
            List<string> titlelist = DataBase.WebToHTML(url, @"((Weekly|Special) Event: )(\w{1,}[ ]?){1,} [|]");
            string titleone = DataBase.ParamsStringConverter(titlelist.ToArray());
            string title = Regex.Replace(titleone, @"[|]", ""); // removes | from the title
            List<string> stringlist = DataBase.WebToHTML(url, @"(<td([ >="":;-]|\w){0,}|<strong>|<p>|<em>){1,}(\w{1,}([ -.#,'!()]|.){0,}){1,}(<strong>|</stron>){0,}(</td>)?");
            Console.WriteLine(i++);
            string rawstring = DataBase.ParamsStringConverter(stringlist.ToArray());
            string[] main = new string[10];
            main[0] = Regex.Replace(rawstring, @"<p>For more information.{0,}", "");//removes not needed part from photo events
            main[1] = Regex.Replace(main[0], @"</?strong>|</?b>", "**"); //html bold to markdown bold
            main[2] = Regex.Replace(main[1], @"</?em>", "*"); // italics to italics
            main[3] = Regex.Replace(main[2], @"<p>", newline); //paragraph to new line
            main[4] = Regex.Replace(main[3], @"(<a href=(.https?://(\w{0,}[-./]){0,}\w{0,}.>))|</a>", "**"); //bolds the link words and removes the link
            main[5] = Regex.Replace(main[4], @"[*][*]#TC2Weekly[*][*]", "`#TC2Weekly`");
            main[6] = Regex.Replace(main[5], @"<u>|</u>", "__");
            main[7] = Regex.Replace(main[6], @".{2}Buy Now.{2}|<t(\w|[ :="";-]){1,}>|</?sup>|</p>| <br ", "");//cleans up not used formating
            main[8] = Regex.Replace(main[7], @"</td>", "\t");//
            main[9] = Regex.Replace(main[8], @"", "");
            DiscordGuild guild = await Program.Client.GetGuildAsync(150283740172517376);
            DiscordRole EventsRole = guild.GetRole(486531401089417226);
            string response = $"**--------------------------------------------------------------**" +
                $"\n**{title}** {EventsRole.Mention}\n" +
                $"{main[9].Trim()}\n" +
                $"**--------------------------------------------------------------**";

            Console.WriteLine(i++);
            Console.WriteLine(response);
            await ctx.RespondAsync(response);
            await ctx.Message.DeleteAsync();
        }
        //*/
    }
}