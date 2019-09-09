using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Server_Settings", Schema = "livebot")]
    internal class ServerSettings
    {
        [Key]
        [Column("id_server")]
        public string ID_Server { get; set; }

        [Column("delete_log")]
        public string Delete_Log { get; set; }

        [Column("user_traffic")]
        public string User_Traffic { get; set; }

        [Column("wkb_log")]
        public string WKB_Log { get; set; }

        [Column("welcome_cwb")]
        public string[] Welcome_Settings { get; set; }
    }
}