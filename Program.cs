using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using smartmetercms.Data;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with SQLite configuration
builder.Services.AddDbContext<smartmetercmsContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("smartmetercmsContext") ?? throw new InvalidOperationException("Connection string 'smartmetercmsContext' not found.")));

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Cookie is accessible only through HTTP requests
    options.Cookie.IsEssential = true; // Essential for the application to run
});

// Add services to the container
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<smartmetercmsContext>();
    try
    {
        dbContext.Database.Migrate();
        Console.WriteLine("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Comment out HTTPS redirection for Render if no SSL
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Use session middleware
app.UseSession();

// Authorization and authentication middleware
app.UseAuthorization();

// Map controller routes
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();