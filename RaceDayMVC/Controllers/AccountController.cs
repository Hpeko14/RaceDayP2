using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

public class AccountController : AppControllerBase
{
    public AccountController(ApiClient api) : base(api) { }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32(SessionKeys.UserId) is not null)
            return RedirectToAction("Index", "Home");
        return View(new LoginVm());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        HttpContext.Session.Clear(); // always start from a clean session
        var result = await Api.PostAsync<UserProfile>("api/auth/login", new { email = vm.Email, password = vm.Password });
        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError("", result.Status == 401 ? "Incorrect email or password." : result.Error);
            return View(vm);
        }

        // The user's id and role are kept in the session for every later page
        HttpContext.Session.SetInt32(SessionKeys.UserId, result.Data.UserID);
        HttpContext.Session.SetString(SessionKeys.Role, result.Data.Role);
        HttpContext.Session.SetString(SessionKeys.Name, result.Data.Name);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.Session.GetInt32(SessionKeys.UserId) is not null)
            return RedirectToAction("Index", "Home");
        return View(new RegisterVm());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await Api.PostAsync<UserProfile>("api/auth/register",
            new { name = vm.Name, email = vm.Email, password = vm.Password, role = vm.Role });
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error);
            return View(vm);
        }

        ShowSuccess("Your account has been created. Please log in.");
        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await Api.PostAsync("api/auth/logout"); // end the API session too
        HttpContext.Session.Clear();
        ShowSuccess("You have been logged out.");
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();
}