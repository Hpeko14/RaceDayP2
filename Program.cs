using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{// Configure session options
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});// Configure session options


// Add DbContext
builder.Services.AddDbContext<RaceDayContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RaceDay")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register ADO.NET user repository
builder.Services.AddScoped<RaceDayAPI.Repositories.IUserRepository, RaceDayAPI.Repositories.AdoUserRepository>();

var app = builder.Build();

// Apply EF Core migrations at startup (will create or update the database schema)
using (var scope = app.Services.CreateScope())
{// Create a scope to get the DbContext
    try
    {// Get the DbContext and apply migrations
        var db = scope.ServiceProvider.GetRequiredService<RaceDayContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log and rethrow so the app fails fast in CI if migrations cannot be applied
        Console.WriteLine($"Failed to apply migrations: {ex.Message}");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSession();
app.UseAuthorization();
app.MapControllers();
app.Run();
