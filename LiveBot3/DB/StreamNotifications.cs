using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Stream_Notification", Schema = "livebot")]
    public class StreamNotifications
    {
        [Key]
        public int Stream_Notification_ID { get; set; }

        public string Server_ID { get; set; }
        public string[] Games { get; set; }
        public string[] Roles_ID { get; set; }
        public string Channel_ID { get; set; }
    }
}