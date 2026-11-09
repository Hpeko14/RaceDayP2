using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Filters;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;//Namespace RaceDayMVC.Controllers

[RoleAuthorize(Role = Roles.Participant)]
public class ParticipantController : AppControllerBase
{//Start of the ParticipantController class
    public ParticipantController(ApiClient api) : base(api) { }

    /// <summary>Enter an event by choosing one of its categories. The API records the enrolment immediately.</summary>
    [HttpPost]
    public async Task<IActionResult> Enrol(int eventId, int categoryId)
    {
        var result = await Api.PostAsync<EnrollmentItem>("api/enrollments", new { categoryId });
        if (result.Status == 401) return SessionExpired();
        if (!result.Success)
        {
            ShowError(result.Error);
            return RedirectToAction("Details", "Home", new { id = eventId });
        }

        ShowSuccess("You are enrolled. Your enrolment is pending until the organiser confirms it.");
        return RedirectToAction("MyEvents");
    }

    public async Task<IActionResult> MyEvents()
    {//Start of the MyEvents method
        var result = await Api.GetAsync<List<EnrollmentItem>>("api/enrollments/my");
        if (result.Status == 401) return SessionExpired();
        if (!result.Success) ViewBag.LoadError = result.Error;
        return View((result.Data ?? new()).OrderBy(e => e.EventDate).ToList());
    }//End of the MyEvents method

    public async Task<IActionResult> MyResults()
    {
        var result = await Api.GetAsync<List<ResultItem>>("api/results/my");
        if (result.Status == 401) return SessionExpired();
        if (!result.Success) ViewBag.LoadError = result.Error;
        return View((result.Data ?? new()).OrderByDescending(r => r.EventDate).ToList());
    }
}//End of the ParticipantController class