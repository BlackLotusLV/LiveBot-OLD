using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Server_Ranks", Schema = "livebot")]
    internal class ServerRanks
    {
        [Key]
        [Column("id_server_rank")]
        public int ID_Server_Rank { get; set; }

        [Column("user_id")]
        public string User_ID { get; set; }

        [Column("server_id")]
        public string Server_ID { get; set; }

        [Column("followers")]
        public long Followers { get; set; }

        [Column("warning_level")]
        public int Warning_Level { get; set; }

        [Column("kick_count")]
        public int Kick_Count { get; set; }

        [Column("ban_count")]
        public int Ban_Count { get; set; }
    }
}