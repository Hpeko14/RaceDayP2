using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Filters;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

[RoleAuthorize]
public class ProfileController : AppControllerBase
{
    public ProfileController(ApiClient api) : base(api) { }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var result = await Api.GetAsync<UserProfile>("api/users/me");
        if (result.Status == 401) return SessionExpired();
        if (!result.Success || result.Data is null)
        {
            ShowError(result.Error);
            return RedirectToAction("Index", "Home");
        }
        return View(ToVm(result.Data));
    }

    [HttpPost]
    public async Task<IActionResult> Update(ProfileVm vm)
    {
        vm.Role = HttpContext.Session.GetString(SessionKeys.Role) ?? "";
        if (!ModelState.IsValid) return View("Index", vm);

        var result = await Api.PutAsync<UserProfile>("api/users/me",
            new { name = vm.Name, email = vm.Email, newPassword = string.IsNullOrWhiteSpace(vm.NewPassword) ? null : vm.NewPassword });
        if (result.Status == 401) return SessionExpired();
        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError("", result.Error);
            return View("Index", vm);
        }

        HttpContext.Session.SetString(SessionKeys.Name, result.Data.Name);
        ShowSuccess("Your profile has been updated.");
        return RedirectToAction("Index");
    }

    private static ProfileVm ToVm(UserProfile u) => new()
    {
        Name = u.Name,
        Email = u.Email,
        Role = u.Role
    };
}