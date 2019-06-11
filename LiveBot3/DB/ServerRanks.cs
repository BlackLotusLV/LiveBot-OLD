using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Server_Ranks", Schema = "livebot")]
    internal class ServerRanks
    {
        [Key]
        public int ID_Server_Rank { get; set; }

        public string User_ID { get; set; }
        public string Server_ID { get; set; }
        public long Followers { get; set; }
    }
}