using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Reaction_Roles", Schema = "livebot")]
    public class ReactionRoles
    {
        [Key]
        public int ID { get; set; }
        public string Role_ID { get; set; }
        public string Server_ID { get; set; }
        public string Message_ID { get; set; }
        public string Reaction_ID { get; set; }
    }
}
