using Newtonsoft.Json;

namespace LiveBot.Json
{
    internal class TCHubJson
    {
        public class TCHub
        {
            [JsonProperty("missions")]
            public Mission[] Missions { get; private set; }

            [JsonProperty("skills")]
            public Skill[] Skills { get; private set; }

            [JsonProperty("brands")]
            public Brand[] Brands { get; private set; }

            [JsonProperty("models")]
            public Model[] Models { get; private set; }

            [JsonProperty("disciplines")]
            public Discipline[] Disciplines { get; private set; }

            [JsonProperty("families")]
            public Family[] Families { get; private set; }

            [JsonProperty("news")]
            public News[] News { get; private set; }
        }

        public class Summit
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

            [JsonProperty("rewards")]
            public Reward[] Rewards { get; set; }
        }

        public class Event
        {
            [JsonProperty("summit_id")]
            public string Summit_ID { get; private set; }

            [JsonProperty("is_mission")]
            public bool Is_Mission { get; private set; }

            [JsonProperty("id")]
            public ulong ID { get; private set; }

            [JsonProperty("constraint_text_id")]
            public string[] Constraint_Text_ID { get; set; }

            [JsonProperty("debug_constraint")]
            public string Debug_Constraint { get; private set; }

            [JsonProperty("img_path")]
            public string Img_Path { get; private set; }

            [JsonProperty("modifiers")]
            public string[] Modifiers { get; private set; }

            public byte[] Image_Byte { get; set; }
        }

        public class Reward
        {
            [JsonProperty("summit_id")]
            public ulong Summit_ID { get; private set; }

            [JsonProperty("debug_name")]
            public string Debug_Name { get; private set; }

            [JsonProperty("level")]
            public int Level { get; private set; }

            [JsonProperty("title_text_id")]
            public string Title_Text_ID { get; private set; }

            [JsonProperty("debug_title")]
            public string Debug_Title { get; private set; }

            [JsonProperty("debug_subtitle")]
            public string Debug_Subtitle { get; private set; }

            [JsonProperty("img_path")]
            public string Img_Path { get; private set; }

            [JsonProperty("type")]
            public string Type { get; private set; }

            [JsonProperty("extra")]
            public Dictionary<string, string> Extra { get; private set; }
        }

        public class Rank
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
            public TierEntries[] Tier_entries { get; private set; }
        }

        public class TierEntries
        {
            [JsonProperty("points")]
            public ulong Points { get; set; }

            [JsonProperty("rank")]
            public ulong Rank { get; set; }
        }

        public class Activities
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

        public class TceSummit
        {
            [JsonProperty("discord_id")]
            public ulong Discord_ID { get; private set; }

            [JsonProperty("subs")]
            public TceSummitSubs[] Subs { get; private set; }

            [JsonProperty("error")]
            public string Error { get; set; }
        }

        public class TceSummitSubs
        {
            [JsonProperty("platform")]
            public string Platform { get; set; }

            [JsonProperty("profile_id")]
            public string Profile_ID { get; set; }
        }

        public class SummitLeaderboardEntries
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

        public class SummitLeaderboard
        {
            [JsonProperty("entries")]
            public SummitLeaderboardEntries[] Entries { get; private set; }

            [JsonProperty("score_format")]
            public string Score_Format { get; private set; }
        }

        public class Mission
        {
            [JsonProperty("id")]
            public ulong ID { get; private set; }

            [JsonProperty("text_id")]
            public string Text_ID { get; private set; }

            [JsonProperty("type")]
            public string Type { get; private set; }

            [JsonProperty("unlock_level")]
            public int Unlock_Level { get; private set; }

            [JsonProperty("image_path")]
            public string IMG_Path { get; private set; }

            [JsonProperty("discipline")]
            public ulong Discipline_ID { get; private set; }
        }

        public class Skill
        {
            [JsonProperty("id")]
            public ulong ID { get; private set; }

            [JsonProperty("text_id")]
            public string Text_ID { get; private set; }

            [JsonProperty("family")]
            public string Family { get; private set; }

            [JsonProperty("type_text_id")]
            public string Type_Text_ID { get; private set; }

            [JsonProperty("score_type")]
            public string Score_Type { get; private set; }

            [JsonProperty("img_path")]
            public string IMG_Path { get; private set; }
        }

        public class Brand
        {
            [JsonProperty("id")]
            public string ID { get; private set; }

            [JsonProperty("text_id")]
            public string Text_ID { get; private set; }

            [JsonProperty("rank")]
            public int Rank { get; private set; }
        }

        public class Model
        {
            [JsonProperty("id")]
            public ulong ID { get; private set; }

            [JsonProperty("text_id")]
            public string Text_ID { get; private set; }

            [JsonProperty("vcat")]
            public string VCat { get; private set; }

            [JsonProperty("brand")]
            public string Brand_ID { get; private set; }
        }

        public class Discipline
        {
            [JsonProperty("id")]
            public ulong ID { get; private set; }

            [JsonProperty("text_id")]
            public string Text_ID { get; private set; }

            [JsonProperty("family")]
            public ulong Family_ID { get; private set; }

            [JsonProperty("img_path")]
            public string IMG_Path { get; private set; }
        }

        public class Family
        {
            [JsonProperty("id")]
            public ulong ID { get; private set; }

            [JsonProperty("text_id")]
            public string Text_ID { get; private set; }
        }

        //Fame board
        public class Fame
        {
            [JsonProperty("best")]
            public FameEntity Best { get; private set; }

            [JsonProperty("is_increasing")]
            public bool Is_Increasing { get; private set; }

            [JsonProperty("scores")]
            public FameEntity[] Scores { get; private set; }
        }

        public class FameEntity
        {
            [JsonProperty("profile_id")]
            public string Profile_ID { get; private set; }

            [JsonProperty("score")]
            public int Score { get; private set; }

            [JsonProperty("rank")]
            public int Rank { get; private set; }
        }

        public class News
        {
            [JsonProperty("newsId")]
            public string ID { get; private set; }

            [JsonProperty("type")]
            public string Type { get; private set; }

            [JsonProperty("placement")]
            public string Placement { get; private set; }

            [JsonProperty("priority")]
            public int Priority { get; private set; }

            [JsonProperty("displayTime")]
            public int DisplayTime { get; private set; }

            [JsonProperty("publicationDate")]
            public DateTime? PublicationDate { get; private set; }

            [JsonProperty("expirationDate")]
            public DateTime? ExpirationDate { get; private set; }

            [JsonProperty("title")]
            public string Title { get; private set; }

            [JsonProperty("body")]
            public string Body { get; private set; }

            [JsonProperty("mediaURL")]
            public string MediaURL { get; private set; }

            [JsonProperty("mediaType")]
            public string MediaType { get; private set; }

            [JsonProperty("profileId")]
            public string ProfileID { get; private set; }

            [JsonProperty("obj")]
            public NewsObj Obj { get; private set; }

            [JsonProperty("links")]
            public NewsLinks[] NewsLinks { get; private set; }
        }

        public class NewsObj
        {
            [JsonProperty("tag")]
            public string Tag { get; private set; }
        }

        public class NewsLinks
        {
            [JsonProperty("type")]
            public string Type { get; private set; }

            [JsonProperty("param")]
            public string Param { get; private set; }

            [JsonProperty("actionName")]
            public string ActionName { get; private set; }
        }
    }
}