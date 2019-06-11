using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiveBot.Commands
{
    [Group("!")]
    [Description("Owner commands")]
    [Hidden]
    [RequireOwner]
    internal class OCommands : BaseCommandModule
    {
        [Command("react")]
        public async Task React(CommandContext ctx, DiscordMessage message, params DiscordEmoji[] emotes)
        {
            foreach (DiscordEmoji emote in emotes)
            {
                await message.CreateReactionAsync(emote);
                await Task.Delay(300);
            }
            await ctx.Message.DeleteAsync();
        }

        [Command("textupdate")]
        [Aliases("txtup")]
        public async Task TextUpdate(CommandContext ctx, string language, string command, params string[] text)
        {
            string location = "TextFiles/" + language.ToLower() + "/" + command.ToLower() + ".txt";
            string f = CustomMethod.ParamsStringConverter(text);
            if (File.Exists(location))
            {
                File.WriteAllText(location, f);
                await ctx.RespondAsync("Command updated");
            }
            else
            {
                await ctx.RespondAsync("Specified file locations incorrect.");
            }
        }
    }
}