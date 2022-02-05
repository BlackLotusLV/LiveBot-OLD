using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("User_Images", Schema = "livebot")]
    internal class UserImages
    {
        [Key]
        [Column("id_user_images", TypeName = "Serial")]
        public int ID_User_Images { get; set; }

        [Required]
        [Column("user_id")]
        public ulong User_ID
        { get => _User_ID; set { _User_ID = Convert.ToUInt64(value); } }

        private ulong _User_ID;

        [Required]
        [Column("bg_id")]
        public int BG_ID { get; set; }
    }
}