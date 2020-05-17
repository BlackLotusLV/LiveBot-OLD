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
        public decimal User_ID { get; set; }

        [Required]
        [Column("bg_id")]
        public int BG_ID { get; set; }
    }
}