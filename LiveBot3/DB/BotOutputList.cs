using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Bot_Output_List", Schema = "livebot")]
    internal class BotOutputList
    {
        [Key]
        [Column("id_output")]
        public int ID { get; set; }

        [Column("command")]
        public string Command { get; set; }

        [Column("language")]
        public string Language { get; set; }

        [Column("command_text")]
        public string Command_Text { get; set; }
    }
}