using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Discipline_List", Schema = "livebot")]
    public class DisciplineList
    {
        [Key]
        public int ID_Discipline { get; set; }

        public string Family { get; set; }
        public string Discipline_Name { get; set; }
    }
}