using DSharpPlus.Entities;
using LiveBot.Automation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using LiveBot.Json;

namespace LiveBot
{
    internal class TimerMethod
    {
        private static DateTime TCHubLastUpdated = new DateTime();
        public static byte[][] EventLogoBitArr = new byte[9][];



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

        public static void UpdateHubInfo()
        {
            List<TCHubJson.Summit> JSummit;
            DateTime endtime;
            using (WebClient wc = new WebClient())
            {
                string JSummitString = wc.DownloadString(Program.TCHubJson.Summit);
                JSummit = JsonConvert.DeserializeObject<List<TCHubJson.Summit>>(JSummitString);
                endtime = CustomMethod.EpochConverter(JSummit[0].End_Date * 1000);

                if (endtime != TCHubLastUpdated)
                {
                    TCHubLastUpdated = endtime;
                    Program.TCHubDictionary = JsonConvert.DeserializeObject<Dictionary<string,string>>(wc.DownloadString(Program.TCHubJson.Dictionary));
                    Program.JSummit = JSummit;
                    Program.TCHubMissions = JsonConvert.DeserializeObject<TCHubJson.TCHub>(wc.DownloadString(Program.TCHubJson.Missions)).Missions;
                    Program.TCHubSkills = JsonConvert.DeserializeObject<TCHubJson.TCHub>(wc.DownloadString(Program.TCHubJson.Skills)).Skills;
                    for (int i = 0; i < JSummit[0].Events.Length; i++)
                    {
                        var ThisEvent = JSummit[0].Events[i];

                            EventLogoBitArr[i] = wc.DownloadData($"https://www.thecrew-hub.com/gen/assets/summits/{ThisEvent.Img_Path}");
                    }

                    Console.WriteLine($"[TCHub] Info downloaded for {JSummit[0].Summit_ID} summit.");
                }
            }
        }
    }
}