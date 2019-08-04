using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Vehicle_List", Schema = "livebot")]
    public class VehicleList
    {
        [Key]
        [Column("id_vehicle")]
        public int ID_Vehicle { get; set; }

        [Column("discipline")]
        public int Discipline { get; set; }

        [Column("brand")]
        public string Brand { get; set; }

        [Column("model")]
        public string Model { get; set; }

        [Column("year")]
        public string Year { get; set; }

        [Column("type")]
        public string Type { get; set; }
        [Column("selected_count")]
        public int Selected_Count { get; set; }
    }
}