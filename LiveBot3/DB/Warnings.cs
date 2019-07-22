using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Warnings", Schema = "livebot")]
    internal class Warnings
    {
        [Key]
        [Column("id_warning")]
        public int ID_Warning { get; set; }

        [Column("reason")]
        public string Reason { get; set; }

        [Column("active")]
        public bool Active { get; set; }

        [Column("date")]
        public string Date { get; set; }

        [Column("admin_id")]
        public string Admin_ID { get; set; }

        [Column("user_id")]
        public string User_ID { get; set; }
    }
}