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
        public ulong Button_ID
        { get => _Button_ID; set { _Button_ID = Convert.ToUInt64(value); } }

        private ulong _Button_ID;

        [Required]
        [Column("server_id")]
        public ulong Server_ID
        { get => _Server_ID; set { _Server_ID = Convert.ToUInt64(value); } }

        private ulong _Server_ID;

        [Required]
        [Column("channel_id")]
        public ulong Channel_ID
        { get => _Channel_ID; set { _Channel_ID = Convert.ToUInt64(value); } }

        private ulong _Channel_ID;
    }
}