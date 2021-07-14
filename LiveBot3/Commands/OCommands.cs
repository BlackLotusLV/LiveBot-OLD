using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

        [Command("buttonmessage")]
        public async Task ButtonMessage(CommandContext ctx,
            DiscordChannel channel,
            [Description("First message content, split by |, then button components split by, and then each button by |\ncustom id, lable, emoji()")][RemainingText]string rawData)
        {
            await ctx.TriggerTypingAsync();
            string[] splitData = rawData.Split('|');
            List<DiscordComponent> buttons = new();

            for (int i = 1; i < splitData.Length; i++)
            {
                string[] ButtonComponents = splitData[i].Split(',');
                buttons.Add(new DiscordButtonComponent(ButtonStyle.Primary, ButtonComponents[0], ButtonComponents[1], false, UInt64.TryParse(ButtonComponents[2], out ulong emojiID) ? new DiscordComponentEmoji(emojiID) : null));
            }
            await new DiscordMessageBuilder()
                .WithContent(splitData[0])
                .AddComponents(buttons)
                .SendAsync(channel);
        }


        [Command("update")]
        public async Task Update(CommandContext ctx, [Description("Which database to update. (All will update all db)")] string db = null)
        {
            string msgcontent;
            switch (db.ToLower())
            {
                case "all":
                    DB.DBLists.LoadAllLists();
                    msgcontent = "All lists updated";
                    break;

                case "vehicle":
                case "vehicles":
                    DB.DBLists.LoadVehicleList();
                    msgcontent = "Vehicle list updated";
                    break;

                case "server":
                    DB.DBLists.LoadServerSettings();
                    msgcontent = "Server settings list updated";
                    break;

                case "bannedw":
                case "banword":
                case "bword":
                    DB.DBLists.LoadBannedWords();
                    msgcontent = "Banned Words list updated";
                    break;

                default:
                    msgcontent = "Couldn't find this table. Nothing was updated\n" +
                        "all - updates all tables\n" +
                        "vehicle - updates the **vehicle list**\n" +
                        "server - updates Server Settings\n" +
                        "bannedw - Update banned words list";
                    break;
            }
            DiscordMessage msg = await ctx.RespondAsync(msgcontent);
            await Task.Delay(10000);
            await msg.DeleteAsync();
        }

        [Command("updatehub")]
        public async Task UpdateHub(CommandContext ctx)
        {
            HubMethods.UpdateHubInfo(true);
            DiscordMessage msg = await ctx.RespondAsync("TCHub info has been force updated.");
            await Task.Delay(10000).ContinueWith(f => msg.DeleteAsync());
        }

        [Command("stopbot")]
        public async Task StopBot(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync();
            System.Environment.Exit(0);
        }

        [Command("getguilds")]
        public async Task GetGuilds(CommandContext ctx)
        {
            StringBuilder sb = new();
            foreach (var guild in Program.Client.Guilds)
            {
                sb.AppendLine($"{guild.Value.Name} ({guild.Value.Id})");
            }
            await ctx.RespondAsync(sb.ToString());
        }

        [Command("leaveguild")]
        public async Task LeaveGuild(CommandContext ctx, DiscordGuild guild)
        {
            await guild.LeaveAsync();
            await ctx.RespondAsync($"The bot has left {guild.Name} guild!");
        }

        [Command("uploadbg")]
        public async Task UploadBackGround(CommandContext ctx, int price, [RemainingText] string name)
        {
            if (ctx.Message.Attachments != null)
            {
                byte[] bg;
                using (WebClient client = new())
                {
                    bg = client.DownloadData(ctx.Message.Attachments[0].Url);
                }
                DB.DBLists.InsertBackgroundImage(new DB.BackgroundImage { Name = name, Price = price, Image = bg });
                await ctx.RespondAsync("Image succesfully uploaded!");
            }
            else
            {
                await ctx.RespondAsync("Something went wrong");
            }
        }

        [Command("updateimg")]
        public async Task UpdateImage(CommandContext ctx, int id, int price = 0)
        {
            DB.BackgroundImage imgEntry = DB.DBLists.BackgroundImage.FirstOrDefault(w => w.ID_BG == id);

            StringBuilder outputMSG = new();

            outputMSG.Append($"{ctx.Member.Mention}, ");
            if (imgEntry != null)
            {
                if (ctx.Message.Attachments != null)
                {
                    byte[] bg;
                    using (WebClient client = new())
                    {
                        bg = client.DownloadData(ctx.Message.Attachments[0].Url);
                    }
                    imgEntry.Image = bg;

                    outputMSG.AppendLine($"background **image** updated");
                }
                else
                {
                    outputMSG.AppendLine("background **image** not set");
                }
                if (price > 0)
                {
                    imgEntry.Price = price;
                    outputMSG.AppendLine($"background **price** updated");
                }
                else
                {
                    outputMSG.AppendLine("background **price** not set");
                }
                DB.DBLists.UpdateBackgroundImages(imgEntry);
            }
            else
            {
                outputMSG.Append($"could not find image with this ID");
            }
            await ctx.RespondAsync(outputMSG.ToString());
        }
    }
}