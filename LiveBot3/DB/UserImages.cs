using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("User_Images", Schema = "livebot")]
    internal class UserImages
    {
        [Key]
        public int ID_User_Images { get; set; }

        public string User_ID { get; set; }
        public int BG_ID { get; set; }
    }
}