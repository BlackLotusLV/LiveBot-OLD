using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Server_Welcome_Settings", Schema = "livebot")]
    internal class ServerWelcomeSettings
    {
        [Key]
        [Column("server_id")]
        public decimal Server_ID { get; set; }

        [Required]
        [Column("channel_id")]
        public decimal Channel_ID { get; set; }

        [Column("welcome_msg")]
        public string Welcome_Message { get; set; }

        [Column("goodbye_msg")]
        public string Goodbye_Message { get; set; }

        [Required]
        [Column("has_screening")]
        public bool HasScreening { get; set; }
    }
}