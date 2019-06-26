using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

namespace LiveBot.Commands
{
    [Group("m")]
    [Description("Music bot commands")]
    public class MusicCommands : BaseCommandModule
    {
        //placeholder - source: https://github.com/DSharpPlus/Example-Bots
        [Command("join"), Description("Joins a voice channel.")]
        [Aliases("connect","conn","con")]
        public async Task Join(CommandContext ctx, DiscordChannel chn = null)
        {
            var vnext = ctx.Client.GetVoiceNext();
            if (vnext == null)
            {
                await ctx.RespondAsync("VNext is not enabled or configured.");
                return;
            }
            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc != null)
            {
                await ctx.RespondAsync("Already connected in this guild.");
                return;
            }
            var vstat = ctx.Member?.VoiceState;
            if (vstat?.Channel == null && chn == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.");
                return;
            }
            if (chn == null)
                chn = vstat.Channel;
            vnc = await vnext.ConnectAsync(chn);
            await ctx.RespondAsync($"Connected to `{chn.Name}`");
        }
    }
}
