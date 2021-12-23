using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using LiveBot.DB;

namespace LiveBot.Commands
{
    [Group("modmail")]
    [Aliases("mm")]
    [Description("Modmail commands.")]
    internal class ModMailCommands : BaseCommandModule
    {
        [GroupCommand]
        [RequireDirectMessage]
        [Description("Opens a Mod Mail chat with a specific server in the bot's DMs.")]
        public async Task ModMail(CommandContext ctx,
                                  [Description("The name of the server that you want to open the Mod Mail with. *Spaces are replaced with `-`*")] string serverName = null)
        {
            if (ctx.Guild is null)
            {
                if (serverName == null)
                {
                    serverName = "no server!";
                }
                var ModMailServers = DBLists.ServerSettings.Where(w => w.ModMailID != 0);
                Dictionary<DiscordGuild, string> GuildNameDict = new();
                StringBuilder GuildNameString = new();
                GuildNameString.AppendLine("The mod-mail is only available on certain servers, here are the server names you can use:");
                foreach (var item in ModMailServers)
                {
                    DiscordGuild Guild = ctx.Client.Guilds.FirstOrDefault(w => w.Key == item.ID_Server).Value;
                    if (Guild != null)
                    {
                        GuildNameDict.Add(Guild, Guild.Name.Replace(' ', '-').ToLower());
                        GuildNameString.AppendLine($"`{Guild.Name.Replace(' ', '-').ToLower()}`");
                    }
                }
                GuildNameString.AppendLine($"To start a modmail write `{Program.CFGJson.CommandPrefix}modmail server-name-here`");
                if (!GuildNameDict.ContainsValue(serverName.Replace(' ', '-').ToLower()))
                {
                    await ctx.RespondAsync(GuildNameString.ToString());
                }
                else
                {
                    DiscordGuild Guild = GuildNameDict.FirstOrDefault(w => w.Value.ToLower() == serverName.ToLower()).Key;
                    bool serverCheck = true;
                    try
                    {
                        await Guild.GetMemberAsync(ctx.User.Id);
                    }
                    catch
                    {
                        await ctx.RespondAsync("You are not in this server. If you think this is an error, please make sure you are set as online and are in fact in the server.");
                        serverCheck = false;
                    }
                    if (serverCheck && DB.DBLists.ModMail.FirstOrDefault(w => w.User_ID == ctx.User.Id && w.IsActive) == null)
                    {
                        Random r = new();
                        string colorID = string.Format("#{0:X6}", r.Next(0x1000000));
                        DB.ModMail newEntry = new()
                        {
                            Server_ID = Guild.Id,
                            User_ID = ctx.User.Id,
                            LastMSGTime = DateTime.UtcNow,
                            ColorHex = colorID,
                            IsActive = true,
                            HasChatted = false
                        };

                        long EntryID = DBLists.InsertModMailGetID(newEntry);
                        await ctx.RespondAsync($"**----------------------------------------------------**\n" +
                            $"Modmail entry **open** with `{serverName.ToLower()}`. Continue to write as you would normally ;)\n*Mod Mail will time out in {Automation.ModMail.TimeoutMinutes} minutes after last message is sent.*");
                        DiscordChannel MMChannel = Guild.GetChannel(ModMailServers.FirstOrDefault(w => w.ID_Server == Guild.Id).ModMailID);
                        DiscordEmbedBuilder ModeratorEmbed = new()
                        {
                            Author = new DiscordEmbedBuilder.EmbedAuthor
                            {
                                Name = $"{ctx.User.Username} ({ctx.User.Id})",
                                IconUrl = ctx.User.AvatarUrl
                            },
                            Title = $"[NEW] #{EntryID} Mod Mail created by {ctx.User.Username}.",
                            Color = new DiscordColor(colorID),
                            Description = ctx.Message.Content
                        };

                        DiscordButtonComponent CloseButton = new(ButtonStyle.Danger, $"close{EntryID}", "Close", false, new DiscordComponentEmoji("✖️"));
                        await new DiscordMessageBuilder()
                            .AddComponents(CloseButton)
                            .WithEmbed(ModeratorEmbed)
                            .SendAsync(MMChannel);
                        Program.Client.Logger.LogInformation(CustomLogEvents.ModMail, "New Mod Mail entry created by {UserName}({UserId}) for {GuildName}", ctx.User.Username, ctx.User.Id,serverName);
                    }
                    else
                    {
                        await ctx.RespondAsync("Seems like you already have an active session ongoing, please close the previous one to start a new one.");
                    }
                }
            }
            else
            {
                await ctx.RespondAsync($"{ctx.Member.Mention}, You can only open a mod mail in Live bot Direct Messages");
            }
        }

        [Command("reply")]
        [RequireUserPermissions(Permissions.KickMembers)]
        [RequireGuild]
        [Description("Responds to the specified Mod Mail chat.")]
        public async Task Reply(CommandContext ctx,
                                [Description("Mod Mail entry ID")] long ModMailID,
                                [Description("Text that is being sent to the user via DM")][RemainingText] string reply)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            ModMail MMEntry = DBLists.ModMail.FirstOrDefault(w => w.ID == ModMailID && w.Server_ID == ctx.Guild.Id);
            if (MMEntry == null)
            {
                await ctx.RespondAsync($"{ctx.User.Mention} Could not find the mod mail entry.");
            }
            else
            {
                if (MMEntry.IsActive)
                {
                    DiscordEmbedBuilder embed = new()
                    {
                        Author = new DiscordEmbedBuilder.EmbedAuthor
                        {
                            IconUrl = ctx.User.AvatarUrl,
                            Name = ctx.User.Username
                        },
                        Title = $"[REPLY] #{MMEntry.ID} Mod Mail Response",
                        Description = $"{ctx.Member.Username} - {reply}",
                        Color = new DiscordColor(MMEntry.ColorHex)
                    };
                    try
                    {
                        DiscordMember member = await ctx.Guild.GetMemberAsync(MMEntry.User_ID);
                        await member.SendMessageAsync($"{ctx.Member.Username} - {reply}");
                    }
                    catch (Exception e)
                    {
                        embed.Description = $"User has left the server, blocked the bot or closed their DMs. Could not send a response!\nHere is what you said `{reply}`";
                        embed.Title = $"[ERROR] {embed.Title}";
                        Console.WriteLine(e.InnerException);
                    }
                    MMEntry.LastMSGTime = DateTime.UtcNow;
                    DBLists.UpdateModMail(MMEntry);

                    DiscordChannel MMChannel = ctx.Guild.GetChannel(DBLists.ServerSettings.FirstOrDefault(w => w.ID_Server == ctx.Guild.Id).ModMailID);
                    await MMChannel.SendMessageAsync(embed: embed);

                    Program.Client.Logger.LogInformation(CustomLogEvents.ModMail, "An admin has responded to Mod Mail entry #{EntryId}", MMEntry.ID);
                }
                else
                {
                    await ctx.RespondAsync($"{ctx.User.Mention}, Mod Mail has timed out, message not sent to user.\n" +
                        $"Here is what you said `{reply}`");
                }
            }
        }

        [Command("close")]
        [RequireDirectMessage]
        [Description("Closes the opened Mod Mail chat.")]
        public async Task Close(CommandContext ctx)
        {
            var modMail = DBLists.ModMail.FirstOrDefault(w => w.User_ID == ctx.User.Id && w.IsActive);
            if (modMail == null)
            {
                await ctx.RespondAsync("You don't have active Mod Mail open.");
            }
            else
            {
                await Automation.ModMail.CloseModMail(modMail, ctx.User, "Mod Mail closed by the user", "**Mod Mail closed!\n----------------------------------------------------**");
                Program.Client.Logger.LogInformation(CustomLogEvents.ModMail, "Mod mail entry #{EntryId} closed by the user", modMail.ID);
            }
        }

        [Command("end")]
        [RequireGuild]
        [RequireUserPermissions(Permissions.KickMembers)]
        [Description("Ends the specified Mod Mail chat.")]
        public async Task End(CommandContext ctx, [Description("Mod Mail entry ID")] long ModMailID)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            var modMail = DBLists.ModMail.FirstOrDefault(w => w.IsActive && w.Server_ID == ctx.Guild.Id && w.ID == ModMailID);
            if (modMail == null)
            {
                await ctx.RespondAsync($"Could not find this Mod Mail entry, please check if everything is correct, it might be already closed.");
            }
            else
            {
                await Automation.ModMail.CloseModMail(
                    modMail,
                    ctx.User,
                    $" Mod Mail closed by {ctx.User.Username}",
                    $"**Mod Mail closed by {ctx.User.Username}!\n----------------------------------------------------**");
                Program.Client.Logger.LogInformation(CustomLogEvents.ModMail, "Mod mail entry #{EntryId} closed by an admin/moderator", modMail.ID);
            }
        }

        [Command("directmessage")]
        [Aliases("dm", "pm")]
        [RequireGuild]
        [RequirePermissions(Permissions.KickMembers)]
        [Description("Bot sends a DM to the specified user with the text you want it to say. It tells where and who it is from as well as logs it in mod mail channel.")]
        public async Task DirrectMessage(CommandContext ctx, DiscordMember member, [RemainingText] string text)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            ServerSettings SSettings = DBLists.ServerSettings.FirstOrDefault(w => w.ID_Server == ctx.Guild.Id);
            if (SSettings.ModMailID != 0)
            {
                string DMMessage = $"You are receiving a Moderator DM from {ctx.Guild.Name} Discord\n{ctx.User.Username} - {text}";
                bool ErrorCheck = false;
                try
                {
                    await member.SendMessageAsync(DMMessage);
                }
                catch
                {
                    ErrorCheck = true;
                }
                if (!ErrorCheck)
                {
                    DiscordChannel MMChannel = ctx.Guild.GetChannel(SSettings.ModMailID);
                    DiscordEmbedBuilder embed = new()
                    {
                        Author = new DiscordEmbedBuilder.EmbedAuthor
                        {
                            IconUrl = member.AvatarUrl,
                            Name = member.Username
                        },
                        Title = $"[MOD DM] Moderator DM to {member.Username}",
                        Description = DMMessage
                    };
                    await MMChannel.SendMessageAsync(embed: embed);
                    Program.Client.Logger.LogInformation(CustomLogEvents.ModMail, "A Dirrect message was sent to {Username}({UserId}) from {User2Name}({User2Id}) through Mod Mail system.", member.Username, member.Id, ctx.Member.Username, ctx.Member.Id);
                }
                else
                {
                    await ctx.RespondAsync($"{ctx.User.Mention}, The user could not be contacted. Either their DMs are closed, they have blocked the bot or they have left the server.\n" +
                        $"Here is what you said `{text}`");
                    Program.Client.Logger.LogError(CustomLogEvents.ModMail, "Failed to contact user {Username}({UserId}) via mod mail.", member.Username, member.Id);
                }
            }
            else
            {
                await ctx.RespondAsync("This server has not set up mod mail channel.");
            }
        }

        [Command("active")]
        [RequireGuild]
        [RequirePermissions(Permissions.KickMembers)]
        [Description("Shows currently active mod mail sessions in this server")]
        public async Task Active(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync();
            await ctx.TriggerTypingAsync();
            var ModMailEntries = DBLists.ModMail.Where(w => w.Server_ID == ctx.Guild.Id && w.IsActive).ToList();
            if (ModMailEntries is null)
            {
                await ctx.RespondAsync($"{ctx.Member.Mention}, This server does not have any active mod mail entries.");
            }
            else
            {
                StringBuilder sb = new();
                foreach (var entry in ModMailEntries)
                {
                    sb.AppendLine($"**ID:** {entry.ID}\t **User:** {entry.User_ID}\t**Has Chatted:** {entry.HasChatted}\t**Time Remaining:** {Automation.ModMail.TimeoutMinutes - (DateTime.UtcNow - entry.LastMSGTime).Minutes} Minutes");
                }
                DiscordEmbedBuilder embed = new()
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        IconUrl = ctx.Guild.IconUrl,
                        Name = $"{ctx.Guild.Name} Open Mod Mail Entries"
                    }
                };
                embed.AddField("ModMail Mail Entries", sb.ToString().Length < 1 ? "No entreis" : sb.ToString());
                await ctx.RespondAsync(embed: embed);
            }
        }
    }
}