using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("User_Settings", Schema = "livebot")]
    internal class UserSettings
    {
        [Key]
        public int ID_User_Settings { get; set; }

        public string User_ID { get; set; }
        public int Image_ID { get; set; }
        public string Background_Colour { get; set; }
        public string Text_Colour { get; set; }
        public string Border_Colour { get; set; }
        public string User_Info { get; set; }
    }
}