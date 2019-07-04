using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Stream_Notification", Schema = "livebot")]
    public class StreamNotifications
    {
        [Key]
        [Column("stream_notification_id")]
        public int Stream_Notification_ID { get; set; }
        [Column("server_id")]
        public string Server_ID { get; set; }
        [Column("games")]
        public string[] Games { get; set; }
        [Column("roles_id")]
        public string[] Roles_ID { get; set; }
        [Column("channel_id")]
        public string Channel_ID { get; set; }
    }
}