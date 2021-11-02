using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace LiveBot.SlashCommands
{
    [SlashCommandGroup("Admin","Admin commands")]
    [SlashRequirePermissions(Permissions.KickMembers)]
    class SlashAdminCommands : ApplicationCommandModule
    {
        [SlashCommand("warn","Warn a user.")]
        [SlashRequireGuild]
        public async Task Warning(InteractionContext ctx,[Option("user", "User to warn")] DiscordUser username, [Option("reason","Why the user is being warned")] string reason)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            await CustomMethod.WarnUserAsync(username, ctx.Member, ctx.Guild, ctx.Channel, reason, false, ctx);
        }
    }
}
