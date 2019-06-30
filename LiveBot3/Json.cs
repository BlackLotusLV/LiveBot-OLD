using Newtonsoft.Json;
using System.Collections.Generic;

namespace LiveBot
{
    class Json
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
        }

        public struct TCE
        {
            [JsonProperty("key")]
            public string Key { get; private set; }
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
            public string Start_Date { get; private set; }
            [JsonProperty("end_date")]
            public string End_Date { get; private set; }
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
            [JsonProperty("total_players")]
            public string Player_Count { get; private set; }

            [JsonProperty("tier_entries")]
            public Tier_Entries[] Tier_entries { get; private set; }
        }
    }
}
