using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Leaderboard", Schema = "livebot")]
    internal class Leaderboard
    {
        [Key]
        [Column("id_user")]
        public string ID_User { get; set; }
        [Column("followers")]
        public long Followers { get; set; }
        [Column("level")]
        public int Level { get; set; }
        [Column("bucks")]
        public long Bucks { get; set; }
    }
}