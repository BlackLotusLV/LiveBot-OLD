using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LiveBot.Automation
{
    class Weather
    {
        static DiscordChannel WeatherChannel;
        static int Interval = Timeout.Infinite;
        static string OldWeather = string.Empty;
        static readonly Timer WeatherTimer = new Timer(e => CheckWeather(), null, 0, Interval);
        public static void StartTimer()
        {
            if (Interval == Timeout.Infinite && !Program.TestBuild)
            {
                Interval = 15000;
                WeatherTimer.Change(0, Interval);
                WeatherChannel = Program.TCGuild.GetChannel(700414491749253220);
                Console.WriteLine("weather timer started");
            }
        }
        private static async void CheckWeather()
        {
            StringBuilder sb = new StringBuilder();
            TimeSpan now = DateTime.UtcNow.TimeOfDay;
            TimeSpan CurrentTime = new TimeSpan(now.Hours, now.Minutes, 0);
            string weathercondition = string.Empty;

            var Weather = DB.DBLists.WeatherSchedule.Where(w => w.Time >= CurrentTime && w.Day.Equals((int)DateTime.Today.DayOfWeek)).OrderBy(o=>o.Time).ToList();
            if (Weather.Count()<60)
            {
                int day = (int)DateTime.Today.DayOfWeek + 1;
                if (day is 7)
                {
                    day = 0;
                }
                try
                {
                    Weather.AddRange(DB.DBLists.WeatherSchedule.Where(w => w.Day.Equals(day)).OrderBy(o => o.Time).ToList().GetRange(0, 61 - Weather.Count()));
                }
                catch (Exception)
                {
                    Weather.AddRange(DB.DBLists.WeatherSchedule.Where(w => w.Day.Equals(day)).OrderBy(o => o.Time).ToList());
                }
            }
            sb.AppendLine($"**--------------------------------------------------------**");
            sb.AppendLine($"Current UTC time is {CurrentTime:hh\\:mm}. Here is the weather for the upcoming hour!");
            for (int i = 0; i < 60; i++)
            {
                var WeatherSpeciffic = Weather.Where(w => w.Time.Hours.Equals(CurrentTime.Hours) && w.Time.Minutes.Equals(CurrentTime.Minutes)).FirstOrDefault();
                if (WeatherSpeciffic is null)
                {
                    sb.AppendLine($"{CurrentTime:hh\\:mm} - Weather is unknown");
                }
                else
                {
                    switch (WeatherSpeciffic.Weather)
                    {
                        case "clear":
                            weathercondition = ":sunny: **Clear**";
                            break;
                        case "*":
                            weathercondition = ":fog: **Fog**";
                            break;
                        case "rain":
                            weathercondition = ":cloud_rain: **Rain**";
                            break;
                        case "rain*":
                            weathercondition = ":fog::cloud_rain: **Fog and Rain**";
                            break;
                        case "snow":
                            weathercondition = ":snowflake: **Snow**";
                            break;
                        case "snow*":
                            weathercondition = ":fog::snowflake: **Fog and Snow**";
                            break;
                    }
                    sb.AppendLine($"{WeatherSpeciffic.Time:hh\\:mm} - {weathercondition}");
                }
                CurrentTime += TimeSpan.FromMinutes(1);
            }
            sb.AppendLine($"**--------------------------------------------------------**");
            string weathertext = sb.ToString();
            if (OldWeather != weathertext)
            {
                var messages = await WeatherChannel.GetMessagesAsync(50);
                if (messages.Count() == 0 || !messages[0].Author.Equals(Program.Client.CurrentUser))
                {
                    await WeatherChannel.DeleteMessagesAsync(messages);
                    await WeatherChannel.SendMessageAsync(weathertext);
                }
                else if (messages[0].Author.Equals(Program.Client.CurrentUser))
                {
                    await messages[0].ModifyAsync(weathertext);
                }
                OldWeather = weathertext;
            }
        }
    }
}
