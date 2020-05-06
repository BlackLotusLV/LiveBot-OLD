using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Reaction_Roles", Schema = "livebot")]
    public class ReactionRoles
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Required]
        [Column("role_id")]
        public string Role_ID { get; set; }

        [Required]
        [Column("server_id")]
        public string Server_ID { get; set; }

        [Required]
        [Column("message_id")]
        public string Message_ID { get; set; }

        [Required]
        [Column("reaction_id")]
        public string Reaction_ID { get; set; }

        [Required]
        [Column("type")]
        public string Type { get; set; }
    }
}