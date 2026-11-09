using System.ComponentModel.DataAnnotations;

namespace RaceDayMVC.Models;

public class LoginVm
{
    [Required(ErrorMessage = "Please enter your email address."), EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Please enter your password.")]
    public string Password { get; set; } = "";
}

public class RegisterVm
{
    [Required(ErrorMessage = "Please enter your name."), MaxLength(50)]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Please enter your email address."), EmailAddress(ErrorMessage = "Please enter a valid email address."), MaxLength(50)]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Please choose a password."), MinLength(8, ErrorMessage = "The password must be at least 8 characters."), MaxLength(60)]
    public string Password { get; set; } = "";

    [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
    public string ConfirmPassword { get; set; } = "";

    [Required(ErrorMessage = "Please choose a role."), RegularExpression("^(Organiser|Participant)$", ErrorMessage = "Please choose Organiser or Participant.")]
    public string Role { get; set; } = "Participant";
}

public class HomeVm
{
    public List<EventItem> Events { get; set; } = new();
    public string? Search { get; set; }
    public string? EventType { get; set; }
}

public class EventDetailsVm
{
    public EventItem Event { get; set; } = new();
    public List<CategoryItem> Categories { get; set; } = new();
    public HashSet<int> EnrolledCategoryIds { get; set; } = new();
    public bool IsParticipant { get; set; }
}

public class ProfileVm
{
    [Required(ErrorMessage = "Please enter your name."), MaxLength(50)]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Please enter your email address."), EmailAddress(ErrorMessage = "Please enter a valid email address."), MaxLength(50)]
    public string Email { get; set; } = "";

    [MinLength(8, ErrorMessage = "The new password must be at least 8 characters."), MaxLength(60)]
    public string? NewPassword { get; set; }

    [Compare(nameof(NewPassword), ErrorMessage = "The passwords do not match.")]
    public string? ConfirmNewPassword { get; set; }

    public string? ProfilePictureUrl { get; set; }
    public string Role { get; set; } = "";
}

public class EventFormVm
{
    public int EventID { get; set; }

    [Required(ErrorMessage = "Please enter the event name."), MaxLength(100)]
    public string Title { get; set; } = "";

    [MaxLength(500, ErrorMessage = "The description can be at most 500 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Please choose the event date.")]
    public DateTime? Date { get; set; }

    [Required(ErrorMessage = "Please enter the location."), MaxLength(100)]
    public string Location { get; set; } = "";

    [Range(0.01, 999.99, ErrorMessage = "Please enter a distance between 0.01 and 999.99 km.")]
    public decimal Distance { get; set; }

    [Required(ErrorMessage = "Please choose the event type."), RegularExpression("^(Run|Walk|Cycle)$", ErrorMessage = "Please choose Run, Walk or Cycle.")]
    public string EventType { get; set; } = "Run";

    public IFormFile? BannerFile { get; set; }
    public string? ExistingBannerUrl { get; set; }
}

public class CategoryFormVm
{
    [Required(ErrorMessage = "Please enter a category name (e.g. 10km, Under 20, Senior)."), MaxLength(50)]
    public string Name { get; set; } = "";

    [Range(0.01, 999.99, ErrorMessage = "Please enter a distance between 0.01 and 999.99 km.")]
    public decimal Distance { get; set; }
}

public class DashboardVm
{
    public List<EventItem> Events { get; set; } = new();
    public int TotalEnrolments => Events.Sum(e => e.EnrollmentCount);
    public List<EventItem> Upcoming => Events.Where(e => e.Date.Date >= DateTime.Today).OrderBy(e => e.Date).ToList();
}

public class CategoriesVm
{
    public EventItem Event { get; set; } = new();
    public List<CategoryItem> Categories { get; set; } = new();
    public CategoryFormVm Form { get; set; } = new();
}

public class EnrolmentsVm
{
    public EventItem Event { get; set; } = new();
    public List<EnrollmentItem> Enrolments { get; set; } = new();
}

public class ResultRow
{
    public EnrollmentItem Enrolment { get; set; } = new();
    public ResultItem? Result { get; set; }
}

public class ResultsVm
{
    public EventItem Event { get; set; } = new();
    public List<ResultRow> Rows { get; set; } = new();
    /// <summary>Results can only be captured once the event date has arrived.</summary>
    public bool CanCapture => Event.Date.Date <= DateTime.Today;
}

/// <summary>One row of the results form as posted by the browser.</summary>
public class ResultInput
{
    public int EnrollmentId { get; set; }
    public int? ResultId { get; set; }
    public string? FinishTime { get; set; }
    public int? Position { get; set; }
}

/// <summary>Small display helpers used by the views.</summary>
public static class Fmt
{
    public static string Ordinal(int n)
    {
        var suffix = (n % 100) is >= 11 and <= 13
            ? "th"
            : (n % 10) switch { 1 => "st", 2 => "nd", 3 => "rd", _ => "th" };
        return $"{n}{suffix}";
    }

    public static string Time(TimeSpan? t) => t is null ? "" : t.Value.ToString(@"hh\:mm\:ss");
    public static string Date(DateTime d) => d.ToString("ddd d MMM yyyy");
    public static string Date(DateTime? d) => d is null ? "" : Date(d.Value);
    public static string Distance(decimal d) => d.ToString("0.##") + " km";

    public static string TypeIcon(string type) => type switch
    {
        "Run" => "🏃",
        "Walk" => "🥾",
        "Cycle" => "🚴",
        _ => "🏁"
    };

    public static string Initial(string? name) =>
        string.IsNullOrWhiteSpace(name) ? "?" : name.Trim()[0].ToString().ToUpperInvariant();
}