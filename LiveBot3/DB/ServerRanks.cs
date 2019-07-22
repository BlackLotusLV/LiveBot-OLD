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
    }
}