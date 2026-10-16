using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{// start of namespace
    [Table("Routes")]
    public class RaceRoute
    {// start of class
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
    }// end of class
}// end of namespace