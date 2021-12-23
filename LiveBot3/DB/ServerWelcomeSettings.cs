using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Server_Welcome_Settings", Schema = "livebot")]
    internal class ServerWelcomeSettings
    {
        [Key]
        [Column("server_id")]
        public ulong Server_ID { get=>_Server_ID; set { _Server_ID = Convert.ToUInt64(value); } }
        private ulong _Server_ID;

        [Required]
        [Column("channel_id")]
        public ulong Channel_ID { get=>_Channel_ID; set { _Channel_ID = Convert.ToUInt64(value); } }
        private ulong _Channel_ID;

        [Column("welcome_msg")]
        public string Welcome_Message { get; set; }

        [Column("goodbye_msg")]
        public string Goodbye_Message { get; set; }

        [Required]
        [Column("has_screening")]
        public bool HasScreening { get; set; }
    }
}