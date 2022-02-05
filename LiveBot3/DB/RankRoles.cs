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
        public ulong Server_ID
        { get => _Server_ID; set { _Server_ID = Convert.ToUInt64(value); } }

        private ulong _Server_ID;

        [Required]
        [Column("role_id")]
        public ulong Role_ID
        { get => _Role_ID; set { _Role_ID = Convert.ToUInt64(value); } }

        private ulong _Role_ID;

        [Required]
        [Column("server_rank")]
        public long Server_Rank { get; set; }
    }
}