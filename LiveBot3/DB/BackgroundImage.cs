using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Background_Image", Schema = "livebot")]
    class BackgroundImage
    {
        [Key]
        public int ID_BG { get; set; }
        public byte[] Image { get; set; }
        public string Name { get; set; }
        public long? Price { get; set; }
    }
}
