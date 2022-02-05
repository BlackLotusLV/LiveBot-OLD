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
        public ulong Server_ID
        { get => _Server_ID; set { _Server_ID = Convert.ToUInt64(value); } }

        private ulong _Server_ID;

        [Required]
        [Column("offense")]
        public string Offense { get; set; }
    }
}