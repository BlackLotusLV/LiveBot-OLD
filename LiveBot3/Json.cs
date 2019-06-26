using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace LiveBot
{
    class Json
    {
        public struct Config
        {
            [JsonProperty("token")]
            public string Token { get; private set; }

            [JsonProperty("prefix")]
            public string CommandPrefix { get; private set; }
        }

        public struct DB
        {
            [JsonProperty("host")]
            public string Host { get; private set; }

            [JsonProperty("username")]
            public string Username { get; private set; }

            [JsonProperty("password")]
            public string Password { get; private set; }

            [JsonProperty("database")]
            public string Database { get; private set; }
        }

        public struct TCE
        {
            [JsonProperty("key")]
            public string Key { get; private set; }
        }

        public struct Summit
        {
            [JsonProperty("id")]
            public ulong Summit_ID { get; private set; }

            [JsonProperty("start_date")]
            public string Start_Date { get; private set; }

            [JsonProperty("ticket_short")]
            public string LinkEnd { get; private set; }
        }

        public struct Event
        {
            [JsonProperty("total_players")]
            public string Player_Count { get; private set; }

            [JsonProperty("tier_entries")]
            public Tier_Entries[] Tier_entries { get; private set; }
        }

    }
}
