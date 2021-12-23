using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveBot.Automation
{
    internal static class GuildEvents
    {
        public static async Task Event_Created(DiscordClient Client, ScheduledGuildEventCreateEventArgs e)
        {
            //WIP
            var serverSettings = DB.DBLists.ServerSettings.FirstOrDefault(w => w.ID_Server == e.Guild.Id);
            if (serverSettings != null && serverSettings.Event_Log != 0)
            {
                var LogChannel = e.Guild.GetChannel(serverSettings.Event_Log);

                DiscordEmbedBuilder embed = new()
                {
                    Title = e.Event.Name,
                    Description = e.Event.Description,
                    Footer = new DiscordEmbedBuilder.EmbedFooter { Text = $"{e.Event.CreationTimestamp}" }
                };
                embed.AddField("Channel Type", e.Event.Type.ToString(), true);
                await LogChannel.SendMessageAsync(embed);
            }
        }
    }
}
