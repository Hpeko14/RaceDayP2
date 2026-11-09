using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Same number/date formatting everywhere (any server locale), so forms behave identically
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

// Session keeps the logged-in user, their role and the API's session cookie
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".RaceDayMvc.Session"; // different from the API's cookie so they never clash on localhost
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// All data comes from the Part 2 API. This project has no database access.
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7205/";
var allowSelfSigned = builder.Configuration.GetValue<bool>("ApiAllowSelfSignedCert");
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(30);
})
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler { UseCookies = false }; // the API session cookie is forwarded manually
        if (allowSelfSigned)
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        return handler;
    });

var app = builder.Build();

app.UseExceptionHandler("/Home/Error");                       // friendly page, never a stack trace
app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();