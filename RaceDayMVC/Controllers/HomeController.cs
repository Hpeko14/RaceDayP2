using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Filters;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

public class HomeController : AppControllerBase
{
    public HomeController(ApiClient api) : base(api) { }

    /// <summary>Participants see upcoming events with filters. Organisers go to their dashboard.</summary>
    [RoleAuthorize]
    public async Task<IActionResult> Index(string? search, string? eventType)
    {
        if (HttpContext.Session.GetString(SessionKeys.Role) == Roles.Organiser)
            return RedirectToAction("Dashboard", "Organiser");

        var query = new List<string> { "upcomingOnly=true" };
        if (!string.IsNullOrWhiteSpace(eventType)) query.Add("eventType=" + Uri.EscapeDataString(eventType));
        if (!string.IsNullOrWhiteSpace(search)) query.Add("search=" + Uri.EscapeDataString(search.Trim()));

        var result = await Api.GetAsync<List<EventItem>>("api/events?" + string.Join("&", query));
        if (result.Status == 401) return SessionExpired();
        if (!result.Success) ViewBag.LoadError = result.Error;

        return View(new HomeVm { Events = result.Data ?? new(), Search = search, EventType = eventType });
    }

    /// <summary>Full event information with its categories and (for participants) the Enter Event buttons.</summary>
    [RoleAuthorize]
    public async Task<IActionResult> Details(int id)
    {
        var ev = await Api.GetAsync<EventItem>($"api/events/{id}");
        if (ev.Status == 401) return SessionExpired();
        if (!ev.Success || ev.Data is null)
        {
            ShowError(ev.Status == 404 ? "That event could not be found." : ev.Error);
            return RedirectToAction("Index");
        }

        var categories = await Api.GetAsync<List<CategoryItem>>($"api/events/{id}/categories");
        var vm = new EventDetailsVm
        {
            Event = ev.Data,
            Categories = categories.Data ?? new(),
            IsParticipant = HttpContext.Session.GetString(SessionKeys.Role) == Roles.Participant
        };
        if (!categories.Success) ViewBag.LoadError = categories.Error;

        if (vm.IsParticipant)
        {
            var mine = await Api.GetAsync<List<EnrollmentItem>>("api/enrollments/my");
            if (mine.Success && mine.Data is not null)
                vm.EnrolledCategoryIds = mine.Data.Select(e => e.CategoryID).ToHashSet();
        }
        return View(vm);
    }

    // Used by the exception handler for any HTTP verb, so no [HttpGet] and no antiforgery check here
    [IgnoreAntiforgeryToken]
    public IActionResult Error(int? code)
    {
        ViewBag.Code = code;
        return View();
    }
}