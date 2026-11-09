namespace RaceDayMVC.Models;

// Shapes of the JSON the Part 2 API returns. No database types live in this project.

public class UserProfile
{
    public int UserID { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
    public string? ProfilePictureUrl { get; set; }
}

public class EventItem
{
    public int EventID { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; } = "";
    public decimal Distance { get; set; }
    public string EventType { get; set; } = "";
    public string? BannerUrl { get; set; }
    public int OrganiserID { get; set; }
    public int EnrollmentCount { get; set; }
}

public class CategoryItem
{
    public int CategoryID { get; set; }
    public int EventID { get; set; }
    public string Name { get; set; } = "";
    public decimal Distance { get; set; }
}

public class EnrollmentItem
{
    public int EnrollmentID { get; set; }
    public int UserID { get; set; }
    public string? ParticipantName { get; set; }
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = "";
    public int EventID { get; set; }
    public string EventTitle { get; set; } = "";
    public DateTime? EventDate { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public string Status { get; set; } = "";
}

public class ResultItem
{
    public int ResultID { get; set; }
    public int EnrollmentID { get; set; }
    public string? ParticipantName { get; set; }
    public int EventID { get; set; }
    public string EventTitle { get; set; } = "";
    public DateTime? EventDate { get; set; }
    public string CategoryName { get; set; } = "";
    public TimeSpan? FinishTime { get; set; }
    public int? Position { get; set; }
    public int TotalFinishers { get; set; }
}