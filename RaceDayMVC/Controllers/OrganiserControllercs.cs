using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Filters;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

/// <summary>Everything an Organiser can do. Every action here is blocked for Participants by the filter.</summary>
[RoleAuthorize(Role = Roles.Organiser)]
public class OrganiserController : AppControllerBase
{
    private static readonly Regex TimePattern = new(@"^\d{1,2}:[0-5]\d:[0-5]\d$");

    public OrganiserController(ApiClient api) : base(api) { }

    // ---------- Dashboard ----------

    public async Task<IActionResult> Dashboard()
    {
        var result = await Api.GetAsync<List<EventItem>>("api/events/mine");
        if (result.Status == 401) return SessionExpired();
        if (!result.Success) ViewBag.LoadError = result.Error;
        return View(new DashboardVm { Events = result.Data ?? new() });
    }

    // ---------- Events ----------

    public async Task<IActionResult> Events()
    {
        var result = await Api.GetAsync<List<EventItem>>("api/events/mine");
        if (result.Status == 401) return SessionExpired();
        if (!result.Success) ViewBag.LoadError = result.Error;
        return View((result.Data ?? new()).OrderByDescending(e => e.Date).ToList());
    }

    [HttpGet]
    public IActionResult CreateEvent() => View("EventForm", new EventFormVm());

    [HttpPost]
    public async Task<IActionResult> CreateEvent(EventFormVm vm)
    {
        if (!ModelState.IsValid) return View("EventForm", vm);

        var result = await Api.PostAsync<EventItem>("api/events", ToBody(vm));
        if (result.Status == 401) return SessionExpired();
        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError("", result.Error);
            return View("EventForm", vm);
        }

        ShowSuccess("Event created. Now add its categories.");
        return RedirectToAction("Categories", new { eventId = result.Data.EventID });
    }

    [HttpGet]
    public async Task<IActionResult> EditEvent(int id)
    {
        var result = await Api.GetAsync<EventItem>($"api/events/{id}");
        if (result.Status == 401) return SessionExpired();
        if (!result.Success || result.Data is null)
        {
            ShowError(result.Error);
            return RedirectToAction("Events");
        }

        var e = result.Data;
        return View("EventForm", new EventFormVm
        {
            EventID = e.EventID,
            Title = e.Title,
            Description = e.Description,
            Date = e.Date,
            Location = e.Location,
            Distance = e.Distance,
            EventType = e.EventType
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditEvent(int id, EventFormVm vm)
    {
        vm.EventID = id;
        if (!ModelState.IsValid) return View("EventForm", vm);

        var result = await Api.PutAsync<EventItem>($"api/events/{id}", ToBody(vm));
        if (result.Status == 401) return SessionExpired();
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error);
            return View("EventForm", vm);
        }

        ShowSuccess("Event updated.");
        return RedirectToAction("Events");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var result = await Api.DeleteAsync($"api/events/{id}");
        if (result.Status == 401) return SessionExpired();

        if (result.Success) ShowSuccess("Event deleted.");
        else ShowError(result.Error);
        return RedirectToAction("Events");
    }

    // ---------- Categories ----------

    public async Task<IActionResult> Categories(int eventId)
    {
        var vm = await LoadCategoriesAsync(eventId);
        if (vm is null) return RedirectToAction("Events");
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(int eventId, CategoryFormVm form)
    {
        if (!ModelState.IsValid)
        {
            var vm = await LoadCategoriesAsync(eventId);
            if (vm is null) return RedirectToAction("Events");
            vm.Form = form;
            return View("Categories", vm);
        }

        var result = await Api.PostAsync<CategoryItem>($"api/events/{eventId}/categories", new { name = form.Name, distance = form.Distance });
        if (result.Status == 401) return SessionExpired();

        if (result.Success) ShowSuccess("Category added.");
        else ShowError(result.Error);
        return RedirectToAction("Categories", new { eventId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int eventId, int categoryId)
    {
        var result = await Api.DeleteAsync($"api/categories/{categoryId}");
        if (result.Status == 401) return SessionExpired();

        if (result.Success) ShowSuccess("Category removed.");
        else ShowError(result.Error);
        return RedirectToAction("Categories", new { eventId });
    }

    // ---------- Enrolments ----------

    public async Task<IActionResult> Enrolments(int eventId)
    {
        var ev = await Api.GetAsync<EventItem>($"api/events/{eventId}");
        if (ev.Status == 401) return SessionExpired();
        if (!ev.Success || ev.Data is null)
        {
            ShowError(ev.Error);
            return RedirectToAction("Events");
        }

        var enrolments = await Api.GetAsync<List<EnrollmentItem>>($"api/events/{eventId}/enrollments");
        if (enrolments.Status == 401) return SessionExpired();
        if (!enrolments.Success) ViewBag.LoadError = enrolments.Error;

        return View(new EnrolmentsVm
        {
            Event = ev.Data,
            Enrolments = (enrolments.Data ?? new()).OrderBy(e => e.CategoryName).ThenBy(e => e.ParticipantName).ToList()
        });
    }

    [HttpPost]
    public async Task<IActionResult> SetStatus(int eventId, int enrollmentId, string status)
    {
        var result = await Api.PutAsync($"api/enrollments/{enrollmentId}/status", new { status });
        if (result.Status == 401) return SessionExpired();

        if (result.Success) ShowSuccess($"Enrolment marked as {status}.");
        else ShowError(result.Error);
        return RedirectToAction("Enrolments", new { eventId });
    }

    // ---------- Results ----------

    public async Task<IActionResult> Results(int eventId)
    {
        var vm = await LoadResultsAsync(eventId);
        if (vm is null) return RedirectToAction("Events");
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> SaveResults(int eventId, List<ResultInput> rows)
    {
        var vm = await LoadResultsAsync(eventId);
        if (vm is null) return RedirectToAction("Events");
        if (!vm.CanCapture)
        {
            ShowError("Results can only be captured once the event has taken place.");
            return RedirectToAction("Results", new { eventId });
        }

        var saved = 0;
        var problems = new List<string>();
        foreach (var row in rows)
        {
            var finishText = row.FinishTime?.Trim();
            if (string.IsNullOrEmpty(finishText) && row.Position is null) continue; // nothing entered for this person

            var name = vm.Rows.FirstOrDefault(r => r.Enrolment.EnrollmentID == row.EnrollmentId)?.Enrolment.ParticipantName ?? "A participant";
            if (!string.IsNullOrEmpty(finishText) && !TimePattern.IsMatch(finishText))
            {
                problems.Add($"{name}: enter the finish time as hh:mm:ss (for example 01:05:30).");
                continue;
            }
            if (row.Position is < 1)
            {
                problems.Add($"{name}: the position must be 1 or higher.");
                continue;
            }

            TimeSpan? finish = string.IsNullOrEmpty(finishText) ? null : TimeSpan.Parse(finishText);
            ApiResult result = row.ResultId is int resultId
                ? await Api.PutAsync($"api/results/{resultId}", new { finishTime = finish, position = row.Position })
                : await Api.PostAsync("api/results", new { enrollmentId = row.EnrollmentId, finishTime = finish, position = row.Position });

            if (result.Status == 401) return SessionExpired();
            if (result.Success) saved++;
            else problems.Add($"{name}: {result.Error}");
        }

        if (saved > 0) ShowSuccess($"{saved} result(s) saved and now visible to participants.");
        if (problems.Count > 0) ShowError(string.Join(" ", problems));
        if (saved == 0 && problems.Count == 0) ShowError("Enter at least one finish time or position.");
        return RedirectToAction("Results", new { eventId });
    }

    // ---------- helpers ----------

    private static object ToBody(EventFormVm vm) => new
    {
        title = vm.Title,
        description = vm.Description,
        date = vm.Date,
        location = vm.Location,
        distance = vm.Distance,
        eventType = vm.EventType
    };

    private async Task<CategoriesVm?> LoadCategoriesAsync(int eventId)
    {
        var ev = await Api.GetAsync<EventItem>($"api/events/{eventId}");
        if (!ev.Success || ev.Data is null)
        {
            ShowError(ev.Status == 401 ? "Your session has expired. Please log in again." : ev.Error);
            return null;
        }
        var categories = await Api.GetAsync<List<CategoryItem>>($"api/events/{eventId}/categories");
        if (!categories.Success) ViewBag.LoadError = categories.Error;
        return new CategoriesVm { Event = ev.Data, Categories = categories.Data ?? new() };
    }

    private async Task<ResultsVm?> LoadResultsAsync(int eventId)
    {
        var ev = await Api.GetAsync<EventItem>($"api/events/{eventId}");
        if (!ev.Success || ev.Data is null)
        {
            ShowError(ev.Status == 401 ? "Your session has expired. Please log in again." : ev.Error);
            return null;
        }

        var enrolments = await Api.GetAsync<List<EnrollmentItem>>($"api/events/{eventId}/enrollments");
        var results = await Api.GetAsync<List<ResultItem>>($"api/events/{eventId}/results");
        if (!enrolments.Success) ViewBag.LoadError = enrolments.Error;
        else if (!results.Success) ViewBag.LoadError = results.Error;

        var byEnrolment = (results.Data ?? new()).ToDictionary(r => r.EnrollmentID);
        return new ResultsVm
        {
            Event = ev.Data,
            Rows = (enrolments.Data ?? new())
                .OrderBy(e => e.CategoryName).ThenBy(e => e.ParticipantName)
                .Select(e => new ResultRow { Enrolment = e, Result = byEnrolment.GetValueOrDefault(e.EnrollmentID) })
                .ToList()
        };
    }
}