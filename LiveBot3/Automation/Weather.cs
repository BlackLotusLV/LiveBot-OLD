using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LiveBot.Automation
{
    class Weather
    {
        static Timer WeatherTimer;
        static DiscordChannel WeatherChannel;
        static int Interval = Timeout.Infinite;
        static string OldWeather = string.Empty;
        public static void StartTimer()
        {
            if (Interval == Timeout.Infinite && !Program.TestBuild)
            {
                Interval = 15000;
                WeatherTimer = new Timer(e => CheckWeather(), null, 0, Interval);
                WeatherChannel = Program.TCGuild.GetChannel(700414491749253220);
                Console.WriteLine("weather timer started");
            }
        }

        private static async void CheckWeather()
        {
            string weathertext = string.Empty;
            string weathercondition = string.Empty;
            bool current = true;
            var entry = DB.DBLists.WeatherSchedule.Where(w => w.Time.Equals(DateTime.UtcNow.TimeOfDay) && w.Day.Equals((int)DateTime.Today.DayOfWeek)).FirstOrDefault();
            if (entry == null)
            {
                current = false;
                entry = DB.DBLists.WeatherSchedule.Where(w => w.Day.Equals((int)DateTime.Today.DayOfWeek)).OrderBy(o => o.Time).Where(w => w.Time > DateTime.UtcNow.TimeOfDay).FirstOrDefault();
                if (entry == null)
                {
                    int day = (int)DateTime.Today.DayOfWeek + 1;
                    if (day>6)
                    {
                        day = 0;
                    }
                    entry = DB.DBLists.WeatherSchedule.Where(w => w.Day.Equals(day)).OrderBy(o => o.Time).FirstOrDefault();
                }
            }
            switch (entry.Weather)
            {
                case "clear":
                    weathercondition = "**Clear** :sunny: ";
                    break;
                case "*":
                    weathercondition = "**Fog** :fog: ";
                    break;
                case "rain":
                    weathercondition = "**Rain** :cloud_rain: ";
                    break;
                case "rain*":
                    weathercondition = "**Fog and Rain** :fog::cloud_rain:";
                    break;
                case "snow":
                    weathercondition = "**Snow** :snowflake: ";
                    break;
                case "snow*":
                    weathercondition = "**Fog and Snow** :fog::snowflake: ";
                    break;
            }
            weathertext = $"{(current?"Current":"Next known")} weather for {(DayOfWeek)entry.Day} at {entry.Time.Hours}:{entry.Time.Minutes}(UTC) is {weathercondition}";

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
