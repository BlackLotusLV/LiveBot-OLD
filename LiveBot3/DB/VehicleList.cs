using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Vehicle_List",Schema ="livebot")]
    public class VehicleList
    {
        [Key]
        public int ID_Vehicle { get; set; }
        public int Discipline { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public string Type { get; set; }
    }
}
