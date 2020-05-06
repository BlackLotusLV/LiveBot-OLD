using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("AM_Banned_Words", Schema = "livebot")]
    internal class AMBannedWords
    {
        [Key]
        [Column("id_banned_word")]
        public int ID { get; set; }

        [Required]
        [Column("word")]
        public string Word { get; set; }

        [Required]
        [Column("server_id")]
        public string Server_ID { get; set; }

        [Required]
        [Column("offense")]
        public string Offense { get; set; }
    }
}