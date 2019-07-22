using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("User_Warnings", Schema = "livebot")]
    internal class UserWarnings
    {
        [Column("warning_level")]
        public int Warning_Level { get; set; }

        [Column("warning_count")]
        public int Warning_Count { get; set; }

        [Column("kick_count")]
        public int Kick_Count { get; set; }

        [Column("ban_count")]
        public int Ban_Count { get; set; }

        [Key]
        [Column("id_user")]
        public string ID_User { get; set; }
    }
}