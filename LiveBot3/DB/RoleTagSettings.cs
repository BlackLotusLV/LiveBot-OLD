using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Role_Tag_Settings", Schema = "livebot")]
    public class RoleTagSettings
    {
        [Key]
        [Column("id_role_tag")]
        public int ID { get; set; }

        [Required]
        [Column("server_id")]
        public ulong Server_ID { get=>_Server_ID; set { _Server_ID = Convert.ToUInt64(value); } }
        private ulong _Server_ID;

        [Required]
        [Column("role_id")]
        public ulong Role_ID { get=>_Role_ID; set { _Role_ID = Convert.ToUInt64(value); } }
        private ulong _Role_ID;

        [Required]
        [Column("channel_id")]
        public ulong Channel_ID { get=>_Channel_ID; set { _Channel_ID = Convert.ToUInt64(value); } }
        private ulong _Channel_ID;

        [Required]
        [Column("cooldown_minutes")]
        public int Cooldown { get; set; }

        [Required]
        [Column("last_used")]
        public DateTime Last_Used { get; set; }

        [Required]
        [Column("emoji_id")]
        public ulong Emoji_ID { get=>_Emoji_ID; set { _Emoji_ID = Convert.ToUInt64(value); } }
        private ulong _Emoji_ID;

        [Required]
        [Column("message")]
        public string Message { get; set; }
    }
}