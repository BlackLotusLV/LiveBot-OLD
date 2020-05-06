using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveBot.DB
{
    [Table("Weather_Schedule", Schema = "livebot")]
    internal class WeatherSchedule
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Required]
        [Column("time")]
        public TimeSpan Time { get; set; }

        [Required]
        [Column("day")]
        public int Day { get; set; }

        [Required]
        [Column("weather")]
        public string Weather { get; set; }
    }
}