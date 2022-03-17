using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace LiveBot.SlashCommands
{
    [SlashCommandGroup("Admin", "Admin commands")]
    [SlashRequirePermissions(Permissions.KickMembers)]
    internal class SlashAdminCommands : ApplicationCommandModule
    {
        [SlashCommand("warn", "Warn a user.")]
        [SlashRequireGuild]
        public async Task Warning(InteractionContext ctx, [Option("user", "User to warn")] DiscordUser username, [Option("reason", "Why the user is being warned")] string reason)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            await Services.WarningService.WarnUserAsync(username, ctx.Member, ctx.Guild, ctx.Channel, reason, false, ctx);
        }
        /*
        [SlashCommand("news", "Posts news article to the news channel")]
        [SlashRequireGuild]
        public async Task News(InteractionContext ctx)
        {
            DiscordSelectComponentOption option1 = new("test 1", "1");
            DiscordSelectComponentOption option2 = new("test 2", "2");
            List<DiscordSelectComponentOption> options = new ();
            options.Add(option1);
            options.Add(option2);
            DiscordInteractionResponseBuilder modal = new DiscordInteractionResponseBuilder().WithTitle("Nodal").WithCustomId($"modal-{ctx.User.Id}")
                .AddComponents(
                    new TextInputComponent("test", "test", "this is a placeholder"),
                    new DiscordSelectComponent("test-select", "test?", options)
                    );

            await ctx.CreateResponseAsync(InteractionResponseType.Modal ,modal);

            var interactivity = ctx.Client.GetInteractivity();
            var response = await interactivity.WaitForModalAsync($"modal-{ctx.User.Id}", ctx.User);

            if (!response.TimedOut)
            {
                var modalInteraction = response.Result.Interaction;
                await modalInteraction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"thing created"));
            }
        }
        */
    }
}