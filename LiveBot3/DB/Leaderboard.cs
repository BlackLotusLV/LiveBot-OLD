﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Leaderboard", Schema = "livebot")]
    internal class Leaderboard
    {
        [Key]
        [Column("id_user")]
        public ulong ID_User
        { get => _ID_User; set { _ID_User = Convert.ToUInt64(value); } }

        private ulong _ID_User;

        [Required]
        [Column("followers")]
        public long Followers { get; set; } = 0;

        [Required]
        [Column("level")]
        public int Level { get; set; } = 0;

        [Required]
        [Column("bucks")]
        public long Bucks { get; set; }= 0;

        [Column("daily_used")]
        public string Daily_Used { get; set; }

        [Required]
        [Column("cookie_given")]
        public int Cookies_Given { get; set; } = 0;

        [Required]
        [Column("cookie_taken")]
        public int Cookies_Taken { get; set; } = 0;

        [Column("cookie_used")]
        public string Cookies_Used { get; set; }
    }
}