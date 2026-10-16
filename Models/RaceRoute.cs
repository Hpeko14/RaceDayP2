using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    [Table("Routes")]
    public class RaceRoute
    {
        [Key]
        [Column("RouteID")]
        public int Id { get; set; }

        [Column("EventID")]
        public int EventId { get; set; }

        [MaxLength(200)]
        [Column("MapURL")]
        public string? MapUrl { get; set; }

        [MaxLength(200)]
        public string? WeatherInfo { get; set; }
    }
}