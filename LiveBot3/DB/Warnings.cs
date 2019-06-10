using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Warnings", Schema = "livebot")]
    class Warnings
    {
        [Key]
        public int ID_Warning { get; set; }
        public string Reason { get; set; }
        public bool Active { get; set; }
        public string Date { get; set; }
        public string Admin_ID { get; set; }
        public string User_ID { get; set; }
    }
}
