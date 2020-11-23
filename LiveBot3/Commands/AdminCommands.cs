using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Linq;
using System.Text;
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
        public async Task Say(CommandContext ctx, DiscordChannel channel, [Description("bot will repeat this")][RemainingText] string word = "")
        {
            await ctx.Message.DeleteAsync();
            await channel.SendMessageAsync(word);
        }

        [Command("warn")]
        [Description("Warns a user")]
        [RequireGuild]
        //[Cooldown(1, 5, CooldownBucketType.Guild)]
        public async Task Warning(CommandContext ctx, DiscordUser username, [RemainingText] string reason = "Reason not specified")
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            await CustomMethod.WarnUserAsync(username, ctx.Member, ctx.Guild, ctx.Channel, reason, false);
        }

        [Command("unwarn")]
        [Description("Removes a warning from a user")]
        [RequireGuild]
        public async Task Unwarning(CommandContext ctx, DiscordUser username, int WarningID = -1)
        {
            await ctx.TriggerTypingAsync();
            await ctx.Message.DeleteAsync();
            var WarnedUserStats = DB.DBLists.ServerRanks.FirstOrDefault(f => ctx.Guild.Id == f.Server_ID && username.Id == f.User_ID);
            var ServerSettings = DB.DBLists.ServerSettings.FirstOrDefault(f => ctx.Guild.Id == f.ID_Server);
            var Warnings = DB.DBLists.Warnings.Where(f => ctx.Guild.Id == f.Server_ID && username.Id == f.User_ID).ToList();
            string MSGOut, modmsg = string.Empty;
            bool check = true;
            DiscordMember member = null;
            if (ServerSettings.WKB_Log != 0)
            {
                try
                {
                    member = await ctx.Guild.GetMemberAsync(username.Id);
                }
                catch (Exception)
                {
                    await ctx.Channel.SendMessageAsync($"{username.Username} is no longer in the server.");
                }

                DiscordChannel modlog = ctx.Guild.GetChannel(Convert.ToUInt64(ServerSettings.WKB_Log));
                if (WarnedUserStats is null)
                {
                    await ctx.RespondAsync($"This user, {username.Username}, has no warning history.");
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
                        DB.Warnings entry = Warnings.FirstOrDefault(f => f.Active is true && f.ID_Warning == WarningID);
                        if (entry is null)
                        {
                            entry = Warnings.Where(f => f.Active is true).OrderBy(f => f.ID_Warning).FirstOrDefault();
                        }
                        entry.Active = false;
                        DB.DBLists.UpdateWarnings(entry);
                        DB.DBLists.UpdateServerRanks(WarnedUserStats);
                    }
                    if (check)
                    {
                        DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor(0xf90707),
                            Author = new DiscordEmbedBuilder.EmbedAuthor
                            {
                                IconUrl = username.AvatarUrl,
                                Name = $"{username.Username} ({username.Id})"
                            },
                            Description = $"{username.Mention} has been unwarned by {ctx.User.Mention}. Warning level now {WarnedUserStats.Warning_Level}"
                        };
                        try
                        {
                            await member.SendMessageAsync($"Your warning level in **{ctx.Guild.Name}** has been lowered to {WarnedUserStats.Warning_Level} by {ctx.User.Mention}");
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
        [RequireGuild]
        public async Task GetKicks(CommandContext ctx, DiscordUser User)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($"{ctx.User.Mention}", embed: CustomMethod.GetUserWarnings(ctx.Guild, User));
        }

        [Command("vote")]
        [Description("starts a vote")]
        [RequireGuild]
        public async Task Vote(CommandContext ctx, [Description("What to vote about?")][RemainingText] string topic)
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
        [RequireGuild]
        public async Task Poll(CommandContext ctx, [Description("Options")][RemainingText] string input)
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
        [RequireGuild]
        public async Task RInfo(CommandContext ctx, DiscordRole role)
        {
            await ctx.RespondAsync($"**Role ID:**{role.Id}");
        }

        [Command("faq")]
        [RequirePermissions(Permissions.BanMembers)]
        [Description("Edits the existing FAQ message")]
        [Priority(10)]
        [RequireGuild]
        public async Task FAQ(CommandContext ctx,
            [Description("FAQ message ID that you want to update")] DiscordMessage faqMsg,
            [Description("What do you want to change? `q` for question, `a` for answer.")] string type,
            [Description("Updated text")][RemainingText] string str1)
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
        [RequirePermissions(Permissions.BanMembers)]
        [Description("Creates an FAQ post. Delimiter is `|`")]
        [Priority(9)]
        [RequireGuild]
        public async Task FAQ(CommandContext ctx, [RemainingText] string str1)
        {
            string[] str2 = str1.Split('|');
            await ctx.RespondAsync($"**Q: {str2[0]}**\n*A: {str2[1].TrimEnd()}*");
            await ctx.Message.DeleteAsync();
        }

        [Command("warncount")]
        [Description("Shows the count of warnings issues by each admin")]
        [RequireGuild]
        public async Task WarnCount(CommandContext ctx)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.AppendLine("```csharp\n\t\tUsername");
            foreach (var item in DB.DBLists.Warnings.Where(w => w.Server_ID == ctx.Guild.Id).GroupBy(w => w.Admin_ID)
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
        [RequireGuild]
        public async Task Prune(CommandContext ctx, int MessageCount = 1)
        {
            if (MessageCount > 100)
            {
                MessageCount = 100;
            }
            await ctx.Message.DeleteAsync().ContinueWith(t => ctx.TriggerTypingAsync());
            await ctx.Channel.DeleteMessagesAsync(await ctx.Channel.GetMessagesBeforeAsync(ctx.Message.Id, MessageCount));
            DiscordMessage info = await ctx.RespondAsync($"{MessageCount} messages deleted");
            await Task.Delay(5000).ContinueWith(t => info.DeleteAsync());
        }

        [Command("banword")]
        [Description("Adds a word to banned word list")]
        [RequireGuild]
        public async Task BanWord(CommandContext ctx,
            [Description("The word that is banned")] string BannedWord,
            [Description("What the user will be warned with when using such word")][RemainingText] string warning)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            string info = "";
            DB.DBLists.LoadBannedWords();
            var duplicate = (from bw in DB.DBLists.AMBannedWords
                             where bw.Server_ID == ctx.Guild.Id
                             where bw.Word == BannedWord.ToLower()
                             select bw).FirstOrDefault();
            if (duplicate is null)
            {
                DB.AMBannedWords newEntry = new DB.AMBannedWords()
                {
                    Word = BannedWord.ToLower(),
                    Offense = warning,
                    Server_ID = ctx.Guild.Id
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
        [RequireGuild]
        public async Task UnbanWord(CommandContext ctx,
            [Description("The word you want to be removed from the list.")] string word)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            string info = "";
            DB.DBLists.LoadBannedWords();
            var DBEntry = (from bw in DB.DBLists.AMBannedWords
                           where bw.Server_ID == ctx.Guild.Id
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

        [Command("cmdupdate")]
        [Description("Changes the text output of a command")]
        public async Task CMDUpdated(CommandContext ctx,
            [Description("The name of the command you want to change the output of")] string command,
            [Description("Language tag (e.g. english is gb)")] string language,
            [Description("Text you want the bot to output for this command")][RemainingText] string BotResponse)
        {
            await ctx.TriggerTypingAsync();
            await ctx.Message.DeleteAsync();
            var BotOutputEntry = DB.DBLists.BotOutputList.FirstOrDefault(w => w.Command.Equals(command) && w.Language.Equals(language));
            if (BotOutputEntry is null)
            {
                await ctx.RespondAsync($"{ctx.Member.Mention}, This combination of command and language tag does not exist in the databse.");
            }
            else
            {
                string completion_response = $"The response for `/{command}` was changed\n**From:** `{BotOutputEntry.Command_Text}`\n**To:** `{BotResponse}`";
                BotOutputEntry.Command_Text = BotResponse;
                DB.DBLists.UpdateBotOutputList(BotOutputEntry);
                DB.DBLists.LoadBotOutputList();
                DiscordMessage OutMSG = await ctx.RespondAsync(completion_response);
                await Task.Delay(10000).ContinueWith(t => OutMSG.DeleteAsync());
            }
        }

        [Command("addweather")]
        [Description("Adds a new entry to the weather list")]
        public async Task AddWeather(CommandContext ctx, string timestring, int day, string weather)
        {
            string[] weatheroptions = new string[] { "clear", "*", "rain", "rain*", "snow", "snow*" };
            DiscordMessage msg;
            TimeSpan time = TimeSpan.Zero;
            bool timecheck = true;
            try
            {
                time = TimeSpan.Parse(timestring);
            }
            catch (Exception)
            {
                timecheck = false;
            }
            if (day < 7 && day > 0 && weatheroptions.Contains(weather.ToLower()) && timecheck)
            {
                var existingEntry = DB.DBLists.WeatherSchedule.FirstOrDefault(w => w.Day.Equals(day) && w.Time.Equals(time));
                if (existingEntry is null)
                {
                    DB.WeatherSchedule newEntry = new DB.WeatherSchedule
                    {
                        Time = time,
                        Day = day,
                        Weather = weather.ToLower()
                    };
                    DB.DBLists.InsertWeatherSchedule(newEntry);
                    DB.DBLists.LoadWeatherSchedule();
                    msg = await ctx.RespondAsync($"Entry added to the list.");
                }
                else
                {
                    msg = await ctx.RespondAsync($"This time and day combination already exists. Do you want to replace this entry?");
                    DiscordEmoji Yes = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
                    DiscordEmoji No = DiscordEmoji.FromName(ctx.Client, ":x:");

                    await msg.CreateReactionAsync(Yes);
                    await Task.Delay(300).ContinueWith(t => msg.CreateReactionAsync(No));
                    var Result = await msg.WaitForReactionAsync(ctx.User, TimeSpan.FromSeconds(30));

                    if (Result.TimedOut)
                    {
                        await msg.ModifyAsync($"You didn't select anything, not changing the entry.");
                        return;
                    }
                    else if (Result.Result.Emoji == Yes)
                    {
                        existingEntry.Weather = weather.ToLower();
                        DB.DBLists.UpdateWeatherSchedule(existingEntry);
                        await msg.ModifyAsync($"Weather entry updated");
                    }
                    else if (Result.Result.Emoji == No)
                    {
                        await msg.ModifyAsync($"Weather entry not changed");
                    }
                    await msg.DeleteAllReactionsAsync();
                }
            }
            else
            {
                msg = await ctx.RespondAsync($"{ctx.Member.Mention}, input incorrect, entry not added to the list.");
            }
            await Task.Delay(10000).ContinueWith(t => msg.DeleteAsync());
        }

        [Command("ban")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Ban(CommandContext ctx, DiscordUser User, [RemainingText] string reason)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            await ctx.Guild.BanMemberAsync(User.Id, 0, reason);
            DiscordMessage msg = await ctx.RespondAsync("User has been banned.");
            await Task.Delay(10000).ContinueWith(t => msg.DeleteAsync());
        }

        [Command("lookup")]
        [Description("Looks up a user by ID")]
        public async Task Lookup(CommandContext ctx, ulong ID)
        {
            await ctx.TriggerTypingAsync();

            DiscordUser user;
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
            };
            try
            {
                user = await ctx.Client.GetUserAsync(ID);
                embed.Title = "User found";
                embed.Description = $"User {user.Username} found by ID({user.Id})";
                embed.ImageUrl = user.AvatarUrl;
                await ctx.RespondAsync(embed: embed);
            }
            catch
            {
                await ctx.RespondAsync("Could not find a user by this ID");
            }
        }
    }
}