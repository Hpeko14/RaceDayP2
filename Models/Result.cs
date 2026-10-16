using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    [Table("Results")]
    public class Result
    {
        [Key]
        [Column("ResultID")]
        public int Id { get; set; }

        [Column("EnrollmentID")]
        public int EnrollmentId { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }
    }
}
