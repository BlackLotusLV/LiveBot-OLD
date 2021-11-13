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
        public decimal Server_ID { get; set; }

        [Required]
        [Column("role_id")]
        public decimal Role_ID { get; set; }

        [Required]
        [Column("channel_id")]
        public decimal Channel_ID { get; set; }

        [Required]
        [Column("cooldown_minutes")]
        public int Cooldown { get; set; }

        [Required]
        [Column("last_used")]
        public DateTime Last_Used { get; set; }

        [Required]
        [Column("emoji_id")]
        public decimal Emoji_ID { get; set; }

        [Required]
        [Column("message")]
        public string Message { get; set; }
    }
}