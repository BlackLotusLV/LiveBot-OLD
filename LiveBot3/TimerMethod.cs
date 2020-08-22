using DSharpPlus;
using DSharpPlus.Entities;
using LiveBot.Automation;
using LiveBot.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace LiveBot
{
    internal class TimerMethod
    {
        private static DateTime TCHubLastUpdated = new DateTime();
        public static byte[][] EventLogoBitArr = new byte[9][];
        public static byte[][] RewardsImageBitArr = new byte[4][];

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

        public static void UpdateHubInfo(bool forced = false)
        {
            List<TCHubJson.Summit> JSummit;
            DateTime endtime;
            using WebClient wc = new WebClient();
            bool Connected = true;
            string JSummitString = "";
            try
            {
                JSummitString = wc.DownloadString(Program.TCHubJson.Summit);
            }
            catch (WebException e)
            {
                Connected = false;
                Program.Client.DebugLogger.LogMessage(LogLevel.Error, "TCHub", "Connection error. Either wrong API link, or the Hub is down.", DateTime.Now, e);
            }
            if (Connected)
            {
                JSummit = JsonConvert.DeserializeObject<List<TCHubJson.Summit>>(JSummitString);
                if (forced)
                {
                    endtime = TCHubLastUpdated;
                }
                else
                {
                    endtime = CustomMethod.EpochConverter(JSummit[0].End_Date * 1000);
                }
                if (endtime != TCHubLastUpdated)
                {
                    TCHubLastUpdated = endtime;
                    Program.TCHubDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(wc.DownloadString(Program.TCHubJson.Dictionary));
                    Program.JSummit = JSummit;
                    Program.TCHub = JsonConvert.DeserializeObject<TCHubJson.TCHub>(wc.DownloadString(Program.TCHubJson.GameData));
                    for (int i = 0; i < JSummit[0].Events.Length; i++)
                    {
                        var ThisEvent = JSummit[0].Events[i];
                        EventLogoBitArr[i] = wc.DownloadData($"https://www.thecrew-hub.com/gen/assets/summits/{ThisEvent.Img_Path}");
                    }
                    for (int i = 0; i < JSummit[0].Rewards.Length; i++)
                    {
                        RewardsImageBitArr[i] = wc.DownloadData($"https://www.thecrew-hub.com/gen/assets/summits/{JSummit[0].Rewards[i].Img_Path}");
                    }
                    Program.Client.DebugLogger.LogMessage(LogLevel.Info, "TCHub", $"[TCHub] Info downloaded for {JSummit[0].Summit_ID} summit.", DateTime.Now);
                }
            }
        }
    }
}