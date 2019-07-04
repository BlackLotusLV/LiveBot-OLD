using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Background_Image", Schema = "livebot")]
    internal class BackgroundImage
    {
        [Key]
        [Column("id_bg")]
        public int ID_BG { get; set; }
        [Column("image")]
        public byte[] Image { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("price")]
        public long? Price { get; set; }
    }
}