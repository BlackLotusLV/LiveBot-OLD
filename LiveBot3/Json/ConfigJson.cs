using Newtonsoft.Json;

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
            public TheCrewExchange TCE { get; private set; }

            [JsonProperty("TCHub")]
            public TheCrewHubApi TCHub { get; private set; }
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

        public struct TheCrewExchange
        {
            [JsonProperty("key")]
            public string Key { get; private set; }

            [JsonProperty("link")]
            public string Link { get; private set; }
        }

        public struct TheCrewHubApi
        {
            [JsonProperty("summit")]
            public string Summit { get; private set; }

            [JsonProperty("gamedata")]
            public string GameData { get; private set; }

            [JsonProperty("dictionary")]
            public string Dictionary { get; private set; }

            [JsonProperty("news")]
            public string News { get; private set; }
        }
    }
}