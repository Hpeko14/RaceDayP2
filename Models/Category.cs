using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        [Column("CategoryID")]
        public int Id { get; set; }

        [Column("EventID")]
        public int EventId { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5,2)")]
        public decimal Distance { get; set; }
    }
}
