using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("User_Warnings", Schema = "livebot")]
    internal class UserWarnings
    {
        public int Warning_Level { get; set; }

        public int Warning_Count { get; set; }
        public int Kick_Count { get; set; }
        public int Ban_Count { get; set; }
        [Key]
        public string ID_User { get; set; }
    }
}