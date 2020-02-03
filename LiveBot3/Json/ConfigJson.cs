using Newtonsoft.Json;
using System.Collections.Generic;

namespace LiveBot
{
    internal class ConfigJson
    {
        public struct Config
        {
            [JsonProperty("LiveBot")]
            public Bot LiveBot { get; private set; }

            [JsonProperty("DevBot")]
            public Bot DevBot { get; private set; }

            [JsonProperty("DataBase")]
            public DB DataBase { get; private set; }

            [JsonProperty("TCE")]
            public TCE TCE { get; private set; }
            [JsonProperty("TCHub")]
            public TCHubAPI TCHub { get; private set; }
        }

        public struct Bot
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

            [JsonProperty("port")]
            public string Port { get; private set; }
        }

        public struct TCE
        {
            [JsonProperty("key")]
            public string Key { get; private set; }

            [JsonProperty("link")]
            public string Link { get; private set; }
        }

        public struct TCHubAPI
        {
            [JsonProperty("summit")]
            public string Summit { get; set; }
            [JsonProperty("missions")]
            public string Missions { get; set; }
            [JsonProperty("dictionary")]
            public string Dictionary { get; set; }
            [JsonProperty("skills")]
            public string Skills { get; set; }

        }

        
    }
}