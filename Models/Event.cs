using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    [Table("Events")]
    public class Event
    {
        [Key]
        [Column("EventID")]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Required, MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        [Column("OrganiserID")]
        public int OrganiserId { get; set; }
    }
}
