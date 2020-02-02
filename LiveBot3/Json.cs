using Newtonsoft.Json;

namespace LiveBot
{
    internal class Json
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

        public struct Summit
        {
            [JsonProperty("summit_id")]
            public ulong Summit_ID { get; private set; }

            [JsonProperty("text_id")]
            public int Text_ID { get; private set; }

            [JsonProperty("color1")]
            public string Color1 { get; private set; }

            [JsonProperty("color2")]
            public string Color2 { get; private set; }

            [JsonProperty("start_date")]
            public long Start_Date { get; private set; }

            [JsonProperty("end_date")]
            public long End_Date { get; private set; }

            [JsonProperty("ticket_full")]
            public string Cover_Big { get; private set; }

            [JsonProperty("ticket_short")]
            public string Cover_Small { get; private set; }

            [JsonProperty("id")]
            public ulong ID { get; private set; }

            [JsonProperty("events")]
            public Event[] Events { get; private set; }
        }

        public struct Event
        {
            [JsonProperty("summit_id")]
            public string Summit_ID { get; private set; }

            [JsonProperty("is_mission")]
            public bool Is_Mission { get; private set; }

            [JsonProperty("id")]
            public ulong ID { get; private set; }

            [JsonProperty("debug_constraint")]
            public string Constraint { get; private set; }

            [JsonProperty("img_path")]
            public string Img_Path { get; private set; }

            [JsonProperty("modifiers")]
            public string[] Modifiers { get; private set; }
        }

        public struct Reward
        {
            [JsonProperty("level")]
            public int Level { get; private set; }

            [JsonProperty("debug_title")]
            public string Title { get; private set; }

            [JsonProperty("debug_subtitle")]
            public string Sub_Title { get; private set; }

            [JsonProperty("img_path")]
            public string Img_Path { get; private set; }

            [JsonProperty("type")]
            public string Type { get; private set; }

            [JsonProperty("extra")]
            public Reward_Extra[] Extra { get; private set; }
        }

        public struct Reward_Extra
        {
            [JsonProperty("currency_type")]
            public string Currency_Type { get; private set; }

            [JsonProperty("currency_amount")]
            public int Currency_Amount { get; private set; }

            [JsonProperty("type")]
            public string Type { get; private set; }

            [JsonProperty("slot_img")]
            public string Part_Img_Path { get; private set; }

            [JsonProperty("affix2")]
            public string Affix2 { get; private set; }

            [JsonProperty("quality_color")]
            public string Rarity_Hex_Code { get; private set; }

            [JsonProperty("bonus_icon")]
            public string Legendary_Bonus { get; private set; }

            [JsonProperty("vcat_icon")]
            public string Discipline { get; private set; }

            [JsonProperty("vcat_color")]
            public string Discipline_Color_Hex { get; private set; }
        }

        public struct Rank
        {
            [JsonProperty("points")]
            public ulong Points { get; private set; }

            [JsonProperty("rank")]
            public int UserRank { get; private set; }

            [JsonProperty("total_players")]
            public string Player_Count { get; private set; }

            [JsonProperty("activities")]
            public Activities[] Activities { get; private set; }

            [JsonProperty("tier_entries")]
            public Tier_Entries[] Tier_entries { get; private set; }
        }

        public struct Tier_Entries
        {
            [JsonProperty("points")]
            public ulong Points { get; set; }

            [JsonProperty("rank")]
            public ulong Rank { get; set; }
        }

        public struct Activities
        {
            [JsonProperty("activity_id")]
            public string Activity_ID { get; private set; }

            [JsonProperty("points")]
            public int Points { get; private set; }

            [JsonProperty("score")]
            public int Score { get; private set; }

            [JsonProperty("rank")]
            public int Rank { get; private set; }
        }

        public struct TCESummit
        {
            [JsonProperty("discord_id")]
            public ulong Discord_ID { get; private set; }

            [JsonProperty("subs")]
            public TCESummitSubs[] Subs { get; private set; }

            [JsonProperty("error")]
            public string Error { get; set; }
        }

        public struct TCESummitSubs
        {
            [JsonProperty("platform")]
            public string Platform { get; private set; }

            [JsonProperty("profile_id")]
            public string Profile_ID { get; private set; }
        }

        public struct SummitLeaderboardEntries
        {
            [JsonProperty("profile_id")]
            public string Profile_ID { get; private set; }

            [JsonProperty("rank")]
            public int Rank { get; private set; }

            [JsonProperty("points")]
            public int Points { get; private set; }

            [JsonProperty("score")]
            public int Score { get; private set; }

            [JsonProperty("formatted_score")]
            public string Formatted_Score { get; private set; }

            [JsonProperty("vehicle_id")]
            public ulong Vehicle_ID { get; private set; }

            [JsonProperty("Vehicle_Level")]
            public int Vehicle_Level { get; private set; }
        }

        public struct SummitLeaderboard
        {
            [JsonProperty("entries")]
            public SummitLeaderboardEntries[] Entries { get; private set; }

            [JsonProperty("score_format")]
            public string Score_Format { get; private set; }
        }
    }
}