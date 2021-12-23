using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Server_Settings", Schema = "livebot")]
    internal class ServerSettings
    {
        [Key,ForeignKey("server_id"),Column("id_server")]
        public ulong ID_Server { get=>_ID_Server; set { _ID_Server = Convert.ToUInt64(value); } }
        private ulong _ID_Server;

        [Required, Column("delete_log")]
        public ulong Delete_Log { get=>_Delete_Log; set {_Delete_Log = Convert.ToUInt64(value); } }
        private ulong _Delete_Log;

        [Required, Column("user_traffic")]
        public ulong User_Traffic { get=>_User_Traffic; set { _User_Traffic = Convert.ToUInt64(value); } }
        private ulong _User_Traffic;

        [Required,Column("wkb_log")]
        public ulong WKB_Log { get=>_WKGB_Log; set { _WKGB_Log = Convert.ToUInt64(value); } }
        private ulong _WKGB_Log;

        [Required,Column("spam_exception")]
        public ulong[] Spam_Exception_Channels { get=> _Spam_Exception_Channels; set { _Spam_Exception_Channels = value.Select(w => Convert.ToUInt64(w)).ToArray(); } }
        private ulong[] _Spam_Exception_Channels;

        [Required,Column("mod_mail")]
        public ulong ModMailID { get=> _ModMailID; set { _ModMailID = Convert.ToUInt64(value); } }
        private ulong _ModMailID;

        [Required,Column("has_link_protection")]
        public bool HasLinkProtection { get; set; }

        [Required,Column("voice_activity_log")]
        public ulong VCLog { get=>_VCLog; set { _VCLog = Convert.ToUInt64(value); } }
        private ulong _VCLog;

        [Required, Column("event_log")]
        public ulong Event_Log { get=>_Event_Log; set { _Event_Log = Convert.ToUInt64(value); } }
        private ulong _Event_Log;
    }
}