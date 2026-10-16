namespace RaceDayAPI.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public int CategoryId { get; set; }
        public DateTime? RegistrationDate { get; set; }
    }
}
