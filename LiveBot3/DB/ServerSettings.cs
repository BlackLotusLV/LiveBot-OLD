using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Server_Settings", Schema = "livebot")]
    internal class ServerSettings
    {
        [Key]
        [Column("id_server")]
        public decimal ID_Server { get; set; }

        [Required]
        [Column("delete_log")]
        public decimal Delete_Log { get; set; }

        [Required]
        [Column("user_traffic")]
        public decimal User_Traffic { get; set; }

        [Required]
        [Column("wkb_log")]
        public decimal WKB_Log { get; set; }

        [Required]
        [Column("welcome_cwb")]
        public string[] Welcome_Settings { get; set; }

        [Required]
        [Column("spam_exception")]
        public decimal[] Spam_Exception_Channels { get; set; }

        [Required]
        [Column("mod_mail")]
        public decimal ModMailID { get; set; }
    }
}