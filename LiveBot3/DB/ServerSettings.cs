using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Server_Settings", Schema = "livebot")]
    internal class ServerSettings
    {
        [Key]
        [ForeignKey("server_id")]
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
        [Column("spam_exception")]
        public decimal[] Spam_Exception_Channels { get; set; }

        [Required]
        [Column("mod_mail")]
        public decimal ModMailID { get; set; }

        [Required]
        [Column("has_link_protection")]
        public bool HasLinkProtection { get; set; }

        [Required]
        [Column("voice_activity_log")]
        public decimal VCLog { get; set; }
    }
}