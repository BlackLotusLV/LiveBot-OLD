using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Mod_Mail", Schema = "livebot")]
    internal class ModMail
    {
        [Key]
        [Column("id_modmail")]
        public long ID { get; set; }

        [Required]
        [Column("server_id")]
        public ulong Server_ID
        { get => _Server_ID; set { _Server_ID = Convert.ToUInt64(value); } }

        private ulong _Server_ID;

        [Required]
        [Column("user_id")]
        public ulong User_ID
        { get => _User_ID; set { _User_ID = Convert.ToUInt64(value); } }

        private ulong _User_ID;

        [Required]
        [Column("last_message_time")]
        public DateTime LastMSGTime { get; set; }

        [Required]
        [Column("has_chatted")]
        public bool HasChatted { get; set; }

        [Required]
        [Column("is_active")]
        public bool IsActive { get; set; }

        [Required]
        [Column("color_hex")]
        public string ColorHex { get; set; }
    }
}