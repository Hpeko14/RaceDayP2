namespace RaceDayAPI.Models
{
    public class Result
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int Position { get; set; }
    }
}
