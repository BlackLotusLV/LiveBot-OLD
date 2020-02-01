using DSharpPlus.Entities;
using LiveBot.Automation;
using System;
using System.Collections.Generic;

namespace LiveBot
{
    internal class TimerMethod
    {
        public static void StreamListCheck(List<LiveStreamer> list, int StreamCheckDelay)
        {
            try
            {
                foreach (var item in list)
                {
                    if (item.Time.AddHours(StreamCheckDelay) < DateTime.Now && item.User.Presence.Activity.ActivityType != ActivityType.Streaming)
                    {
                        Console.WriteLine($"{item.User.Username} removed for time out");
                        list.Remove(item);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("[System] LiveStream list is empty!");
            }
        }

        public async static void ActivatedRolesCheck(List<ActivateRolesTimer> list)
        {
            try
            {
                foreach (var item in list)
                {
                    if (item.Time.AddMinutes(5) < DateTime.Now)
                    {
                        await item.Role.ModifyAsync(mentionable: false);
                        list.Remove(item);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("[System] ActivateRolesTimer list is empty!");
            }
        }
    }
}