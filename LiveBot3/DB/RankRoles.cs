using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Rank_Roles", Schema = "livebot")]
    internal class RankRoles
    {
        [Key]
        [Column("id_rank_roles", TypeName = "serial")]
        public int ID_Rank_Roles { get; set; }

        [Required]
        [Column("server_id")]
        public string Server_ID { get; set; }

        [Required]
        [Column("role_id")]
        public string Role_ID { get; set; }

        [Required]
        [Column("server_rank")]
        public long Server_Rank { get; set; }
    }
}