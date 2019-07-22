using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("User_Settings", Schema = "livebot")]
    internal class UserSettings
    {
        [Key]
        [Column("id_user_settings")]
        public int ID_User_Settings { get; set; }

        [Column("user_id")]
        public string User_ID { get; set; }

        [Column("image_id")]
        public int Image_ID { get; set; }

        [Column("background_colour")]
        public string Background_Colour { get; set; }

        [Column("text_colour")]
        public string Text_Colour { get; set; }

        [Column("border_colour")]
        public string Border_Colour { get; set; }

        [Column("user_info")]
        public string User_Info { get; set; }
    }
}