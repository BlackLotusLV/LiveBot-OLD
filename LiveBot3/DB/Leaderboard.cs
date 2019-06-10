using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Leaderboard", Schema = "livebot")]
    class Leaderboard
    {
        [Key]
        public string ID_User { get; set; }
        public long Followers { get; set; }
        public int Level { get; set; }
        public long Bucks { get; set; }
    }
}
