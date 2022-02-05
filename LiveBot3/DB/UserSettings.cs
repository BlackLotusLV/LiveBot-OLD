﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("User_Settings", Schema = "livebot")]
    internal class UserSettings
    {
        [Key]
        [Column("id_user_settings")]
        public int ID_User_Settings { get; set; }

        [Required]
        [Column("user_id")]
        public ulong User_ID
        { get => _User_ID; set { _User_ID = Convert.ToUInt64(value); } }

        private ulong _User_ID;

        [Required]
        [Column("image_id")]
        public int Image_ID { get; set; }

        [Required]
        [Column("user_info")]
        public string User_Info { get; set; }
    }
}