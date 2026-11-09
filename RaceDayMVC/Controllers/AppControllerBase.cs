using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

public abstract class AppControllerBase : Controller
{
    protected readonly ApiClient Api;
    protected AppControllerBase(ApiClient api) => Api = api;

    protected void ShowSuccess(string message) => TempData["Success"] = message;
    protected void ShowError(string message) => TempData["Error"] = message;

    /// <summary>The API no longer recognises our session (expired): sign out here too and go to Login.</summary>
    protected IActionResult SessionExpired()
    {
        HttpContext.Session.Clear();
        ShowError("Your session has expired. Please log in again.");
        return RedirectToAction("Login", "Account");
    }
}