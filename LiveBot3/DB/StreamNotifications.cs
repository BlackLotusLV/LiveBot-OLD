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

        [Required]
        [Column("server_id")]
        public ulong Server_ID
        { get => _Server_ID; set { _Server_ID = Convert.ToUInt64(value); } }

        private ulong _Server_ID;

        [Column("games")]
        public string[] Games { get; set; }

        [Column("roles_id")]
        public ulong[] Roles_ID
        { get => _Roles_ID; set { _Roles_ID = value.Select(w => Convert.ToUInt64(w)).ToArray(); } }

        private ulong[] _Roles_ID;

        [Required]
        [Column("channel_id")]
        public ulong Channel_ID
        { get => _Channel_ID; set { _Channel_ID = Convert.ToUInt64(value); } }

        private ulong _Channel_ID;
    }
}