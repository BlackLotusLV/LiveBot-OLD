using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Button_Roles", Schema = "livebot")]
    public class ButtonRoles
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Required]
        [Column("button_id")]
        public decimal Button_ID { get; set; }

        [Required]
        [Column("server_id")]
        public decimal Server_ID { get; set; }

        [Required]
        [Column("channel_id")]
        public decimal Channel_ID { get; set; }
    }
}