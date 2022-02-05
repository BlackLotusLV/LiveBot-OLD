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

        [Required]
        [Column("user_id")]
        public ulong User_ID
        { get => _User_ID; set { _User_ID = Convert.ToUInt64(value); } }

        private ulong _User_ID;

        [Required]
        [Column("server_id")]
        public ulong Server_ID
        { get => _Server_ID; set { _Server_ID = Convert.ToUInt64(value); } }

        private ulong _Server_ID;

        [Required]
        [Column("followers")]
        public long Followers { get; set; }

        [Required]
        [Column("warning_level")]
        public int Warning_Level { get; set; }

        [Required]
        [Column("kick_count")]
        public int Kick_Count { get; set; }

        [Required]
        [Column("ban_count")]
        public int Ban_Count { get; set; }
    }
}