using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    [Table("Enrollments")]
    public class Enrollment
    {
        [Key]
        [Column("EnrollmentID")]
        public int Id { get; set; }

        [Column("UserID")]
        public int UserId { get; set; }

        [Column("CategoryID")]
        public int CategoryId { get; set; }

        [Column(TypeName = "date")]
        public DateTime RegistrationDate { get; set; }
    }
}
