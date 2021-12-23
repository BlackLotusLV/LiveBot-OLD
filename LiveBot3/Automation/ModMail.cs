namespace LiveBot.Automation
{
    internal static class ModMail
    {
        public static readonly int TimeoutMinutes = 120;

        public static async Task ModMailDM(DiscordClient Client, MessageCreateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                var MMEntry = DB.DBLists.ModMail.FirstOrDefault(w => w.User_ID == e.Author.Id && w.IsActive);
                if (e.Guild == null && MMEntry != null && !(e.Message.Content.StartsWith($"{Program.CFGJson.CommandPrefix}modmail") || e.Message.Content.StartsWith($"{Program.CFGJson.CommandPrefix}mm")))
                {
                    DiscordGuild Guild = Client.Guilds.FirstOrDefault(w => w.Value.Id == MMEntry.Server_ID).Value;
                    DiscordChannel ModMailChannel = Guild.GetChannel(DB.DBLists.ServerSettings.FirstOrDefault(w => w.ID_Server == MMEntry.Server_ID).ModMailID);
                    DiscordEmbedBuilder embed = new()
                    {
                        Author = new DiscordEmbedBuilder.EmbedAuthor
                        {
                            IconUrl = e.Author.AvatarUrl,
                            Name = $"{e.Author.Username} ({e.Author.Id})"
                        },
                        Color = new DiscordColor(MMEntry.ColorHex),
                        Title = $"[INBOX] #{MMEntry.ID} Mod Mail user message.",
                        Description = e.Message.Content
                    };

                    if (e.Message.Attachments != null)
                    {
                        foreach (var attachment in e.Message.Attachments)
                        {
                            embed.AddField("Atachment", attachment.Url, false);
                        }
                    }

                    await ModMailChannel.SendMessageAsync(embed: embed);

                    MMEntry.HasChatted = true;
                    MMEntry.LastMSGTime = DateTime.UtcNow;
                    DB.DBLists.UpdateModMail(MMEntry);

                    Client.Logger.LogInformation(CustomLogEvents.ModMail, "New Mod Mail message sent to {ChannelName}({ChannelId}) in {GuildName} from {Username}({UserId})", ModMailChannel.Name, ModMailChannel.Id, ModMailChannel.Guild.Name, e.Author.Username,e.Author.Id);
                }
            });
            await Task.Delay(1);
        }

        public static async Task ModMailCloser()
        {
            var TimedOutEntry = DB.DBLists.ModMail.FirstOrDefault(w => w.IsActive && (DateTime.UtcNow - w.LastMSGTime) > TimeSpan.FromMinutes(TimeoutMinutes));
            if (TimedOutEntry != null)
            {
                DiscordUser User = await Program.Client.GetUserAsync(TimedOutEntry.User_ID);
                await CloseModMail(
                    TimedOutEntry,
                    User,
                    "Mod Mail entry auto closed.",
                    "**Mod Mail auto closed!\n----------------------------------------------------**");
            }
        }

        public static async Task CloseModMail(DB.ModMail ModMail, DiscordUser Closer, string ClosingText, string ClosingTextToUser)
        {
            ModMail.IsActive = false;
            string DMNotif = string.Empty;
            DiscordGuild Guild = await Program.Client.GetGuildAsync(ModMail.Server_ID);
            DiscordChannel ModMailChannel = Guild.GetChannel(DB.DBLists.ServerSettings.FirstOrDefault(w => w.ID_Server == Guild.Id).ModMailID);
            DiscordEmbedBuilder embed = new()
            {
                Title = $"[CLOSED] #{ModMail.ID} {ClosingText}",
                Color = new DiscordColor(ModMail.ColorHex),
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = $"{Closer.Username} ({Closer.Id})",
                    IconUrl = Closer.AvatarUrl
                },
            };
            try
            {
                DiscordMember member = await Guild.GetMemberAsync(ModMail.User_ID);
                await member.SendMessageAsync(ClosingTextToUser);
            }
            catch
            {
                DMNotif = "User could not be contacted anymore, either blocked the bot, left the server or turned off DMs";
            }
            DB.DBLists.UpdateModMail(ModMail);
            await ModMailChannel.SendMessageAsync(DMNotif, embed: embed);
        }

        public static async Task ModMailButton(object Client, InteractionCreateEventArgs e)
        {
            if (e.Interaction.Type == InteractionType.Component && !e.Interaction.User.IsBot && e.Interaction.Guild != null && e.Interaction.Data.CustomId.Contains("close"))
            {
                var MMEntry = DB.DBLists.ModMail.FirstOrDefault(w => w.Server_ID == e.Interaction.Guild.Id && w.IsActive && $"{w.ID}" == e.Interaction.Data.CustomId.Replace("close", ""));
                if (MMEntry != null)
                {
                    await CloseModMail(
                        MMEntry,
                        e.Interaction.User,
                        $" Mod Mail closed by {e.Interaction.User.Username}",
                        $"**Mod Mail closed by {e.Interaction.User.Username}!\n----------------------------------------------------**");
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                }
            }
        }
    }
}